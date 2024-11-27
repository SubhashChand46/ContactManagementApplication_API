using CMS.JwtGenerator;
using Contact_Management_Application.Models;
using Contact_Management_Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Principal;

namespace Contact_Management_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountLoginFile _accountLogin;
        private readonly IJwtAuthManager _jwtAuthManager;
        public AccountController(AccountLoginFile accountLogin, IJwtAuthManager jwtAuthManager)
        {
            _accountLogin = accountLogin;
            _jwtAuthManager = jwtAuthManager;
           
        }
        [HttpPost("Login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] Account account)
        {
            if (string.IsNullOrWhiteSpace(account.Email) || string.IsNullOrWhiteSpace(account.Password))
            {
                return BadRequest("All fields are required.");
            }
            var accounts = _accountLogin.GetAllAccounts();
            var existingAccount = accounts.FirstOrDefault(a =>
                a.Email.Equals(account.Email, StringComparison.OrdinalIgnoreCase) &&
                a.Password == account.Password);
            if (existingAccount == null)
            {
                return NotFound(new { message = "User not found. Please register.", status = 0 });
            }
            else
            {
                Claim[] claims = new[]
                {
                  new Claim(ClaimTypes.Email, existingAccount.Email),
                };
                var token = _jwtAuthManager.GenerateTokens(account.Email, claims, DateTime.Now);
                return Ok(new { message = "Login successfully.", account = existingAccount, status = 1, token });
            }
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] Account account)
        {
            if (string.IsNullOrWhiteSpace(account.Email) || string.IsNullOrWhiteSpace(account.Password))
            {
                return BadRequest("All fields are required.");
            }
            var accounts = _accountLogin.GetAllAccounts();
            if (accounts.Any(a => a.Email.Equals(account.Email, StringComparison.OrdinalIgnoreCase)))
            {
                return Conflict(new { message = "User already exists.", status = 0 });
            }
            account.Id = accounts.Count > 0 ? accounts.Max(c => c.Id) + 1 : 1;
            accounts.Add(account);
            _accountLogin.SaveAccounts(accounts);
            return Ok(new { message = "Account successfully created.", status = 1 });
        }

    }
}
