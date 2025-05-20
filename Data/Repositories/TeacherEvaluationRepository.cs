using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem1.Data.Repositories
{
    public class TeacherEvaluationRepository : Repository<TeacherEvaluation>
    {
        public TeacherEvaluationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<TeacherEvaluation>> GetEvaluationsByStudentIdAsync(string studentId)
        {
            return await _dbSet
                .Where(e => e.StudentId == studentId)
                .OrderByDescending(e => e.SubmissionDate)
                .ToListAsync();
        }

        public async Task<List<TeacherEvaluation>> GetStudentEvaluationsAsync(string studentId)
        {
            return await _dbSet
                .Where(e => e.StudentId == studentId)
                .OrderByDescending(e => e.SubmissionDate)
                .Include(e => e.Student)
                .ToListAsync();
        }

        public async Task<TeacherEvaluation> GetEvaluationAsync(string studentId, string courseCode)
        {
            return await _dbSet
                .Include(e => e.Student)
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseCode == courseCode);
        }

        public async Task<List<TeacherEvaluation>> GetEvaluationsByInstructorAsync(string instructorName)
        {
            return await _dbSet
                .Where(e => e.InstructorName == instructorName)
                .OrderByDescending(e => e.SubmissionDate)
                .Include(e => e.Student)
                .ToListAsync();
        }

        public async Task<List<TeacherEvaluation>> GetEvaluationsByCourseCodeAsync(string courseCode)
        {
            return await _dbSet
                .Where(e => e.CourseCode == courseCode)
                .OrderByDescending(e => e.SubmissionDate)
                .Include(e => e.Student)
                .ToListAsync();
        }

        public async Task<TeacherEvaluation> CreateEvaluationAsync(
            string studentId,
            string courseCode,
            string instructorName,
            int teachingRating,
            int contentRating,
            int assessmentRating,
            int communicationRating,
            int overallRating,
            string comments,
            bool isAnonymous)
        {
            var evaluation = new TeacherEvaluation
            {
                StudentId = studentId,
                CourseCode = courseCode,
                InstructorName = instructorName,
                TeachingRating = teachingRating,
                ContentRating = contentRating,
                AssessmentRating = assessmentRating,
                CommunicationRating = communicationRating,
                OverallRating = overallRating,
                Comments = comments,
                SubmissionDate = DateTime.Now,
                IsAnonymous = isAnonymous
            };

            await _dbSet.AddAsync(evaluation);
            await SaveChangesAsync();

            return evaluation;
        }
    }
}