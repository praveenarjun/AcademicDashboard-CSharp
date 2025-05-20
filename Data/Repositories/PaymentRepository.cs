using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem1.Data.Repositories
{
    public class PaymentRepository : Repository<Payment>
    {
        public PaymentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Payment>> GetPaymentsByStudentIdAsync(string studentId)
        {
            return await _dbSet
                .Where(p => p.StudentId == studentId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<Payment> GetPaymentByIdAsync(int paymentId)
        {
            return await _dbSet.FindAsync(paymentId);
        }

        public async Task<Payment> CreatePaymentAsync(string studentId, decimal amount, string description)
        {
            // Generate a unique receipt number
            string receiptNumber = $"REC-{DateTime.Now.Year}-{DateTime.Now.Month}-{Guid.NewGuid().ToString().Substring(0, 8)}";
            
            // Create a new payment record
            var payment = new Payment
            {
                StudentId = studentId,
                Amount = amount,
                PaymentDate = DateTime.Now,
                PaymentMethod = "Online",
                PaymentStatus = "Completed",
                TransactionId = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                Description = description,
                ReceiptNumber = receiptNumber
            };
            
            await _dbSet.AddAsync(payment);
            await SaveChangesAsync();
            
            return payment;
        }
    }
}