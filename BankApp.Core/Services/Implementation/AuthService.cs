using BankApp.Core.Services.Interface;
using BankApp.Data.DTO;
using BankApp.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace BankApp.Core.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor,IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task<ResponseDto<string>> ExternalLogin(string email, string firstName, string surname)
        {
            var response = new ResponseDto<string>();
            var user = await _userManager.FindByNameAsync(email);
            if (user != null)
            {
                var isInRole = await _userManager.IsInRoleAsync(user, "Admin");
                if (isInRole)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Result = "/Account/AccountIndex";
                }
                else
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Result = "/Account/AccountIndex"; 
                }

            }
            else
            {
                if (!string.IsNullOrEmpty(firstName.Trim()) && !string.IsNullOrEmpty(email.Trim()) &&
                    !string.IsNullOrEmpty(surname.Trim()))
                {
                    var newuser = new ApplicationUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        FullName = firstName,
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true
                    };
                    await _userManager.CreateAsync(newuser);
                    await _signInManager.SignInAsync(newuser, isPersistent: false);

                    response.StatusCode = StatusCodes.Status200OK;
                    response.Result = "/Account/AccountIndex";
                }
                else
                {
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Result = "/Auth/Login";
                }

            }
            return response;
        }

       
    }
}
