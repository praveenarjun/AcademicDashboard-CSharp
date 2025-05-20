using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentEnrollmentSystem1.Models
{
    public class TimetableEntry
    {
        [Key]
        public int TimetableEntryId { get; set; }

        [Required]
        public string StudentId { get; set; }

        [Required]
        public string CourseCode { get; set; }

        [Required]
        public string CourseName { get; set; }

        [Required]
        public string Day { get; set; }

        [Required]
        public string TimeSlot { get; set; }

        [Required]
        public string Room { get; set; }

        // Navigation property
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }
    }
}