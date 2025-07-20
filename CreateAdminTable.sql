-- CreateAdminTable.sql
-- This script enhances the Admin table security and adds password reset functionality

-- Use this script with MySQL/MariaDB database

-- Drop and recreate the Admin table with improved security fields
DROP TABLE IF EXISTS `Admins`;

CREATE TABLE `Admins` (
    `AdminId` CHAR(36) NOT NULL,
    `Username` VARCHAR(100) NOT NULL,
    `Password` VARCHAR(255) NOT NULL,  -- Increased length for password hashing
    `FirstName` VARCHAR(100) NULL,
    `LastName` VARCHAR(100) NULL,
    `Email` VARCHAR(255) NULL,
    `Role` VARCHAR(50) NULL DEFAULT 'Admin',
    `CreatedAt` DATETIME NOT NULL,
    `LastLogin` DATETIME NULL,
    `PasswordResetToken` VARCHAR(100) NULL,  -- For password reset functionality
    `PasswordResetExpiry` DATETIME NULL,     -- Token expiration timestamp
    PRIMARY KEY (`AdminId`),
    UNIQUE KEY `UK_Admins_Username` (`Username`)
);

-- Insert a default SuperAdmin user with BCrypt hashed password
-- The password 'admin123' is hashed with BCrypt
-- WARNING: Change this default password immediately after deployment!
INSERT INTO `Admins` (`AdminId`, `Username`, `Password`, `FirstName`, `LastName`, `Email`, `Role`, `CreatedAt`)
VALUES 
('11111111-1111-1111-1111-111111111111', 'admin', '$2a$11$1mN9MtoLb5x./cJJfF9DUOs7O6w2HMInYQ6D.U1OU1Vm4eMjMKmNa', 'System', 'Administrator', 'admin@freshly.com', 'SuperAdmin', NOW());

-- Security reminder
-- IMPORTANT: The default admin password should be changed immediately after deployment!
-- Default credentials: username: admin, password: admin123