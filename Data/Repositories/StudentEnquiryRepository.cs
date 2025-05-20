using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem1.Data.Repositories
{
    public class StudentEnquiryRepository : Repository<StudentEnquiry>
    {
        public StudentEnquiryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<StudentEnquiry>> GetEnquiriesByStudentIdAsync(string studentId)
        {
            return await _dbSet
                .Where(e => e.StudentId == studentId)
                .OrderByDescending(e => e.SubmissionDate)
                .Include(e => e.Student)
                .ToListAsync();
        }

        public async Task<List<StudentEnquiry>> GetAllPendingEnquiriesAsync()
        {
            return await _dbSet
                .Where(e => e.Status == "Pending")
                .OrderByDescending(e => e.SubmissionDate)
                .Include(e => e.Student)
                .ToListAsync();
        }

        public async Task<StudentEnquiry> GetEnquiryByIdAsync(int enquiryId)
        {
            return await _dbSet
                .Include(e => e.Student)
                .FirstOrDefaultAsync(e => e.EnquiryId == enquiryId);
        }

        public async Task<StudentEnquiry> CreateEnquiryAsync(string studentId, string enquiryType, string subject, string message)
        {
            var enquiry = new StudentEnquiry
            {
                StudentId = studentId,
                EnquiryType = enquiryType,
                Subject = subject,
                Message = message,
                SubmissionDate = DateTime.Now,
                Status = "Pending"
            };

            await _dbSet.AddAsync(enquiry);
            await SaveChangesAsync();

            return enquiry;
        }

        public async Task<bool> RespondToEnquiryAsync(int enquiryId, string response, string? respondedBy)
        {
            var enquiry = await _dbSet.FindAsync(enquiryId);
            if (enquiry == null)
                return false;

            enquiry.Response = response;
            enquiry.ResponseDate = DateTime.Now;
            enquiry.RespondedBy = respondedBy ?? "Unknown";
            enquiry.Status = "Resolved";

            await SaveChangesAsync();
            return true;
        }
    }
}