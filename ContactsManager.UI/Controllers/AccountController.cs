﻿using ContactsManager.Core.Domain.IdentityEntities;
using ContactsManager.Core.DTO;
using ContactsManager.Core.Enums;
using CRUD_Application.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.UI.Controllers
{
	[Route("[controller]")]
	public class AccountController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly RoleManager<ApplicationRole> _roleManager;

		public AccountController(UserManager<ApplicationUser> userManager, 
			SignInManager<ApplicationUser> signInManager,
			RoleManager<ApplicationRole> roleManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
		}

		[HttpGet]
		[Route("[action]")]
		[Authorize("NotAuthenticated")]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		[Route("[action]")]
		[Authorize("NotAuthenticated")]
		public async Task<IActionResult> Register(RegisterDTO registerDTO)
		{
			//check for validation errors
			if (!ModelState.IsValid)
			{
				ViewBag.Errors = ModelState.Values.SelectMany(temp => temp.Errors).Select(temp => temp.ErrorMessage);
				return View(registerDTO);
			}

			ApplicationUser applicationUser = new ApplicationUser()
			{
				Email = registerDTO.Email,
				PhoneNumber = registerDTO.Phone,
				UserName = registerDTO.Email,
				PersonName = registerDTO.PersonName
			};

			IdentityResult result = await _userManager.CreateAsync(applicationUser, registerDTO.Password);

			if (result.Succeeded)
			{
				//check status of radion button
				if(registerDTO.UserType == Core.Enums.UserTypeOptions.Admin)
				{
					//Create 'Admin' role
					if (await _roleManager.FindByNameAsync(UserTypeOptions.Admin.ToString()) is null)
					{
						ApplicationRole applicationRole = new ApplicationRole()
						{
							Name = UserTypeOptions.Admin.ToString(),
						};
						await _roleManager.CreateAsync(applicationRole);
					}
					//Add the new user into the 'Admin' role
					await _userManager.AddToRoleAsync(applicationUser, UserTypeOptions.Admin.ToString());
				}
				else
				{
					await _userManager.AddToRoleAsync(applicationUser, UserTypeOptions.User.ToString());
				}
				await _signInManager.SignInAsync(applicationUser, isPersistent: false);

				return RedirectToAction(nameof(PersonsController.Index), "Persons");
			}
			else
			{
				foreach(IdentityError error in result.Errors)
				{
					ModelState.AddModelError("Register", error.Description);
				}

				return View(registerDTO);
			}
		}

		[Route("[action]")]
		[HttpGet]
		[Authorize("NotAuthenticated")]
		public async Task<IActionResult> Login()
		{
			return View();
		}

		[Route("[action]")]
		[HttpPost]
		[Authorize("NotAuthenticated")]
		public async Task<IActionResult> Login(LoginDTO loginDTO, string? returnUrl)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.Errors = ModelState.Values.SelectMany(temp => temp.Errors).Select(temp => temp.ErrorMessage);
				return View(loginDTO);
			}

			var result = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, 
				isPersistent: false, lockoutOnFailure: false);

			if (result.Succeeded)
			{
				//Admin
				ApplicationUser user = await _userManager.FindByEmailAsync(loginDTO.Email);

				if (user != null)
				{
					if (await _userManager.IsInRoleAsync(user, UserTypeOptions.Admin.ToString()))
					{
						return RedirectToAction("Index", "Home", new { area = "Admin" });
					}
				}

				if(!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
				{
					return LocalRedirect(returnUrl); //so they cannot redirect to another website - not post the information there as well
				}
				return RedirectToAction(nameof(PersonsController.Index), "Persons");
			}
			else
			{
				ModelState.AddModelError("Login", "Invalid email or password");
				return View(loginDTO);
			};
		}

		[Route("[action]")]
		[Authorize]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction(nameof(PersonsController.Index), "Persons");
		}

		public async Task<IActionResult> IsEmailAlreadyRegistered(string email)
		{
			ApplicationUser user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				return Json(true);
			}
			else
			{
				return Json(false);
			}
		}
	}
}
