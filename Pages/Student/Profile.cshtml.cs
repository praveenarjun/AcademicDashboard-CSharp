using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using StudentEnrollmentSystem1.Data.Repositories;
using StudentEnrollmentSystem1.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem1.Pages.Student
{
    public class ProfileModel : PageModel
    {
        private readonly StudentRepository _studentRepository;
        private readonly StudentBankDetailsRepository _bankDetailsRepository;
        private readonly ILogger<ProfileModel> _logger;

        public ProfileModel(
            StudentRepository studentRepository,
            StudentBankDetailsRepository bankDetailsRepository,
            ILogger<ProfileModel> logger)
        {
            _studentRepository = studentRepository;
            _bankDetailsRepository = bankDetailsRepository;
            _logger = logger;
        }

        [BindProperty]
        public string StudentId { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Student name is required")]
        [Display(Name = "Full Name")]
        public string StudentName { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Father's name is required")]
        public string FatherName { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Mother's name is required")]
        public string MotherName { get; set; }

        public string Branch { get; set; }
        public DateTime RegistrationDate { get; set; }

        // Password change properties
        [BindProperty]
        [Required(ErrorMessage = "Current password is required")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        // Bank details properties
        [BindProperty]
        [Required(ErrorMessage = "Bank name is required")]
        public string BankName { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Account number is required")]
        [RegularExpression(@"^\d{9,18}$", ErrorMessage = "Account number must be between 9 and 18 digits")]
        public string AccountNumber { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "IFSC code is required")]
        [RegularExpression(@"^[A-Z]{4}0[A-Z0-9]{6}$", ErrorMessage = "Invalid IFSC code format")]
        public string IFSC { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Account holder name is required")]
        public string AccountHolderName { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if student is logged in
            StudentId = HttpContext.Session.GetString("StudentId");
            if (string.IsNullOrEmpty(StudentId))
            {
                return RedirectToPage("/Student/Login");
            }

            // Get student details
            var student = await _studentRepository.GetStudentByIdAsync(StudentId);
            if (student == null)
            {
                ErrorMessage = "Student not found.";
                return RedirectToPage("/Student/Login");
            }

            // Set student information
            StudentName = student.StudentName;
            Email = student.Email;
            PhoneNumber = student.PhoneNumber;
            FatherName = student.FatherName;
            MotherName = student.MotherName;
            Branch = student.Course;
            RegistrationDate = student.RegistrationDate;

            // Get bank details if available
            var bankDetails = await _bankDetailsRepository.GetBankDetailsByStudentIdAsync(StudentId);
            if (bankDetails != null)
            {
                BankName = bankDetails.BankName;
                AccountNumber = bankDetails.AccountNumber;
                IFSC = bankDetails.IFSC;
                AccountHolderName = bankDetails.AccountHolderName;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateProfileAsync()
        {
            try
            {
                // Check if student is logged in
                if (string.IsNullOrEmpty(StudentId))
                {
                    StudentId = HttpContext.Session.GetString("StudentId");
                    if (string.IsNullOrEmpty(StudentId))
                    {
                        return RedirectToPage("/Student/Login");
                    }
                }

                // Validate model
                if (!ModelState.IsValid)
                {
                    // Filter validation errors to only include profile fields
                    var profileKeys = new[] { "StudentName", "Email", "PhoneNumber", "FatherName", "MotherName" };
                    foreach (var key in ModelState.Keys)
                    {
                        if (!Array.Exists(profileKeys, k => k == key) && ModelState.ContainsKey(key))
                        {
                            ModelState[key].Errors.Clear();
                        }
                    }

                    if (!ModelState.IsValid)
                    {
                        ErrorMessage = "Please correct the errors in the form.";
                        return Page();
                    }
                }

                // Get current student data
                var student = await _studentRepository.GetStudentByIdAsync(StudentId);
                if (student == null)
                {
                    ErrorMessage = "Student not found.";
                    return RedirectToPage("/Student/Login");
                }

                // Update student information
                student.StudentName = StudentName;
                student.Email = Email;
                student.PhoneNumber = PhoneNumber;
                student.FatherName = FatherName;
                student.MotherName = MotherName;

                // Save changes
                await _studentRepository.UpdateStudentAsync(student);

                // Update session data if name changed
                if (HttpContext.Session.GetString("StudentName") != StudentName)
                {
                    HttpContext.Session.SetString("StudentName", StudentName);
                }

                SuccessMessage = "Profile updated successfully.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student profile: {Message}", ex.Message);
                ErrorMessage = "An error occurred while updating your profile. Please try again.";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            try
            {
                // Check if student is logged in
                if (string.IsNullOrEmpty(StudentId))
                {
                    StudentId = HttpContext.Session.GetString("StudentId");
                    if (string.IsNullOrEmpty(StudentId))
                    {
                        return RedirectToPage("/Student/Login");
                    }
                }

                // Validate model for password fields only
                if (!ModelState.IsValid)
                {
                    // Filter validation errors to only include password fields
                    var passwordKeys = new[] { "CurrentPassword", "NewPassword", "ConfirmPassword" };
                    foreach (var key in ModelState.Keys)
                    {
                        if (!Array.Exists(passwordKeys, k => k == key) && ModelState.ContainsKey(key))
                        {
                            ModelState[key].Errors.Clear();
                        }
                    }

                    if (!ModelState.IsValid)
                    {
                        ErrorMessage = "Please correct the errors in the password form.";
                        return Page();
                    }
                }

                // Get current student data
                var student = await _studentRepository.GetStudentByIdAsync(StudentId);
                if (student == null)
                {
                    ErrorMessage = "Student not found.";
                    return RedirectToPage("/Student/Login");
                }

                // Verify current password
                if (student.Password != CurrentPassword)
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                    ErrorMessage = "Current password is incorrect.";
                    return Page();
                }

                // Update password
                student.Password = NewPassword;

                // Save changes
                await _studentRepository.UpdateStudentAsync(student);

                SuccessMessage = "Password changed successfully.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing student password: {Message}", ex.Message);
                ErrorMessage = "An error occurred while changing your password. Please try again.";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostUpdateBankDetailsAsync()
        {
            try
            {
                // Check if student is logged in
                if (string.IsNullOrEmpty(StudentId))
                {
                    StudentId = HttpContext.Session.GetString("StudentId");
                    if (string.IsNullOrEmpty(StudentId))
                    {
                        return RedirectToPage("/Student/Login");
                    }
                }

                // Validate model for bank details fields only
                if (!ModelState.IsValid)
                {
                    // Filter validation errors to only include bank details fields
                    var bankKeys = new[] { "BankName", "AccountNumber", "IFSC", "AccountHolderName" };
                    foreach (var key in ModelState.Keys)
                    {
                        if (!Array.Exists(bankKeys, k => k == key) && ModelState.ContainsKey(key))
                        {
                            ModelState[key].Errors.Clear();
                        }
                    }

                    if (!ModelState.IsValid)
                    {
                        ErrorMessage = "Please correct the errors in the bank details form.";
                        return Page();
                    }
                }

                // Check if bank details already exist
                var bankDetails = await _bankDetailsRepository.GetBankDetailsByStudentIdAsync(StudentId);
                
                if (bankDetails == null)
                {
                    // Create new bank details
                    bankDetails = new StudentBankDetails
                    {
                        StudentId = StudentId,
                        BankName = BankName,
                        AccountNumber = AccountNumber,
                        IFSC = IFSC,
                        AccountHolderName = AccountHolderName
                    };

                    await _bankDetailsRepository.CreateBankDetailsAsync(bankDetails);
                }
                else
                {
                    // Update existing bank details
                    bankDetails.BankName = BankName;
                    bankDetails.AccountNumber = AccountNumber;
                    bankDetails.IFSC = IFSC;
                    bankDetails.AccountHolderName = AccountHolderName;

                    await _bankDetailsRepository.UpdateBankDetailsAsync(bankDetails);
                }

                SuccessMessage = "Bank details updated successfully.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student bank details: {Message}", ex.Message);
                ErrorMessage = "An error occurred while updating your bank details. Please try again.";
                return Page();
            }
        }
    }
}