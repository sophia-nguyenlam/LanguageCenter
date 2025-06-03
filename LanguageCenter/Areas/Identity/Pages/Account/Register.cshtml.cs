#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using LanguageCenter.Models;

namespace LanguageCenter.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Role { get; set; }  // Get from query string

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be between {2} and {1} characters.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "Full Name")]
            public string FullName { get; set; }

            [DataType(DataType.Date)]
            [Display(Name = "Date of Birth")]
            public DateTime? DateOfBirth { get; set; }

            [Display(Name = "Gender")]
            public string Gender { get; set; }

            [Required]
            [Display(Name = "Address")]
            public string Address { get; set; }

            [Required]
            public string Role { get; set; } // Hidden field
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (string.IsNullOrEmpty(Role) || Role == "Admin" || !await _roleManager.RoleExistsAsync(Role))
            {
                return RedirectToPage("/Account/SelectRole", new { area = "Identity" });
            }

            Input = new InputModel { Role = Role };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Backup role in POST
            if (string.IsNullOrEmpty(Input.Role))
                Input.Role = Role;

            if (string.IsNullOrEmpty(Input.Role) || Input.Role == "Admin" || !await _roleManager.RoleExistsAsync(Input.Role))
            {
                ModelState.AddModelError("Input.Role", "You cannot register as Admin.");
                return Page();
            }

            if (ModelState.IsValid)
            {
                var user = CreateUser();

                user.FullName = Input.FullName;
                user.DateOfBirth = Input.DateOfBirth;
                user.Gender = Input.Gender;
                user.Address = Input.Address;
                user.AvatarUrl = null;
                user.IsActive = true;

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created.");

                    await _userManager.AddToRoleAsync(user, Input.Role);

                    // Đăng nhập luôn sau khi đăng ký thành công
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return Input.Role switch
                    {
                        "Admin" => RedirectToPage("/Admin/Dashboard"),
                        "Teacher" => RedirectToPage("/Teacher/Home"),
                        "Student" => RedirectToPage("/Student/Home"),
                        _ => LocalRedirect(returnUrl)
                    };
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Cannot create an instance of '{nameof(ApplicationUser)}'. Ensure it has a parameterless constructor.");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
                throw new NotSupportedException("This requires a user store that supports email.");
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
