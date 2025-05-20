using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentEnrollmentSystem1.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        
        [Required]
        public string StudentId { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }
        
        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        
        [Required]
        public string PaymentMethod { get; set; } = "Online";
        
        [Required]
        public string PaymentStatus { get; set; } = "Completed";
        
        public string TransactionId { get; set; }
        
        public string Description { get; set; }
        
        public string ReceiptNumber { get; set; }
        
        // Navigation property
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }
    }
}