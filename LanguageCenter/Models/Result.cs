namespace LanguageCenter.Models
{
    public class Result
    {
        public int Id { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public string StudentId { get; set; } = null!;
        public ApplicationUser Student { get; set; } = null!;

        public double? Score { get; set; }

        public string? Feedback { get; set; }
    }
}
