using BankApp.Core.Utils;
using BankApp.Data;
using BankApp.Data.DTO;
using BankApp.Data.Enums;
using BankApp.Data.Models;
using BankApp.Core.Repository.AccountRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace BankApp.Core.Repository.TransactionRepository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _db;
        ILogger<TransactionRepository> _logger;
        private AppSettings _settings;
        private static string _ourBankSettlementAccount;
        private readonly IAccountRepository _repo;
        public TransactionRepository(ApplicationDbContext db, ILogger<TransactionRepository> logger, IOptions<AppSettings> settings, IAccountRepository repo)
        {
            _db = db;
            _logger = logger;
            _settings = settings.Value;
            _ourBankSettlementAccount = _settings.OurBankSettlementAccount;
            _repo = repo;
        }
        public Response CreateNewTransaction(Transaction transaction)
        {
            Response response = new Response();
            _db.Transactions.Add(transaction);
            _db.SaveChanges();
            response.ResponseCode = "00";
            response.ResponseMessage = "Transaction created successfully!";
            response.Data = null;

            return response;

        }

        public Response FindTransactionByDate(DateTime date)
        {
            Response response = new Response();
            var transactions = _db.Transactions.Where(x => x.TransactionDate == date).ToList();

            if (transactions.Count == 0)
            {
                response.ResponseCode = "400"; 
                response.ResponseMessage = "No transactions found for the specified date.";
            }
            else
            {
                response.ResponseCode = "00";
                response.ResponseMessage = "Transactions found successfully!";
                response.Data = transactions;
            }

            return response;
        }
        public Response MakeFundsTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin)
        {
            Response response = new Response();
            Account sourceAccount;
            Account destinationAccount;
            Transaction transaction = new Transaction();

            var authUser = _repo.Authenticate(FromAccount, TransactionPin);
            if (authUser == null) throw new ApplicationException("Invalid credentials");

            try
            {
                // For a funds transfer, our bankSettlementAccount is the destination getting the money from the user's account
                sourceAccount = _repo.GetByAccountNumber(FromAccount);
                destinationAccount = _repo.GetByAccountNumber(ToAccount);

                // Parse the current balances as decimals with default values of 0 if null or empty
                decimal sourceBalance = decimal.Parse(sourceAccount.CurrentAccountBalance ?? "0");
                decimal destinationBalance = decimal.Parse(destinationAccount.CurrentAccountBalance ?? "0");

                // Check if the transfer amount is greater than the balance in the source account
                if (Amount > sourceBalance)
                {
                    transaction.TransactionStatus = TranStatus.Failed;
                    response.ResponseCode = "05"; 
                    response.ResponseMessage = "Insufficient balance for transfer";
                    response.Data = null;
                    return response;
                }
                else if (Amount > 200000)
                {
                    response.ResponseCode = "06";
                    response.ResponseMessage = "Transfer must not exceed 200,000";
                    response.Data = null;
                    return response;
                }
                else
                {
                    sourceBalance -= Amount;

                    destinationBalance += Amount;

                    // Update the sourceAccount's balance as a string
                    sourceAccount.CurrentAccountBalance = sourceBalance.ToString();

                    // Update the destinationAccount's balance as a string
                    destinationAccount.CurrentAccountBalance = destinationBalance.ToString();

                    // Check if there is an update
                    if ((_db.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified &&
                         _db.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
                    {
                        transaction.TransactionStatus = TranStatus.Success;
                        response.ResponseCode = "00";
                        response.ResponseMessage = "Transaction successful!";
                        response.Data = null;
                        
                    }
                    else
                    {
                        transaction.TransactionStatus = TranStatus.Failed;
                        response.ResponseCode = "02";
                        response.ResponseMessage = "Transaction failed!";
                        response.Data = null;
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AN ERROR OCCURRED... => {ex.Message}");
            }

            transaction.TransactionType = TranType.Transfer;
            transaction.TransactionSourceAccount = FromAccount;
            transaction.TransactionDestinationAccount = ToAccount;
            transaction.TransactionAmount = Amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionParticulars = $"NEW TRANSACTION FROM SOURCE => {JsonConvert.SerializeObject(transaction.TransactionSourceAccount)} TO DESTINATION ACCOUNT => {JsonConvert.SerializeObject(transaction.TransactionDestinationAccount)} ON DATE => {transaction.TransactionDate} FOR AMOUNT => {JsonConvert.SerializeObject(transaction.TransactionAmount)} TRANSACTION TYPE => {transaction.TransactionType} TRANSACTION STATUS => {transaction.TransactionStatus}";

            _db.Transactions.Add(transaction);
            _db.SaveChanges();

            return response;
        }

        public Response MakeDeposit(string AccountNumber, decimal Amount, string TransactionPin)
        {
            Response response = new Response();
            Account sourceAccount;
            Account destinationAccount;
            Transaction transaction = new Transaction();

            try
            {
                if (!Regex.IsMatch(AccountNumber, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$"))
                {
                    response.ResponseCode = "06"; 
                    response.ResponseMessage = "Account Number must be 10-digit";
                    response.Data = null;
                    return response;

                }
                else if (Amount > 200000) 
                {
                    response.ResponseCode = "06";
                    response.ResponseMessage = "Deposit must not exceed 200,000";
                    response.Data = null;
                    return response;
                }
                var authUser = _repo.Authenticate(AccountNumber, TransactionPin);
                if (authUser == null) throw new ApplicationException("Invalid credentials");

                sourceAccount = _repo.GetByAccountNumber(_ourBankSettlementAccount);
                destinationAccount = _repo.GetByAccountNumber(AccountNumber);

                decimal currentBalance = decimal.Parse(sourceAccount.CurrentAccountBalance ?? "0");
                decimal destinationBalance = decimal.Parse(destinationAccount.CurrentAccountBalance ?? "0");

      
                if (Amount <= 0)
                {
                    transaction.TransactionStatus = TranStatus.Failed;
                    response.ResponseCode = "04"; 
                    response.ResponseMessage = "Invalid deposit amount";
                    response.Data = null;
                    return response;
                }
                else if (Amount > currentBalance)
                {
                    transaction.TransactionStatus = TranStatus.Failed;
                    response.ResponseCode = "05"; 
                    response.ResponseMessage = "Insufficient balance for deposit";
                    response.Data = null;
                    return response;
                }
                else
                {
                    // Update sourceAccount's balance
                    currentBalance -= Amount;
                    sourceAccount.CurrentAccountBalance = currentBalance.ToString();

                    // Update destinationAccount's balance by adding the Amount
                    destinationBalance += Amount;
                    destinationAccount.CurrentAccountBalance = destinationBalance.ToString();

                    // Check if there is an update
                    if ((_db.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified &&
                        _db.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
                    {
                        transaction.TransactionStatus = TranStatus.Success;
                        response.ResponseCode = "00";
                        response.ResponseMessage = "Transaction successful!";
                        response.Data = null;
                        
                    }
                    else
                    {
                        transaction.TransactionStatus = TranStatus.Failed;
                        response.ResponseCode = "02";
                        response.ResponseMessage = "Transaction failed!";
                        response.Data = null;
                        return response;
                    }
                }
            }
            catch (ApplicationException ex)
            {
                transaction.TransactionStatus = TranStatus.Failed;
                response.ResponseCode = "03";
                response.ResponseMessage = "Invalid username or pin";
                response.Data = null;
                _logger.LogError($"Invalid username or pin: {ex.Message}");
            }

            decimal transactionAmount = Amount;

            transaction.TransactionType = TranType.Deposit;
            transaction.TransactionSourceAccount = _ourBankSettlementAccount;
            transaction.TransactionDestinationAccount = AccountNumber;
            transaction.TransactionAmount = Amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionParticulars = $"NEW TRANSACTION FROM SOURCE => {JsonConvert.SerializeObject(transaction.TransactionSourceAccount)} TO DESTINATION ACCOUNT => {JsonConvert.SerializeObject(transaction.TransactionDestinationAccount)} ON DATE => {transaction.TransactionDate} FOR AMOUNT => {transactionAmount} TRANSACTION TYPE => {transaction.TransactionType} TRANSACTION STATUS => {transaction.TransactionStatus}";

            _db.Transactions.Add(transaction);
            _db.SaveChanges();

            return response;
        }


        public Response MakeWithdrawal(string AccountNumber, decimal Amount, string TransactionPin)
        {
            Response response = new Response();
            Account sourceAccount;
            Account destinationAccount;
            Transaction transaction = new Transaction();

            var authUser = _repo.Authenticate(AccountNumber, TransactionPin);
            if (authUser == null) throw new ApplicationException("Invalid credentials");

            try
            {
                // For a withdrawal, our bankSettlementAccount is the destination getting the money from the user's account
                sourceAccount = _repo.GetByAccountNumber(AccountNumber);
                destinationAccount = _repo.GetByAccountNumber(_ourBankSettlementAccount);

                if (decimal.TryParse(sourceAccount.CurrentAccountBalance, out decimal currentBalance))
                {
                    // Check if the withdrawal amount is greater than the balance
                    if (Amount > currentBalance)
                    {
                        transaction.TransactionStatus = TranStatus.Failed;
                        response.ResponseCode = "01";
                        response.ResponseMessage = "Insufficient balance";
                        response.Data = null;
                        return response;
                        
                    }
                    else if (Amount > 200000)
                    {
                        response.ResponseCode = "06";
                        response.ResponseMessage = "Withdrawal must not exceed 200,000";
                        response.Data = null;
                        return response;
                    }
                    else
                    {
                        currentBalance -= Amount;
                       
                        sourceAccount.CurrentAccountBalance = currentBalance.ToString();
                        
                        if (decimal.TryParse(destinationAccount.CurrentAccountBalance, out decimal destinationBalance))
                        {
                            destinationBalance += Amount;

                            destinationAccount.CurrentAccountBalance = destinationBalance.ToString();

                            transaction.TransactionStatus = TranStatus.Success;
                            response.ResponseCode = "00";
                            response.ResponseMessage = "Transaction successful!";
                            response.Data = null;
                        }
                        else
                        {
                            transaction.TransactionStatus = TranStatus.Failed;
                            response.ResponseCode = "02";
                            response.ResponseMessage = "Transaction failed!";
                            response.Data = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AN ERROR OCCURRED... => {ex.Message}");
            }

            transaction.TransactionType = TranType.Withdrawal;
            transaction.TransactionSourceAccount = AccountNumber;
            transaction.TransactionDestinationAccount = _ourBankSettlementAccount;
            transaction.TransactionAmount = Amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionParticulars = $"NEW TRANSACTION FROM SOURCE => {JsonConvert.SerializeObject(transaction.TransactionSourceAccount)} TO DESTINATION ACCOUNT => {JsonConvert.SerializeObject(transaction.TransactionDestinationAccount)} ON DATE => {transaction.TransactionDate} FOR AMOUNT => {JsonConvert.SerializeObject(transaction.TransactionAmount)} TRANSACTION TYPE => {transaction.TransactionType} TRANSACTION STATUS => {transaction.TransactionStatus}";

            _db.Transactions.Add(transaction);
            _db.SaveChanges();

            return response;
        }

        public Response GetAll()
        {
            Response response = new Response();

            try
            {
                var transactions = _db.Transactions.ToList();
                if (transactions.Count == 0)
                {
                    response.ResponseCode = "404"; 
                    response.ResponseMessage = "No transactions found.";
                    response.Data = null;
                }
                else
                {
                    response.ResponseCode = "00"; 
                    response.ResponseMessage = "Transactions retrieved successfully!";
                    response.Data = transactions;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AN ERROR OCCURRED... => {ex.Message}");
                response.ResponseCode = "500"; 
                response.ResponseMessage = "An error occurred while retrieving transactions.";
                response.Data = null;
            }

            return response;
        }
        public async Task<Transaction> GetById(int Id)
        {
            var account = await _db.Transactions.Where(x => x.Id == Id).FirstOrDefaultAsync();
            if (account == null) return null;

            return account;
        }

        public async Task DeleteTransaction(int Id)
        {
            var response = new Response();
            var account = _db.Transactions.Find(Id);
            if (account == null)
            {
                response.ResponseCode = "400";
                response.ResponseMessage = "There was a problem deleting an Transaction";
                response.Data = null;

            }
            _db.Remove(account);
            await _db.SaveChangesAsync();
        }
    }
}
