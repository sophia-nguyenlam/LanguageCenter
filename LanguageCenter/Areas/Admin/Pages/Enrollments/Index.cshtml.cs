// Index.cshtml.cs
using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Areas.Admin.Pages.Enrollments
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Enrollment> Enrollments { get; set; } = new();

        public string? SearchTerm { get; set; }
        public string? StatusFilter { get; set; }
        public new int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((decimal)TotalRecords / PageSize);

        public async Task OnGetAsync(string? searchTerm, string? statusFilter, int page = 1)
        {
            SearchTerm = searchTerm;
            StatusFilter = statusFilter;
            Page = page;

            var query = _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(e => e.Student.FullName.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                query = query.Where(e => e.Status == statusFilter);
            }

            TotalRecords = await query.CountAsync();

            Enrollments = await query
                .OrderByDescending(e => e.EnrollDate)
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int id, string newStatus)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
                return NotFound();

            enrollment.Status = newStatus;
            await _context.SaveChangesAsync();

            return RedirectToPage(new { SearchTerm, StatusFilter, Page });
        }
    }
}
