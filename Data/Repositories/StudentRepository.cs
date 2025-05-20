using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentEnrollmentSystem1.Models;
using System;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem1.Data.Repositories
{
    public class StudentRepository : Repository<Student>
    {
        private readonly ILogger<StudentRepository> _logger;

        public StudentRepository(ApplicationDbContext context, ILogger<StudentRepository> logger = null) : base(context)
        {
            _logger = logger;
        }

        public async Task<Student> GetStudentByIdAsync(string studentId)
        {
            _logger?.LogInformation($"Getting student by ID: {studentId}");
            return await _dbSet.FirstOrDefaultAsync(s => s.StudentId == studentId);
        }

        public async Task<Student> GetStudentByEmailAsync(string email)
        {
            _logger?.LogInformation($"Getting student by email: {email}");
            return await _dbSet.FirstOrDefaultAsync(s => s.Email == email);
        }

        public async Task<bool> UpdatePaymentStatusAsync(string studentId, decimal amountPaid)
        {
            _logger?.LogInformation($"Updating payment status for student {studentId} with amount {amountPaid}");
            var student = await GetStudentByIdAsync(studentId);
            if (student == null)
            {
                _logger?.LogWarning($"Student with ID {studentId} not found for payment update");
                return false;
            }

            try
            {
                _logger?.LogInformation($"Before update: Student {studentId} payment status: {student.PaymentStatus}, amount paid: {student.AmountPaid}");

                student.PaymentStatus = true;
                student.AmountPaid = amountPaid;

                _logger?.LogInformation($"After update (before save): Student {studentId} payment status: {student.PaymentStatus}, amount paid: {student.AmountPaid}");

                await SaveChangesAsync();

                // Verify the update
                var updatedStudent = await GetStudentByIdAsync(studentId);
                if (updatedStudent != null)
                {
                    _logger?.LogInformation($"After save: Student {studentId} payment status: {updatedStudent.PaymentStatus}, amount paid: {updatedStudent.AmountPaid}");
                }

                _logger?.LogInformation($"Payment status updated successfully for student {studentId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error updating payment status for student {studentId}");
                return false;
            }
        }

        public async Task<string> GenerateStudentIdAsync()
        {
            _logger?.LogInformation("Generating new student ID");
            string prefix = "IU";
            Random random = new Random();
            string studentId;
            bool exists;

            do
            {
                // Generate a random 5-digit number
                int randomNumber = random.Next(10000, 100000);
                studentId = $"{prefix}{randomNumber}";

                // Check if this ID already exists
                exists = await _dbSet.AnyAsync(s => s.StudentId == studentId);
            } while (exists);

            _logger?.LogInformation($"Generated student ID: {studentId}");
            return studentId;
        }

        public override async Task AddAsync(Student student)
        {
            _logger?.LogInformation($"Adding new student: {student.StudentName}, {student.Email}, {student.Course}");
            try
            {
                await _dbSet.AddAsync(student);
                _logger?.LogInformation($"Student added to context: {student.StudentId}");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error adding student {student.StudentName} to context");
                throw;
            }
        }

        public override async Task SaveChangesAsync()
        {
            _logger?.LogInformation("Saving changes to database");
            try
            {
                await _context.SaveChangesAsync();
                _logger?.LogInformation("Changes saved successfully");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error saving changes to database");
                throw;
            }
        }

        public async Task<int> GetStudentCountAsync()
        {
            _logger?.LogInformation("Getting student count");
            return await _dbSet.CountAsync();
        }

        public async Task<bool> UpdateStudentAsync(Student student)
        {
            _logger?.LogInformation($"Updating student: {student.StudentId}, {student.StudentName}");
            try
            {
                _dbSet.Update(student);
                await SaveChangesAsync();
                _logger?.LogInformation($"Student {student.StudentId} updated successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error updating student {student.StudentId}");
                return false;
            }
        }
    }
}