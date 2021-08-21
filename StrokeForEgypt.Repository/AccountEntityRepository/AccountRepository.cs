using AutoMapper;
using StrokeForEgypt.BaseRepository;
using StrokeForEgypt.DAL;
using StrokeForEgypt.Entity.AccountEntity;
using System;
using System.Linq;
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
                Account account = DBContext.Account
                                       .SingleOrDefault(a => !string.IsNullOrEmpty(Email) &&
                                                             !string.IsNullOrEmpty(Password) &&
                                                             a.Email.ToLower().Trim() == Email.ToLower().Trim());

                if (account != null && BC.Verify(Password, account.PasswordHash))
                {
                    return account;
                }
            }
            return null;
        }

        public Account Login(string LoginToken)
        {
            if (!string.IsNullOrEmpty(LoginToken))
            {
                Account account = DBContext.Account
                                        .SingleOrDefault(a => !string.IsNullOrEmpty(LoginToken) &&
                                                              a.LoginToken.ToLower().Trim() == LoginToken.ToLower().Trim());
                if (account != null)
                {
                    return account;
                }
            }

            return null;
        }

        public Account Login(Guid Token)
        {
            if (Token != Guid.Empty)
            {
                Account account = DBContext.Account
                                        .SingleOrDefault(a => a.Token == Token);
                if (account != null)
                {
                    return account;
                }
            }

            return null;
        }

        public Account Register(Account account)
        {
            if (!string.IsNullOrEmpty(account.PasswordHash))
            {
                // hash password
                account.PasswordHash = BC.HashPassword(account.PasswordHash);
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
