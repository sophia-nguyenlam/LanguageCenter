using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LanguageCenter.Areas.Admin.Pages.ClassSessions
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ClassSession ClassSession { get; set; } = new();

        public List<SelectListItem> CourseList { get; set; } = new();
        public List<SelectListItem> TeacherList { get; set; } = new();

        private async Task LoadDropdownsAsync()
        {
            CourseList = await _context.Courses
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToListAsync();

            TeacherList = await _context.Users
                .Where(u => u.TeacherProfile != null)
                .Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.FullName
                })
                .ToListAsync();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadDropdownsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            ModelState.Remove("ClassSession.Course");
            ModelState.Remove("ClassSession.Teacher");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid!");
                await LoadDropdownsAsync(); // Load lại dropdown nếu lỗi
                return Page();
            }

            _context.ClassSessions.Add(ClassSession);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
