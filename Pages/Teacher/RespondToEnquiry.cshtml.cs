using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentEnrollmentSystem1.Data.Repositories;
using StudentEnrollmentSystem1.Models;
using System;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem1.Pages.Teacher
{
    public class RespondToEnquiryModel : PageModel
    {
        private readonly StudentEnquiryRepository _enquiryRepository;

        public RespondToEnquiryModel(StudentEnquiryRepository enquiryRepository)
        {
            _enquiryRepository = enquiryRepository;
        }

        [BindProperty(SupportsGet = true)]
        public int EnquiryId { get; set; }

        [BindProperty]
        public string Response { get; set; }

        public StudentEnquiry Enquiry { get; set; }
        public string TeacherName { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if teacher is logged in
            if (HttpContext.Session.GetString("TeacherId") == null)
            {
                return RedirectToPage("/Teacher/Login");
            }

            // Get teacher name from session
            TeacherName = HttpContext.Session.GetString("TeacherName");

            if (EnquiryId <= 0)
            {
                TempData["ErrorMessage"] = "Invalid enquiry ID.";
                return RedirectToPage("/Teacher/Dashboard");
            }

            // Get the enquiry details
            Enquiry = await _enquiryRepository.GetEnquiryByIdAsync(EnquiryId);

            if (Enquiry == null || Enquiry.Status == "Resolved")
            {
                TempData["ErrorMessage"] = "The requested enquiry was not found or has already been resolved.";
                return RedirectToPage("/Teacher/Dashboard");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Check if teacher is logged in
            if (HttpContext.Session.GetString("TeacherId") == null)
            {
                return RedirectToPage("/Teacher/Login");
            }

            // Get teacher name from session
            TeacherName = HttpContext.Session.GetString("TeacherName");

            if (EnquiryId <= 0)
            {
                TempData["ErrorMessage"] = "Invalid enquiry ID.";
                return RedirectToPage("/Teacher/Dashboard");
            }

            if (string.IsNullOrEmpty(Response))
            {
                TempData["ErrorMessage"] = "Response is required.";
                return RedirectToPage();
            }

            try
            {
                // Respond to the enquiry
                var result = await _enquiryRepository.RespondToEnquiryAsync(EnquiryId, Response, TeacherName);

                if (result)
                {
                    TempData["SuccessMessage"] = "Your response has been submitted successfully.";
                    return RedirectToPage("/Teacher/Dashboard");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to submit response. The enquiry may not exist or has already been resolved.";
                    return RedirectToPage();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while submitting your response: {ex.Message}. Please try again.";
                return RedirectToPage();
            }
        }
    }
}