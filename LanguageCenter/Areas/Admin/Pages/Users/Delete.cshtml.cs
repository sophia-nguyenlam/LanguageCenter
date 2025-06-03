using LanguageCenter.Models;
using LanguageCenter.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LanguageCenter.Areas.Admin.Pages.Users
{
    public class DeleteModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public DeleteModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [BindProperty]
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

            User = user; // Gán User để hiển thị thông tin trên trang

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
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

            // Xóa profile Teacher hoặc Student nếu có
            if (User.TeacherProfile != null)
            {
                _context.Remove(User.TeacherProfile);
            }

            if (User.StudentProfile != null)
            {
                _context.Remove(User.StudentProfile);
            }

            await _context.SaveChangesAsync(); // Lưu các thay đổi xóa profile trước

            var result = await _userManager.DeleteAsync(User);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Failed to delete user.");
                return Page();
            }

            return RedirectToPage("Index");
        }
    }
}
