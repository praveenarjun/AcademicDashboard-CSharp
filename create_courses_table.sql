-- Create Courses table if it doesn't exist
CREATE TABLE IF NOT EXISTS `Courses` (
  `Name` varchar(50) NOT NULL,
  `Fee` decimal(10,2) NOT NULL,
  `Description` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`Name`),
  UNIQUE KEY `IX_Courses_Name` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;