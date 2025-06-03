using LanguageCenter.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<TeacherProfile> TeacherProfiles { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<ClassSession> ClassSessions { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Result> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 1-1: ApplicationUser <-> StudentProfile
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.StudentProfile)
                .WithOne(sp => sp.User)
                .HasForeignKey<StudentProfile>(sp => sp.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Tắt cascade delete

            // 1-1: ApplicationUser <-> TeacherProfile
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.TeacherProfile)
                .WithOne(tp => tp.User)
                .HasForeignKey<TeacherProfile>(tp => tp.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Tắt cascade delete

            // 1-n: ApplicationUser (Teacher) <-> ClassSession
            builder.Entity<ClassSession>()
                .HasOne(cs => cs.Teacher)
                .WithMany(t => t.ClassSessionsAsTeacher)
                .HasForeignKey(cs => cs.TeacherId)
                .OnDelete(DeleteBehavior.Restrict); // Tắt cascade delete

            // 1-n: Course <-> ClassSession
            builder.Entity<ClassSession>()
                .HasOne(cs => cs.Course)
                .WithMany(c => c.ClassSessions)
                .HasForeignKey(cs => cs.CourseId)
                .OnDelete(DeleteBehavior.Restrict); // Tắt cascade delete tránh lỗi multiple cascade paths

            // 1-n: ClassSession <-> Attendance
            builder.Entity<Attendance>()
                .HasOne(a => a.ClassSession)
                .WithMany(cs => cs.Attendances)
                .HasForeignKey(a => a.ClassSessionId)
                .OnDelete(DeleteBehavior.Restrict); // Tắt cascade delete

            // 1-n: ApplicationUser (Student) <-> Attendance
            builder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany(s => s.AttendancesAsStudent)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict); // Tắt cascade delete

            // 1-n: ApplicationUser (Student) <-> Enrollment
            builder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany() // Nếu không có collection bên ApplicationUser
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict); // Tắt cascade delete

            // 1-n: Course <-> Enrollment
            builder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany()
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict); // Tắt cascade delete

            // 1-n: ApplicationUser (Student) <-> Result
            builder.Entity<Result>()
                .HasOne(r => r.Student)
                .WithMany()
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.Restrict); // Tắt cascade delete

            // 1-n: Course <-> Result
            builder.Entity<Result>()
                .HasOne(r => r.Course)
                .WithMany()
                .HasForeignKey(r => r.CourseId)
                .OnDelete(DeleteBehavior.Restrict); // Tắt cascade delete
        }


    }
}
