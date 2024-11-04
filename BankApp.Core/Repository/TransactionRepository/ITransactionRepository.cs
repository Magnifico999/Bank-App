using BankApp.Data.DTO;
using BankApp.Data.Models;

namespace BankApp.Core.Repository.TransactionRepository
{
    public interface ITransactionRepository
    {
        Response CreateNewTransaction(Transaction transaction);
        Response FindTransactionByDate(DateTime date);
        Response MakeDeposit(string AccountNumber, decimal Amount, string TransactionPin);
        Response MakeWithdrawal(string AccountNumber, decimal Amount, string TransactionPin);
        Response MakeFundsTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin);
        Response GetAll();
        Task<Transaction> GetById(int Id);
        Task DeleteTransaction(int Id);
    }
}
