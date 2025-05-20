using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using StudentEnrollmentSystem1.Data;

namespace StudentEnrollmentSystem1.Models
{
    [Table("Courses")]
    public class Course
    {
        [Key]
        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Fee { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string Description { get; set; }
    }

    public static class CourseData
    {
        public static List<Course> GetCourses()
        {
            return new List<Course>
            {
                new Course { Name = "BTech", Fee = 120000, Description = "Bachelor of Technology" },
                new Course { Name = "BBA", Fee = 80000, Description = "Bachelor of Business Administration" },
                new Course { Name = "BCA", Fee = 70000, Description = "Bachelor of Computer Applications" },
                new Course { Name = "MBA", Fee = 150000, Description = "Master of Business Administration" },
                new Course { Name = "MCA", Fee = 100000, Description = "Master of Computer Applications" }
            };
        }

        public static void SeedCourses(ApplicationDbContext context)
        {
            if (!context.Courses.Any())
            {
                context.Courses.AddRange(GetCourses());
                context.SaveChanges();
            }
        }
    }
}