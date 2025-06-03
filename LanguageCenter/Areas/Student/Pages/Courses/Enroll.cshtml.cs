using LanguageCenter.Data;
using LanguageCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Areas.Student.Pages.Courses
{
    [Authorize(Roles = "Student")]
    public class EnrollModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EnrollModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Course Course { get; set; } = null!;
        public bool IsAlreadyEnrolled { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

            if (Course == null) return NotFound();

            // Check if student is already enrolled
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var existingEnrollment = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.StudentId == user.Id && e.CourseId == id);
                
                IsAlreadyEnrolled = existingEnrollment != null;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {

            Console.WriteLine("Enroll OnPostAsync called");
            Console.WriteLine($"Course ID: {id}");

            
            if (id == null) return NotFound();

            Course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
            if (Course == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ErrorMessage = "User not found. Please log in again.";
                return Page();
            }

            // Check if already enrolled
            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == user.Id && e.CourseId == id);

            if (existingEnrollment != null)
            {
                IsAlreadyEnrolled = true;
                ErrorMessage = "You are already enrolled in this course.";
                return Page();
            }

            // Create new enrollment
            var enrollment = new Enrollment
            {
                StudentId = user.Id,
                CourseId = Course.Id,
                EnrollDate = DateTime.Now,
                Status = "Pending"
            };

            try
            {
                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();
                
                // Redirect to enrollments page with success message
                TempData["SuccessMessage"] = $"Successfully enrolled in {Course.Name}! Your enrollment is pending approval.";
                return RedirectToPage("Index"); 
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while processing your enrollment. Please try again.";
                return Page();
            }
        }
    }
}
