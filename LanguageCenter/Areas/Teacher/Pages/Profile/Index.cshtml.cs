using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Areas.Teacher.Pages.Profile
{
    [Authorize(Roles = "Teacher")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public ApplicationUser CurrentUser { get; set; } = default!;

        [BindProperty]
        public TeacherProfile TeacherProfile { get; set; } = default!;

        [BindProperty]
        public string Address { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Load the user with related teacher profile
            CurrentUser = await _context.Users
                .Include(u => u.TeacherProfile)
                .FirstOrDefaultAsync(u => u.Id == user.Id) ?? user;

            TeacherProfile = CurrentUser.TeacherProfile ?? new TeacherProfile { UserId = user.Id };
            Address = CurrentUser.Address;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Load the current user and teacher profile
            var dbUser = await _context.Users
                .Include(u => u.TeacherProfile)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            if (dbUser == null)
            {
                return NotFound();
            }

            // Update user address
            dbUser.Address = Address;

            // Update or create teacher profile
            if (dbUser.TeacherProfile == null)
            {
                dbUser.TeacherProfile = new TeacherProfile
                {
                    UserId = user.Id,
                    Expertise = TeacherProfile.Expertise,
                    YearsOfExperience = TeacherProfile.YearsOfExperience
                };
            }
            else
            {
                dbUser.TeacherProfile.Expertise = TeacherProfile.Expertise;
                dbUser.TeacherProfile.YearsOfExperience = TeacherProfile.YearsOfExperience;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Profile updated successfully!";

            return RedirectToPage();
        }
    }
}
