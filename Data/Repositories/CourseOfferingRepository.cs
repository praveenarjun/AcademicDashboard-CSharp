using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem1.Data.Repositories
{
    public class CourseOfferingRepository : Repository<CourseOffering>
    {
        public CourseOfferingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<CourseOffering> GetCourseOfferingByCodeAsync(string courseCode)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.CourseCode == courseCode);
        }

        public async Task<List<CourseOffering>> GetCourseOfferingsByBranchAsync(string branch)
        {
            return await _dbSet.Where(c => c.Branch == branch && c.IsActive).ToListAsync();
        }

        public async Task<bool> UpdateEnrollmentCountAsync(string courseCode, int change)
        {
            var course = await GetCourseOfferingByCodeAsync(courseCode);
            if (course == null)
                return false;

            // Check if adding students would exceed max enrollment
            if (course.CurrentEnrollment + change > course.MaxEnrollment && change > 0)
                return false;

            course.CurrentEnrollment += change;
            await SaveChangesAsync();
            return true;
        }

        public async Task SeedCourseOfferingsAsync()
        {
            if (!await _dbSet.AnyAsync())
            {
                var courseOfferings = CourseOfferingData.GetCourseOfferings();
                await _dbSet.AddRangeAsync(courseOfferings);
                await SaveChangesAsync();
            }
        }

        public async Task<List<CourseOffering>> GetAllCourseOfferingsAsync()
        {
            return await _dbSet.Where(c => c.IsActive).ToListAsync();
        }
        
        public async Task<List<CourseOffering>> GetCourseOfferingsByDepartmentAsync(string department)
        {
            // Map department to branch
            string branch = MapDepartmentToBranch(department);
            
            // Get courses for the specified branch
            return await _dbSet.Where(c => c.Branch == branch && c.IsActive).ToListAsync();
        }
        
        private string MapDepartmentToBranch(string department)
        {
            // Map department names to branch codes
            switch (department.ToLower())
            {
                case "computer science":
                case "information technology":
                case "engineering":
                    return "BTech";
                case "business":
                case "management":
                case "finance":
                case "marketing":
                    return "BBA";
                case "arts":
                case "humanities":
                    return "BA";
                case "science":
                    return "BSc";
                default:
                    // Default to all branches if department doesn't match
                    return department;
            }
        }
    }
}