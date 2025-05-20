using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentEnrollmentSystem1.Data;
using StudentEnrollmentSystem1.Data.Repositories;
using StudentEnrollmentSystem1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem1.Pages.Student
{
    public class TimetableMatchingModel : PageModel
    {
        private readonly StudentRepository _studentRepository;
        private readonly CourseOfferingRepository _courseOfferingRepository;
        private readonly CourseEnrollmentRepository _courseEnrollmentRepository;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TimetableMatchingModel> _logger;

        public TimetableMatchingModel(
            StudentRepository studentRepository,
            CourseOfferingRepository courseOfferingRepository,
            CourseEnrollmentRepository courseEnrollmentRepository,
            ApplicationDbContext context,
            ILogger<TimetableMatchingModel> logger)
        {
            _studentRepository = studentRepository;
            _courseOfferingRepository = courseOfferingRepository;
            _courseEnrollmentRepository = courseEnrollmentRepository;
            _context = context;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public string StudentId { get; set; }

        [BindProperty]
        public List<string> SelectedDays { get; set; } = new List<string>();

        [BindProperty]
        public List<string> SelectedTimeSlots { get; set; } = new List<string>();

        public string StudentName { get; set; }
        public string Branch { get; set; }
        public int TotalCredits { get; set; }
        public bool HasSearched { get; set; } = false;
        public List<CourseOffering> MatchingCourses { get; set; } = new List<CourseOffering>();
        public List<CourseOffering> EnrolledCourseDetails { get; set; } = new List<CourseOffering>();
        public List<TimetableEntry> CurrentTimetable { get; set; } = new List<TimetableEntry>();

        public class TimetableEntry
        {
            public string Day { get; set; }
            public string TimeSlot { get; set; }
            public string CourseCode { get; set; }
            public string CourseName { get; set; }
            public string Room { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            _logger.LogInformation("Loading timetable matching page for student: {StudentId}", StudentId);

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
            var enrolledCourses = await _courseEnrollmentRepository.GetStudentEnrollmentsAsync(StudentId, true);
            _logger.LogInformation("Student has {EnrolledCount} enrolled courses", enrolledCourses.Count);

            // Calculate total credits
            TotalCredits = enrolledCourses.Sum(e => e.Credits);
            _logger.LogInformation("Total credits: {TotalCredits}", TotalCredits);

            // Get course details for enrolled courses
            foreach (var enrollment in enrolledCourses)
            {
                var courseDetails = await _courseOfferingRepository.GetCourseOfferingByCodeAsync(enrollment.CourseCode);
                if (courseDetails != null)
                {
                    EnrolledCourseDetails.Add(courseDetails);
                    _logger.LogInformation("Added course details: {CourseCode}, {CourseName}",
                        courseDetails.CourseCode, courseDetails.CourseName);
                }
            }

            // Generate timetable based on enrolled courses
            GenerateTimetable();
            _logger.LogInformation("Generated timetable with {EntryCount} entries", CurrentTimetable.Count);

            // Save timetable to database
            await SaveTimetableToDatabase();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Processing timetable matching request for student: {StudentId}", StudentId);

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
            var enrolledCourses = await _courseEnrollmentRepository.GetStudentEnrollmentsAsync(StudentId, true);
            _logger.LogInformation("Student has {EnrolledCount} enrolled courses", enrolledCourses.Count);

            // Calculate total credits
            TotalCredits = enrolledCourses.Sum(e => e.Credits);
            _logger.LogInformation("Total credits: {TotalCredits}", TotalCredits);

            // Get course details for enrolled courses
            foreach (var enrollment in enrolledCourses)
            {
                var courseDetails = await _courseOfferingRepository.GetCourseOfferingByCodeAsync(enrollment.CourseCode);
                if (courseDetails != null)
                {
                    EnrolledCourseDetails.Add(courseDetails);
                    _logger.LogInformation("Added course details: {CourseCode}, {CourseName}",
                        courseDetails.CourseCode, courseDetails.CourseName);
                }
            }

            // Generate timetable based on enrolled courses
            GenerateTimetable();
            _logger.LogInformation("Generated timetable with {EntryCount} entries", CurrentTimetable.Count);

            // Save timetable to database
            await SaveTimetableToDatabase();

            // Validate selection
            if (SelectedDays == null || !SelectedDays.Any() || SelectedTimeSlots == null || !SelectedTimeSlots.Any())
            {
                _logger.LogWarning("Invalid selection: No days or time slots selected");
                TempData["ErrorMessage"] = "Please select at least one day and one time slot.";
                return Page();
            }

            _logger.LogInformation("Selected days: {Days}, Selected time slots: {TimeSlots}",
                string.Join(", ", SelectedDays), string.Join(", ", SelectedTimeSlots));

            // Get all available courses for the student's branch
            var availableCourses = await _courseOfferingRepository.GetCourseOfferingsByBranchAsync(Branch);
            _logger.LogInformation("Found {CourseCount} available courses for branch {Branch}",
                availableCourses.Count, Branch);

            // Filter out courses the student is already enrolled in
            var enrolledCourseIds = enrolledCourses.Select(e => e.CourseCode).ToList();
            availableCourses = availableCourses.Where(c => !enrolledCourseIds.Contains(c.CourseCode)).ToList();
            _logger.LogInformation("{CourseCount} courses available after filtering out enrolled courses",
                availableCourses.Count);

            // Check for timetable conflicts
            bool checkForConflicts = CurrentTimetable.Any();
            _logger.LogInformation("Checking for timetable conflicts: {CheckConflicts}", checkForConflicts);

            // Filter courses based on selected days and time slots
            MatchingCourses = new List<CourseOffering>();

            foreach (var course in availableCourses)
            {
                bool scheduleMatches = IsScheduleMatching(course.Schedule, SelectedDays, SelectedTimeSlots);
                bool noConflict = true;

                // Check for conflicts with existing timetable if needed
                if (checkForConflicts && scheduleMatches)
                {
                    noConflict = !HasTimetableConflict(course.Schedule, CurrentTimetable);
                }

                if (scheduleMatches && noConflict)
                {
                    _logger.LogInformation("Adding matching course: {CourseCode} - {CourseName}",
                        course.CourseCode, course.CourseName);
                    MatchingCourses.Add(course);
                }
                else if (scheduleMatches && !noConflict)
                {
                    _logger.LogInformation("Course {CourseCode} matches schedule but conflicts with existing timetable",
                        course.CourseCode);
                }
            }

            _logger.LogInformation("Found {MatchCount} matching courses", MatchingCourses.Count);

            HasSearched = true;
            return Page();
        }

        private async Task SaveTimetableToDatabase()
        {
            try
            {
                _logger.LogInformation("Saving timetable to database for student: {StudentId}", StudentId);

                // First, delete any existing timetable entries for this student
                using (var connection = new MySql.Data.MySqlClient.MySqlConnection(_context.Database.GetConnectionString()))
                {
                    await connection.OpenAsync();

                    // Delete existing entries
                    using (var command = new MySql.Data.MySqlClient.MySqlCommand(
                        "DELETE FROM TimetableEntries WHERE StudentId = @StudentId", connection))
                    {
                        command.Parameters.AddWithValue("@StudentId", StudentId);
                        await command.ExecuteNonQueryAsync();
                        _logger.LogInformation("Deleted existing timetable entries for student: {StudentId}", StudentId);
                    }

                    // Insert new entries
                    foreach (var entry in CurrentTimetable)
                    {
                        using (var command = new MySql.Data.MySqlClient.MySqlCommand(
                            @"INSERT INTO TimetableEntries 
                              (StudentId, CourseCode, CourseName, Day, TimeSlot, Room) 
                              VALUES (@StudentId, @CourseCode, @CourseName, @Day, @TimeSlot, @Room)",
                            connection))
                        {
                            command.Parameters.AddWithValue("@StudentId", StudentId);
                            command.Parameters.AddWithValue("@CourseCode", entry.CourseCode);
                            command.Parameters.AddWithValue("@CourseName", entry.CourseName);
                            command.Parameters.AddWithValue("@Day", entry.Day);
                            command.Parameters.AddWithValue("@TimeSlot", entry.TimeSlot);
                            command.Parameters.AddWithValue("@Room", entry.Room);

                            await command.ExecuteNonQueryAsync();
                            _logger.LogInformation("Inserted timetable entry: {Day}, {TimeSlot}, {CourseCode}",
                                entry.Day, entry.TimeSlot, entry.CourseCode);
                        }
                    }
                }

                _logger.LogInformation("Successfully saved timetable to database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving timetable to database: {Message}", ex.Message);
                throw;
            }
        }

        private void GenerateTimetable()
        {
            _logger.LogInformation("Generating timetable for {CourseCount} courses", EnrolledCourseDetails.Count);

            // Clear existing timetable
            CurrentTimetable.Clear();

            foreach (var course in EnrolledCourseDetails)
            {
                _logger.LogInformation("Processing schedule for course: {CourseCode}", course.CourseCode);

                if (string.IsNullOrEmpty(course.Schedule))
                {
                    _logger.LogWarning("Course {CourseCode} has no schedule", course.CourseCode);
                    continue;
                }

                // Parse the schedule (e.g., "Mon,Wed,Fri 10:00-11:00")
                var scheduleParts = course.Schedule.Split(' ');
                if (scheduleParts.Length != 2)
                {
                    _logger.LogWarning("Invalid schedule format for course {CourseCode}: {Schedule}",
                        course.CourseCode, course.Schedule);
                    continue;
                }

                var daysPart = scheduleParts[0]; // "Mon,Wed,Fri"
                var timePart = scheduleParts[1]; // "10:00-11:00"

                // Process each day
                var days = daysPart.Split(',');
                foreach (var day in days)
                {
                    string fullDayName = GetFullDayName(day);

                    // Add to timetable
                    CurrentTimetable.Add(new TimetableEntry
                    {
                        Day = fullDayName,
                        TimeSlot = timePart,
                        CourseCode = course.CourseCode,
                        CourseName = course.CourseName,
                        Room = course.Room
                    });

                    _logger.LogInformation("Added timetable entry: {Day}, {TimeSlot}, {CourseCode}, {Room}",
                        fullDayName, timePart, course.CourseCode, course.Room);
                }
            }

            // Sort the timetable by day and time slot for better display
            CurrentTimetable = CurrentTimetable
                .OrderBy(t => t.Day)
                .ThenBy(t => t.TimeSlot)
                .ToList();
        }

        private bool HasTimetableConflict(string courseSchedule, List<TimetableEntry> timetable)
        {
            // Parse the course schedule (e.g., "Mon,Wed,Fri 10:00-11:00")
            var scheduleParts = courseSchedule.Split(' ');
            if (scheduleParts.Length != 2)
                return false;

            var daysPart = scheduleParts[0]; // "Mon,Wed,Fri"
            var timePart = scheduleParts[1]; // "10:00-11:00"

            // Get the days for this course
            var courseDays = daysPart.Split(',')
                .Select(day => GetFullDayName(day))
                .ToList();

            // Check for conflicts with existing timetable entries
            foreach (var entry in timetable)
            {
                if (courseDays.Contains(entry.Day) && entry.TimeSlot == timePart)
                {
                    // Found a conflict
                    return true;
                }
            }

            return false;
        }

        private bool IsScheduleMatching(string schedule, List<string> selectedDays, List<string> selectedTimeSlots)
        {
            if (string.IsNullOrEmpty(schedule))
                return false;

            // Parse the schedule (e.g., "Mon,Wed,Fri 10:00-11:00")
            var scheduleParts = schedule.Split(' ');
            if (scheduleParts.Length != 2)
                return false;

            var daysPart = scheduleParts[0]; // "Mon,Wed,Fri"
            var timePart = scheduleParts[1]; // "10:00-11:00"

            // Check if any of the course days match the selected days
            var courseDays = daysPart.Split(',');
            bool dayMatches = false;

            foreach (var day in courseDays)
            {
                string fullDayName = GetFullDayName(day);
                if (selectedDays.Contains(fullDayName))
                {
                    dayMatches = true;
                    break;
                }
            }

            if (!dayMatches)
                return false;

            // Check if the course time slot matches any of the selected time slots
            return selectedTimeSlots.Contains(timePart);
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
}