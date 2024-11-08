using AutoMapper;
using BankApp.Data.DTO;
using BankApp.Data.Models;
using BankApp.Core.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IAccountService _service;
        protected Response _response;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(IAccountService service, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult AccountIndex()
        {
            return View();
        }


        public async Task<IActionResult> AccountDetails(int id)
        {
            var accountDetail = await _service.GetById(id);
            return View(accountDetail);
        }

        public IActionResult CreateAccount()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAccount(RegisterNewAccountModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }

            try
            {
                var account = _mapper.Map<Account>(model);
                var result = await _service.Create(account, model.Pin, model.ConfirmPin);

                if (result.ResponseCode == "EmailExists")
                {
                    ModelState.AddModelError("Email", "Email already in use");
                }
                else if (result.ResponseCode == "PhoneNumberExists")
                {
                    ModelState.AddModelError("PhoneNumber", "Phone number already in use");
                }
                else if (result.ResponseCode == "PinsDoNotMatch")
                {
                    ModelState.AddModelError(string.Empty, "Pins do not match");
                }
                else if (result.ResponseCode == "Success")
                {
                    TempData["success"] = "Account created successfully";
                    return RedirectToAction(nameof(AccountIndex));
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }


        public async Task<IActionResult> UpdateAccount(int id)
        {
            var account = await _service.GetById(id);

            if (account == null)
            {
                return NotFound(); 
            }
            UpdateAccountModel updateModel = new()
            {
                Id = account.Id,
                FirstName = account.FirstName,
                LastName = account.LastName,
                PhoneNumber = account.PhoneNumber,
                Email = account.Email,
                Pin = string.Empty,
                ConfirmPin = string.Empty, 
                DateLastUpdated = account.DateLastUpdated 
            };

            return View(updateModel); 
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAccount(UpdateAccountModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var account = _mapper.Map<Account>(model);

                var updateResult = await _service.Update(account);

                if (updateResult.ResponseCode == "EmailExists")
                {
                    ModelState.AddModelError("Email", "Email already in use");
                    return View(model);
                }
                else if (updateResult.ResponseCode == "PhoneNumberExist")
                {
                    ModelState.AddModelError("PhoneNumber", "Phone number already in use");
                    return View(model);
                }
                TempData["success"] = "Account updated successfully";

                return RedirectToAction(nameof(AccountIndex));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the account: " + ex.Message);
            }
        }


        public async Task<IActionResult> DeleteAccount(int Id)
        {

            var account = await _service.GetById(Id);

            if (account == null)
            {
                return NotFound();
            }
            AccountDTO deleteModel = new()
            {
                Id = account.Id,
                FirstName = account.FirstName,  
                LastName = account.LastName,
                AccountNumberGenerated = account.AccountNumberGenerated,
                AccountType = account.AccountType,
                CurrentAccountBalance = account.CurrentAccountBalance,
                PhoneNumber = account.PhoneNumber,
                Email = account.Email,
                DateLastUpdated = account.DateLastUpdated
            };

            return View(deleteModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount(AccountDTO model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Phone number or email already exists.");
                return View(model);
            }

            var account = _mapper.Map<Account>(model);

            await _service.Delete(model.Id);
            TempData["success"] = "Account deleted successfully";

            return RedirectToAction(nameof(AccountIndex));



        }

        #region API CALLS
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var records = _service.GetAllAccounts();

            return Json(new { data = records });
        }
        #endregion
    }
}
