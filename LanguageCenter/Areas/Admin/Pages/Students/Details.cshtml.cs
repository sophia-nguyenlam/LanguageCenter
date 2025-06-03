using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Areas.Admin.Pages.Students
{
    [Authorize(Roles = "Admin")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public StudentProfile? StudentProfile { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            StudentProfile = await _context.StudentProfiles
                .Include(sp => sp.User)
                .FirstOrDefaultAsync(sp => sp.Id == id);

            if (StudentProfile == null) return NotFound();

            return Page();
        }
    }
}
