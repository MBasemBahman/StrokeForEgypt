using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StrokeForEgypt.BaseRepository;
using StrokeForEgypt.DAL;
using StrokeForEgypt.Entity.AccountEntity;
using StrokeForEgypt.Entity.AuthEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using static StrokeForEgypt.Common.EnumData;
using BC = BCrypt.Net.BCrypt;

namespace StrokeForEgypt.Repository.AccountEntityRepository
{
    public class AccountRepository : AppBaseRepository<Account>
    {
        private readonly BaseDBContext DBContext;
        private readonly IMapper _Mapper;

        public AccountRepository(BaseDBContext DBContext, IMapper Mapper) : base(DBContext)
        {
            this.DBContext = DBContext;
            _Mapper = Mapper;
        }

        public Account Login(string Email, string Password, string LoginToken)
        {
            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password))
            {
                var account = DBContext.Account
                                       .SingleOrDefault(a => !string.IsNullOrEmpty(Email) &&
                                                             !string.IsNullOrEmpty(Password) &&
                                                             a.Email.ToLower().Trim() == Email.ToLower().Trim());

                if (account != null && BC.Verify(Password, account.Password))
                {
                    return account;
                }
            }
            return null;
        }

        public bool Login(string LoginToken)
        {
            if (!string.IsNullOrEmpty(LoginToken))
            {
                return DBContext.Account.Any(a => !string.IsNullOrEmpty(LoginToken) &&
                                                  a.LoginToken.ToLower().Trim() == LoginToken.ToLower().Trim());
            }

            return false;
        }

        public Account Register(Account account)
        {
            if (!string.IsNullOrEmpty(account.Password))
            {
                // hash password
                account.Password = BC.HashPassword(account.Password);
            }

            account.Token = NewToken();

            return account;
        }

        public Guid NewToken()
        {
            Guid token = Guid.NewGuid();
            while (Any(a => a.Token == token))
            {
                token = Guid.NewGuid();
            }

            return token;
        }

    }
}
