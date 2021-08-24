using AutoMapper;
using StrokeForEgypt.BaseRepository;
using StrokeForEgypt.DAL;
using StrokeForEgypt.Entity.AccountEntity;
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

            if (!string.IsNullOrEmpty(account.LoginTokenHash))
            {
                // hash LoginToken
                account.LoginTokenHash = BC.HashPassword(account.LoginTokenHash);
            }

            if (!string.IsNullOrEmpty(account.VerificationCodeHash))
            {
                // hash VerificationCode
                account.VerificationCodeHash = BC.HashPassword(account.VerificationCodeHash);
            }

            return account;
        }
    }
}
