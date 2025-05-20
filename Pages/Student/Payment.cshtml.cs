using Microsoft.AspNetCore.Http;
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
    public class PaymentModel : PageModel
    {
        private readonly CourseRepository _courseRepository;
        private readonly StudentRepository _studentRepository;
        private readonly PaymentRepository _paymentRepository;
        private readonly ILogger<PaymentModel> _logger;

        public PaymentModel(
            CourseRepository courseRepository,
            StudentRepository studentRepository,
            PaymentRepository paymentRepository,
            ILogger<PaymentModel> logger)
        {
            _courseRepository = courseRepository;
            _studentRepository = studentRepository;
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public string Course { get; set; }

        [BindProperty]
        public decimal Amount { get; set; }

        public Course CourseDetails { get; set; }

        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentEmail { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            _logger.LogInformation("Loading payment page for course: {Course}", Course);

            // Get student ID from session or TempData
            string studentId = TempData["StudentId"]?.ToString();
            if (string.IsNullOrEmpty(studentId))
            {
                studentId = HttpContext.Session.GetString("StudentId");
            }

            // Check if student has already paid
            if (!string.IsNullOrEmpty(studentId))
            {
                var student = await _studentRepository.GetStudentByIdAsync(studentId);
                if (student != null && student.PaymentStatus)
                {
                    _logger.LogInformation("Student {StudentId} has already paid, redirecting to confirmation page", studentId);
                    TempData["SuccessMessage"] = "You have already completed your payment.";
                    return RedirectToPage("/Student/Confirmation");
                }
            }

            // Get course details from database
            CourseDetails = await _courseRepository.GetCourseByNameAsync(Course);

            if (CourseDetails == null)
            {
                _logger.LogWarning("Course details not found for: {Course}", Course);
                CourseDetails = new Course { Name = Course, Fee = 0, Description = "Unknown Course" };
            }
            else
            {
                _logger.LogInformation("Found course details: {CourseName}, Fee: {Fee}", CourseDetails.Name, CourseDetails.Fee);
            }

            // Set the amount to the course fee
            Amount = CourseDetails.Fee;
            _logger.LogInformation("Set payment amount to course fee: {Amount}", Amount);

            // Get student details from TempData (if coming from registration)
            if (TempData != null)
            {
                StudentId = TempData["StudentId"]?.ToString();
                StudentName = TempData["StudentName"]?.ToString();
                StudentEmail = TempData["Email"]?.ToString();

                if (!string.IsNullOrEmpty(StudentId))
                {
                    _logger.LogInformation("Found student details in TempData: {StudentId}, {StudentName}", StudentId, StudentName);
                }
                else
                {
                    // Try to get student details from session
                    StudentId = HttpContext.Session.GetString("StudentId");
                    StudentName = HttpContext.Session.GetString("StudentName");
                    StudentEmail = HttpContext.Session.GetString("Email");

                    if (!string.IsNullOrEmpty(StudentId))
                    {
                        _logger.LogInformation("Found student details in Session: {StudentId}, {StudentName}", StudentId, StudentName);

                        // Store in TempData for the POST action
                        TempData["StudentId"] = StudentId;
                        TempData["StudentName"] = StudentName;
                        TempData["Email"] = StudentEmail;
                    }
                    else
                    {
                        _logger.LogWarning("No student details found in TempData or Session");
                    }
                }

                // Preserve the values for the POST action
                TempData.Keep("StudentId");
                TempData.Keep("StudentName");
                TempData.Keep("Email");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Processing payment");

            // Get student ID from TempData
            string studentId = TempData["StudentId"]?.ToString();
            string studentName = TempData["StudentName"]?.ToString();

            if (string.IsNullOrEmpty(studentId))
            {
                _logger.LogWarning("Student information not found in TempData");

                // Try to get from session
                studentId = HttpContext.Session.GetString("StudentId");
                studentName = HttpContext.Session.GetString("StudentName");

                if (string.IsNullOrEmpty(studentId))
                {
                    _logger.LogError("Student information not found in Session either");
                    ErrorMessage = "Student information not found. Please try again.";
                    return Page();
                }
                else
                {
                    _logger.LogInformation("Found student information in Session: {StudentId}", studentId);
                    TempData["StudentId"] = studentId;
                    TempData["StudentName"] = studentName;
                }
            }
            else
            {
                _logger.LogInformation("Found student information in TempData: {StudentId}", studentId);
            }

            try
            {
                // Process the payment
                _logger.LogInformation("Updating payment status for student {StudentId}, amount: {Amount}", studentId, Amount);

                // Make sure Amount is not 0
                if (Amount <= 0)
                {
                    // Try to get the course fee again
                    var course = await _courseRepository.GetCourseByNameAsync(Course);
                    if (course != null && course.Fee > 0)
                    {
                        Amount = course.Fee;
                        _logger.LogInformation("Updated amount from course: {Amount}", Amount);
                    }
                    else
                    {
                        _logger.LogWarning("Amount is still 0 or negative, using default value of 1000");
                        Amount = 1000; // Default value if we can't get the course fee
                    }
                }

                var result = await _studentRepository.UpdatePaymentStatusAsync(studentId, Amount);

                if (!result)
                {
                    _logger.LogError("Payment processing failed for student {StudentId}", studentId);
                    ErrorMessage = "Payment processing failed. Please try again.";
                    return Page();
                }

                _logger.LogInformation("Payment status updated successfully for student {StudentId}", studentId);

                // Create payment record
                try
                {
                    string description = $"Course Fee Payment for {Course}";
                    _logger.LogInformation("Creating payment record: {Description}", description);
                    var payment = await _paymentRepository.CreatePaymentAsync(studentId, Amount, description);

                    // Store payment ID in TempData for receipt
                    TempData["PaymentId"] = payment.PaymentId;
                    TempData["ReceiptNumber"] = payment.ReceiptNumber;
                    _logger.LogInformation("Payment record created: {PaymentId}, {ReceiptNumber}", payment.PaymentId, payment.ReceiptNumber);
                }
                catch (Exception ex)
                {
                    // Payment record creation failed, but student payment status was updated
                    // We'll continue anyway since the core payment was processed
                    _logger.LogWarning(ex, "Failed to create payment record, but payment status was updated");
                }

                // Preserve student info for confirmation page
                TempData.Keep("StudentId");
                TempData.Keep("StudentName");
                TempData.Keep("Email");

                // Add amount paid to TempData (convert to string since TempData can't serialize decimal)
                TempData["AmountPaid"] = Amount.ToString();

                // Add success message
                TempData["SuccessMessage"] = $"Payment of ₹{Amount.ToString("N2")} for {studentName} has been processed successfully.";
                _logger.LogInformation("Payment successful, redirecting to confirmation page with amount: {Amount}", Amount);

                // Redirect to the confirmation page
                return RedirectToPage("/Student/Confirmation", new { studentId = studentId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment for student {StudentId}", studentId);
                ErrorMessage = $"An error occurred during payment processing: {ex.Message}";
                return Page();
            }
        }
    }
}