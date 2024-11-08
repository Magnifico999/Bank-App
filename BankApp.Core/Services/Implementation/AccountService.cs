using BankApp.Core.Services.Interface;
using BankApp.Data.DTO;
using BankApp.Data.Models;
using BankApp.Core.Repository.AccountRepository;

namespace BankApp.Core.Services.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repo;

        public AccountService(IAccountRepository repo)
        {
            _repo = repo;
        }
        public Account Authenticate(string AccountNumber, string Pin)
        {
            return _repo.Authenticate(AccountNumber, Pin);
        }

        public Task<Response> Create(Account account, string Pin, string ConfirmPin)
        {
            return _repo.Create(account, Pin, ConfirmPin);
        }

        public async Task Delete(int Id)
        {
            await _repo.Delete(Id);
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return _repo.GetAllAccounts();
        }

        public Account GetByAccountNumber(string AccountNumber)
        {
            return _repo.GetByAccountNumber(AccountNumber);
        }

        public Task<Account> GetById(int Id)
        {
            return _repo.GetById(Id);
        }

        public async Task<Response> Update(Account account, string Pin = null)
        {
            return await _repo.Update(account, Pin);
        }
    }
}
