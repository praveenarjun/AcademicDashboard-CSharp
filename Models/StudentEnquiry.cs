using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentEnrollmentSystem1.Models
{
    public class StudentEnquiry
    {
        [Key]
        public int EnquiryId { get; set; }

        [Required]
        public string StudentId { get; set; }

        [Required]
        public string EnquiryType { get; set; } // "Contact", "Timetable", "Evaluation"

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public DateTime SubmissionDate { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Pending"; // "Pending", "In Progress", "Resolved"

        public string? Response { get; set; }

        public DateTime? ResponseDate { get; set; }

        public string? RespondedBy { get; set; }

        // Navigation property
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }
    }
}