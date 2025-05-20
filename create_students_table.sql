-- Create Students table if it doesn't exist
CREATE TABLE IF NOT EXISTS `Students` (
  `StudentId` varchar(10) NOT NULL,
  `StudentName` varchar(100) NOT NULL,
  `PhoneNumber` varchar(20) NOT NULL,
  `FatherName` varchar(100) NOT NULL,
  `MotherName` varchar(100) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `Password` varchar(100) NOT NULL,
  `Course` varchar(50) NOT NULL,
  `RegistrationDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `PaymentStatus` tinyint(1) NOT NULL DEFAULT '0',
  `AmountPaid` decimal(10,2) NOT NULL DEFAULT '0.00',
  PRIMARY KEY (`StudentId`),
  UNIQUE KEY `IX_Students_Email` (`Email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;