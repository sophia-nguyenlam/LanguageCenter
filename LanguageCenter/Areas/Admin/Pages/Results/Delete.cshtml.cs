using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Pages.Results
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Result Result { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Result = await _context.Results
                .Include(r => r.Student)
                .Include(r => r.Course)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (Result == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var resultToDelete = await _context.Results.FindAsync(id);

            if (resultToDelete == null)
            {
                return NotFound();
            }

            _context.Results.Remove(resultToDelete);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
