namespace LanguageCenter.Models
{
    public class ClassSession
    {
        public int Id { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public string TeacherId { get; set; } = null!;
        public ApplicationUser Teacher { get; set; } = null!;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string? Location { get; set; }

        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }

}
