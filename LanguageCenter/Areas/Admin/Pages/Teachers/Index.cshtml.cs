using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Areas.Admin.Pages.Teachers
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ViewModel chứa dữ liệu cho View
        public class TeachersViewModel
        {
            public List<TeacherProfile> TeacherProfiles { get; set; } = new();
            public List<ApplicationUser> UsersWithoutProfile { get; set; } = new();
        }

        [BindProperty]
        public TeachersViewModel TeachersData { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Load tất cả TeacherProfiles kèm User để hiển thị
            TeachersData.TeacherProfiles = await _context.TeacherProfiles
                .Include(tp => tp.User)
                .OrderBy(tp => tp.User.FullName)
                .ToListAsync();

            // Lấy tất cả user có role "Teacher"
            var teacherUsers = await _userManager.GetUsersInRoleAsync("Teacher");

            // Lấy Id các user đã có profile
            var userIdsWithProfile = TeachersData.TeacherProfiles.Select(tp => tp.UserId).ToHashSet();

            // Lọc ra các user chưa có profile
            TeachersData.UsersWithoutProfile = teacherUsers
                .Where(u => !userIdsWithProfile.Contains(u.Id))
                .OrderBy(u => u.FullName)
                .ToList();
        }
    }
}
