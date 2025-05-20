using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using StudentEnrollmentSystem1.Data.Repositories;
using StudentEnrollmentSystem1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem1.Pages.Student
{
    public class CourseRegistrationModel : PageModel
    {
        private readonly StudentRepository _studentRepository;
        private readonly CourseOfferingRepository _courseOfferingRepository;
        private readonly CourseEnrollmentRepository _courseEnrollmentRepository;
        private readonly ILogger<CourseRegistrationModel> _logger;

        public CourseRegistrationModel(
            StudentRepository studentRepository,
            CourseOfferingRepository courseOfferingRepository,
            CourseEnrollmentRepository courseEnrollmentRepository,
            ILogger<CourseRegistrationModel> logger)
        {
            _studentRepository = studentRepository;
            _courseOfferingRepository = courseOfferingRepository;
            _courseEnrollmentRepository = courseEnrollmentRepository;
            _logger = logger;
        }

        // No minimum courses requirement

        [BindProperty(SupportsGet = true)]
        public string StudentId { get; set; }

        public string StudentName { get; set; }
        public string Branch { get; set; }
        public int CreditsUsed { get; set; }
        public int MaxCredits { get; set; } = 20; // Default max credits per semester

        public List<CourseOffering> AvailableCourses { get; set; } = new List<CourseOffering>();
        public List<CourseEnrollment> EnrolledCourses { get; set; } = new List<CourseEnrollment>();
        public List<CourseEnrollment> EnrollmentHistory { get; set; } = new List<CourseEnrollment>();

        public async Task<IActionResult> OnGetAsync()
        {
            _logger.LogInformation("Loading course registration page for student: {StudentId}", StudentId);

            // Try to get student ID from session if not provided in the URL
            if (string.IsNullOrEmpty(StudentId))
            {
                StudentId = HttpContext.Session.GetString("StudentId");
                _logger.LogInformation("Retrieved StudentId from session: {StudentId}", StudentId);
            }

            if (string.IsNullOrEmpty(StudentId))
            {
                _logger.LogWarning("Student ID is missing");
                TempData["ErrorMessage"] = "Student ID is required.";
                return RedirectToPage("/Index");
            }

            // Get student details
            var student = await _studentRepository.GetStudentByIdAsync(StudentId);
            if (student == null)
            {
                _logger.LogWarning("Student not found: {StudentId}", StudentId);
                TempData["ErrorMessage"] = "Student not found.";
                return RedirectToPage("/Index");
            }

            // Set student information
            StudentName = student.StudentName;
            Branch = student.Course; // Using the course as branch
            _logger.LogInformation("Student details: {StudentName}, Branch: {Branch}", StudentName, Branch);

            // Get enrolled courses
            EnrolledCourses = await _courseEnrollmentRepository.GetStudentEnrollmentsAsync(StudentId, true);
            _logger.LogInformation("Student has {EnrolledCount} enrolled courses", EnrolledCourses.Count);

            // Get enrollment history
            EnrollmentHistory = await _courseEnrollmentRepository.GetEnrollmentHistoryAsync(StudentId);

            // Calculate credits used
            CreditsUsed = await _courseEnrollmentRepository.GetStudentTotalCreditsAsync(StudentId);
            _logger.LogInformation("Student has used {CreditsUsed} out of {MaxCredits} credits", CreditsUsed, MaxCredits);

            // Get available courses for the student's branch
            AvailableCourses = await _courseOfferingRepository.GetCourseOfferingsByBranchAsync(Branch);
            _logger.LogInformation("Found {AvailableCount} courses for branch {Branch}", AvailableCourses.Count, Branch);

            // Filter out courses the student is already enrolled in
            var enrolledCourseIds = EnrolledCourses.Select(e => e.CourseCode).ToList();
            AvailableCourses = AvailableCourses.Where(c => !enrolledCourseIds.Contains(c.CourseCode)).ToList();
            _logger.LogInformation("{AvailableCount} courses available for enrollment after filtering", AvailableCourses.Count);

            // Ensure course offerings are seeded
            if (AvailableCourses.Count == 0)
            {
                _logger.LogWarning("No available courses found, attempting to seed course offerings");
                await _courseOfferingRepository.SeedCourseOfferingsAsync();
                AvailableCourses = await _courseOfferingRepository.GetCourseOfferingsByBranchAsync(Branch);
                AvailableCourses = AvailableCourses.Where(c => !enrolledCourseIds.Contains(c.CourseCode)).ToList();
                _logger.LogInformation("After seeding: {AvailableCount} courses available for enrollment", AvailableCourses.Count);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostEnrollCourseAsync(string courseCode, string studentId)
        {
            // Update StudentId property with the value from the form
            if (!string.IsNullOrEmpty(studentId))
            {
                StudentId = studentId;
            }

            _logger.LogInformation("Enrolling student {StudentId} in course {CourseCode}", StudentId, courseCode);

            if (string.IsNullOrEmpty(StudentId) || string.IsNullOrEmpty(courseCode))
            {
                _logger.LogWarning("Missing StudentId or CourseCode");
                TempData["ErrorMessage"] = "Student ID and Course Code are required.";
                return RedirectToPage(new { studentId = StudentId });
            }

            // Get current credits
            int currentCredits = await _courseEnrollmentRepository.GetStudentTotalCreditsAsync(StudentId);
            _logger.LogInformation("Current credits: {CurrentCredits}", currentCredits);

            // Get course credits
            var course = await _courseOfferingRepository.GetCourseOfferingByCodeAsync(courseCode);
            if (course == null)
            {
                _logger.LogWarning("Course not found: {CourseCode}", courseCode);
                TempData["ErrorMessage"] = "Course not found.";
                return RedirectToPage(new { studentId = StudentId });
            }

            _logger.LogInformation("Course details: {CourseName}, Credits: {Credits}", course.CourseName, course.Credits);

            // Check if adding this course would exceed max credits
            if (currentCredits + course.Credits > MaxCredits)
            {
                _logger.LogWarning("Exceeds max credits: {CurrentCredits} + {CourseCredits} > {MaxCredits}",
                    currentCredits, course.Credits, MaxCredits);
                TempData["ErrorMessage"] = $"Cannot enroll in this course. It would exceed your maximum credit limit of {MaxCredits}.";
                return RedirectToPage(new { studentId = StudentId });
            }

            // Enroll in the course
            var result = await _courseEnrollmentRepository.EnrollCourseAsync(StudentId, courseCode);
            if (result)
            {
                _logger.LogInformation("Successfully enrolled in course: {CourseCode}, {CourseName}", courseCode, course.CourseName);
                TempData["SuccessMessage"] = $"Successfully enrolled in {course.CourseName}.";
            }
            else
            {
                _logger.LogWarning("Failed to enroll in course: {CourseCode}", courseCode);
                TempData["ErrorMessage"] = "Failed to enroll in the course. You may already be enrolled or the course is full.";
            }

            return RedirectToPage(new { studentId = StudentId });
        }

        public async Task<IActionResult> OnPostDropCourseAsync(string courseCode, string studentId)
        {
            // Update StudentId property with the value from the form
            if (!string.IsNullOrEmpty(studentId))
            {
                StudentId = studentId;
            }

            _logger.LogInformation("Dropping course {CourseCode} for student {StudentId}", courseCode, StudentId);

            if (string.IsNullOrEmpty(StudentId) || string.IsNullOrEmpty(courseCode))
            {
                _logger.LogWarning("Missing StudentId or CourseCode");
                TempData["ErrorMessage"] = "Student ID and Course Code are required.";
                return RedirectToPage(new { studentId = StudentId });
            }

            // Get course details
            var course = await _courseOfferingRepository.GetCourseOfferingByCodeAsync(courseCode);
            if (course == null)
            {
                _logger.LogWarning("Course not found: {CourseCode}", courseCode);
                TempData["ErrorMessage"] = "Course not found.";
                return RedirectToPage(new { studentId = StudentId });
            }

            _logger.LogInformation("Course details: {CourseName}, Credits: {Credits}", course.CourseName, course.Credits);

            // Drop the course
            var result = await _courseEnrollmentRepository.DropCourseAsync(StudentId, courseCode);
            if (result)
            {
                _logger.LogInformation("Successfully dropped course: {CourseCode}, {CourseName}", courseCode, course.CourseName);
                TempData["SuccessMessage"] = $"Successfully dropped {course.CourseName}.";
            }
            else
            {
                _logger.LogWarning("Failed to drop course: {CourseCode}", courseCode);
                TempData["ErrorMessage"] = "Failed to drop the course. You may not be enrolled in this course.";
            }

            return RedirectToPage(new { studentId = StudentId });
        }

        // Check if student can proceed to timetable
        public bool CanProceedToTimetable()
        {
            // No minimum courses requirement, can always proceed
            return true;
        }
    }
}