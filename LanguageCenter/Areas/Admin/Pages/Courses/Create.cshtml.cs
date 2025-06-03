using LanguageCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LanguageCenter.Areas.Admin.Pages.Courses
{
    public class CreateModel : PageModel
    {
        private readonly LanguageCenter.Data.ApplicationDbContext _context;

        public CreateModel(LanguageCenter.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Course Course { get; set; } = default!;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Courses.Add(Course);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
