using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentEnrollmentSystem1.Data.Repositories;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem1.Pages.Teacher
{
    public class LoginModel : PageModel
    {
        private readonly TeacherRepository _teacherRepository;

        public LoginModel(TeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }

        [BindProperty]
        public LoginInput LoginData { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Validate teacher credentials
            bool isValid = await _teacherRepository.ValidateTeacherCredentialsAsync(LoginData.Email, LoginData.Password);

            if (!isValid)
            {
                ErrorMessage = "Invalid email or password.";
                return Page();
            }

            // Get teacher details
            var teacher = await _teacherRepository.GetTeacherByEmailAsync(LoginData.Email);

            // Store teacher ID and name in session
            HttpContext.Session.SetString("TeacherId", teacher.TeacherId);
            HttpContext.Session.SetString("TeacherName", teacher.TeacherName);
            HttpContext.Session.SetString("Department", teacher.Department);

            return RedirectToPage("/Teacher/Dashboard");
        }
    }

    public class LoginInput
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}