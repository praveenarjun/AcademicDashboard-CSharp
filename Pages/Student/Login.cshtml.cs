using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using StudentEnrollmentSystem1.Data.Repositories;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem1.Pages.Student
{
    public class LoginModel : PageModel
    {
        private readonly StudentRepository _studentRepository;
        private readonly PaymentRepository _paymentRepository;
        private readonly CourseEnrollmentRepository _courseEnrollmentRepository;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(
            StudentRepository studentRepository,
            PaymentRepository paymentRepository,
            CourseEnrollmentRepository courseEnrollmentRepository,
            ILogger<LoginModel> logger)
        {
            _studentRepository = studentRepository;
            _paymentRepository = paymentRepository;
            _courseEnrollmentRepository = courseEnrollmentRepository;
            _logger = logger;
        }

        [BindProperty]
        public LoginInput LoginData { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            _logger.LogInformation("Loading login page");

            // Check if there's a success message from registration
            if (TempData["SuccessMessage"] != null)
            {
                _logger.LogInformation($"Success message found: {TempData["SuccessMessage"]}");
                ViewData["SuccessMessage"] = TempData["SuccessMessage"].ToString();
            }

            // Check if there are any students in the database
            try
            {
                var studentCount = _studentRepository.GetStudentCountAsync().Result;
                _logger.LogInformation($"Found {studentCount} students in the database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking student count");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Processing login attempt");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid");
                return Page();
            }

            try
            {
                // Validate student credentials
                _logger.LogInformation($"Attempting to login with email: {LoginData.Email}");
                var student = await _studentRepository.GetStudentByEmailAsync(LoginData.Email);

                if (student == null)
                {
                    _logger.LogWarning($"No student found with email: {LoginData.Email}");
                    ErrorMessage = "Invalid email or password.";
                    return Page();
                }

                _logger.LogInformation($"Found student: {student.StudentId}, {student.StudentName}");

                if (student.Password != LoginData.Password)
                {
                    _logger.LogWarning($"Invalid password for student: {student.StudentId}");
                    ErrorMessage = "Invalid email or password.";
                    return Page();
                }

                // Store student information in session
                _logger.LogInformation($"Storing student information in session: {student.StudentId}");
                HttpContext.Session.SetString("StudentId", student.StudentId);
                HttpContext.Session.SetString("StudentName", student.StudentName);
                HttpContext.Session.SetString("Course", student.Course);
                HttpContext.Session.SetString("Email", student.Email);

                _logger.LogInformation($"Login successful for student: {student.StudentId}");

                // Check if student has made payment
                var payments = await _paymentRepository.GetPaymentsByStudentIdAsync(student.StudentId);
                bool hasPayment = payments != null && payments.Any(p => p.PaymentStatus == "Completed");

                // Check if student has enrolled in courses
                var enrollments = await _courseEnrollmentRepository.GetStudentEnrollmentsAsync(student.StudentId, true);
                bool hasEnrollments = enrollments != null && enrollments.Any();

                // Determine where to redirect the student
                if (!hasPayment)
                {
                    // If student hasn't made payment, redirect to payment page
                    _logger.LogInformation($"Student {student.StudentId} has not made payment. Redirecting to payment page.");
                    return RedirectToPage("/Student/Payment", new { course = student.Course });
                }
                else if (hasPayment && !hasEnrollments)
                {
                    // If student has made payment but hasn't enrolled in courses, redirect to course registration
                    _logger.LogInformation($"Student {student.StudentId} has made payment but not enrolled in courses. Redirecting to course registration.");
                    return RedirectToPage("/Student/CourseRegistration", new { studentId = student.StudentId });
                }
                else
                {
                    // If student has made payment and enrolled in courses, redirect to dashboard
                    _logger.LogInformation($"Student {student.StudentId} has made payment and enrolled in courses. Redirecting to dashboard.");
                    return RedirectToPage("/Student/Dashboard", new { studentId = student.StudentId });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                ErrorMessage = "An error occurred during login. Please try again.";
                return Page();
            }
        }
    }

    public class LoginInput
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}