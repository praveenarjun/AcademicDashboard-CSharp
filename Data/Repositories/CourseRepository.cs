using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentEnrollmentSystem1.Data.Repositories
{
    public class CourseRepository : Repository<Course>
    {
        public CourseRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Course> GetCourseByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task SeedCoursesAsync()
        {
            if (!await _dbSet.AnyAsync())
            {
                var courses = CourseData.GetCourses();
                await _dbSet.AddRangeAsync(courses);
                await _context.SaveChangesAsync();
            }
        }
    }
}