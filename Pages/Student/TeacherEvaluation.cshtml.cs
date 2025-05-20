using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using StudentEnrollmentSystem1.Data.Repositories;
using StudentEnrollmentSystem1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem1.Pages.Student
{
    public class TeacherEvaluationModel : PageModel
    {
        private readonly StudentRepository _studentRepository;
        private readonly CourseOfferingRepository _courseOfferingRepository;
        private readonly CourseEnrollmentRepository _courseEnrollmentRepository;
        private readonly TeacherEvaluationRepository _evaluationRepository;
        private readonly ILogger<TeacherEvaluationModel> _logger;

        public TeacherEvaluationModel(
            StudentRepository studentRepository,
            CourseOfferingRepository courseOfferingRepository,
            CourseEnrollmentRepository courseEnrollmentRepository,
            TeacherEvaluationRepository evaluationRepository,
            ILogger<TeacherEvaluationModel> logger)
        {
            _studentRepository = studentRepository;
            _courseOfferingRepository = courseOfferingRepository;
            _courseEnrollmentRepository = courseEnrollmentRepository;
            _evaluationRepository = evaluationRepository;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public string StudentId { get; set; }

        [BindProperty]
        public string CourseCode { get; set; }

        [BindProperty]
        public int TeachingRating { get; set; }

        [BindProperty]
        public int ContentRating { get; set; }

        [BindProperty]
        public int AssessmentRating { get; set; }

        [BindProperty]
        public int CommunicationRating { get; set; }

        [BindProperty]
        public int OverallRating { get; set; }

        [BindProperty]
        public string Comments { get; set; }

        [BindProperty]
        public bool IsAnonymous { get; set; } = true;

        public string StudentName { get; set; }
        public string Branch { get; set; }
        public List<CourseOffering> EnrolledCourses { get; set; } = new List<CourseOffering>();
        public List<TeacherEvaluation> PreviousEvaluations { get; set; } = new List<TeacherEvaluation>();

        public async Task<IActionResult> OnGetAsync()
        {
            _logger.LogInformation("Loading teacher evaluation page for student: {StudentId}", StudentId);

            if (string.IsNullOrEmpty(StudentId))
            {
                // Try to get student ID from TempData
                if (TempData != null && TempData.ContainsKey("StudentId"))
                {
                    StudentId = TempData["StudentId"].ToString();
                    TempData.Keep("StudentId");
                    _logger.LogInformation("Retrieved student ID from TempData: {StudentId}", StudentId);
                }
                else
                {
                    _logger.LogWarning("No student ID provided");
                    // If no student ID, redirect to login
                    return RedirectToPage("/Student/Login");
                }
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
            Branch = student.Course;
            _logger.LogInformation("Student details: {StudentName}, Branch: {Branch}", StudentName, Branch);

            // Get enrolled courses
            var enrollments = await _courseEnrollmentRepository.GetStudentEnrollmentsAsync(StudentId, true);
            _logger.LogInformation("Student has {EnrolledCount} enrolled courses", enrollments.Count);

            // Get course details for enrolled courses
            foreach (var enrollment in enrollments)
            {
                var courseDetails = await _courseOfferingRepository.GetCourseOfferingByCodeAsync(enrollment.CourseCode);
                if (courseDetails != null)
                {
                    EnrolledCourses.Add(courseDetails);
                    _logger.LogInformation("Added course details: {CourseCode}, {CourseName}",
                        courseDetails.CourseCode, courseDetails.CourseName);
                }
            }

            // Get previous evaluations
            PreviousEvaluations = await _evaluationRepository.GetStudentEvaluationsAsync(StudentId);
            _logger.LogInformation("Student has {EvaluationCount} previous evaluations", PreviousEvaluations.Count);

            // Filter out courses that have already been evaluated
            var evaluatedCourseCodes = PreviousEvaluations.Select(e => e.CourseCode).ToList();
            EnrolledCourses = EnrolledCourses.Where(c => !evaluatedCourseCodes.Contains(c.CourseCode)).ToList();
            _logger.LogInformation("{CourseCount} courses available for evaluation after filtering", EnrolledCourses.Count);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Processing teacher evaluation submission for student: {StudentId}", StudentId);

            if (string.IsNullOrEmpty(StudentId))
            {
                _logger.LogWarning("Student ID is missing");
                TempData["ErrorMessage"] = "Student ID is required.";
                return RedirectToPage();
            }

            if (string.IsNullOrEmpty(CourseCode))
            {
                _logger.LogWarning("No course selected for evaluation");
                TempData["ErrorMessage"] = "Please select a course to evaluate.";
                return RedirectToPage();
            }

            _logger.LogInformation("Evaluating course: {CourseCode}", CourseCode);

            // Check if student has already evaluated this course
            var existingEvaluation = await _evaluationRepository.GetEvaluationAsync(StudentId, CourseCode);
            if (existingEvaluation != null)
            {
                _logger.LogWarning("Student has already evaluated this course: {CourseCode}", CourseCode);
                TempData["ErrorMessage"] = "You have already submitted an evaluation for this course.";
                return RedirectToPage();
            }

            // Get course details
            var course = await _courseOfferingRepository.GetCourseOfferingByCodeAsync(CourseCode);
            if (course == null)
            {
                _logger.LogWarning("Course not found: {CourseCode}", CourseCode);
                TempData["ErrorMessage"] = "Course not found.";
                return RedirectToPage();
            }

            _logger.LogInformation("Course details: {CourseName}, Instructor: {Instructor}",
                course.CourseName, course.Instructor);

            // Validate ratings
            if (TeachingRating < 1 || ContentRating < 1 || AssessmentRating < 1 ||
                CommunicationRating < 1 || OverallRating < 1)
            {
                _logger.LogWarning("Invalid ratings provided");
                TempData["ErrorMessage"] = "Please provide ratings for all categories.";
                return RedirectToPage();
            }

            _logger.LogInformation("Ratings - Teaching: {Teaching}, Content: {Content}, Assessment: {Assessment}, " +
                "Communication: {Communication}, Overall: {Overall}",
                TeachingRating, ContentRating, AssessmentRating, CommunicationRating, OverallRating);

            try
            {
                // Create the evaluation
                await _evaluationRepository.CreateEvaluationAsync(
                    StudentId,
                    CourseCode,
                    course.Instructor,
                    TeachingRating,
                    ContentRating,
                    AssessmentRating,
                    CommunicationRating,
                    OverallRating,
                    Comments,
                    IsAnonymous
                );

                _logger.LogInformation("Evaluation submitted successfully for course: {CourseCode}", CourseCode);
                TempData["SuccessMessage"] = "Your evaluation has been submitted successfully. Thank you for your feedback!";

                // Clear form fields
                CourseCode = string.Empty;
                TeachingRating = 0;
                ContentRating = 0;
                AssessmentRating = 0;
                CommunicationRating = 0;
                OverallRating = 0;
                Comments = string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting evaluation for course: {CourseCode}", CourseCode);
                TempData["ErrorMessage"] = "An error occurred while submitting your evaluation. Please try again.";
            }

            return RedirectToPage();
        }
    }
}