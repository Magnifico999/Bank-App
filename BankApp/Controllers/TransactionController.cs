using AutoMapper;
using BankApp.Data.DTO;
using BankApp.Data.Models;
using BankApp.Core.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using BankApp.Data;
using Microsoft.AspNetCore.Identity;

namespace BankApp.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ITransactionService _service;
        IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        protected Response _response;
        private const int PageSize = 5;

        public TransactionController(ITransactionService service, IMapper mapper, IHttpContextAccessor httpContextAccessor, ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _service = service;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _db = db;
            _userManager = userManager;
        }
        public IActionResult TransactionIndex()
        {
            return View();
        }
        

        public async Task<IActionResult> TransactionDetails(int id)
        {
            var transactionDetail = await _service.GetById(id);
            return View(transactionDetail);
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
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                Response response = _service.MakeDeposit(userId, AccountNumber, Amount, TransactionPin);

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
        public async Task<IActionResult> MakeTransfer()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized(); 
            }

            var userAccount = _db.Accounts.FirstOrDefault(a => a.Email == user.Email);

            if (userAccount == null)
            {
                return NotFound();
            }

            ViewBag.FromAccountNumber = userAccount.AccountNumberGenerated;

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MakeTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                Response response = _service.MakeFundsTransfer(userId, FromAccount, ToAccount, Amount, TransactionPin);

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
                    ModelState.AddModelError(string.Empty, "Check the pin, account number or if the amount exceeds 200,000.");

                }
            }
            catch (Exception ex)
            {

                if (ex.Message.Contains("Transfer amount should not be within the certain range"))
                {
                    TempData["error"] = "Transfer amount should not be within the transfer range";
                    ModelState.AddModelError("Transfer", "Transfer amount should not be within the transfer range");
                    return BadRequest(ModelState);
                }
                else
                {
                    TempData["error"] = "An error occurred while transferring";
                    ModelState.AddModelError(string.Empty, "An error occurred while transferring");
                }
                
            }
            return View();
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
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Response response = _service.MakeWithdrawal(userId, AccountNumber, Amount, TransactionPin);

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
        #region API CALLS
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var records = _service.GetAll(userId);

            return Json(records);
        }
        #endregion
    }
}
