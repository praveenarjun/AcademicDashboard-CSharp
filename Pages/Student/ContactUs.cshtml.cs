using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentEnrollmentSystem1.Data.Repositories;
using StudentEnrollmentSystem1.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem1.Pages.Student
{
    public class ContactUsModel : PageModel
    {
        private readonly StudentRepository _studentRepository;
        private readonly StudentEnquiryRepository _enquiryRepository;

        public ContactUsModel(
            StudentRepository studentRepository,
            StudentEnquiryRepository enquiryRepository)
        {
            _studentRepository = studentRepository;
            _enquiryRepository = enquiryRepository;
        }

        [BindProperty(SupportsGet = true)]
        public string StudentId { get; set; }

        [BindProperty]
        public string EnquiryType { get; set; }

        [BindProperty]
        public string Subject { get; set; }

        [BindProperty]
        public string Message { get; set; }

        public string StudentName { get; set; }
        public List<StudentEnquiry> PreviousEnquiries { get; set; } = new List<StudentEnquiry>();

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(StudentId))
            {
                // Try to get student ID from TempData
                if (TempData != null && TempData.ContainsKey("StudentId"))
                {
                    StudentId = TempData["StudentId"].ToString();
                    TempData.Keep("StudentId");
                }
                else
                {
                    // If no student ID, redirect to login
                    return RedirectToPage("/Student/Login");
                }
            }

            // Get student details
            var student = await _studentRepository.GetStudentByIdAsync(StudentId);
            if (student == null)
            {
                TempData["ErrorMessage"] = "Student not found.";
                return RedirectToPage("/Index");
            }

            // Set student information
            StudentName = student.StudentName;

            // Get previous enquiries
            PreviousEnquiries = await _enquiryRepository.GetEnquiriesByStudentIdAsync(StudentId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(StudentId))
            {
                TempData["ErrorMessage"] = "Student ID is required.";
                return RedirectToPage();
            }

            if (string.IsNullOrEmpty(EnquiryType) || string.IsNullOrEmpty(Subject) || string.IsNullOrEmpty(Message))
            {
                TempData["ErrorMessage"] = "All fields are required.";
                return RedirectToPage();
            }

            try
            {
                // Create the enquiry
                var enquiry = await _enquiryRepository.CreateEnquiryAsync(StudentId, EnquiryType, Subject, Message);

                TempData["SuccessMessage"] = "Your enquiry has been submitted successfully. We will get back to you soon.";

                // Clear form fields
                EnquiryType = string.Empty;
                Subject = string.Empty;
                Message = string.Empty;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while submitting your enquiry. Please try again.";
            }

            return RedirectToPage();
        }
    }
}