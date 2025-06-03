using LanguageCenter.Models;
using LanguageCenter.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace LanguageCenter.Areas.Student.Pages.HomeModel
{
    [Authorize(Roles = "Student")]
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
        public List<Enrollment> Enrollments { get; set; } = new();

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return;

            CurrentUser = user;

            Enrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.StudentId == user.Id)
                .ToListAsync();
        }
    }
}
