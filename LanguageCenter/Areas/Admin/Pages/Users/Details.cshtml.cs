using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LanguageCenter.Areas.Admin.Pages.Users
{
    public class DetailsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DetailsModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // Hide base User property to avoid confusion
        public new ApplicationUser User { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.Users
                .Include(u => u.TeacherProfile)
                .Include(u => u.StudentProfile)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            User = user;
            return Page();
        }
    }
}
