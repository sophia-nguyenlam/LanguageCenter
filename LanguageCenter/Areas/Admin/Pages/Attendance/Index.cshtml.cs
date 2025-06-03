using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Areas.Admin.Pages.Attendance
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Models.Attendance> AttendanceRecords { get; set; } = new();
        public string? SearchTerm { get; set; }
        public string? PresentFilter { get; set; }
        public new int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((decimal)TotalRecords / PageSize);

        public async Task OnGetAsync(string? searchTerm, string? presentFilter, int page = 1)
        {
            SearchTerm = searchTerm;
            PresentFilter = presentFilter;
            Page = page;

            var query = _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.ClassSession)
                    .ThenInclude(cs => cs.Course)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(a => a.Student.FullName.Contains(searchTerm));
            }

            if (presentFilter == "present")
            {
                query = query.Where(a => a.IsPresent);
            }
            else if (presentFilter == "absent")
            {
                query = query.Where(a => !a.IsPresent);
            }

            TotalRecords = await query.CountAsync();

            AttendanceRecords = await query
                .OrderByDescending(a => a.ClassSession.StartTime)
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int id, bool newStatus)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }

            attendance.IsPresent = newStatus;
            await _context.SaveChangesAsync();

            return RedirectToPage(new { SearchTerm, PresentFilter, Page });
        }
    }
}
