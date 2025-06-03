using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LanguageCenter.Areas.Teacher.Pages.Attendance
{
    [Authorize(Roles = "Teacher")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<LanguageCenter.Models.Attendance> AttendanceRecords { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? PresentFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public new int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        
        // Attendance Statistics
        public int OverallAttendanceRate { get; set; }
        public int TotalPresent { get; set; }
        public int TotalAbsent { get; set; }
        public int TotalSessions { get; set; }
        public Dictionary<string, int> CourseAttendanceRates { get; set; } = new();
        public Dictionary<string, int> StudentAbsenceRates { get; set; } = new();

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return;

            // Get sessions taught by this teacher
            var teacherSessionIds = await _context.ClassSessions
                .Where(cs => cs.TeacherId == user.Id)
                .Select(cs => cs.Id)
                .ToListAsync();

            // Check if there are any sessions at all
            if (!teacherSessionIds.Any())
            {
                // If no sessions found, set appropriate defaults for statistics
                OverallAttendanceRate = 0;
                TotalPresent = 0;
                TotalAbsent = 0;
                TotalSessions = 0;
                return;
            }

            // Query attendance records for those sessions
            var query = _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.ClassSession)
                    .ThenInclude(cs => cs.Course)
                .Where(a => teacherSessionIds.Contains(a.ClassSessionId));

            // Calculate attendance statistics for this teacher
            await CalculateAttendanceStatisticsAsync(teacherSessionIds);

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(a => a.Student.FullName.Contains(SearchTerm));
            }

            if (PresentFilter == "present")
            {
                query = query.Where(a => a.IsPresent);
            }
            else if (PresentFilter == "absent")
            {
                query = query.Where(a => !a.IsPresent);
            }

            TotalRecords = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalRecords / (double)PageSize);

            AttendanceRecords = await query
                .OrderByDescending(a => a.ClassSession.StartTime)
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
        
        private async Task CalculateAttendanceStatisticsAsync(List<int> teacherSessionIds)
        {
            // Default values in case there are no records
            TotalPresent = 0;
            TotalAbsent = 0;
            OverallAttendanceRate = 0;
            TotalSessions = 0;
            
            // If no sessions, return with default values
            if (!teacherSessionIds.Any())
            {
                return;
            }
            
            // Get current month's data for statistics
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
            
            // Get all attendance records for this teacher's sessions
            var allAttendanceRecords = await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.ClassSession)
                    .ThenInclude(cs => cs.Course)
                .Where(a => teacherSessionIds.Contains(a.ClassSessionId))
                .Where(a => a.ClassSession.StartTime >= startOfMonth && a.ClassSession.StartTime <= endOfMonth)
                .ToListAsync();
                
            // Calculate overall attendance rate
            TotalPresent = allAttendanceRecords.Count(a => a.IsPresent);
            TotalAbsent = allAttendanceRecords.Count(a => !a.IsPresent);
            int totalAttendance = TotalPresent + TotalAbsent;
            
            OverallAttendanceRate = totalAttendance > 0 
                ? (int)Math.Round((double)TotalPresent / totalAttendance * 100) 
                : 0;
                
            // Count total sessions taught this month
            TotalSessions = await _context.ClassSessions
                .CountAsync(cs => teacherSessionIds.Contains(cs.Id) && 
                            cs.StartTime >= startOfMonth && 
                            cs.StartTime <= endOfMonth);
                
            // Calculate attendance rates by course
            var courseGroups = allAttendanceRecords
                .GroupBy(a => a.ClassSession.Course.Name);
                
            CourseAttendanceRates = new Dictionary<string, int>();
            foreach (var course in courseGroups)
            {
                int presentCount = course.Count(a => a.IsPresent);
                int totalCount = course.Count();
                int rate = totalCount > 0 ? (int)Math.Round((double)presentCount / totalCount * 100) : 0;
                CourseAttendanceRates.Add(course.Key, rate);
            }
            
            // Calculate student absence rates (for students with at least one absence)
            var studentGroups = allAttendanceRecords
                .GroupBy(a => a.Student.FullName);
                
            StudentAbsenceRates = new Dictionary<string, int>();
            foreach (var student in studentGroups)
            {
                int absentCount = student.Count(a => !a.IsPresent);
                int totalCount = student.Count();
                if (absentCount > 0)
                {
                    int rate = (int)Math.Round((double)absentCount / totalCount * 100);
                    StudentAbsenceRates.Add(student.Key, rate);
                }
            }
            
            // Sort by highest absence rate
            StudentAbsenceRates = StudentAbsenceRates
                .OrderByDescending(pair => pair.Value)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int id, bool newStatus)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToPage("/Account/Login", new { area = "Identity" });

            var attendance = await _context.Attendances
                .Include(a => a.ClassSession)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attendance == null)
                return NotFound();

            // Verify this teacher is allowed to update this attendance
            var classSession = await _context.ClassSessions
                .FirstOrDefaultAsync(cs => cs.Id == attendance.ClassSessionId && cs.TeacherId == user.Id);

            if (classSession == null)
                return Forbid();

            // Update attendance status
            attendance.IsPresent = newStatus;
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
        
        public async Task<IActionResult> OnPostBulkUpdateAsync(List<int> selectedIds, string action)
        {
            if (selectedIds == null || !selectedIds.Any())
                return RedirectToPage();
                
            var user = await _userManager.GetUserAsync(User);
            if (user == null) 
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            
            // Get teacher's session IDs to validate permissions
            var teacherSessionIds = await _context.ClassSessions
                .Where(cs => cs.TeacherId == user.Id)
                .Select(cs => cs.Id)
                .ToListAsync();
                
            // Get selected attendance records that belong to this teacher's sessions
            var attendances = await _context.Attendances
                .Include(a => a.ClassSession)
                .Where(a => selectedIds.Contains(a.Id))
                .Where(a => teacherSessionIds.Contains(a.ClassSessionId))
                .ToListAsync();
            
            bool newStatus = action == "markPresent";
            
            // Update all selected attendance records
            foreach (var attendance in attendances)
            {
                attendance.IsPresent = newStatus;
            }
            
            await _context.SaveChangesAsync();
            
            return RedirectToPage();
        }
    }
}
