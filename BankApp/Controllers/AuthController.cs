using BankApp.Data.DTO;
using BankApp.Data.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BankApp.Core.Services.Interface;

namespace BankApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAuthService _authService;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IAuthService authService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
        }
        
        public IActionResult Register() => View(new RegisterVM());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View(registerVM);

            var user = await _userManager.FindByEmailAsync(registerVM.Email);
            if (user != null)
            {
                ModelState.AddModelError(string.Empty, "Email address is already taken.");
                return View(registerVM);
            }

            var newUser = new ApplicationUser()
            {
                FullName = registerVM.Name,
                Email = registerVM.Email,
                UserName = registerVM.Email
               
            };

            var newUserResponse = await _userManager.CreateAsync(newUser, registerVM.Password);

            if (newUserResponse.Succeeded)
            {
                return View("RegisterCompleted");
            }
            else
            {
                foreach (var error in newUserResponse.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(registerVM);
            }
        }
        public IActionResult Login() => View(new LoginVM());

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid) return View(loginVM);

            var user = await _userManager.FindByEmailAsync(loginVM.Email);
            if (user != null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
                if (passwordCheck)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("AccountIndex", "Account");
                    }
                }
                TempData["Error"] = "Wrong credentials. Please, try again!";
                return View(loginVM);
            }

            TempData["Error"] = "Wrong credentials. Please, try again!";
            return View(loginVM);
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult AccessDenied(string ReturnUrl)
        {
            return View();
        }
        //public async Task Google_Login()
        //{
        //    await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
        //    {
        //        RedirectUri = Url.Action("GoogleResponse")
        //    });
        //}


        //public async Task<IActionResult> GoogleResponse()
        //{
        //    var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //    if (result?.Succeeded == true && result?.Principal != null)
        //    {
        //        var claims = result.Principal.Claims.Select(x => new
        //        {
        //            x.Issuer,
        //            x.OriginalIssuer,
        //            x.Type,
        //            x.Value
        //        });
        //        var email = result.Principal.FindFirstValue(ClaimTypes.Email);
        //        var FirstName = result.Principal.FindFirstValue(ClaimTypes.GivenName);
        //        var Surname = result.Principal.FindFirstValue(ClaimTypes.Surname);



        //        var response = await _authService.ExternalLogin(email, FirstName, Surname);

        //        if (response.Result != null)
        //        {
        //            return Redirect(response.Result);
        //        }
        //        else
        //        {
        //            return BadRequest(response.ErrorMessages);
        //        }
        //    }

        //    return BadRequest("Unauthorized User");
        //}

    }
}

