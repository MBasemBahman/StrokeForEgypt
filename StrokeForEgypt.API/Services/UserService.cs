using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StrokeForEgypt.API.Authorization;
using StrokeForEgypt.API.Helpers;
using StrokeForEgypt.API.Models.Accounts;
using StrokeForEgypt.DAL;
using StrokeForEgypt.Entity.AccountEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using BC = BCrypt.Net.BCrypt;

namespace StrokeForEgypt.API.Services
{
    public interface IAccountService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress);
        AuthenticateResponse RefreshToken(string token, string ipAddress);
        void RevokeToken(string token, string ipAddress);
        Account GetById(int id);
    }

    public class AccountService : IAccountService
    {
        private readonly BaseDBContext _context;
        private readonly IJwtUtils _jwtUtils;
        private readonly AppSettings _appSettings;

        public AccountService(
            BaseDBContext context,
            IJwtUtils jwtUtils,
            IOptions<AppSettings> appSettings)
        {
            _context = context;
            _jwtUtils = jwtUtils;
            _appSettings = appSettings.Value;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress)
        {
            Account account = null;

            if (!string.IsNullOrEmpty(model.Email) && !string.IsNullOrEmpty(model.Password))
            {
                account = _context.Account
                                  .Include(a => a.RefreshTokens)
                                  .SingleOrDefault(x => x.Email == model.Email);

                // validate
                if (account == null || !BC.Verify(model.Password, account.PasswordHash))
                {
                    throw new AppException("Email or password is incorrect");
                }
            }
            else if (!string.IsNullOrEmpty(model.LoginToken))
            {
                string LoginToken = BC.HashPassword(model.LoginToken);
                account = _context.Account.SingleOrDefault(x => x.LoginTokenHash == model.LoginToken);

                // validate
                if (account == null)
                {
                    throw new AppException("Login is incorrect");
                }
            }

            // authentication successful so generate jwt and refresh tokens
            string jwtToken = _jwtUtils.GenerateJwtToken(account);
            RefreshToken refreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
            account.RefreshTokens.Add(refreshToken);

            // remove old refresh tokens from account
            RemoveOldRefreshTokens(account);

            // save changes to db
            _context.Update(account);
            _context.SaveChanges();

            return new AuthenticateResponse(account, jwtToken, refreshToken.Token);
        }

        public AuthenticateResponse RefreshToken(string token, string ipAddress)
        {
            Account account = GetAccountByRefreshToken(token);
            RefreshToken refreshToken = account.RefreshTokens.Single(x => x.Token == token);

            if (refreshToken.IsRevoked)
            {
                // revoke all descendant tokens in case this token has been compromised
                RevokeDescendantRefreshTokens(refreshToken, account, ipAddress, $"Attempted reuse of revoked ancestor token: {token}");
                _context.Update(account);
                _context.SaveChanges();
            }

            if (!refreshToken.IsActive)
            {
                throw new AppException("Invalid token");
            }

            // replace old refresh token with a new one (rotate token)
            RefreshToken newRefreshToken = RotateRefreshToken(refreshToken, ipAddress);
            account.RefreshTokens.Add(newRefreshToken);

            // remove old refresh tokens from account
            RemoveOldRefreshTokens(account);

            // save changes to db
            _context.Update(account);
            _context.SaveChanges();

            // generate new jwt
            string jwtToken = _jwtUtils.GenerateJwtToken(account);

            return new AuthenticateResponse(account, jwtToken, newRefreshToken.Token);
        }

        public void RevokeToken(string token, string ipAddress)
        {
            Account account = GetAccountByRefreshToken(token);
            RefreshToken refreshToken = account.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive)
            {
                throw new AppException("Invalid token");
            }

            // revoke token and save
            RevokeRefreshToken(refreshToken, ipAddress, "Revoked without replacement");
            _context.Update(account);
            _context.SaveChanges();
        }

        public Account GetById(int id)
        {
            Account account = _context.Account.Find(id);
            if (account == null)
            {
                throw new KeyNotFoundException("Account not found");
            }

            return account;
        }

        // helper methods

        private Account GetAccountByRefreshToken(string token)
        {
            Account account = _context.Account.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));

            if (account == null)
            {
                throw new AppException("Invalid token");
            }

            return account;
        }

        private RefreshToken RotateRefreshToken(RefreshToken refreshToken, string ipAddress)
        {
            RefreshToken newRefreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
            RevokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
            return newRefreshToken;
        }

        private void RemoveOldRefreshTokens(Account account)
        {
            // remove old inactive refresh tokens from account based on TTL in app settings
            account.RefreshTokens.RemoveAll(x =>
                !x.IsActive &&
                x.CreatedAt.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
        }

        private void RevokeDescendantRefreshTokens(RefreshToken refreshToken, Account account, string ipAddress, string reason)
        {
            // recursively traverse the refresh token chain and ensure all descendants are revoked
            if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
            {
                RefreshToken childToken = account.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
                if (childToken.IsActive)
                {
                    RevokeRefreshToken(childToken, ipAddress, reason);
                }
                else
                {
                    RevokeDescendantRefreshTokens(childToken, account, ipAddress, reason);
                }
            }
        }

        private void RevokeRefreshToken(RefreshToken token, string ipAddress, string reason = null, string replacedByToken = null)
        {
            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReasonRevoked = reason;
            token.ReplacedByToken = replacedByToken;
        }
    }
}
