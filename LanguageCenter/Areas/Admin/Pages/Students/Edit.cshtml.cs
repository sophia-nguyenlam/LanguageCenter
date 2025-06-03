using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Areas.Admin.Pages.Students
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public StudentProfile StudentProfile { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            StudentProfile = await _context.StudentProfiles
                .Include(sp => sp.User)
                .FirstOrDefaultAsync(sp => sp.Id == id);

            if (StudentProfile == null) return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("StudentProfile.User");
            ModelState.Remove("StudentProfile.UserId");
            if (!ModelState.IsValid)
            {
                // Reload User for View to avoid null ref error
                StudentProfile = await _context.StudentProfiles
                    .Include(sp => sp.User)
                    .FirstOrDefaultAsync(sp => sp.Id == StudentProfile.Id);

                return Page();
            }

            var studentInDb = await _context.StudentProfiles.FindAsync(StudentProfile.Id);

            if (studentInDb == null) return NotFound();

            // Update allowed fields only
            studentInDb.EnrollmentDate = StudentProfile.EnrollmentDate;
            studentInDb.ParentContact = StudentProfile.ParentContact;

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
