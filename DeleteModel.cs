using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Pages.ClassSessions
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
            ClassSession = new ClassSession(); // Initialize the property to avoid nullability issues
        }

        [BindProperty]
        public ClassSession ClassSession { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            ClassSession = await _context.ClassSessions
                .Include(cs => cs.Course)
                .Include(cs => cs.Teacher)
                .FirstOrDefaultAsync(cs => cs.Id == id);

            if (ClassSession == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var session = await _context.ClassSessions.FindAsync(id);

            if (session != null)
            {
                _context.ClassSessions.Remove(session);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
