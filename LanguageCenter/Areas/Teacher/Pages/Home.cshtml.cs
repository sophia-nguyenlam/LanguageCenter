using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Areas.Teacher.Pages
{
    [Authorize(Roles = "Teacher")]
    public class HomeModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public HomeModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public ApplicationUser CurrentUser { get; set; } = null!;
        public TeacherProfile? TeacherProfile { get; set; }
        public List<ClassSession> UpcomingSessions { get; set; } = new();

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return;

            CurrentUser = user;

            // Get teacher profile
            TeacherProfile = await _context.TeacherProfiles
                .FirstOrDefaultAsync(tp => tp.UserId == user.Id);

            // Get upcoming sessions for the teacher
            var now = DateTime.Now;
            UpcomingSessions = await _context.ClassSessions
                .Include(cs => cs.Course)
                .Where(cs => cs.TeacherId == user.Id && cs.StartTime >= now)
                .OrderBy(cs => cs.StartTime)
                .Take(5)
                .ToListAsync();
        }
    }
}
