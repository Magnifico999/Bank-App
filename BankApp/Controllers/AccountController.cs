using AutoMapper;
using BankApp.Data.DTO;
using BankApp.Data.Models;
using BankApp.Core.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IAccountService _service;
        protected Response _response;
        private readonly IMapper _mapper;
        
        public AccountController(IAccountService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }
        public IActionResult AccountIndex(int page = 1)
        {
            const int PageSize = 5; 
            var accounts = _service.GetAllAccounts();

            if (accounts != null && accounts.Any())
            {
                var accountModels = _mapper.Map<List<GetAccountModel>>(accounts);

                var paginatedAccounts = accountModels
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();

                var totalAccountsCount = accountModels.Count;
                var model = new PaginatedList<GetAccountModel>(paginatedAccounts, totalAccountsCount, page, PageSize);
                return View(model);
            }
            else
            {
                var emptyList = new PaginatedList<GetAccountModel>(new List<GetAccountModel>(), 0, 1, PageSize);
                return View(emptyList);
            }
        }



        public IActionResult Search(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return RedirectToAction(nameof(AccountIndex));
            }

            var accounts = _service.GetAllAccounts();

            if (accounts == null)
            {
                return BadRequest("It can't be empty");
            }

            searchString = searchString.ToLower();

            var searchResults = accounts
                .Where(account =>
                    account.AccountName.ToLower().Contains(searchString) ||
                    account.FirstName.ToLower().Contains(searchString) ||
                    account.LastName.ToLower().Contains(searchString) ||
                    account.AccountType.ToString().ToLower() == searchString ||
                    account.PhoneNumber.Contains(searchString) ||
                    account.AccountNumberGenerated.Contains(searchString))
                .Select(account => new GetAccountModel
                {
                    Id = account.Id,
                    FirstName = account.FirstName,
                    LastName = account.LastName,
                    AccountName = account.AccountName,
                    PhoneNumber = account.PhoneNumber,
                    Email = account.Email,
                    CurrentAccountBalance = account.CurrentAccountBalance,
                    AccountType = account.AccountType,
                    AccountNumberGenerated = account.AccountNumberGenerated,
                    DateCreated = account.DateCreated,
                    DateLastUpdated = account.DateLastUpdated
                })
                .ToList();

            if (searchResults.Count > 0)
            {
                var totalItems = accounts.Count();
                TempData["success"] = "Search successful";
                return View("AccountIndex", new PaginatedList<GetAccountModel>(searchResults, totalItems, 1, 1)); 
            }

            return View("AccountIndex", new PaginatedList<GetAccountModel>(new List<GetAccountModel>(), 0, 1, 1));
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


    }
}
