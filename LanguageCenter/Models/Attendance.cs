namespace LanguageCenter.Models
{
    public class Attendance
    {
        public int Id { get; set; }

        public int ClassSessionId { get; set; }
        public ClassSession ClassSession { get; set; } = null!;

        public string StudentId { get; set; } = null!;
        public ApplicationUser Student { get; set; } = null!;

        public bool IsPresent { get; set; }
    }

}
