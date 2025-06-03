using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Areas.Admin.Pages.Results
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Result Result { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Result = await _context.Results
                .Include(r => r.Course)
                .Include(r => r.Student)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (Result == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //ModelState.Remove("Input.UserId");
            // Validate Score nếu có
            if (Result.Score.HasValue && (Result.Score < 0 || Result.Score > 100))
            {
                ModelState.AddModelError("Result.Score", "Score must be between 0 and 100.");
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid. Errors:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"- {error.ErrorMessage}");
                }

                // Reload navigation properties để UI có dữ liệu hiển thị lại khi trả về form lỗi
                Result = await _context.Results
                    .Include(r => r.Course)
                    .Include(r => r.Student)
                    .FirstOrDefaultAsync(r => r.Id == Result.Id);

                if (Result == null)
                    return NotFound();

                return Page();
            }

            var existingResult = await _context.Results.FindAsync(Result.Id);

            if (existingResult == null)
                return NotFound();

            existingResult.Score = Result.Score;
            existingResult.Feedback = Result.Feedback;

            await _context.SaveChangesAsync();

            return RedirectToPage("Details", new { id = Result.Id });
        }
    }
}
