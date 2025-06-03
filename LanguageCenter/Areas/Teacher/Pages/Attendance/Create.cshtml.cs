using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Areas.Teacher.Pages.Attendance
{
    [Authorize(Roles = "Teacher")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public ClassSession ClassSession { get; set; } = null!;
        public List<ApplicationUser> Students { get; set; } = new();

        [BindProperty]
        public int ClassSessionId { get; set; }

        [BindProperty]
        public List<AttendanceInputModel> Attendances { get; set; } = new();

        public class AttendanceInputModel
        {
            public string StudentId { get; set; } = null!;
            public bool IsPresent { get; set; } = true;
        }

        public async Task<IActionResult> OnGetAsync(int? sessionId)
        {
            if (sessionId == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Get the class session
            ClassSession = await _context.ClassSessions
                .Include(cs => cs.Course)
                .Include(cs => cs.Attendances)
                .FirstOrDefaultAsync(cs => cs.Id == sessionId && cs.TeacherId == user.Id);

            if (ClassSession == null) return NotFound();

            ClassSessionId = ClassSession.Id;

            // Check if attendance has already been taken for this session
            if (ClassSession.Attendances.Any())
            {
                // Redirect to edit page or show message
                TempData["AlertMessage"] = "Attendance has already been taken for this session.";
                return RedirectToPage("/Sessions/Details", new { id = ClassSessionId });
            }

            // Get all students enrolled in the course who have confirmed status
            var enrolledStudentIds = await _context.Enrollments
                .Where(e => e.CourseId == ClassSession.CourseId && e.Status == "Confirmed")
                .Select(e => e.StudentId)
                .ToListAsync();

            Students = await _context.Users
                .Where(u => enrolledStudentIds.Contains(u.Id))
                .OrderBy(u => u.FullName)
                .ToListAsync();
                
            if (!Students.Any())
            {
                // Check if there are ANY students (even in non-confirmed status)
                var anyEnrollments = await _context.Enrollments
                    .AnyAsync(e => e.CourseId == ClassSession.CourseId);
                
                if (anyEnrollments)
                {
                    TempData["AlertMessage"] = "There are enrolled students, but none with confirmed status.";
                }
                else
                {
                    TempData["AlertMessage"] = "There are no students enrolled in this course yet.";
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var classSession = await _context.ClassSessions
                .FirstOrDefaultAsync(cs => cs.Id == ClassSessionId && cs.TeacherId == user.Id);

            if (classSession == null) return NotFound();

            // Check if attendance already exists
            var existingAttendance = await _context.Attendances
                .AnyAsync(a => a.ClassSessionId == ClassSessionId);

            if (existingAttendance)
            {
                TempData["AlertMessage"] = "Attendance has already been recorded for this session.";
                return RedirectToPage("/Sessions/Details", new { id = ClassSessionId });
            }

            // Create new attendance records
            foreach (var attendance in Attendances)
            {
                _context.Attendances.Add(new Models.Attendance
                {
                    ClassSessionId = ClassSessionId,
                    StudentId = attendance.StudentId,
                    IsPresent = attendance.IsPresent
                });
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Attendance recorded successfully!";
            return RedirectToPage("/Sessions/Details", new { id = ClassSessionId });
        }
    }
}
