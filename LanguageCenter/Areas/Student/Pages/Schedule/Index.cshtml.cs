using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Areas.Student.Pages.Schedule
{
    [Authorize(Roles = "Student")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<ClassSession> ClassSessions { get; set; } = new();
        public List<ClassSession> UpcomingSessions { get; set; } = new();
        public List<ClassSession> CompletedSessions { get; set; } = new();
        public ApplicationUser CurrentUser { get; set; } = null!;
        private Dictionary<int, bool> AttendanceCache { get; set; } = new();

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return;

            CurrentUser = user;

            // Get all enrollments for the current user
            var userEnrollments = await _context.Enrollments
                .Where(e => e.StudentId == user.Id && (e.Status == "Confirmed" || e.Status == "Completed"))
                .Select(e => e.CourseId)
                .ToListAsync();

            if (userEnrollments.Any())
            {
                // Get class sessions for enrolled courses
                ClassSessions = await _context.ClassSessions
                    .Include(cs => cs.Course)
                    .Include(cs => cs.Teacher)
                    .Where(cs => userEnrollments.Contains(cs.CourseId))
                    .OrderBy(cs => cs.StartTime)
                    .ToListAsync();

                var now = DateTime.Now;
                UpcomingSessions = ClassSessions
                    .Where(cs => cs.StartTime > now || (cs.StartTime <= now && cs.EndTime >= now))
                    .OrderBy(cs => cs.StartTime)
                    .ToList();
                CompletedSessions = ClassSessions.Where(cs => cs.EndTime < now).ToList();

                // Fetch attendance records for completed sessions
                if (CompletedSessions.Any())
                {
                    var attendances = await _context.Attendances
                        .Where(a => a.StudentId == user.Id && CompletedSessions.Select(cs => cs.Id).Contains(a.ClassSessionId))
                        .ToListAsync();

                    foreach (var attendance in attendances)
                    {
                        AttendanceCache[attendance.ClassSessionId] = attendance.IsPresent;
                    }
                }
            }
        }

        public bool? GetAttendanceForSession(int sessionId)
        {
            return AttendanceCache.TryGetValue(sessionId, out bool value) ? value : null;
        }
    }
}
