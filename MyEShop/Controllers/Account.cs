using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MyEShop.Data.Repository;
using MyEShop.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;

namespace MyEShop.Controllers
{
    public class AccountController : Controller
    {
       private readonly IUserRepository _userRepository;

       public AccountController(IUserRepository userRepository)
       {
           _userRepository = userRepository;
       }

        #region  Register

        public IActionResult Register()
       {


           return View();
       }
       [HttpPost]
       public IActionResult Register(RegisterViewModel viewModel)
       {
           if (!ModelState.IsValid)
           {
               return View(viewModel);
           }

           if (_userRepository.IsExsistUserByUserName(viewModel.UserName.ToLower()))
           {
               ModelState.AddModelError("UserName","یک کاربر با این نام کاربری قبلا ثبت شده است");

               return View(viewModel);
           }

           Users user = new Users()
           {
               UserName = viewModel.UserName
               ,Password = viewModel.Password
               ,IsAdmin = false
               ,RegisterDate = DateTime.Now


           };
           _userRepository.AdUser(user);

           return View("Success",viewModel);
       }

        #endregion

        #region Login

        public IActionResult Login()
        {
            return View();

        }

        [HttpPost]
        public IActionResult Login(LoginViewModel viewModel )
        {

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var  user=_userRepository.GetUsersForLogin(viewModel.UserName, viewModel.Password);
            if (user==null)
            {
                ModelState.AddModelError("UserName","اطلاعات صحیح نیست");
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
             

            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            var properties = new AuthenticationProperties
            {
                IsPersistent = viewModel.ReMemberMe
            };

            HttpContext.SignInAsync(principal, properties);

            return Redirect($"/");

        }


        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Account/Login");

        }

        #endregion

        #region Account

        public IActionResult Account()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
           
            return View();
        }

        #endregion
    }
}
