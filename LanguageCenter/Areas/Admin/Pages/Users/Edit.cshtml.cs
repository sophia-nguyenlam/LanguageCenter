using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace LanguageCenter.Areas.Admin.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public EditModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required]
            public string Id { get; set; } = default!;

            [Display(Name = "Username")]
            public string UserName { get; set; } = default!;

            [Required, EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = default!;

            [Required, StringLength(100)]
            [Display(Name = "Full Name")]
            public string FullName { get; set; } = default!;

            [Display(Name = "Role")]
            public string Role { get; set; } = default!;

            [Display(Name = "Gender")]
            public string? Gender { get; set; }

            [Display(Name = "Date of Birth")]
            [DataType(DataType.Date)]
            public DateTime? DateOfBirth { get; set; }

            [StringLength(200)]
            [Display(Name = "Address")]
            public string? Address { get; set; }

            [Display(Name = "Active")]
            public bool IsActive { get; set; }

            public TeacherProfileInputModel TeacherProfile { get; set; } = new();
            public StudentProfileInputModel StudentProfile { get; set; } = new();
        }

        public class TeacherProfileInputModel
        {
            [StringLength(200)]
            [Display(Name = "Expertise")]
            public string? Expertise { get; set; }

            [Range(0, 100)]
            [Display(Name = "Years of Experience")]
            public int? YearsOfExperience { get; set; }
        }

        public class StudentProfileInputModel
        {
            [Display(Name = "Enrollment Date")]
            [DataType(DataType.Date)]
            public DateTime? EnrollmentDate { get; set; }

            [StringLength(100)]
            [Display(Name = "Parent Contact")]
            public string? ParentContact { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.Users
                .Include(u => u.TeacherProfile)
                .Include(u => u.StudentProfile)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            Input = new InputModel
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                FullName = user.FullName!,
                Role = user.Role!,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                Address = user.Address,
                IsActive = user.IsActive
            };

            if (user.Role == "Teacher" && user.TeacherProfile != null)
            {
                Input.TeacherProfile = new TeacherProfileInputModel
                {
                    Expertise = user.TeacherProfile.Expertise,
                    YearsOfExperience = user.TeacherProfile.YearsOfExperience
                };
            }
            else if (user.Role == "Student" && user.StudentProfile != null)
            {
                Input.StudentProfile = new StudentProfileInputModel
                {
                    EnrollmentDate = user.StudentProfile.EnrollmentDate,
                    ParentContact = user.StudentProfile.ParentContact
                };
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.Users
                .Include(u => u.TeacherProfile)
                .Include(u => u.StudentProfile)
                .FirstOrDefaultAsync(u => u.Id == Input.Id);

            if (user == null)
                return NotFound();

            // Update main user info (role and username are readonly)
            user.Email = Input.Email;
            user.FullName = Input.FullName;
            user.Gender = Input.Gender ?? string.Empty;
            user.DateOfBirth = Input.DateOfBirth;
            user.Address = Input.Address ?? string.Empty;
            user.IsActive = Input.IsActive;

            // Update TeacherProfile
            if (user.Role == "Teacher")
            {
                if (user.TeacherProfile == null)
                {
                    user.TeacherProfile = new TeacherProfile();
                    _context.TeacherProfiles.Add(user.TeacherProfile);
                }
                if (Input.TeacherProfile.Expertise != null)
                {
                    user.TeacherProfile.Expertise = Input.TeacherProfile.Expertise;
                }
                user.TeacherProfile.YearsOfExperience = Input.TeacherProfile.YearsOfExperience ?? 0;
            }
            else if (user.Role == "Student")
            {
                if (user.StudentProfile == null)
                {
                    user.StudentProfile = new StudentProfile();
                    _context.StudentProfiles.Add(user.StudentProfile);
                }

                user.StudentProfile.EnrollmentDate = Input.StudentProfile.EnrollmentDate ?? DateTime.MinValue;
                user.StudentProfile.ParentContact = Input.StudentProfile.ParentContact;
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError(string.Empty, err.Description);

                return Page();
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("Details", new { id = user.Id });
        }
    }
}


