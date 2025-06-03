using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Areas.Admin.Pages.Students
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public StudentProfile StudentProfile { get; set; } = new();

        // List users who have role "Student" but do NOT have StudentProfile yet
        public List<ApplicationUser> UsersWithoutProfile { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var studentUsers = await _userManager.GetUsersInRoleAsync("Student");

            var existingProfileUserIds = await _context.StudentProfiles
                .Select(sp => sp.UserId)
                .ToListAsync();

            UsersWithoutProfile = studentUsers
                .Where(u => !existingProfileUserIds.Contains(u.Id))
                .OrderBy(u => u.FullName)
                .ToList();

            if (!UsersWithoutProfile.Any())
            {
                // No users to create profile for
                TempData["ErrorMessage"] = "All users with Student role already have profiles.";
                return RedirectToPage("./Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Reload UsersWithoutProfile for redisplay
                var studentUsers = await _userManager.GetUsersInRoleAsync("Student");
                var existingProfileUserIds = await _context.StudentProfiles
                    .Select(sp => sp.UserId)
                    .ToListAsync();
                UsersWithoutProfile = studentUsers
                    .Where(u => !existingProfileUserIds.Contains(u.Id))
                    .OrderBy(u => u.FullName)
                    .ToList();

                return Page();
            }

            // Check UserId valid and not have profile yet
            if (await _context.StudentProfiles.AnyAsync(sp => sp.UserId == StudentProfile.UserId))
            {
                ModelState.AddModelError("StudentProfile.UserId", "This user already has a student profile.");
                return Page();
            }

            StudentProfile.EnrollmentDate = DateTime.UtcNow;

            _context.StudentProfiles.Add(StudentProfile);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
