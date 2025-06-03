using System.ComponentModel.DataAnnotations;
using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Areas.Admin.Pages.Teachers
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
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            public int Id { get; set; }

            [Required]
            public string UserId { get; set; } = null!;

            public string Expertise { get; set; } = string.Empty;

            [Range(0, 100)]
            public int YearsOfExperience { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var profile = await _context.TeacherProfiles.FindAsync(id);

            if (profile == null) return NotFound();

            Input = new InputModel
            {
                Id = profile.Id,
                UserId = profile.UserId,
                Expertise = profile.Expertise,
                YearsOfExperience = profile.YearsOfExperience
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Input.UserId");
            if (!ModelState.IsValid) return Page();

            var profile = await _context.TeacherProfiles.FindAsync(Input.Id);
            if (profile == null) return NotFound();

            // Only update editable fields (not UserId to keep relation safe)
            profile.Expertise = Input.Expertise;
            profile.YearsOfExperience = Input.YearsOfExperience;

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
