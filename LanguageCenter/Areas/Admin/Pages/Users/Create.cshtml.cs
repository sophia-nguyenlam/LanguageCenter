using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System;

namespace LanguageCenter.Areas.Admin.Pages.Users
{
    public class CreateModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public CreateModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required]
            [Display(Name = "Username")]
            public string UserName { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
            public string Password { get; set; } = string.Empty;

            [Required]
            [Display(Name = "Role")]
            public string Role { get; set; } = string.Empty;

            [Required]
            [Display(Name = "Full Name")]
            public string FullName { get; set; } = string.Empty;

            public string Gender { get; set; } = string.Empty;

            [DataType(DataType.Date)]
            [Display(Name = "Date of Birth")]
            public DateTime? DateOfBirth { get; set; }

            public string Address { get; set; } = string.Empty;

            [Display(Name = "Active")]
            public bool IsActive { get; set; } = true;
        }

        public void OnGet()
        {
            // Optionally initialize default values
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Validate role
            var allowedRoles = new[] { "Admin", "Teacher", "Student" };
            if (Array.IndexOf(allowedRoles, Input.Role) < 0)
            {
                ModelState.AddModelError("Input.Role", "Invalid role selected.");
                return Page();
            }

            // Check if username or email already exists
            if (await _userManager.FindByNameAsync(Input.UserName) != null)
            {
                ModelState.AddModelError("Input.UserName", "Username already taken.");
                return Page();
            }
            if (await _userManager.FindByEmailAsync(Input.Email) != null)
            {
                ModelState.AddModelError("Input.Email", "Email already registered.");
                return Page();
            }

            // Create new ApplicationUser instance with data from Input
            var user = new ApplicationUser
            {
                UserName = Input.UserName,
                Email = Input.Email,
                FullName = Input.FullName,
                Gender = Input.Gender,
                DateOfBirth = Input.DateOfBirth,
                Address = Input.Address,
                IsActive = Input.IsActive,
                Role = Input.Role,
                CreatedDate = DateTime.UtcNow,
            };

            // Create user with password
            var result = await _userManager.CreateAsync(user, Input.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }

            // Create role if not exist
            if (!await _roleManager.RoleExistsAsync(Input.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole(Input.Role));
            }

            // Add user to role
            await _userManager.AddToRoleAsync(user, Input.Role);

            // Create related profile based on Role
            if (Input.Role == "Teacher")
            {
                var teacherProfile = new TeacherProfile
                {
                    UserId = user.Id,
                    Expertise = string.Empty,
                    YearsOfExperience = 0
                };
                _context.TeacherProfiles.Add(teacherProfile);
                await _context.SaveChangesAsync();
            }
            else if (Input.Role == "Student")
            {
                var studentProfile = new StudentProfile
                {
                    UserId = user.Id,
                    EnrollmentDate = DateTime.Now,
                    ParentContact = null
                };
                _context.StudentProfiles.Add(studentProfile);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Index");
        }
    }
}
