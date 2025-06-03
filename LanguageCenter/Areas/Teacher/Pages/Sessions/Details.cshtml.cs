using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Areas.Teacher.Pages.Sessions
{
    [Authorize(Roles = "Teacher")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DetailsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public ClassSession ClassSession { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            ClassSession = await _context.ClassSessions
                .Include(cs => cs.Course)
                .Include(cs => cs.Attendances)
                    .ThenInclude(a => a.Student)
                .FirstOrDefaultAsync(cs => cs.Id == id && cs.TeacherId == user.Id);

            if (ClassSession == null) return NotFound();

            return Page();
        }
    }
}
