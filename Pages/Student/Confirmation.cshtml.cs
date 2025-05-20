using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using StudentEnrollmentSystem1.Data;
using StudentEnrollmentSystem1.Data.Repositories;
using StudentEnrollmentSystem1.Models;
using System;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem1.Pages.Student
{
    public class ConfirmationModel : PageModel
    {
        private readonly StudentRepository _studentRepository;
        private readonly ILogger<ConfirmationModel> _logger;

        public ConfirmationModel(
            StudentRepository studentRepository,
            ILogger<ConfirmationModel> logger)
        {
            _studentRepository = studentRepository;
            _logger = logger;
        }

        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentEmail { get; set; }
        public string Course { get; set; }
        public bool PaymentStatus { get; set; }
        public decimal AmountPaid { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            _logger.LogInformation("Loading confirmation page");

            // Get student details from TempData
            if (TempData != null)
            {
                StudentId = TempData["StudentId"]?.ToString();
                StudentName = TempData["StudentName"]?.ToString();
                StudentEmail = TempData["Email"]?.ToString();

                // Check if AmountPaid is in TempData (from recent payment)
                if (TempData["AmountPaid"] != null)
                {
                    try
                    {
                        string amountStr = TempData["AmountPaid"].ToString();
                        if (decimal.TryParse(amountStr, out decimal amount))
                        {
                            AmountPaid = amount;
                            _logger.LogInformation("Found amount paid in TempData: {AmountPaid}", AmountPaid);
                            PaymentStatus = true; // If we have an amount, payment was successful
                        }
                        else
                        {
                            _logger.LogWarning("Could not parse amount from TempData: {AmountStr}", amountStr);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing AmountPaid from TempData");
                    }
                }
            }

            // If we have a student ID, get the full details from the database
            if (!string.IsNullOrEmpty(StudentId))
            {
                _logger.LogInformation("Getting student details from database: {StudentId}", StudentId);
                var student = await _studentRepository.GetStudentByIdAsync(StudentId);
                if (student != null)
                {
                    _logger.LogInformation("Found student in database: {StudentId}, {StudentName}", StudentId, student.StudentName);
                    StudentName = student.StudentName;
                    StudentEmail = student.Email;
                    Course = student.Course;

                    // Only use database values if we don't have values from TempData
                    if (AmountPaid == 0)
                    {
                        PaymentStatus = student.PaymentStatus;
                        AmountPaid = student.AmountPaid;
                        _logger.LogInformation("Using payment details from database: Status={PaymentStatus}, Amount={AmountPaid}",
                            PaymentStatus, AmountPaid);
                    }

                    // Set success message based on context
                    if (TempData["AmountPaid"] != null)
                    {
                        // If we just processed a payment, set message for course registration
                        TempData["SuccessMessage"] = "Payment successful! You can now proceed to course registration.";
                    }
                    else
                    {
                        // Otherwise, set message for login
                        TempData["SuccessMessage"] = "Registration and payment successful! You can now login with your email and password.";
                    }
                }
                else
                {
                    _logger.LogWarning("Student not found in database: {StudentId}", StudentId);
                    // If student not found in database, redirect to home
                    return RedirectToPage("/Index");
                }
            }
            else
            {
                _logger.LogWarning("No student ID found in TempData");
                // If no student ID in TempData, redirect to home
                return RedirectToPage("/Index");
            }

            return Page();
        }
    }
}