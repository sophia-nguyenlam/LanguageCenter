using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Areas.Admin.Pages.Students
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public class StudentsViewModel
        {
            public List<StudentProfile> StudentProfiles { get; set; } = new();
            public List<ApplicationUser> UsersWithoutProfile { get; set; } = new();
        }

        public StudentsViewModel StudentsData { get; set; } = new();

        public async Task OnGetAsync()
        {
            // 1. Load StudentProfiles including User navigation property
            StudentsData.StudentProfiles = await _context.StudentProfiles
                .Include(sp => sp.User)
                .ToListAsync();

            // 2. Load all users with role "Student"
            var studentUsers = await _userManager.GetUsersInRoleAsync("Student");

            // 3. Find users who do NOT have a StudentProfile yet
            var usersWithProfileIds = StudentsData.StudentProfiles.Select(sp => sp.UserId).ToHashSet();

            StudentsData.UsersWithoutProfile = studentUsers
                .Where(u => !usersWithProfileIds.Contains(u.Id))
                .OrderBy(u => u.FullName)
                .ToList();
        }
    }
}
