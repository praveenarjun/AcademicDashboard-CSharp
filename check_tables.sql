-- Check if Students table exists
SHOW TABLES LIKE 'Students';

-- Show structure of Students table
DESCRIBE Students;

-- Check if Courses table exists
SHOW TABLES LIKE 'Courses';

-- Show structure of Courses table
DESCRIBE Courses;

-- Count records in Students table
SELECT COUNT(*) FROM Students;

-- Count records in Courses table
SELECT COUNT(*) FROM Courses;

-- Insert test courses if none exist
INSERT IGNORE INTO Courses (Name, Fee, Description)
VALUES 
('BTech', 120000, 'Bachelor of Technology'),
('BBA', 80000, 'Bachelor of Business Administration'),
('BCA', 70000, 'Bachelor of Computer Applications'),
('MBA', 150000, 'Master of Business Administration'),
('MCA', 100000, 'Master of Computer Applications');