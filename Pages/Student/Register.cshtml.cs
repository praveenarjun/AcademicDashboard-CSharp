using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using StudentEnrollmentSystem1.Data.Repositories;
using StudentEnrollmentSystem1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem1.Pages.Student
{
    public class RegisterModel : PageModel
    {
        private readonly StudentRepository _studentRepository;
        private readonly CourseRepository _courseRepository;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            StudentRepository studentRepository,
            CourseRepository courseRepository,
            ILogger<RegisterModel> logger)
        {
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _logger = logger;
        }

        [BindProperty]
        public Models.Student Student { get; set; }

        public List<SelectListItem> CourseOptions { get; set; }

        public async Task OnGetAsync()
        {
            _logger.LogInformation("Loading registration page");

            // Ensure courses are seeded
            await _courseRepository.SeedCoursesAsync();

            await PopulateCourseOptionsAsync();

            // Initialize a new Student object if not already initialized
            if (Student == null)
            {
                Student = new Models.Student
                {
                    RegistrationDate = DateTime.Now,
                    PaymentStatus = false,
                    AmountPaid = 0
                };
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Starting student registration process");

            // Remove StudentId validation error if it exists
            if (ModelState.ContainsKey("Student.StudentId"))
            {
                _logger.LogInformation("Removing StudentId validation error");
                ModelState.Remove("Student.StudentId");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning($"Validation error: {error.ErrorMessage}");
                }
                await PopulateCourseOptionsAsync();
                return Page();
            }

            try
            {
                // Check if email already exists
                _logger.LogInformation($"Checking if email {Student.Email} already exists");
                var existingStudent = await _studentRepository.GetStudentByEmailAsync(Student.Email);
                if (existingStudent != null)
                {
                    _logger.LogWarning($"Email {Student.Email} is already registered");
                    ModelState.AddModelError("Student.Email", "This email is already registered.");
                    await PopulateCourseOptionsAsync();
                    return Page();
                }

                // Generate a unique student ID
                _logger.LogInformation("Generating student ID");
                Student.StudentId = await _studentRepository.GenerateStudentIdAsync();
                _logger.LogInformation($"Generated student ID: {Student.StudentId}");

                // Set registration date
                Student.RegistrationDate = DateTime.Now;

                // Set default payment status
                Student.PaymentStatus = false;
                Student.AmountPaid = 0;

                // Save the student to the database
                _logger.LogInformation($"Saving student to database: {Student.StudentName}, {Student.Email}, {Student.Course}");
                try
                {
                    await _studentRepository.AddAsync(Student);
                    _logger.LogInformation("Student added to context, now saving changes");
                    await _studentRepository.SaveChangesAsync();
                    _logger.LogInformation($"Student saved successfully with ID: {Student.StudentId}");

                    // Verify the student was saved
                    var savedStudent = await _studentRepository.GetStudentByIdAsync(Student.StudentId);
                    if (savedStudent != null)
                    {
                        _logger.LogInformation($"Verified student was saved: {savedStudent.StudentId}, {savedStudent.StudentName}");
                    }
                    else
                    {
                        _logger.LogWarning($"Could not verify student was saved with ID: {Student.StudentId}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving student to database");
                    throw;
                }

                // Store student in TempData to pass to the confirmation page
                TempData["StudentId"] = Student.StudentId;
                TempData["StudentName"] = Student.StudentName;
                TempData["Course"] = Student.Course;
                TempData["Email"] = Student.Email;

                // Add success message for login page
                TempData["SuccessMessage"] = $"Registration successful! Please login with your email and password.";

                // Redirect to login page
                _logger.LogInformation($"Redirecting to login page after successful registration");
                return RedirectToPage("/Student/Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during student registration");
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                await PopulateCourseOptionsAsync();
                return Page();
            }
        }

        private async Task PopulateCourseOptionsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching courses for dropdown");
                // Check if courses exist, if not seed them
                var courses = await _courseRepository.GetAllCoursesAsync();
                _logger.LogInformation($"Found {courses?.Count ?? 0} courses");

                if (courses == null || !courses.Any())
                {
                    _logger.LogWarning("No courses found, seeding courses");
                    await _courseRepository.SeedCoursesAsync();
                    _logger.LogInformation("Courses seeded, fetching again");
                    courses = await _courseRepository.GetAllCoursesAsync();
                    _logger.LogInformation($"Found {courses?.Count ?? 0} courses after seeding");
                }

                CourseOptions = courses.Select(c => new SelectListItem
                {
                    Value = c.Name,
                    Text = $"{c.Name} - {c.Description}"
                }).ToList();

                _logger.LogInformation($"Populated {CourseOptions.Count} course options for dropdown");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error populating course options");
                // Provide a fallback option
                CourseOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "BTech", Text = "BTech - Bachelor of Technology" },
                    new SelectListItem { Value = "BBA", Text = "BBA - Bachelor of Business Administration" }
                };
                _logger.LogInformation("Using fallback course options");
            }
        }
    }
}