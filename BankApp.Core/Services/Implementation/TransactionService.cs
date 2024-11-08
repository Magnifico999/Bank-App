using BankApp.Core.Repository.TransactionRepository;
using BankApp.Core.Services.Interface;
using BankApp.Data.DTO;
using BankApp.Data.Models;

namespace BankApp.Core.Services.Implementation
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repo;

        public TransactionService(ITransactionRepository repo)
        {
            _repo = repo;
        }
        public Response CreateNewTransaction(string userId, Transaction transaction)
        {
            return _repo.CreateNewTransaction(userId, transaction);
        }

        public async Task DeleteTransaction(int Id)
        {
            await  _repo.DeleteTransaction(Id);
        }

        public Response FindTransactionByDate(DateTime date)
        {
            return _repo.FindTransactionByDate(date);
        }

        public Response GetAll(string userId)
        {
            return _repo.GetAll(userId);
        }

        public async Task<Transaction> GetById(int Id)
        {
           return await _repo.GetById(Id);
        }

        public Response MakeDeposit(string userId, string AccountNumber, decimal Amount, string TransactionPin)
        {
            return _repo.MakeDeposit(userId, AccountNumber, Amount, TransactionPin);
        }

        public Response MakeFundsTransfer(string userId, string FromAccount, string ToAccount, decimal Amount, string TransactionPin)
        {
            return _repo.MakeFundsTransfer(userId, FromAccount, ToAccount, Amount, TransactionPin);
        }

        public Response MakeWithdrawal(string userId, string AccountNumber, decimal Amount, string TransactionPin)
        {
            return _repo.MakeWithdrawal(userId, AccountNumber, Amount, TransactionPin);
        }
    }
}
