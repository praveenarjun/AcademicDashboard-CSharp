using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentEnrollmentSystem1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem1.Data.Repositories
{
    public class TeacherRepository : Repository<Teacher>
    {
        private readonly ILogger<TeacherRepository> _logger;

        public TeacherRepository(ApplicationDbContext context, ILogger<TeacherRepository> logger = null) : base(context)
        {
            _logger = logger;
        }

        public async Task<Teacher> GetTeacherByIdAsync(string teacherId)
        {
            return await _dbSet.FirstOrDefaultAsync(t => t.TeacherId == teacherId);
        }

        public async Task<Teacher> GetTeacherByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(t => t.Email == email);
        }

        public async Task<bool> ValidateTeacherCredentialsAsync(string email, string password)
        {
            var teacher = await GetTeacherByEmailAsync(email);
            if (teacher == null)
                return false;

            // In a real application, you would use a password hasher
            return teacher.Password == password;
        }

        public async Task<string> RegisterTeacherAsync(Teacher teacher)
        {
            try
            {
                _logger?.LogInformation("Registering teacher: {Email}, {Name}, {Department}",
                    teacher.Email, teacher.TeacherName, teacher.Department);

                // Generate a unique teacher ID
                teacher.TeacherId = await GenerateUniqueTeacherIdAsync();
                _logger?.LogInformation("Generated teacher ID: {TeacherId}", teacher.TeacherId);

                // Set registration date
                teacher.RegistrationDate = DateTime.Now;

                // Add to database
                await _dbSet.AddAsync(teacher);
                _logger?.LogInformation("Added teacher to DbSet");

                // Save changes
                await _context.SaveChangesAsync();
                _logger?.LogInformation("Saved changes to database");

                return teacher.TeacherId;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error registering teacher: {Message}", ex.Message);
                throw;
            }
        }

        private async Task<string> GenerateUniqueTeacherIdAsync()
        {
            string teacherId;
            bool exists;

            do
            {
                // Generate a random 3-digit number
                var random = new Random();
                var randomNumber = random.Next(100, 1000);
                teacherId = $"T{randomNumber}";

                // Check if it already exists
                exists = await _dbSet.AnyAsync(t => t.TeacherId == teacherId);
            } while (exists);

            return teacherId;
        }
    }
}