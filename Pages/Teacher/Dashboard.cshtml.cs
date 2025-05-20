using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentEnrollmentSystem1.Data.Repositories;
using StudentEnrollmentSystem1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem1.Pages.Teacher
{
    public class DashboardModel : PageModel
    {
        private readonly CourseOfferingRepository _courseOfferingRepository;
        private readonly StudentEnquiryRepository _enquiryRepository;
        private readonly TeacherEvaluationRepository _evaluationRepository;

        public DashboardModel(
            CourseOfferingRepository courseOfferingRepository,
            StudentEnquiryRepository enquiryRepository,
            TeacherEvaluationRepository evaluationRepository)
        {
            _courseOfferingRepository = courseOfferingRepository;
            _enquiryRepository = enquiryRepository;
            _evaluationRepository = evaluationRepository;
        }

        public string TeacherId { get; set; }
        public string TeacherName { get; set; }
        public string Department { get; set; }
        public int CourseCount { get; set; }

        public List<CourseOffering> Courses { get; set; } = new List<CourseOffering>();
        public List<StudentEnquiry> StudentEnquiries { get; set; } = new List<StudentEnquiry>();
        public List<TeacherEvaluation> TeacherEvaluations { get; set; } = new List<TeacherEvaluation>();

        public double AverageRating { get; set; }
        public double AverageTeachingRating { get; set; }
        public double AverageContentRating { get; set; }
        public double AverageAssessmentRating { get; set; }
        public double AverageCommunicationRating { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if teacher is logged in
            if (HttpContext.Session.GetString("TeacherId") == null)
            {
                return RedirectToPage("/Teacher/Login");
            }

            // Get teacher information from session
            TeacherId = HttpContext.Session.GetString("TeacherId");
            TeacherName = HttpContext.Session.GetString("TeacherName");
            Department = HttpContext.Session.GetString("Department");

            // Get courses for this teacher's department
            Courses = await _courseOfferingRepository.GetCourseOfferingsByDepartmentAsync(Department);
            
            // Filter to show only courses taught by this teacher
            var teacherCourses = Courses.Where(c => c.Instructor == TeacherName).ToList();
            
            // If the teacher doesn't have any courses assigned yet, show all department courses
            if (!teacherCourses.Any())
            {
                // Limit to a reasonable number of courses (e.g., 5) to avoid overwhelming the teacher
                Courses = Courses.Take(5).ToList();
                Console.WriteLine($"No courses found for teacher {TeacherName}. Showing {Courses.Count} department courses.");
            }
            else
            {
                Courses = teacherCourses;
                Console.WriteLine($"Found {Courses.Count} courses taught by teacher {TeacherName}.");
            }
            
            CourseCount = Courses.Count;

            // Get all pending enquiries
            // In a real app, you would filter by department or courses taught
            StudentEnquiries = await _enquiryRepository.GetAllPendingEnquiriesAsync();

            // Get teacher evaluations
            TeacherEvaluations = await _evaluationRepository.GetEvaluationsByInstructorAsync(TeacherName);
            
            // Log the number of evaluations found
            Console.WriteLine($"Found {TeacherEvaluations.Count} evaluations for teacher: {TeacherName}");

            // Calculate average ratings
            if (TeacherEvaluations.Any())
            {
                AverageRating = TeacherEvaluations.Average(e => e.OverallRating);
                AverageTeachingRating = TeacherEvaluations.Average(e => e.TeachingRating);
                AverageContentRating = TeacherEvaluations.Average(e => e.ContentRating);
                AverageAssessmentRating = TeacherEvaluations.Average(e => e.AssessmentRating);
                AverageCommunicationRating = TeacherEvaluations.Average(e => e.CommunicationRating);
            }

            return Page();
        }
    }
}