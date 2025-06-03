using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanguageCenter.Data;
using LanguageCenter.Models; // Giả sử đây là namespace chứa các model User, Course...
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Areas.Admin.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Tổng số user
        public int TotalUsers { get; set; }

        // Tổng số học sinh active
        public int ActiveStudents { get; set; }

        // Tổng số giáo viên
        public int TotalTeachers { get; set; }

        // Tổng số khóa học
        public int TotalCourses { get; set; }

        // Danh sách recent users (ví dụ lấy 5 user mới nhất)
        public List<UserViewModel> RecentUsers { get; set; } = new();

        // Labels cho biểu đồ tháng (tháng 1 đến 12)
        public List<string> MonthLabels { get; set; } = new()
            { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        // Số lượng đăng ký theo tháng (tháng 1-12) trong năm hiện tại
        public List<int> MonthlyRegistrations { get; set; } = new();

        public async Task OnGetAsync()
        {
            var currentYear = DateTime.UtcNow.Year;

            // Tổng số user
            TotalUsers = await _context.Users.CountAsync();

            // Tổng số học sinh active (giả sử User có Role và IsActive)
            ActiveStudents = await _context.Users
                .Where(u => u.Role == "Student" && u.IsActive)
                .CountAsync();

            // Tổng số giáo viên
            TotalTeachers = await _context.Users
                .Where(u => u.Role == "Teacher")
                .CountAsync();

            // Tổng số khóa học
            TotalCourses = await _context.Courses.CountAsync();

            // Lấy 5 user mới nhất (sắp xếp theo CreatedDate giảm dần)
            RecentUsers = await _context.Users
                .OrderByDescending(u => u.CreatedDate)
                .Take(5)
                .Select(u => new UserViewModel
                {
                    Email = u.Email,
                    CreatedDate = u.CreatedDate,
                    IsActive = u.IsActive
                })
                .ToListAsync();

            // Truy vấn số đăng ký theo tháng trong năm hiện tại, 1 query duy nhất
            var enrollments = await _context.Enrollments
                .Where(e => e.EnrollDate.Year == currentYear)
                .GroupBy(e => e.EnrollDate.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToListAsync();

            // Khởi tạo danh sách 12 tháng với giá trị 0
            MonthlyRegistrations = new List<int>(new int[12]);

            // Gán số lượng đăng ký tương ứng từng tháng
            foreach (var item in enrollments)
            {
                MonthlyRegistrations[item.Month - 1] = item.Count;
            }
        }

        public class UserViewModel
        {
            public string Email { get; set; }
            public DateTime CreatedDate { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
