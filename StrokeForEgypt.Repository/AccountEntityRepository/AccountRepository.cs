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

        public Account GetByToken(Guid Token)
        {
            return DBContext.Account.Single(a => a.Token == Token);
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
