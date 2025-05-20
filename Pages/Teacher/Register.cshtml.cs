using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using StudentEnrollmentSystem1.Data.Repositories;
using StudentEnrollmentSystem1.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem1.Pages.Teacher
{
    public class RegisterModel : PageModel
    {
        private readonly TeacherRepository _teacherRepository;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(TeacherRepository teacherRepository, ILogger<RegisterModel> logger)
        {
            _teacherRepository = teacherRepository;
            _logger = logger;
        }

        [BindProperty]
        public Models.Teacher Teacher { get; set; }

        public List<SelectListItem> DepartmentOptions { get; set; }

        public void OnGet()
        {
            _logger.LogInformation("Teacher registration page loaded");
            PopulateDepartmentOptions();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Teacher registration form submitted");

            // Remove TeacherId from ModelState validation since it will be generated
            ModelState.Remove("Teacher.TeacherId");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning("Validation error: {ErrorMessage}", error.ErrorMessage);
                }

                PopulateDepartmentOptions();
                return Page();
            }

            try
            {
                _logger.LogInformation("Checking if email already exists: {Email}", Teacher.Email);

                // Check if email already exists
                var existingTeacher = await _teacherRepository.GetTeacherByEmailAsync(Teacher.Email);
                if (existingTeacher != null)
                {
                    _logger.LogWarning("Email already exists: {Email}", Teacher.Email);
                    ModelState.AddModelError("Teacher.Email", "This email is already registered.");
                    PopulateDepartmentOptions();
                    return Page();
                }

                _logger.LogInformation("Registering new teacher: {Name}, {Email}, {Department}",
                    Teacher.TeacherName, Teacher.Email, Teacher.Department);

                // Register the teacher
                var teacherId = await _teacherRepository.RegisterTeacherAsync(Teacher);

                _logger.LogInformation("Teacher registered successfully with ID: {TeacherId}", teacherId);

                // Redirect to login page
                TempData["SuccessMessage"] = "Registration successful! You can now login.";
                return RedirectToPage("/Teacher/Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during teacher registration: {Message}", ex.Message);
                ModelState.AddModelError(string.Empty, "An error occurred during registration. Please try again.");
                PopulateDepartmentOptions();
                return Page();
            }
        }

        private void PopulateDepartmentOptions()
        {
            DepartmentOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Computer Science", Text = "Computer Science" },
                new SelectListItem { Value = "Business Administration", Text = "Business Administration" },
                new SelectListItem { Value = "Engineering", Text = "Engineering" },
                new SelectListItem { Value = "Mathematics", Text = "Mathematics" },
                new SelectListItem { Value = "Physics", Text = "Physics" },
                new SelectListItem { Value = "Chemistry", Text = "Chemistry" },
                new SelectListItem { Value = "Biology", Text = "Biology" },
                new SelectListItem { Value = "Economics", Text = "Economics" },
                new SelectListItem { Value = "Psychology", Text = "Psychology" },
                new SelectListItem { Value = "Sociology", Text = "Sociology" }
            };
        }
    }
}