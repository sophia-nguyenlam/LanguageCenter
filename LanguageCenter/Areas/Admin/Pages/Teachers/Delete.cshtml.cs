using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Areas.Admin.Pages.Teachers
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TeacherProfile TeacherProfile { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            TeacherProfile = await _context.TeacherProfiles
                .Include(tp => tp.User)
                .FirstOrDefaultAsync(tp => tp.Id == id);

            if (TeacherProfile == null) return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null) return NotFound();

            var profile = await _context.TeacherProfiles.FindAsync(id);

            if (profile != null)
            {
                _context.TeacherProfiles.Remove(profile);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
