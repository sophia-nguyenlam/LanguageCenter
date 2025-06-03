using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Areas.Student.Pages.Results
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

        public List<Result> Results { get; set; } = new();
        public ApplicationUser CurrentUser { get; set; } = null!;
        public double AverageScore { get; set; }
        public int CompletedCourses { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return;

            CurrentUser = user;

            Results = await _context.Results
                .Include(r => r.Course)
                .Where(r => r.StudentId == user.Id)
                .OrderByDescending(r => r.Course.Name)
                .ToListAsync();

            if (Results.Any())
            {
                var scoresWithValues = Results.Where(r => r.Score.HasValue).ToList();
                if (scoresWithValues.Any())
                {
                    AverageScore = scoresWithValues.Average(r => r.Score!.Value);
                }
                CompletedCourses = Results.Count;
            }
        }
    }
}
