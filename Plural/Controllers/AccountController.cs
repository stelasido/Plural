using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Plural.Data.Entities;
using Plural.ViewModels;

namespace Plural.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<StoreUser> _sigInManager;
        private readonly UserManager<StoreUser> _userManager;
        private readonly IConfiguration _config;
        public AccountController(ILogger<AccountController> logger, SignInManager<StoreUser> sigInManager,
            UserManager<StoreUser> userManager, IConfiguration config)
        {
            _config = config;
            _logger = logger;
            _userManager = userManager;
            _sigInManager = sigInManager;
        }


        public IActionResult Login()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "App");
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _sigInManager.PasswordSignInAsync(model.Username, 
                    model.Password, model.RememberMe, true);

                if(result.Succeeded)
                {
                    if(Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"].First());
                    }
                    else
                    {
                        return RedirectToAction("Shop", "App");
                    }
                }
            }
            ModelState.AddModelError("", "FFFAILED LOGIN");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _sigInManager.SignOutAsync();
            return RedirectToAction("Index", "App");
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody]LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                if(user != null)
                {
                    var result = await _sigInManager.CheckPasswordSignInAsync(user, model.Password, false);

                    if(result.Succeeded)
                    {
                        // CREATE A TOKEN
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                            _config["Tokens:Issuer"],
                            _config["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddMinutes(3000),
                            signingCredentials: creds
                            );

                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo

                        };
                        return Created("", results);
                    }
                }
            }
            return BadRequest();
        }
    }
}