using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEShop.Data.Repository;
using MyEShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace MyEShop.Controllers
{
    public class AccountController : Controller
    {
       private readonly IUserRepository _userRepository;
       private readonly IAccountRepository _accountRepository;

       public AccountController(IUserRepository userRepository, IAccountRepository accountRepository)
       {
           _userRepository = userRepository;
           _accountRepository = accountRepository;
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
                return View(viewModel);
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

        #region Address

        [Authorize]
        public IActionResult AddAddress()
        {
            return View(new AddAddressViewModel());
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddAddress(AddAddressViewModel viewModel)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
            if (!ModelState.IsValid)
                return View(viewModel);

            var address = new Address()
            {
                FullAddress = viewModel.FullAddress,
                Phone = viewModel.Phone,
                City = viewModel.City,
                PostalCode = viewModel.PostalCode,
                Province = viewModel.Province,
                UserId = userId
            };
            _accountRepository.AddAddress(address);
            TempData["SuccessMessage"] = "آدرس جدید با موفقیت اضافه شد.";
            return RedirectToAction("AddressList");
        }

        [Authorize]
        public IActionResult AddressList()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
            var addresses = _accountRepository.GetAddressesByUserId(userId);
            return View(addresses); // View کامل با Layout
        }

        [Authorize]
        public IActionResult EditAddress(int addressId)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
            var address = _accountRepository.GetAddressById(addressId);
            if (address == null || address.UserId != userId)
                return RedirectToAction("AddressList");

            var model = new EditAddressViewModel()
            {
                FullAddress = address.FullAddress,
                City = address.City,
                Phone = address.Phone,
                PostalCode = address.PostalCode,
                Province = address.Province
            };
            ViewBag.AddressId = addressId;
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult EditAddress(int addressId, EditAddressViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AddressId = addressId;
                return View(viewModel);
            }
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
            var address = _accountRepository.GetAddressById(addressId);
            if (address != null && address.UserId == userId)
            {
                address.City = viewModel.City;
                address.FullAddress = viewModel.FullAddress;
                address.Phone = viewModel.Phone;
                address.PostalCode = viewModel.PostalCode;
                address.Province = viewModel.Province;
                _accountRepository.UpdateAddress(address);
            }
            TempData["SuccessMessage"] = "آدرس با موفقیت ویرایش شد.";
            return RedirectToAction("AddressList");
        }

        [Authorize]
        [HttpPost]
        public IActionResult DeleteAddress(int addressId)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
            var address = _accountRepository.GetAddressById(addressId);
            if (address != null && address.UserId == userId)
            {
                _accountRepository.DeleteAddress(address);
                TempData["SuccessMessage"] = "آدرس با موفقیت حذف شد.";
            }
            return RedirectToAction("AddressList");
        }

        #endregion
    }
}
