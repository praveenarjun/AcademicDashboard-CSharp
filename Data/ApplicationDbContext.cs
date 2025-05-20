using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentEnrollmentSystem1.Models;
using System;
using System.Linq;

namespace StudentEnrollmentSystem1.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ILoggerFactory loggerFactory = null)
            : base(options)
        {
            _loggerFactory = loggerFactory;
        }

        // Define DbSets for each entity
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseOffering> CourseOfferings { get; set; }
        public DbSet<CourseEnrollment> CourseEnrollments { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<StudentEnquiry> StudentEnquiries { get; set; }
        public DbSet<TeacherEvaluation> TeacherEvaluations { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<TimetableEntry> Timetables { get; set; }
        public DbSet<StudentBankDetails> StudentBankDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Add logging if logger factory is provided
            if (_loggerFactory != null)
            {
                optionsBuilder.UseLoggerFactory(_loggerFactory);
            }

            // Enable sensitive data logging in development
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                optionsBuilder.EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Student entity
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.StudentId);

                entity.Property(e => e.StudentId)
                    .HasColumnType("varchar(10)")
                    .IsRequired();

                entity.Property(e => e.StudentName)
                    .HasColumnType("varchar(100)")
                    .IsRequired();

                entity.Property(e => e.PhoneNumber)
                    .HasColumnType("varchar(20)")
                    .IsRequired();

                entity.Property(e => e.FatherName)
                    .HasColumnType("varchar(100)")
                    .IsRequired();

                entity.Property(e => e.MotherName)
                    .HasColumnType("varchar(100)")
                    .IsRequired();

                entity.Property(e => e.Email)
                    .HasColumnType("varchar(100)")
                    .IsRequired();

                entity.Property(e => e.Password)
                    .HasColumnType("varchar(100)")
                    .IsRequired();

                entity.Property(e => e.Course)
                    .HasColumnType("varchar(50)")
                    .IsRequired();

                entity.Property(e => e.RegistrationDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.PaymentStatus)
                    .HasColumnType("bit")
                    .HasDefaultValue(false);

                entity.Property(e => e.AmountPaid)
                    .HasColumnType("decimal(10,2)")
                    .HasDefaultValue(0);

                entity.HasIndex(e => e.Email)
                    .IsUnique();

                // Define relationship with Course
                entity.HasOne<Course>()
                    .WithMany()
                    .HasForeignKey(e => e.Course)
                    .HasPrincipalKey(c => c.Name)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Course entity
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.Name);

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(50)")
                    .IsRequired();

                entity.Property(e => e.Fee)
                    .HasColumnType("decimal(10,2)")
                    .IsRequired();

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(200)");

                entity.HasIndex(e => e.Name)
                    .IsUnique();
            });

            // Configure CourseOffering entity
            modelBuilder.Entity<CourseOffering>(entity =>
            {
                entity.HasKey(e => e.CourseCode);

                entity.Property(e => e.CourseCode)
                    .HasColumnType("varchar(10)")
                    .IsRequired();

                entity.Property(e => e.CourseName)
                    .HasColumnType("varchar(100)")
                    .IsRequired();

                entity.Property(e => e.Branch)
                    .HasColumnType("varchar(50)")
                    .IsRequired();

                entity.Property(e => e.Credits)
                    .IsRequired();

                entity.Property(e => e.Instructor)
                    .HasColumnType("varchar(100)")
                    .IsRequired();

                entity.Property(e => e.Schedule)
                    .HasColumnType("varchar(100)")
                    .IsRequired();

                entity.Property(e => e.Room)
                    .HasColumnType("varchar(20)")
                    .IsRequired();

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(200)");

                entity.HasIndex(e => e.CourseCode)
                    .IsUnique();
            });

            // Configure CourseEnrollment entity
            modelBuilder.Entity<CourseEnrollment>(entity =>
            {
                entity.HasKey(e => e.EnrollmentId);

                entity.Property(e => e.StudentId)
                    .HasColumnType("varchar(10)")
                    .IsRequired();

                entity.Property(e => e.CourseCode)
                    .HasColumnType("varchar(10)")
                    .IsRequired();

                entity.Property(e => e.CourseName)
                    .HasColumnType("varchar(100)")
                    .IsRequired();

                entity.Property(e => e.Credits)
                    .IsRequired();

                entity.Property(e => e.EnrollmentDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DropDate)
                    .HasColumnType("datetime")
                    .IsRequired(false);

                entity.Property(e => e.IsActive)
                    .HasColumnType("bit")
                    .HasDefaultValue(true);

                // Define relationship with Student
                entity.HasOne(e => e.Student)
                    .WithMany()
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Create a composite index on StudentId and CourseCode
                entity.HasIndex(e => new { e.StudentId, e.CourseCode });
            });

            // Configure Payment entity
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.PaymentId);

                entity.Property(e => e.StudentId)
                    .HasColumnType("varchar(10)")
                    .IsRequired();

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(10,2)")
                    .IsRequired();

                entity.Property(e => e.PaymentDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.PaymentMethod)
                    .HasColumnType("varchar(50)")
                    .IsRequired();

                entity.Property(e => e.PaymentStatus)
                    .HasColumnType("varchar(20)")
                    .IsRequired();

                entity.Property(e => e.TransactionId)
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(200)");

                entity.Property(e => e.ReceiptNumber)
                    .HasColumnType("varchar(50)");

                // Define relationship with Student
                entity.HasOne(e => e.Student)
                    .WithMany()
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure StudentEnquiry entity
            modelBuilder.Entity<StudentEnquiry>(entity =>
            {
                entity.HasKey(e => e.EnquiryId);

                entity.Property(e => e.StudentId)
                    .HasColumnType("varchar(10)")
                    .IsRequired();

                entity.Property(e => e.EnquiryType)
                    .HasColumnType("varchar(20)")
                    .IsRequired();

                entity.Property(e => e.Subject)
                    .HasColumnType("varchar(100)")
                    .IsRequired();

                entity.Property(e => e.Message)
                    .HasColumnType("text")
                    .IsRequired();

                entity.Property(e => e.SubmissionDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Status)
                    .HasColumnType("varchar(20)")
                    .HasDefaultValue("Pending");

                entity.Property(e => e.Response)
                    .HasColumnType("text");

                entity.Property(e => e.ResponseDate)
                    .HasColumnType("datetime")
                    .IsRequired(false);

                entity.Property(e => e.RespondedBy)
                    .HasColumnType("varchar(100)");

                // Define relationship with Student
                entity.HasOne(e => e.Student)
                    .WithMany()
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure TeacherEvaluation entity
            modelBuilder.Entity<TeacherEvaluation>(entity =>
            {
                entity.HasKey(e => e.EvaluationId);

                entity.Property(e => e.StudentId)
                    .HasColumnType("varchar(10)")
                    .IsRequired();

                entity.Property(e => e.CourseCode)
                    .HasColumnType("varchar(10)")
                    .IsRequired();

                entity.Property(e => e.InstructorName)
                    .HasColumnType("varchar(100)")
                    .IsRequired();

                entity.Property(e => e.TeachingRating)
                    .IsRequired();

                entity.Property(e => e.ContentRating)
                    .IsRequired();

                entity.Property(e => e.AssessmentRating)
                    .IsRequired();

                entity.Property(e => e.CommunicationRating)
                    .IsRequired();

                entity.Property(e => e.OverallRating)
                    .IsRequired();

                entity.Property(e => e.Comments)
                    .HasColumnType("text");

                entity.Property(e => e.SubmissionDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsAnonymous)
                    .HasDefaultValue(true);

                // Define relationship with Student
                entity.HasOne(e => e.Student)
                    .WithMany()
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Teacher entity
            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.HasKey(e => e.TeacherId);

                entity.Property(e => e.TeacherId)
                    .HasColumnType("varchar(10)")
                    .IsRequired();

                entity.Property(e => e.TeacherName)
                    .HasColumnType("varchar(100)")
                    .IsRequired();

                entity.Property(e => e.Email)
                    .HasColumnType("varchar(100)")
                    .IsRequired();

                entity.Property(e => e.Password)
                    .HasColumnType("varchar(100)")
                    .IsRequired();

                entity.Property(e => e.Department)
                    .HasColumnType("varchar(50)")
                    .IsRequired();

                entity.Property(e => e.RegistrationDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.Email)
                    .IsUnique();
            });

            // Configure StudentBankDetails entity
            modelBuilder.Entity<StudentBankDetails>(entity =>
            {
                entity.HasKey(e => e.BankDetailsId);

                entity.Property(e => e.StudentId)
                    .HasColumnType("varchar(10)")
                    .IsRequired();

                entity.Property(e => e.BankName)
                    .HasColumnType("varchar(100)")
                    .IsRequired();

                entity.Property(e => e.AccountNumber)
                    .HasColumnType("varchar(20)")
                    .IsRequired();

                entity.Property(e => e.IFSC)
                    .HasColumnType("varchar(20)")
                    .IsRequired();

                entity.Property(e => e.AccountHolderName)
                    .HasColumnType("varchar(100)")
                    .IsRequired();

                entity.Property(e => e.LastUpdated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Define relationship with Student
                entity.HasOne(e => e.Student)
                    .WithMany()
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Create a unique index on StudentId
                entity.HasIndex(e => e.StudentId)
                    .IsUnique();
            });
        }
    }
}