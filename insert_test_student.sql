-- Insert a test student
INSERT INTO Students (
    StudentId, 
    StudentName, 
    PhoneNumber, 
    FatherName, 
    MotherName, 
    Email, 
    Password, 
    Course, 
    RegistrationDate, 
    PaymentStatus, 
    AmountPaid
)
VALUES (
    'IU12345', 
    'Test Student', 
    '1234567890', 
    'Test Father', 
    'Test Mother', 
    'test@example.com', 
    'password123', 
    'BTech', 
    NOW(), 
    0, 
    0
);