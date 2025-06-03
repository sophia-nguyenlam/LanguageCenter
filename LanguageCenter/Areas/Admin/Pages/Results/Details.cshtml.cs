using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Areas.Admin.Pages.Results
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Result Result { get; set; } = null!;
        public StudentProfile? StudentProfile { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var result = await _context.Results
                .Include(r => r.Student)
                .Include(r => r.Course)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (result == null)
                return NotFound();

            Result = result;

            StudentProfile = await _context.StudentProfiles
                .FirstOrDefaultAsync(sp => sp.UserId == result.StudentId);

            return Page();
        }
    }
}
