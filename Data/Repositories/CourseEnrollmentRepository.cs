using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem1.Data.Repositories
{
    public class CourseEnrollmentRepository : Repository<CourseEnrollment>
    {
        private readonly CourseOfferingRepository _courseOfferingRepository;

        public CourseEnrollmentRepository(
            ApplicationDbContext context,
            CourseOfferingRepository courseOfferingRepository
            )
            : base(context)
        {
            _courseOfferingRepository = courseOfferingRepository;
        }

        public async Task<List<CourseEnrollment>> GetStudentEnrollmentsAsync(string studentId, bool activeOnly = true)
        {
            var query = _dbSet.Where(e => e.StudentId == studentId);

            if (activeOnly)
                query = query.Where(e => e.IsActive);

            return await query.ToListAsync();
        }

        public async Task<int> GetStudentTotalCreditsAsync(string studentId)
        {
            var activeEnrollments = await _dbSet
                .Where(e => e.StudentId == studentId && e.IsActive)
                .ToListAsync();

            return activeEnrollments.Sum(e => e.Credits);
        }

        public async Task<bool> EnrollCourseAsync(string studentId, string courseCode)
        {
            // Check if student is already enrolled in this course
            var existingEnrollment = await _dbSet.FirstOrDefaultAsync(
                e => e.StudentId == studentId && e.CourseCode == courseCode && e.IsActive);

            if (existingEnrollment != null)
                return false;

            // Get course details
            var courseOffering = await _courseOfferingRepository.GetCourseOfferingByCodeAsync(courseCode);
            if (courseOffering == null)
                return false;

            // Check if course has available slots
            if (courseOffering.CurrentEnrollment >= courseOffering.MaxEnrollment)
                return false;

            // Create new enrollment
            var enrollment = new CourseEnrollment
            {
                StudentId = studentId,
                CourseCode = courseCode,
                CourseName = courseOffering.CourseName,
                Credits = courseOffering.Credits,
                EnrollmentDate = DateTime.Now,
                IsActive = true
            };

            await AddAsync(enrollment);

            // Update course enrollment count
            await _courseOfferingRepository.UpdateEnrollmentCountAsync(courseCode, 1);

            await SaveChangesAsync();
            return true;
        }

        public async Task<bool> DropCourseAsync(string studentId, string courseCode)
        {
            // Find the active enrollment
            var enrollment = await _dbSet.FirstOrDefaultAsync(
                e => e.StudentId == studentId && e.CourseCode == courseCode && e.IsActive);

            if (enrollment == null)
                return false;

            // Mark as dropped
            enrollment.IsActive = false;
            enrollment.DropDate = DateTime.Now;

            // Update course enrollment count
            await _courseOfferingRepository.UpdateEnrollmentCountAsync(courseCode, -1);

            await SaveChangesAsync();
            return true;
        }

        public async Task<List<CourseEnrollment>> GetEnrollmentHistoryAsync(string studentId)
        {
            return await _dbSet
                .Where(e => e.StudentId == studentId)
                .OrderByDescending(e => e.EnrollmentDate)
                .ToListAsync();
        }
    }
}