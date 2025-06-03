using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Areas.Admin.Pages.Results
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Result Result { get; set; } = new Result();

        public SelectList StudentList { get; set; } = default!;
        public SelectList CourseList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            // Lọc chỉ user có role là "Student"
            var students = await _context.Users
                .Where(u => u.Role == UserRoles.Student)
                .Select(u => new
                {
                    u.Id,
                    Name = u.FullName + " (" + u.Email + ")"
                })
                .ToListAsync();

            StudentList = new(students, "Id", "Name");
            CourseList = new SelectList(await _context.Courses.ToListAsync(), "Id", "Name");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("===========================================");
            Console.WriteLine("OnPostAsync called!");
            Console.WriteLine($"Result.StudentId: {Result?.StudentId}");
            Console.WriteLine($"Result.CourseId: {Result?.CourseId}");
            Console.WriteLine($"Result.Score: {Result?.Score}");
            Console.WriteLine($"Result.Feedback: {Result?.Feedback}");
            Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
            Console.WriteLine("===========================================");
            ModelState.Remove("Result.Course");
            ModelState.Remove("Result.Student");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid. Errors:");
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"Key: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }

                // Reload data for dropdowns
                var students = await _context.Users
                    .Where(u => u.Role == UserRoles.Student)
                    .Select(u => new { u.Id, Name = u.FullName + " (" + u.Email + ")" })
                    .ToListAsync();

                StudentList = new SelectList(students, "Id", "Name");
                CourseList = new SelectList(await _context.Courses.ToListAsync(), "Id", "Title");

                return Page();
            }

            try
            {
                Console.WriteLine("Attempting to save result...");
                _context.Results.Add(Result);
                await _context.SaveChangesAsync();
                Console.WriteLine("Result saved successfully!");
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving result: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return Page();
            }
        }
    }
}
