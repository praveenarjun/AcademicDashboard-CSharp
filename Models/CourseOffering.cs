using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StudentEnrollmentSystem1.Data;

namespace StudentEnrollmentSystem1.Models
{
    public class CourseOffering
    {
        [Key]
        public string CourseCode { get; set; }
        
        [Required]
        public string CourseName { get; set; }
        
        [Required]
        public string Branch { get; set; }  // e.g., BTech, BBA, etc.
        
        [Required]
        public int Credits { get; set; }
        
        [Required]
        public string Instructor { get; set; }
        
        [Required]
        public string Schedule { get; set; }  // e.g., "Mon,Wed,Fri 10:00-11:00"
        
        [Required]
        public string Room { get; set; }
        
        [Required]
        public int MaxEnrollment { get; set; }
        
        public int CurrentEnrollment { get; set; } = 0;
        
        [Required]
        public bool IsActive { get; set; } = true;
        
        public string Description { get; set; }
    }
    
    public static class CourseOfferingData
    {
        public static List<CourseOffering> GetCourseOfferings()
        {
            return new List<CourseOffering>
            {
                // BTech Courses (20 courses)
                new CourseOffering { 
                    CourseCode = "CS101", 
                    CourseName = "Introduction to Computer Science", 
                    Branch = "BTech", 
                    Credits = 4, 
                    Instructor = "Dr. Smith", 
                    Schedule = "Mon,Wed,Fri 10:00-11:00", 
                    Room = "A101", 
                    MaxEnrollment = 60,
                    Description = "Fundamentals of computer science and programming"
                },
                new CourseOffering { 
                    CourseCode = "CS201", 
                    CourseName = "Data Structures", 
                    Branch = "BTech", 
                    Credits = 4, 
                    Instructor = "Dr. Johnson", 
                    Schedule = "Tue,Thu 13:00-15:00", 
                    Room = "A102", 
                    MaxEnrollment = 50,
                    Description = "Study of data structures and algorithms"
                },
                new CourseOffering { 
                    CourseCode = "CS301", 
                    CourseName = "Database Systems", 
                    Branch = "BTech", 
                    Credits = 3, 
                    Instructor = "Dr. Williams", 
                    Schedule = "Mon,Wed 14:00-15:30", 
                    Room = "B201", 
                    MaxEnrollment = 45,
                    Description = "Introduction to database design and SQL"
                },
                new CourseOffering { 
                    CourseCode = "CS401", 
                    CourseName = "Software Engineering", 
                    Branch = "BTech", 
                    Credits = 3, 
                    Instructor = "Dr. Brown", 
                    Schedule = "Tue,Thu 10:00-11:30", 
                    Room = "B202", 
                    MaxEnrollment = 40,
                    Description = "Software development methodologies and practices"
                },
                new CourseOffering { 
                    CourseCode = "CS102", 
                    CourseName = "Programming Fundamentals", 
                    Branch = "BTech", 
                    Credits = 4, 
                    Instructor = "Dr. Roberts", 
                    Schedule = "Mon,Wed,Fri 13:00-14:00", 
                    Room = "A103", 
                    MaxEnrollment = 55,
                    Description = "Introduction to programming concepts and problem-solving"
                },
                new CourseOffering { 
                    CourseCode = "CS202", 
                    CourseName = "Algorithms", 
                    Branch = "BTech", 
                    Credits = 4, 
                    Instructor = "Dr. Lee", 
                    Schedule = "Tue,Thu 15:00-17:00", 
                    Room = "A104", 
                    MaxEnrollment = 45,
                    Description = "Design and analysis of algorithms"
                },
                new CourseOffering { 
                    CourseCode = "CS302", 
                    CourseName = "Operating Systems", 
                    Branch = "BTech", 
                    Credits = 3, 
                    Instructor = "Dr. Garcia", 
                    Schedule = "Mon,Wed 16:00-17:30", 
                    Room = "B203", 
                    MaxEnrollment = 40,
                    Description = "Principles of operating systems"
                },
                new CourseOffering { 
                    CourseCode = "CS402", 
                    CourseName = "Computer Networks", 
                    Branch = "BTech", 
                    Credits = 3, 
                    Instructor = "Dr. Martinez", 
                    Schedule = "Tue,Thu 08:00-09:30", 
                    Room = "B204", 
                    MaxEnrollment = 35,
                    Description = "Fundamentals of computer networking"
                },
                new CourseOffering { 
                    CourseCode = "CS103", 
                    CourseName = "Discrete Mathematics", 
                    Branch = "BTech", 
                    Credits = 3, 
                    Instructor = "Dr. Wilson", 
                    Schedule = "Mon,Wed,Fri 09:00-10:00", 
                    Room = "A105", 
                    MaxEnrollment = 50,
                    Description = "Mathematical foundations of computer science"
                },
                new CourseOffering { 
                    CourseCode = "CS203", 
                    CourseName = "Object-Oriented Programming", 
                    Branch = "BTech", 
                    Credits = 4, 
                    Instructor = "Dr. Anderson", 
                    Schedule = "Tue,Thu 11:00-13:00", 
                    Room = "A106", 
                    MaxEnrollment = 45,
                    Description = "Object-oriented design and programming"
                },
                new CourseOffering { 
                    CourseCode = "CS303", 
                    CourseName = "Web Development", 
                    Branch = "BTech", 
                    Credits = 3, 
                    Instructor = "Dr. Thomas", 
                    Schedule = "Mon,Wed 11:00-12:30", 
                    Room = "B205", 
                    MaxEnrollment = 40,
                    Description = "Web application development"
                },
                new CourseOffering { 
                    CourseCode = "CS403", 
                    CourseName = "Artificial Intelligence", 
                    Branch = "BTech", 
                    Credits = 4, 
                    Instructor = "Dr. Jackson", 
                    Schedule = "Tue,Thu 14:00-16:00", 
                    Room = "B206", 
                    MaxEnrollment = 35,
                    Description = "Introduction to artificial intelligence"
                },
                new CourseOffering { 
                    CourseCode = "CS104", 
                    CourseName = "Computer Architecture", 
                    Branch = "BTech", 
                    Credits = 3, 
                    Instructor = "Dr. White", 
                    Schedule = "Mon,Wed,Fri 14:00-15:00", 
                    Room = "A107", 
                    MaxEnrollment = 45,
                    Description = "Computer organization and architecture"
                },
                new CourseOffering { 
                    CourseCode = "CS204", 
                    CourseName = "Mobile App Development", 
                    Branch = "BTech", 
                    Credits = 3, 
                    Instructor = "Dr. Harris", 
                    Schedule = "Tue,Thu 09:00-10:30", 
                    Room = "A108", 
                    MaxEnrollment = 40,
                    Description = "Development of mobile applications"
                },
                new CourseOffering { 
                    CourseCode = "CS304", 
                    CourseName = "Cloud Computing", 
                    Branch = "BTech", 
                    Credits = 3, 
                    Instructor = "Dr. Martin", 
                    Schedule = "Mon,Wed 09:00-10:30", 
                    Room = "B207", 
                    MaxEnrollment = 35,
                    Description = "Introduction to cloud computing technologies"
                },
                new CourseOffering { 
                    CourseCode = "CS404", 
                    CourseName = "Machine Learning", 
                    Branch = "BTech", 
                    Credits = 4, 
                    Instructor = "Dr. Clark", 
                    Schedule = "Tue,Thu 16:00-18:00", 
                    Room = "B208", 
                    MaxEnrollment = 30,
                    Description = "Introduction to machine learning algorithms"
                },
                new CourseOffering { 
                    CourseCode = "CS105", 
                    CourseName = "Digital Logic Design", 
                    Branch = "BTech", 
                    Credits = 3, 
                    Instructor = "Dr. Lewis", 
                    Schedule = "Mon,Wed,Fri 15:00-16:00", 
                    Room = "A109", 
                    MaxEnrollment = 40,
                    Description = "Fundamentals of digital logic design"
                },
                new CourseOffering { 
                    CourseCode = "CS205", 
                    CourseName = "Computer Graphics", 
                    Branch = "BTech", 
                    Credits = 3, 
                    Instructor = "Dr. Walker", 
                    Schedule = "Tue,Thu 13:00-14:30", 
                    Room = "A110", 
                    MaxEnrollment = 35,
                    Description = "Principles of computer graphics"
                },
                new CourseOffering { 
                    CourseCode = "CS305", 
                    CourseName = "Information Security", 
                    Branch = "BTech", 
                    Credits = 3, 
                    Instructor = "Dr. Hall", 
                    Schedule = "Mon,Wed 13:00-14:30", 
                    Room = "B209", 
                    MaxEnrollment = 30,
                    Description = "Fundamentals of information security"
                },
                new CourseOffering { 
                    CourseCode = "CS405", 
                    CourseName = "Big Data Analytics", 
                    Branch = "BTech", 
                    Credits = 4, 
                    Instructor = "Dr. Young", 
                    Schedule = "Tue,Thu 09:00-11:00", 
                    Room = "B210", 
                    MaxEnrollment = 25,
                    Description = "Analysis of large-scale data"
                },
                
                // BBA Courses (20 courses)
                new CourseOffering { 
                    CourseCode = "BA101", 
                    CourseName = "Principles of Management", 
                    Branch = "BBA", 
                    Credits = 3, 
                    Instructor = "Prof. Davis", 
                    Schedule = "Mon,Wed,Fri 09:00-10:00", 
                    Room = "C101", 
                    MaxEnrollment = 70,
                    Description = "Introduction to management principles and practices"
                },
                new CourseOffering { 
                    CourseCode = "BA201", 
                    CourseName = "Financial Accounting", 
                    Branch = "BBA", 
                    Credits = 4, 
                    Instructor = "Prof. Miller", 
                    Schedule = "Tue,Thu 09:00-11:00", 
                    Room = "C102", 
                    MaxEnrollment = 60,
                    Description = "Fundamentals of financial accounting"
                },
                new CourseOffering { 
                    CourseCode = "BA301", 
                    CourseName = "Marketing Management", 
                    Branch = "BBA", 
                    Credits = 3, 
                    Instructor = "Prof. Wilson", 
                    Schedule = "Mon,Wed 11:00-12:30", 
                    Room = "D201", 
                    MaxEnrollment = 55,
                    Description = "Marketing strategies and consumer behavior"
                },
                new CourseOffering { 
                    CourseCode = "BA401", 
                    CourseName = "Business Ethics", 
                    Branch = "BBA", 
                    Credits = 3, 
                    Instructor = "Prof. Taylor", 
                    Schedule = "Fri 13:00-16:00", 
                    Room = "D202", 
                    MaxEnrollment = 50,
                    Description = "Ethical considerations in business decisions"
                },
                new CourseOffering { 
                    CourseCode = "BA102", 
                    CourseName = "Business Economics", 
                    Branch = "BBA", 
                    Credits = 3, 
                    Instructor = "Prof. Anderson", 
                    Schedule = "Mon,Wed,Fri 10:00-11:00", 
                    Room = "C103", 
                    MaxEnrollment = 65,
                    Description = "Economic principles applied to business"
                },
                new CourseOffering { 
                    CourseCode = "BA202", 
                    CourseName = "Business Statistics", 
                    Branch = "BBA", 
                    Credits = 4, 
                    Instructor = "Prof. Thomas", 
                    Schedule = "Tue,Thu 11:00-13:00", 
                    Room = "C104", 
                    MaxEnrollment = 55,
                    Description = "Statistical methods for business analysis"
                },
                new CourseOffering { 
                    CourseCode = "BA302", 
                    CourseName = "Human Resource Management", 
                    Branch = "BBA", 
                    Credits = 3, 
                    Instructor = "Prof. White", 
                    Schedule = "Mon,Wed 13:00-14:30", 
                    Room = "D203", 
                    MaxEnrollment = 50,
                    Description = "Managing human resources in organizations"
                },
                new CourseOffering { 
                    CourseCode = "BA402", 
                    CourseName = "Strategic Management", 
                    Branch = "BBA", 
                    Credits = 4, 
                    Instructor = "Prof. Harris", 
                    Schedule = "Tue,Thu 14:00-16:00", 
                    Room = "D204", 
                    MaxEnrollment = 45,
                    Description = "Business strategy formulation and implementation"
                },
                new CourseOffering { 
                    CourseCode = "BA103", 
                    CourseName = "Business Communication", 
                    Branch = "BBA", 
                    Credits = 3, 
                    Instructor = "Prof. Martin", 
                    Schedule = "Mon,Wed,Fri 11:00-12:00", 
                    Room = "C105", 
                    MaxEnrollment = 60,
                    Description = "Effective communication in business contexts"
                },
                new CourseOffering { 
                    CourseCode = "BA203", 
                    CourseName = "Cost Accounting", 
                    Branch = "BBA", 
                    Credits = 4, 
                    Instructor = "Prof. Clark", 
                    Schedule = "Tue,Thu 13:00-15:00", 
                    Room = "C106", 
                    MaxEnrollment = 50,
                    Description = "Principles of cost accounting"
                },
                new CourseOffering { 
                    CourseCode = "BA303", 
                    CourseName = "Operations Management", 
                    Branch = "BBA", 
                    Credits = 3, 
                    Instructor = "Prof. Lewis", 
                    Schedule = "Mon,Wed 15:00-16:30", 
                    Room = "D205", 
                    MaxEnrollment = 45,
                    Description = "Managing operations in organizations"
                },
                new CourseOffering { 
                    CourseCode = "BA403", 
                    CourseName = "International Business", 
                    Branch = "BBA", 
                    Credits = 3, 
                    Instructor = "Prof. Walker", 
                    Schedule = "Tue,Thu 16:00-17:30", 
                    Room = "D206", 
                    MaxEnrollment = 40,
                    Description = "Global business operations and strategies"
                },
                new CourseOffering { 
                    CourseCode = "BA104", 
                    CourseName = "Business Law", 
                    Branch = "BBA", 
                    Credits = 3, 
                    Instructor = "Prof. Hall", 
                    Schedule = "Mon,Wed,Fri 13:00-14:00", 
                    Room = "C107", 
                    MaxEnrollment = 55,
                    Description = "Legal aspects of business operations"
                },
                new CourseOffering { 
                    CourseCode = "BA204", 
                    CourseName = "Management Accounting", 
                    Branch = "BBA", 
                    Credits = 4, 
                    Instructor = "Prof. Young", 
                    Schedule = "Tue,Thu 09:00-11:00", 
                    Room = "C108", 
                    MaxEnrollment = 45,
                    Description = "Accounting for management decision-making"
                },
                new CourseOffering { 
                    CourseCode = "BA304", 
                    CourseName = "Consumer Behavior", 
                    Branch = "BBA", 
                    Credits = 3, 
                    Instructor = "Prof. King", 
                    Schedule = "Mon,Wed 09:00-10:30", 
                    Room = "D207", 
                    MaxEnrollment = 40,
                    Description = "Understanding consumer decision-making processes"
                },
                new CourseOffering { 
                    CourseCode = "BA404", 
                    CourseName = "Entrepreneurship", 
                    Branch = "BBA", 
                    Credits = 3, 
                    Instructor = "Prof. Wright", 
                    Schedule = "Tue,Thu 11:00-12:30", 
                    Room = "D208", 
                    MaxEnrollment = 35,
                    Description = "Starting and managing new ventures"
                },
                new CourseOffering { 
                    CourseCode = "BA105", 
                    CourseName = "Organizational Behavior", 
                    Branch = "BBA", 
                    Credits = 3, 
                    Instructor = "Prof. Scott", 
                    Schedule = "Mon,Wed,Fri 14:00-15:00", 
                    Room = "C109", 
                    MaxEnrollment = 50,
                    Description = "Human behavior in organizational settings"
                },
                new CourseOffering { 
                    CourseCode = "BA205", 
                    CourseName = "Corporate Finance", 
                    Branch = "BBA", 
                    Credits = 4, 
                    Instructor = "Prof. Green", 
                    Schedule = "Tue,Thu 13:00-15:00", 
                    Room = "C110", 
                    MaxEnrollment = 40,
                    Description = "Financial management in corporations"
                },
                new CourseOffering { 
                    CourseCode = "BA305", 
                    CourseName = "Digital Marketing", 
                    Branch = "BBA", 
                    Credits = 3, 
                    Instructor = "Prof. Adams", 
                    Schedule = "Mon,Wed 11:00-12:30", 
                    Room = "D209", 
                    MaxEnrollment = 35,
                    Description = "Marketing in the digital age"
                },
                new CourseOffering { 
                    CourseCode = "BA405", 
                    CourseName = "Business Analytics", 
                    Branch = "BBA", 
                    Credits = 4, 
                    Instructor = "Prof. Baker", 
                    Schedule = "Tue,Thu 15:00-17:00", 
                    Room = "D210", 
                    MaxEnrollment = 30,
                    Description = "Data-driven decision making in business"
                },
                
                // BCA Courses (20 courses)
                new CourseOffering { 
                    CourseCode = "CA101", 
                    CourseName = "Computer Fundamentals", 
                    Branch = "BCA", 
                    Credits = 3, 
                    Instructor = "Dr. Anderson", 
                    Schedule = "Mon,Wed,Fri 11:00-12:00", 
                    Room = "E101", 
                    MaxEnrollment = 65,
                    Description = "Introduction to computer systems and applications"
                },
                new CourseOffering { 
                    CourseCode = "CA201", 
                    CourseName = "Programming in C++", 
                    Branch = "BCA", 
                    Credits = 4, 
                    Instructor = "Dr. Thomas", 
                    Schedule = "Tue,Thu 11:00-13:00", 
                    Room = "E102", 
                    MaxEnrollment = 55,
                    Description = "Object-oriented programming using C++"
                },
                new CourseOffering { 
                    CourseCode = "CA301", 
                    CourseName = "Database Management Systems", 
                    Branch = "BCA", 
                    Credits = 4, 
                    Instructor = "Dr. White", 
                    Schedule = "Mon,Wed 13:00-15:00", 
                    Room = "E201", 
                    MaxEnrollment = 50,
                    Description = "Design and implementation of database systems"
                },
                new CourseOffering { 
                    CourseCode = "CA401", 
                    CourseName = "Web Development", 
                    Branch = "BCA", 
                    Credits = 3, 
                    Instructor = "Dr. Harris", 
                    Schedule = "Tue,Thu 14:00-15:30", 
                    Room = "E202", 
                    MaxEnrollment = 45,
                    Description = "Web application development using HTML, CSS, and JavaScript"
                },
                new CourseOffering { 
                    CourseCode = "CA102", 
                    CourseName = "Digital Logic", 
                    Branch = "BCA", 
                    Credits = 3, 
                    Instructor = "Dr. Martin", 
                    Schedule = "Mon,Wed,Fri 09:00-10:00", 
                    Room = "E103", 
                    MaxEnrollment = 60,
                    Description = "Fundamentals of digital logic and design"
                },
                new CourseOffering { 
                    CourseCode = "CA202", 
                    CourseName = "Data Structures", 
                    Branch = "BCA", 
                    Credits = 4, 
                    Instructor = "Dr. Clark", 
                    Schedule = "Tue,Thu 09:00-11:00", 
                    Room = "E104", 
                    MaxEnrollment = 50,
                    Description = "Implementation and analysis of data structures"
                },
                new CourseOffering { 
                    CourseCode = "CA302", 
                    CourseName = "Operating Systems", 
                    Branch = "BCA", 
                    Credits = 3, 
                    Instructor = "Dr. Lewis", 
                    Schedule = "Mon,Wed 15:00-16:30", 
                    Room = "E203", 
                    MaxEnrollment = 45,
                    Description = "Concepts and design of operating systems"
                },
                new CourseOffering { 
                    CourseCode = "CA402", 
                    CourseName = "Mobile Application Development", 
                    Branch = "BCA", 
                    Credits = 3, 
                    Instructor = "Dr. Walker", 
                    Schedule = "Tue,Thu 16:00-17:30", 
                    Room = "E204", 
                    MaxEnrollment = 40,
                    Description = "Development of applications for mobile devices"
                },
                new CourseOffering { 
                    CourseCode = "CA103", 
                    CourseName = "Discrete Mathematics", 
                    Branch = "BCA", 
                    Credits = 3, 
                    Instructor = "Dr. Hall", 
                    Schedule = "Mon,Wed,Fri 10:00-11:00", 
                    Room = "E105", 
                    MaxEnrollment = 55,
                    Description = "Mathematical foundations for computer science"
                },
                new CourseOffering { 
                    CourseCode = "CA203", 
                    CourseName = "Object-Oriented Programming", 
                    Branch = "BCA", 
                    Credits = 4, 
                    Instructor = "Dr. Young", 
                    Schedule = "Tue,Thu 13:00-15:00", 
                    Room = "E106", 
                    MaxEnrollment = 45,
                    Description = "Object-oriented design and programming concepts"
                },
                new CourseOffering { 
                    CourseCode = "CA303", 
                    CourseName = "Computer Networks", 
                    Branch = "BCA", 
                    Credits = 3, 
                    Instructor = "Dr. King", 
                    Schedule = "Mon,Wed 09:00-10:30", 
                    Room = "E205", 
                    MaxEnrollment = 40,
                    Description = "Fundamentals of computer networking"
                },
                new CourseOffering { 
                    CourseCode = "CA403", 
                    CourseName = "Software Engineering", 
                    Branch = "BCA", 
                    Credits = 4, 
                    Instructor = "Dr. Wright", 
                    Schedule = "Tue,Thu 11:00-13:00", 
                    Room = "E206", 
                    MaxEnrollment = 35,
                    Description = "Software development methodologies and practices"
                },
                new CourseOffering { 
                    CourseCode = "CA104", 
                    CourseName = "Computer Organization", 
                    Branch = "BCA", 
                    Credits = 3, 
                    Instructor = "Dr. Scott", 
                    Schedule = "Mon,Wed,Fri 13:00-14:00", 
                    Room = "E107", 
                    MaxEnrollment = 50,
                    Description = "Computer architecture and organization"
                },
                new CourseOffering { 
                    CourseCode = "CA204", 
                    CourseName = "Programming in Java", 
                    Branch = "BCA", 
                    Credits = 4, 
                    Instructor = "Dr. Green", 
                    Schedule = "Tue,Thu 15:00-17:00", 
                    Room = "E108", 
                    MaxEnrollment = 40,
                    Description = "Java programming language and applications"
                },
                new CourseOffering { 
                    CourseCode = "CA304", 
                    CourseName = "Information Security", 
                    Branch = "BCA", 
                    Credits = 3, 
                    Instructor = "Dr. Adams", 
                    Schedule = "Mon,Wed 11:00-12:30", 
                    Room = "E207", 
                    MaxEnrollment = 35,
                    Description = "Principles of information security and cryptography"
                },
                new CourseOffering { 
                    CourseCode = "CA404", 
                    CourseName = "Cloud Computing", 
                    Branch = "BCA", 
                    Credits = 3, 
                    Instructor = "Dr. Baker", 
                    Schedule = "Tue,Thu 09:00-10:30", 
                    Room = "E208", 
                    MaxEnrollment = 30,
                    Description = "Introduction to cloud computing technologies"
                },
                new CourseOffering { 
                    CourseCode = "CA105", 
                    CourseName = "Digital Design", 
                    Branch = "BCA", 
                    Credits = 3, 
                    Instructor = "Dr. Carter", 
                    Schedule = "Mon,Wed,Fri 14:00-15:00", 
                    Room = "E109", 
                    MaxEnrollment = 45,
                    Description = "Digital system design and implementation"
                },
                new CourseOffering { 
                    CourseCode = "CA205", 
                    CourseName = "Computer Graphics", 
                    Branch = "BCA", 
                    Credits = 3, 
                    Instructor = "Dr. Evans", 
                    Schedule = "Tue,Thu 13:00-14:30", 
                    Room = "E110", 
                    MaxEnrollment = 35,
                    Description = "Principles and applications of computer graphics"
                },
                new CourseOffering { 
                    CourseCode = "CA305", 
                    CourseName = "Artificial Intelligence", 
                    Branch = "BCA", 
                    Credits = 4, 
                    Instructor = "Dr. Foster", 
                    Schedule = "Mon,Wed 13:00-15:00", 
                    Room = "E209", 
                    MaxEnrollment = 30,
                    Description = "Introduction to artificial intelligence concepts"
                },
                new CourseOffering { 
                    CourseCode = "CA405", 
                    CourseName = "Data Mining", 
                    Branch = "BCA", 
                    Credits = 3, 
                    Instructor = "Dr. Gray", 
                    Schedule = "Tue,Thu 15:00-16:30", 
                    Room = "E210", 
                    MaxEnrollment = 25,
                    Description = "Techniques for discovering patterns in large data sets"
                },
                
                // MBA Courses (20 courses)
                new CourseOffering { 
                    CourseCode = "MB101", 
                    CourseName = "Strategic Management", 
                    Branch = "MBA", 
                    Credits = 3, 
                    Instructor = "Prof. White", 
                    Schedule = "Mon,Wed 16:00-17:30", 
                    Room = "F101", 
                    MaxEnrollment = 45,
                    Description = "Business strategy formulation and implementation"
                },
                new CourseOffering { 
                    CourseCode = "MB201", 
                    CourseName = "Organizational Behavior", 
                    Branch = "MBA", 
                    Credits = 3, 
                    Instructor = "Prof. Harris", 
                    Schedule = "Tue,Thu 16:00-17:30", 
                    Room = "F102", 
                    MaxEnrollment = 40,
                    Description = "Human behavior in organizational settings"
                },
                new CourseOffering { 
                    CourseCode = "MB301", 
                    CourseName = "Financial Management", 
                    Branch = "MBA", 
                    Credits = 4, 
                    Instructor = "Prof. Martin", 
                    Schedule = "Mon,Wed 14:00-16:00", 
                    Room = "F201", 
                    MaxEnrollment = 35,
                    Description = "Financial decision-making in organizations"
                },
                new CourseOffering { 
                    CourseCode = "MB401", 
                    CourseName = "Marketing Management", 
                    Branch = "MBA", 
                    Credits = 3, 
                    Instructor = "Prof. Clark", 
                    Schedule = "Tue,Thu 14:00-15:30", 
                    Room = "F202", 
                    MaxEnrollment = 30,
                    Description = "Marketing strategies and implementation"
                },
                new CourseOffering { 
                    CourseCode = "MB102", 
                    CourseName = "Managerial Economics", 
                    Branch = "MBA", 
                    Credits = 3, 
                    Instructor = "Prof. Lewis", 
                    Schedule = "Mon,Wed 09:00-10:30", 
                    Room = "F103", 
                    MaxEnrollment = 40,
                    Description = "Economic principles for managerial decision-making"
                },
                new CourseOffering { 
                    CourseCode = "MB202", 
                    CourseName = "Business Research Methods", 
                    Branch = "MBA", 
                    Credits = 3, 
                    Instructor = "Prof. Walker", 
                    Schedule = "Tue,Thu 09:00-10:30", 
                    Room = "F104", 
                    MaxEnrollment = 35,
                    Description = "Research methodologies for business"
                },
                new CourseOffering { 
                    CourseCode = "MB302", 
                    CourseName = "Operations Management", 
                    Branch = "MBA", 
                    Credits = 3, 
                    Instructor = "Prof. Hall", 
                    Schedule = "Mon,Wed 11:00-12:30", 
                    Room = "F203", 
                    MaxEnrollment = 30,
                    Description = "Managing operations in organizations"
                },
                new CourseOffering { 
                    CourseCode = "MB402", 
                    CourseName = "Business Ethics and CSR", 
                    Branch = "MBA", 
                    Credits = 3, 
                    Instructor = "Prof. Young", 
                    Schedule = "Tue,Thu 11:00-12:30", 
                    Room = "F204", 
                    MaxEnrollment = 25,
                    Description = "Ethical considerations and corporate social responsibility"
                },
                new CourseOffering { 
                    CourseCode = "MB103", 
                    CourseName = "Accounting for Managers", 
                    Branch = "MBA", 
                    Credits = 4, 
                    Instructor = "Prof. King", 
                    Schedule = "Mon,Wed 13:00-15:00", 
                    Room = "F105", 
                    MaxEnrollment = 35,
                    Description = "Accounting principles for managerial decision-making"
                },
                new CourseOffering { 
                    CourseCode = "MB203", 
                    CourseName = "Human Resource Management", 
                    Branch = "MBA", 
                    Credits = 3, 
                    Instructor = "Prof. Wright", 
                    Schedule = "Tue,Thu 13:00-14:30", 
                    Room = "F106", 
                    MaxEnrollment = 30,
                    Description = "Managing human resources in organizations"
                },
                new CourseOffering { 
                    CourseCode = "MB303", 
                    CourseName = "International Business", 
                    Branch = "MBA", 
                    Credits = 3, 
                    Instructor = "Prof. Scott", 
                    Schedule = "Mon,Wed 09:00-10:30", 
                    Room = "F205", 
                    MaxEnrollment = 25,
                    Description = "Global business operations and strategies"
                },
                new CourseOffering { 
                    CourseCode = "MB403", 
                    CourseName = "Project Management", 
                    Branch = "MBA", 
                    Credits = 3, 
                    Instructor = "Prof. Green", 
                    Schedule = "Tue,Thu 09:00-10:30", 
                    Room = "F206", 
                    MaxEnrollment = 20,
                    Description = "Planning, executing, and managing projects"
                },
                new CourseOffering { 
                    CourseCode = "MB104", 
                    CourseName = "Business Analytics", 
                    Branch = "MBA", 
                    Credits = 4, 
                    Instructor = "Prof. Adams", 
                    Schedule = "Mon,Wed 15:00-17:00", 
                    Room = "F107", 
                    MaxEnrollment = 30,
                    Description = "Data-driven decision making in business"
                },
                new CourseOffering { 
                    CourseCode = "MB204", 
                    CourseName = "Consumer Behavior", 
                    Branch = "MBA", 
                    Credits = 3, 
                    Instructor = "Prof. Baker", 
                    Schedule = "Tue,Thu 15:00-16:30", 
                    Room = "F108", 
                    MaxEnrollment = 25,
                    Description = "Understanding consumer decision-making processes"
                },
                new CourseOffering { 
                    CourseCode = "MB304", 
                    CourseName = "Corporate Finance", 
                    Branch = "MBA", 
                    Credits = 4, 
                    Instructor = "Prof. Carter", 
                    Schedule = "Mon,Wed 11:00-13:00", 
                    Room = "F207", 
                    MaxEnrollment = 20,
                    Description = "Financial management in corporations"
                },
                new CourseOffering { 
                    CourseCode = "MB404", 
                    CourseName = "Digital Marketing", 
                    Branch = "MBA", 
                    Credits = 3, 
                    Instructor = "Prof. Evans", 
                    Schedule = "Tue,Thu 11:00-12:30", 
                    Room = "F208", 
                    MaxEnrollment = 15,
                    Description = "Marketing in the digital age"
                },
                new CourseOffering { 
                    CourseCode = "MB105", 
                    CourseName = "Leadership and Change Management", 
                    Branch = "MBA", 
                    Credits = 3, 
                    Instructor = "Prof. Foster", 
                    Schedule = "Mon,Wed 16:00-17:30", 
                    Room = "F109", 
                    MaxEnrollment = 25,
                    Description = "Leadership skills and managing organizational change"
                },
                new CourseOffering { 
                    CourseCode = "MB205", 
                    CourseName = "Supply Chain Management", 
                    Branch = "MBA", 
                    Credits = 3, 
                    Instructor = "Prof. Gray", 
                    Schedule = "Tue,Thu 16:00-17:30", 
                    Room = "F110", 
                    MaxEnrollment = 20,
                    Description = "Managing supply chains and logistics"
                },
                new CourseOffering { 
                    CourseCode = "MB305", 
                    CourseName = "Investment Analysis", 
                    Branch = "MBA", 
                    Credits = 4, 
                    Instructor = "Prof. Hill", 
                    Schedule = "Mon,Wed 13:00-15:00", 
                    Room = "F209", 
                    MaxEnrollment = 15,
                    Description = "Analysis of investment opportunities"
                },
                new CourseOffering { 
                    CourseCode = "MB405", 
                    CourseName = "Entrepreneurship", 
                    Branch = "MBA", 
                    Credits = 3, 
                    Instructor = "Prof. Irwin", 
                    Schedule = "Tue,Thu 13:00-14:30", 
                    Room = "F210", 
                    MaxEnrollment = 10,
                    Description = "Starting and managing new ventures"
                },
                
                // MCA Courses (20 courses)
                new CourseOffering { 
                    CourseCode = "MC101", 
                    CourseName = "Advanced Database Systems", 
                    Branch = "MCA", 
                    Credits = 4, 
                    Instructor = "Dr. Martin", 
                    Schedule = "Mon,Wed 09:00-11:00", 
                    Room = "G101", 
                    MaxEnrollment = 40,
                    Description = "Advanced concepts in database management"
                },
                new CourseOffering { 
                    CourseCode = "MC201", 
                    CourseName = "Web Technologies", 
                    Branch = "MCA", 
                    Credits = 3, 
                    Instructor = "Dr. Clark", 
                    Schedule = "Tue,Thu 14:00-15:30", 
                    Room = "G102", 
                    MaxEnrollment = 35,
                    Description = "Modern web development technologies and frameworks"
                },
                new CourseOffering { 
                    CourseCode = "MC301", 
                    CourseName = "Software Engineering", 
                    Branch = "MCA", 
                    Credits = 4, 
                    Instructor = "Dr. Lewis", 
                    Schedule = "Mon,Wed 11:00-13:00", 
                    Room = "G201", 
                    MaxEnrollment = 30,
                    Description = "Advanced software development methodologies"
                },
                new CourseOffering { 
                    CourseCode = "MC401", 
                    CourseName = "Mobile Computing", 
                    Branch = "MCA", 
                    Credits = 3, 
                    Instructor = "Dr. Walker", 
                    Schedule = "Tue,Thu 16:00-17:30", 
                    Room = "G202", 
                    MaxEnrollment = 25,
                    Description = "Mobile application development and technologies"
                },
                new CourseOffering { 
                    CourseCode = "MC102", 
                    CourseName = "Advanced Programming", 
                    Branch = "MCA", 
                    Credits = 4, 
                    Instructor = "Dr. Hall", 
                    Schedule = "Mon,Wed 13:00-15:00", 
                    Room = "G103", 
                    MaxEnrollment = 35,
                    Description = "Advanced programming concepts and techniques"
                },
                new CourseOffering { 
                    CourseCode = "MC202", 
                    CourseName = "Computer Networks", 
                    Branch = "MCA", 
                    Credits = 3, 
                    Instructor = "Dr. Young", 
                    Schedule = "Tue,Thu 09:00-10:30", 
                    Room = "G104", 
                    MaxEnrollment = 30,
                    Description = "Advanced networking concepts and protocols"
                },
                new CourseOffering { 
                    CourseCode = "MC302", 
                    CourseName = "Artificial Intelligence", 
                    Branch = "MCA", 
                    Credits = 4, 
                    Instructor = "Dr. King", 
                    Schedule = "Mon,Wed 15:00-17:00", 
                    Room = "G203", 
                    MaxEnrollment = 25,
                    Description = "Artificial intelligence algorithms and applications"
                },
                new CourseOffering { 
                    CourseCode = "MC402", 
                    CourseName = "Cloud Computing", 
                    Branch = "MCA", 
                    Credits = 3, 
                    Instructor = "Dr. Wright", 
                    Schedule = "Tue,Thu 11:00-12:30", 
                    Room = "G204", 
                    MaxEnrollment = 20,
                    Description = "Cloud computing technologies and services"
                },
                new CourseOffering { 
                    CourseCode = "MC103", 
                    CourseName = "Data Mining", 
                    Branch = "MCA", 
                    Credits = 3, 
                    Instructor = "Dr. Scott", 
                    Schedule = "Mon,Wed 09:00-10:30", 
                    Room = "G105", 
                    MaxEnrollment = 30,
                    Description = "Data mining techniques and applications"
                },
                new CourseOffering { 
                    CourseCode = "MC203", 
                    CourseName = "Information Security", 
                    Branch = "MCA", 
                    Credits = 3, 
                    Instructor = "Dr. Green", 
                    Schedule = "Tue,Thu 13:00-14:30", 
                    Room = "G106", 
                    MaxEnrollment = 25,
                    Description = "Information security principles and practices"
                },
                new CourseOffering { 
                    CourseCode = "MC303", 
                    CourseName = "Machine Learning", 
                    Branch = "MCA", 
                    Credits = 4, 
                    Instructor = "Dr. Adams", 
                    Schedule = "Mon,Wed 11:00-13:00", 
                    Room = "G205", 
                    MaxEnrollment = 20,
                    Description = "Machine learning algorithms and applications"
                },
                new CourseOffering { 
                    CourseCode = "MC403", 
                    CourseName = "Big Data Analytics", 
                    Branch = "MCA", 
                    Credits = 4, 
                    Instructor = "Dr. Baker", 
                    Schedule = "Tue,Thu 15:00-17:00", 
                    Room = "G206", 
                    MaxEnrollment = 15,
                    Description = "Analysis of large-scale data sets"
                },
                new CourseOffering { 
                    CourseCode = "MC104", 
                    CourseName = "Advanced Operating Systems", 
                    Branch = "MCA", 
                    Credits = 3, 
                    Instructor = "Dr. Carter", 
                    Schedule = "Mon,Wed 13:00-14:30", 
                    Room = "G107", 
                    MaxEnrollment = 25,
                    Description = "Advanced concepts in operating systems"
                },
                new CourseOffering { 
                    CourseCode = "MC204", 
                    CourseName = "Software Testing", 
                    Branch = "MCA", 
                    Credits = 3, 
                    Instructor = "Dr. Evans", 
                    Schedule = "Tue,Thu 09:00-10:30", 
                    Room = "G108", 
                    MaxEnrollment = 20,
                    Description = "Software testing methodologies and tools"
                },
                new CourseOffering { 
                    CourseCode = "MC304", 
                    CourseName = "Internet of Things", 
                    Branch = "MCA", 
                    Credits = 3, 
                    Instructor = "Dr. Foster", 
                    Schedule = "Mon,Wed 15:00-16:30", 
                    Room = "G207", 
                    MaxEnrollment = 15,
                    Description = "IoT technologies and applications"
                },
                new CourseOffering { 
                    CourseCode = "MC404", 
                    CourseName = "Natural Language Processing", 
                    Branch = "MCA", 
                    Credits = 4, 
                    Instructor = "Dr. Gray", 
                    Schedule = "Tue,Thu 11:00-13:00", 
                    Room = "G208", 
                    MaxEnrollment = 10,
                    Description = "Processing and understanding human language"
                },
                new CourseOffering { 
                    CourseCode = "MC105", 
                    CourseName = "Computer Vision", 
                    Branch = "MCA", 
                    Credits = 4, 
                    Instructor = "Dr. Hill", 
                    Schedule = "Mon,Wed 09:00-11:00", 
                    Room = "G109", 
                    MaxEnrollment = 20,
                    Description = "Computer vision algorithms and applications"
                },
                new CourseOffering { 
                    CourseCode = "MC205", 
                    CourseName = "Distributed Systems", 
                    Branch = "MCA", 
                    Credits = 3, 
                    Instructor = "Dr. Irwin", 
                    Schedule = "Tue,Thu 13:00-14:30", 
                    Room = "G110", 
                    MaxEnrollment = 15,
                    Description = "Distributed computing systems and algorithms"
                },
                new CourseOffering { 
                    CourseCode = "MC305", 
                    CourseName = "Data Visualization", 
                    Branch = "MCA", 
                    Credits = 3, 
                    Instructor = "Dr. Jones", 
                    Schedule = "Mon,Wed 11:00-12:30", 
                    Room = "G209", 
                    MaxEnrollment = 10,
                    Description = "Techniques for visualizing complex data"
                },
                new CourseOffering { 
                    CourseCode = "MC405", 
                    CourseName = "Blockchain Technology", 
                    Branch = "MCA", 
                    Credits = 3, 
                    Instructor = "Dr. Kelly", 
                    Schedule = "Tue,Thu 15:00-16:30", 
                    Room = "G210", 
                    MaxEnrollment = 5,
                    Description = "Blockchain principles and applications"
                }
            };
        }
        
        public static void SeedCourseOfferings(ApplicationDbContext context)
        {
            if (!context.Set<CourseOffering>().Any())
            {
                context.Set<CourseOffering>().AddRange(GetCourseOfferings());
                context.SaveChanges();
            }
        }
    }
}