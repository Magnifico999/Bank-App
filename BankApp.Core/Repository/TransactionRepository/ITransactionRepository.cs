using BankApp.Data.DTO;
using BankApp.Data.Models;

namespace BankApp.Core.Repository.TransactionRepository
{
    public interface ITransactionRepository
    {
        Response CreateNewTransaction(string userId, Transaction transaction);
        Response FindTransactionByDate(DateTime date);
        Response MakeDeposit(string userId, string AccountNumber, decimal Amount, string TransactionPin);
        Response MakeWithdrawal(string userId, string AccountNumber, decimal Amount, string TransactionPin);
        Response MakeFundsTransfer(string userId, string FromAccount, string ToAccount, decimal Amount, string TransactionPin);
        Response GetAll(string userId);
        Task<Transaction> GetById(int Id);
        Task DeleteTransaction(int Id);
    }
}
