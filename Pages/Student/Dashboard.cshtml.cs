using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentEnrollmentSystem1.Data.Repositories;
using StudentEnrollmentSystem1.Models;
using System;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem1.Pages.Student
{
    public class DashboardModel : PageModel
    {
        private readonly StudentRepository _studentRepository;
        private readonly CourseEnrollmentRepository _courseEnrollmentRepository;

        public DashboardModel(
            StudentRepository studentRepository,
            CourseEnrollmentRepository courseEnrollmentRepository)
        {
            _studentRepository = studentRepository;
            _courseEnrollmentRepository = courseEnrollmentRepository;
        }

        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentEmail { get; set; }
        public string Branch { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool PaymentStatus { get; set; }
        public decimal AmountPaid { get; set; }
        public int EnrolledCourses { get; set; }
        public int TotalCredits { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if student is logged in via session
            StudentId = HttpContext.Session.GetString("StudentId");
            if (string.IsNullOrEmpty(StudentId))
            {
                // If no student ID in session, redirect to login
                return RedirectToPage("/Student/Login");
            }

            // Get student details
            var student = await _studentRepository.GetStudentByIdAsync(StudentId);
            if (student == null)
            {
                // Clear invalid session
                HttpContext.Session.Clear();
                TempData["ErrorMessage"] = "Student not found.";
                return RedirectToPage("/Student/Login");
            }

            // Set student information
            StudentName = student.StudentName;
            StudentEmail = student.Email;
            Branch = student.Course;
            RegistrationDate = student.RegistrationDate;
            PaymentStatus = student.PaymentStatus;
            AmountPaid = student.AmountPaid;

            // Get enrollment information
            try
            {
                var enrollments = await _courseEnrollmentRepository.GetStudentEnrollmentsAsync(StudentId, true);
                EnrolledCourses = enrollments?.Count ?? 0;
                TotalCredits = await _courseEnrollmentRepository.GetStudentTotalCreditsAsync(StudentId);
            }
            catch (Exception)
            {
                // Handle case where enrollments might not be available yet
                EnrolledCourses = 0;
                TotalCredits = 0;
            }

            return Page();
        }
    }
}