using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using StudentEnrollmentSystem1.Data.Repositories;
using StudentEnrollmentSystem1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem1.Pages.Student
{
    public class TimetableModel : PageModel
    {
        private readonly StudentRepository _studentRepository;
        private readonly CourseOfferingRepository _courseOfferingRepository;
        private readonly CourseEnrollmentRepository _courseEnrollmentRepository;
        private readonly ILogger<TimetableModel> _logger;

        public TimetableModel(
            StudentRepository studentRepository,
            CourseOfferingRepository courseOfferingRepository,
            CourseEnrollmentRepository courseEnrollmentRepository,
            ILogger<TimetableModel> logger)
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
        public int TotalCredits { get; set; }

        public List<CourseEnrollment> EnrolledCourses { get; set; } = new List<CourseEnrollment>();
        public List<CourseOffering> EnrolledCourseDetails { get; set; } = new List<CourseOffering>();

        public List<string> Days { get; set; } = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
        public List<string> TimeSlots { get; set; } = new List<string>
        {
            "09:00-10:00",
            "10:00-11:00",
            "11:00-12:00",
            "12:00-13:00",
            "13:00-14:00",
            "14:00-15:00",
            "15:00-16:00",
            "16:00-17:00"
        };

        public List<TimetableEntry> Timetable { get; set; } = new List<TimetableEntry>();

        public async Task<IActionResult> OnGetAsync()
        {
            _logger.LogInformation("Loading timetable page for student: {StudentId}", StudentId);

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
            Branch = student.Course;
            _logger.LogInformation("Student details: {StudentName}, Branch: {Branch}", StudentName, Branch);

            // Get enrolled courses
            EnrolledCourses = await _courseEnrollmentRepository.GetStudentEnrollmentsAsync(StudentId, true);
            _logger.LogInformation("Student has {EnrolledCount} enrolled courses", EnrolledCourses.Count);

            // No minimum courses requirement check

            // Calculate total credits
            TotalCredits = EnrolledCourses.Sum(e => e.Credits);
            _logger.LogInformation("Total credits: {TotalCredits}", TotalCredits);

            // Get course details for enrolled courses
            foreach (var enrollment in EnrolledCourses)
            {
                var courseDetails = await _courseOfferingRepository.GetCourseOfferingByCodeAsync(enrollment.CourseCode);
                if (courseDetails != null)
                {
                    EnrolledCourseDetails.Add(courseDetails);
                    _logger.LogInformation("Added course details: {CourseCode}, {CourseName}",
                        courseDetails.CourseCode, courseDetails.CourseName);
                }
            }

            // Generate timetable
            GenerateTimetable();
            _logger.LogInformation("Generated timetable with {EntryCount} entries", Timetable.Count);

            return Page();
        }

        private void GenerateTimetable()
        {
            _logger.LogInformation("Generating random timetable for {CourseCount} courses", EnrolledCourseDetails.Count);

            // Clear existing timetable
            Timetable.Clear();

            // Create a random number generator
            Random random = new Random();

            // Track allocated slots to avoid conflicts
            Dictionary<string, HashSet<string>> allocatedSlots = new Dictionary<string, HashSet<string>>();
            foreach (var day in Days)
            {
                allocatedSlots[day] = new HashSet<string>();
            }

            foreach (var course in EnrolledCourseDetails)
            {
                _logger.LogInformation("Generating random schedule for course: {CourseCode}", course.CourseCode);

                // Determine how many days per week this course meets (1-3 days)
                int daysPerWeek = random.Next(1, 4);
                _logger.LogInformation("Course {CourseCode} will meet {DaysPerWeek} days per week", course.CourseCode, daysPerWeek);

                // Randomly select days
                List<string> selectedDays = new List<string>();
                List<string> availableDays = new List<string>(Days);

                for (int i = 0; i < daysPerWeek && availableDays.Count > 0; i++)
                {
                    int dayIndex = random.Next(availableDays.Count);
                    selectedDays.Add(availableDays[dayIndex]);
                    availableDays.RemoveAt(dayIndex);
                }

                // For each selected day, find an available time slot
                foreach (var day in selectedDays)
                {
                    // Get available time slots for this day
                    List<string> availableTimeSlots = TimeSlots
                        .Where(ts => !allocatedSlots[day].Contains(ts))
                        .ToList();

                    if (availableTimeSlots.Count == 0)
                    {
                        _logger.LogWarning("No available time slots for {Day} for course {CourseCode}",
                            day, course.CourseCode);
                        continue;
                    }

                    // Randomly select a time slot
                    string timeSlot = availableTimeSlots[random.Next(availableTimeSlots.Count)];

                    // Mark this slot as allocated
                    allocatedSlots[day].Add(timeSlot);

                    // Generate a random room number
                    string room = $"Room {random.Next(100, 500)}";

                    // Add to timetable
                    Timetable.Add(new TimetableEntry
                    {
                        Day = day,
                        TimeSlot = timeSlot,
                        CourseCode = course.CourseCode,
                        CourseName = course.CourseName,
                        Room = room
                    });

                    _logger.LogInformation("Added random timetable entry: {Day}, {TimeSlot}, {CourseCode}, {Room}",
                        day, timeSlot, course.CourseCode, room);
                }
            }

            // Sort the timetable by day and time slot for better display
            Timetable = Timetable
                .OrderBy(t => Days.IndexOf(t.Day))
                .ThenBy(t => TimeSlots.IndexOf(t.TimeSlot))
                .ToList();
        }

        private string GetFullDayName(string shortDay)
        {
            switch (shortDay.ToLower())
            {
                case "mon": return "Monday";
                case "tue": return "Tuesday";
                case "wed": return "Wednesday";
                case "thu": return "Thursday";
                case "fri": return "Friday";
                default: return shortDay; // If it's already the full name or unknown
            }
        }
    }

    public class TimetableEntry
    {
        public string Day { get; set; }
        public string TimeSlot { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string Room { get; set; }
    }
}