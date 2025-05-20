using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentEnrollmentSystem1.Models
{
    public class CourseEnrollment
    {
        [Key]
        public int EnrollmentId { get; set; }
        
        [Required]
        public string StudentId { get; set; }
        
        [Required]
        public string CourseCode { get; set; }
        
        [Required]
        public string CourseName { get; set; }
        
        [Required]
        public int Credits { get; set; }
        
        [Required]
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;
        
        public DateTime? DropDate { get; set; }
        
        [Required]
        public bool IsActive { get; set; } = true;
        
        // Navigation property
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }
    }
}