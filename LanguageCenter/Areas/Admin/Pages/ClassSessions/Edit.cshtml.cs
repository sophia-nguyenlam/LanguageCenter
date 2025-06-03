using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LanguageCenter.Data;
using LanguageCenter.Models;

namespace LanguageCenter.Areas.Admin.Pages.ClassSessions
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ClassSession ClassSession { get; set; } = null!;

        public SelectList CourseList { get; set; } = null!;
        public SelectList TeacherList { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            ClassSession = await _context.ClassSessions
                .Include(c => c.Course)
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ClassSession == null) return NotFound();

            CourseList = new SelectList(await _context.Courses.ToListAsync(), "Id", "Name");
            TeacherList = new SelectList(await _context.Users.Where(u => u.Role == "Teacher").ToListAsync(), "Id", "FullName");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                CourseList = new SelectList(await _context.Courses.ToListAsync(), "Id", "Name");
                TeacherList = new SelectList(await _context.Users.Where(u => u.Role == "Teacher").ToListAsync(), "Id", "FullName");
                return Page();
            }

            _context.Attach(ClassSession).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ClassSessions.Any(e => e.Id == ClassSession.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("Details", new { id = ClassSession.Id });
        }
    }
}
