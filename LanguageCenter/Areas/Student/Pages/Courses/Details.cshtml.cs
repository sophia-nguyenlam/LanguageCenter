using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Areas.Student.Pages.Courses
{
    // [Authorize(Roles = "Student")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DetailsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Course Course { get; set; } = null!;
        public bool IsAlreadyEnrolled { get; set; }
        public string? EnrollmentStatus { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {

            Console.WriteLine($"DetailsModel.OnGetAsync called with id: {id}");
            if (id == null) return NotFound();

            Course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
            if (Course == null) return NotFound();

            // Check if student is already enrolled
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var existingEnrollment = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.StudentId == user.Id && e.CourseId == id);
                
                if (existingEnrollment != null)
                {
                    IsAlreadyEnrolled = true;
                    EnrollmentStatus = existingEnrollment.Status;
                }
            }

            return Page();
        }
    }
}
