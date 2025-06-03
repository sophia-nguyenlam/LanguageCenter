using LanguageCenter.Models;
using LanguageCenter.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LanguageCenter.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // GET: SelectRole - display role selection page
        [HttpGet]
        public IActionResult SelectRole()
        {
            var roles = new List<string> { "Admin", "Teacher", "Student" };
            return View(roles);
        }

        // POST: SelectRole - process selected role
        [HttpPost]
        public IActionResult SelectRole(string selectedRole)
        {
            if (string.IsNullOrEmpty(selectedRole))
            {
                ModelState.AddModelError("", "Please select a role.");
                var roles = new List<string> { "Admin", "Teacher", "Student" };
                return View(roles);
            }

            return RedirectToAction("Register", new { role = selectedRole });
        }

        // GET: Register - show registration form, optionally with role
        [HttpGet]
        public IActionResult Register(string role)
        {
            var model = new RegisterViewModel();
            if (!string.IsNullOrEmpty(role))
            {
                model.Role = role;
            }
            return View(model);
        }

        // POST: Register - handle user registration
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Ngăn không cho đăng ký Admin qua form
            if (model.Role == "Admin")
            {
                ModelState.AddModelError("", "Registration with Admin role is not allowed.");
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Address = model.Address,
                IsActive = true,
                Role = model.Role
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Kiểm tra role tồn tại
                if (!await _roleManager.RoleExistsAsync(model.Role))
                {
                    ModelState.AddModelError("", "The specified role does not exist.");
                    return View(model);
                }

                await _userManager.AddToRoleAsync(user, model.Role);

                await _signInManager.SignInAsync(user, isPersistent: false);

                // Chuyển về trang Home hoặc Dashboard chung
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

    }
}
