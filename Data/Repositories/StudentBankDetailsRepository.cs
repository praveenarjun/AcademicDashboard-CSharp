using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem1.Models;
using System;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem1.Data.Repositories
{
    public class StudentBankDetailsRepository : Repository<StudentBankDetails>
    {
        public StudentBankDetailsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<StudentBankDetails> GetBankDetailsByStudentIdAsync(string studentId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(b => b.StudentId == studentId);
        }

        public async Task<StudentBankDetails> CreateBankDetailsAsync(StudentBankDetails bankDetails)
        {
            bankDetails.LastUpdated = DateTime.Now;
            await _dbSet.AddAsync(bankDetails);
            await SaveChangesAsync();
            return bankDetails;
        }

        public async Task<bool> UpdateBankDetailsAsync(StudentBankDetails bankDetails)
        {
            bankDetails.LastUpdated = DateTime.Now;
            _dbSet.Update(bankDetails);
            await SaveChangesAsync();
            return true;
        }
    }
}