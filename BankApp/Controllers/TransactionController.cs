using AutoMapper;
using BankApp.Data.DTO;
using BankApp.Data.Models;
using BankApp.Core.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ITransactionService _service;
        IMapper _mapper;
        protected Response _response;
        private const int PageSize = 5;

        public TransactionController(ITransactionService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }
        public IActionResult TransactionIndex(int page = 1)
        {
            var transactionsResponse = _service.GetAll();

            if (transactionsResponse.ResponseCode == "00" && transactionsResponse.Data is List<Transaction> transactions)
            {
                var paginatedTransactions = transactions
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();

                var model = new PaginatedList<Transaction>(paginatedTransactions, transactions.Count, page, PageSize);
                return View(model);
            }
            else
            {
                var emptyList = new PaginatedList<Transaction>(new List<Transaction>(), 0, 1, PageSize);
                return View(emptyList);
            }
        }
        public IActionResult Search(string searchString, int page = 1)
        {
            var transactionsResponse = _service.GetAll();

            if (transactionsResponse.ResponseCode == "00" && transactionsResponse.Data is List<Transaction> transactions)
            {
                if (!string.IsNullOrEmpty(searchString))
                {
                    transactions = transactions.Where(t =>
                        t.TransactionUniqueReference.Contains(searchString) ||
                        t.TransactionAmount.ToString().Contains(searchString) ||
                        t.TransactionSourceAccount.ToString().Contains(searchString) ||
                        t.TransactionDestinationAccount.ToString().Contains(searchString) ||
                        t.TransactionAmount.ToString().Contains(searchString) ||
                        t.TransactionType.ToString().Contains(searchString)).ToList();
                }

                var paginatedTransactions = transactions
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();

                var model = new PaginatedList<Transaction>(paginatedTransactions, transactions.Count, page, PageSize);
                return View("TransactionIndex", model);
            }
            else
            {
                return View(new List<Transaction>());
            }
        }

        public async Task<IActionResult> TransactionDetails(int id)
        {
            var transactionDetail = await _service.GetById(id);
            return View(transactionDetail);
        }

        public IActionResult SearchTransactions(string searchString = null, DateTime? searchDate = null)
        {
            var transactionsResponse = _service.GetAll();

            if (transactionsResponse.ResponseCode == "00" && transactionsResponse.Data is List<Transaction> transactions)
            {
                if (!string.IsNullOrEmpty(searchString))
                {
                    transactions = transactions.Where(t =>
                        t.TransactionUniqueReference.Contains(searchString)).ToList();
                }

                if (searchDate != null)
                {
                    transactions = transactions.Where(t =>
                        t.TransactionDate.Date == searchDate.Value.Date).ToList();
                }

                return View("TransactionIndex");
            }
            else
            {
                return View(new List<Transaction>());
            }
        }

        public IActionResult MakeDeposit()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MakeDeposit(string AccountNumber, decimal Amount, string TransactionPin)
        {
            try
            {
                Response response = _service.MakeDeposit(AccountNumber, Amount, TransactionPin);

                if (response != null && response.ResponseCode == "03")
                {
                    TempData["error"] = "Invalid account number or pin";
                    ModelState.AddModelError(string.Empty, "Invalid account number or pin");
                    return View();
                }
                
                else if (response != null && response.ResponseCode == "00")
                {
                    TempData["success"] = "Deposit successful";
                    return RedirectToAction("TransactionIndex");
                }
                else
                {
                    TempData["error"] = "Failed to make a deposit.";
                    ModelState.AddModelError(string.Empty, "Check the pin, account number or if the amount exceeds 200000.");
                    return View();
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Deposit amount should not be within the deposit range"))
                {
                    TempData["error"] = "Deposit amount should not be within the deposit range";
                    ModelState.AddModelError("Deposit", "Deposit amount should not be within the deposit range");
                    return BadRequest(ModelState);
                }
                else
                {
                    TempData["error"] = "An error occurred while depositing";
                    ModelState.AddModelError(string.Empty, "An error occurred while depositing");
                }
                return View();
            }
        }

        public IActionResult MakeTransfer()
        {

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MakeTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin)
        {
            try
            {
                Response response = _service.MakeFundsTransfer(FromAccount, ToAccount, Amount, TransactionPin);

                if (response != null && response.ResponseCode == "03")
                {
                    TempData["error"] = "Invalid account number or pin";
                    ModelState.AddModelError(string.Empty, "Invalid account number or pin");
                    return View();
                }

                else if (response != null && response.ResponseCode == "00")
                {
                    TempData["success"] = "Transfer successful";
                    return RedirectToAction("TransactionIndex");
                }
                else
                {
                    TempData["error"] = "Failed to make a transfer.";
                    ModelState.AddModelError(string.Empty, "Check the pin, account number or if the amount exceeds 200000.");
                    return View();
                }
            }catch (Exception ex)
            {
                if (ex.Message.Contains("Transfer amount should not be within the certain range"))
                {
                    TempData["error"] = "Transfer amount should not be within the transfer range";
                    ModelState.AddModelError("Transfer", "Transfer amount should not be within the transfer range");
                    return BadRequest(ModelState);
                }
                else
                {
                    TempData["error"] = "An error occurred while Transferring";
                    ModelState.AddModelError(string.Empty, "An error occurred while Transferring");
                }
                return View();
            }

            

        }
        public IActionResult MakeWithdrawal()
        {

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MakeWithdrawal(string AccountNumber, decimal Amount, string TransactionPin)
        {
            try
            {
                Response response = _service.MakeWithdrawal(AccountNumber, Amount, TransactionPin);

                if (response != null && response.ResponseCode == "03")
                {
                    TempData["error"] = "Invalid account number or pin";
                    ModelState.AddModelError(string.Empty, "Invalid account number or pin");
                    return View();
                }
                

                else if (response != null && response.ResponseCode == "00")
                {
                    TempData["success"] = "Withdraw successful";
                    return RedirectToAction("TransactionIndex");
                }
                else
                {
                    TempData["error"] = "Failed to make a withdrawal.";
                    ModelState.AddModelError(string.Empty, "Check the pin, account number or if the amount exceeds 200000.");
                    return View();
                }

            }catch (Exception ex)
            {
                if (ex.Message.Contains("Withdrawal amount should not be within the certain range"))
                {
                    TempData["error"] = "Withdrawal amount should not be within the withdraw range";
                    ModelState.AddModelError("Withdrawal", "Withdrawal amount should not be within the withdraw range");
                    return BadRequest(ModelState);
                }
                else
                {
                    TempData["error"] = "An error occurred while withdrawaing";
                    ModelState.AddModelError(string.Empty, "An error occurred while withdrawing");
                }
                return View();
            }



        }
        public async Task<IActionResult> DeleteTransaction(int Id)
        {

            var account = await _service.GetById(Id);

            if (account == null)
            {
                return NotFound();
            }
            TransactionRequest updateModel = new TransactionRequest
            {
                Id = account.Id,
                TransactionAmount = account.TransactionAmount,
                TransactionDate = account.TransactionDate,
                TransactionUniqueReference = account.TransactionUniqueReference,
                TransactionSourceAccount = account.TransactionSourceAccount,
                TransactionDestinationAccount = account.TransactionDestinationAccount,
                TransactionStatus = account.TransactionStatus,
                TransactionParticulars = account.TransactionParticulars
            };

            return View(updateModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTransaction(TransactionRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var account = _mapper.Map<Transaction>(model);

            await _service.DeleteTransaction(model.Id);
            TempData["success"] = "Transaction deleted successfully";

            return RedirectToAction(nameof(TransactionIndex));

        }
    }
}
