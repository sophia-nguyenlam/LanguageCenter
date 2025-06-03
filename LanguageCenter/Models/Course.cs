using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LanguageCenter.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DurationInWeeks { get; set; }

        [Precision(18, 2)]
        public decimal TuitionFee { get; set; }

        public string? Level { get; set; }

        public string? Language { get; set; }

        public ICollection<ClassSession> ClassSessions { get; set; } = new List<ClassSession>();
    }
}
