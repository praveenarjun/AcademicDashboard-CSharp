using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentEnrollmentSystem1.Data.Repositories;
using StudentEnrollmentSystem1.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem1.Pages.Student
{
    public class PaymentHistoryModel : PageModel
    {
        private readonly StudentRepository _studentRepository;
        private readonly PaymentRepository _paymentRepository;

        public PaymentHistoryModel(
            StudentRepository studentRepository,
            PaymentRepository paymentRepository)
        {
            _studentRepository = studentRepository;
            _paymentRepository = paymentRepository;
        }

        [BindProperty(SupportsGet = true)]
        public string StudentId { get; set; }

        public string StudentName { get; set; }
        public string Branch { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal TotalAmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public List<Payment> PaymentHistory { get; set; } = new List<Payment>();

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(StudentId))
            {
                // Try to get student ID from TempData
                if (TempData != null && TempData.ContainsKey("StudentId"))
                {
                    StudentId = TempData["StudentId"].ToString();
                    TempData.Keep("StudentId");
                }
                else
                {
                    // If no student ID, redirect to login
                    return RedirectToPage("/Student/Login");
                }
            }

            // Get student details
            var student = await _studentRepository.GetStudentByIdAsync(StudentId);
            if (student == null)
            {
                TempData["ErrorMessage"] = "Student not found.";
                return RedirectToPage("/Index");
            }

            // Set student information
            StudentName = student.StudentName;
            Branch = student.Course;
            AmountPaid = student.AmountPaid;
            
            // Get payment history
            PaymentHistory = await _paymentRepository.GetPaymentsByStudentIdAsync(StudentId);
            
            // Calculate total amount paid
            TotalAmountPaid = 0;
            foreach (var payment in PaymentHistory)
            {
                TotalAmountPaid += payment.Amount;
            }
            
            // Set payment date from the most recent payment or use registration date as fallback
            if (PaymentHistory.Count > 0)
            {
                PaymentDate = PaymentHistory[0].PaymentDate; // First payment is the most recent due to ordering
            }
            else
            {
                PaymentDate = student.RegistrationDate; // Fallback to registration date
            }

            return Page();
        }
    }
}