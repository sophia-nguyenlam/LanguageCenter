using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Areas.Teacher.Pages.Sessions
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

        public List<ClassSession> Sessions { get; set; } = new();
        
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? TimeFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public new int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalSessions { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var query = _context.ClassSessions
                .Include(cs => cs.Course)
                .Include(cs => cs.Attendances)
                .Where(cs => cs.TeacherId == user.Id);

            // Apply search filters
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(cs => cs.Course.Name.Contains(SearchTerm));
            }

            var now = DateTime.Now;
            if (TimeFilter == "upcoming")
            {
                query = query.Where(cs => cs.StartTime > now);
            }
            else if (TimeFilter == "past")
            {
                query = query.Where(cs => cs.EndTime < now);
            }
            else if (TimeFilter == "today")
            {
                var today = now.Date;
                var tomorrow = today.AddDays(1);
                query = query.Where(cs => cs.StartTime >= today && cs.StartTime < tomorrow);
            }

            // Get total count for pagination
            TotalSessions = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalSessions / (double)PageSize);

            // Apply pagination
            Sessions = await query
                .OrderBy(cs => cs.StartTime)
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return Page();
        }
    }
}
