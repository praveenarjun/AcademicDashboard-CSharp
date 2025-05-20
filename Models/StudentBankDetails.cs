using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentEnrollmentSystem1.Models
{
    public class StudentBankDetails
    {
        [Key]
        public int BankDetailsId { get; set; }

        [Required]
        public string StudentId { get; set; }

        [Required]
        [Display(Name = "Bank Name")]
        public string BankName { get; set; }

        [Required]
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        [Required]
        [Display(Name = "IFSC Code")]
        public string IFSC { get; set; }

        [Required]
        [Display(Name = "Account Holder Name")]
        public string AccountHolderName { get; set; }

        [Display(Name = "Last Updated")]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }
    }
}