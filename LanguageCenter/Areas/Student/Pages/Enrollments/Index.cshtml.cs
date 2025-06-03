using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Areas.Student.Pages.Enrollments
{
    [Authorize(Roles = "Student")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Enrollment> Enrollments { get; set; } = new();
        public ApplicationUser CurrentUser { get; set; } = null!;

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return;

            CurrentUser = user;

            Enrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.StudentId == user.Id)
                .OrderByDescending(e => e.EnrollDate)
                .ToListAsync();
        }
    }
}
