using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentEnrollmentSystem1.Models
{
    public class TeacherEvaluation
    {
        [Key]
        public int EvaluationId { get; set; }
        
        [Required]
        public string StudentId { get; set; }
        
        [Required]
        public string CourseCode { get; set; }
        
        [Required]
        public string InstructorName { get; set; }
        
        [Required]
        [Range(1, 5)]
        public int TeachingRating { get; set; }
        
        [Required]
        [Range(1, 5)]
        public int ContentRating { get; set; }
        
        [Required]
        [Range(1, 5)]
        public int AssessmentRating { get; set; }
        
        [Required]
        [Range(1, 5)]
        public int CommunicationRating { get; set; }
        
        [Required]
        [Range(1, 5)]
        public int OverallRating { get; set; }
        
        public string Comments { get; set; }
        
        [Required]
        public DateTime SubmissionDate { get; set; } = DateTime.Now;
        
        public bool IsAnonymous { get; set; } = true;
        
        // Navigation properties
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }
    }
}