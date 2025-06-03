using System.Threading.Tasks;
using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Areas.Admin.Pages.Courses
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Course Input { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound();

            Input = course;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var courseToUpdate = await _context.Courses.FindAsync(Input.Id);

            if (courseToUpdate == null)
                return NotFound();

            // C?p nh?t c�c tr??ng c?n s?a
            courseToUpdate.Name = Input.Name;
            courseToUpdate.Description = Input.Description;
            courseToUpdate.DurationInWeeks = Input.DurationInWeeks;
            courseToUpdate.TuitionFee = Input.TuitionFee;
            courseToUpdate.Level = Input.Level;
            courseToUpdate.Language = Input.Language;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // X? l� l?i n?u c?n, v� d? log ho?c hi?n th? th�ng b�o
                ModelState.AddModelError(string.Empty, "Unable to save changes. Please try again.");
                return Page();
            }

            // Redirect v? trang danh s�ch ho?c chi ti?t t�y �
            return RedirectToPage("./Index");
        }
    }
}
