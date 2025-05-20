using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentEnrollmentSystem1.Models
{
    public class Student
    {
        [Key]
        [ScaffoldColumn(false)] // Hide from form scaffolding
        public string StudentId { get; set; } // Format: IU + 5 random numbers

        [Required(ErrorMessage = "Student name is required")]
        [Display(Name = "Student Name")]
        public string StudentName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Display(Name = "Phone Number")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Father's name is required")]
        [Display(Name = "Father's Name")]
        public string FatherName { get; set; }

        [Required(ErrorMessage = "Mother's name is required")]
        [Display(Name = "Mother's Name")]
        public string MotherName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Course is required")]
        [Display(Name = "Course")]
        public string Course { get; set; }

        [Display(Name = "Registration Date")]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [Display(Name = "Payment Status")]
        public bool PaymentStatus { get; set; } = false;

        [Display(Name = "Amount Paid")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal AmountPaid { get; set; } = 0;
    }
}