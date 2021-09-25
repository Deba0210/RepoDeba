﻿using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class IdentityHome : Controller
    {
        private UserManager<IdentityUser> _userManager;
        private SignInManager<IdentityUser> _signInManager;
        public IdentityHome(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult NewUserRegistration()
        {
            identityRegisterVM regVm = new identityRegisterVM();
            regVm.Title = "New User Registration";
            //regVm.DateOfBirth = DateTime.Now;
            //List<string> lstGender = new List<string> { "--Select--", "M", "F" };
            //ViewBag.Genders = new SelectList(lstGender);
            return View(regVm);
        }

        [HttpPost]
        public async Task<IActionResult> NewUserRegistration(identityRegisterVM user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var iUser = new IdentityUser();
                    iUser.UserName = user.UserName;
                    iUser.Email = user.EmailId;
                    var result = await _userManager.CreateAsync(iUser, user.UserPassword);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(iUser, isPersistent: false);
                        return RedirectToAction("ShowUsers", "Home");
                    }
                    foreach (var e in result.Errors)
                    {
                        ModelState.AddModelError("", e.Description);
                    }
                }
                return View(user);
            }
            catch (Exception ex)
            {

                return RedirectToAction("UserError");
            }
        }

        [HttpGet]
        public IActionResult LogIn()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {

                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(LoginVm user)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(user.UserName, user.UserPassword, user.RememeberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Id, Password combination doesn't match!");

            }
            return View(user);

        }

        public async Task<IActionResult> LogOut(LoginVm user)
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");

        }


    }
}
