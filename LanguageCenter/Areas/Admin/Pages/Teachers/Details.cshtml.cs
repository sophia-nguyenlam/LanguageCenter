using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LanguageCenter.Data;
using LanguageCenter.Models;

namespace LanguageCenter.Areas.Admin.Pages.Teachers
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public TeacherProfile? TeacherProfile { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            TeacherProfile = await _context.TeacherProfiles
                                .Include(t => t.User)
                                .FirstOrDefaultAsync(m => m.Id == id);

            if (TeacherProfile == null) return NotFound();

            return Page();
        }
    }
}
