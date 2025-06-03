using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Areas.Admin.Pages.Enrollments
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Enrollment Enrollment { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (Enrollment == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Enrollment == null || Enrollment.Id == 0)
            {
                return NotFound();
            }

            var enrollmentToDelete = await _context.Enrollments.FindAsync(Enrollment.Id);

            if (enrollmentToDelete == null)
            {
                return NotFound();
            }

            _context.Enrollments.Remove(enrollmentToDelete);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
