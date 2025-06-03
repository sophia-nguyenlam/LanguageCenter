using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LanguageCenter.Data;
using LanguageCenter.Models;

namespace LanguageCenter.Areas.Admin.Pages.ClassSessions
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public ClassSession ClassSession { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            ClassSession = await _context.ClassSessions
                .Include(c => c.Course)
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ClassSession == null) return NotFound();

            return Page();
        }
    }
}
