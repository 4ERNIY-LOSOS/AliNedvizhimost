-- Create the database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'AliNedvizhimost')
BEGIN
    CREATE DATABASE AliNedvizhimost COLLATE Cyrillic_General_CI_AS;
END
GO

-- Use the newly created database
USE AliNedvizhimost;
GO

-- Drop tables if they exist to ensure a clean slate
DROP TABLE IF EXISTS Messages;
DROP TABLE IF EXISTS Properties;
DROP TABLE IF EXISTS Users;
GO

-- 1. Users Table
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    FirstName NVARCHAR(255),
    LastName NVARCHAR(255),
    Role NVARCHAR(50) NOT NULL DEFAULT 'Customer' CHECK (Role IN ('Customer', 'Admin'))
);
GO

PRINT 'Users table created successfully.';
GO

-- 2. PropertyInfo Table
CREATE TABLE PropertyInfo (
    PropertyId INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(255) NOT NULL,
    Address NVARCHAR(500) NOT NULL,
    Price DECIMAL(18, 2) NOT NULL,
    Area FLOAT NOT NULL,
    Rooms INT NOT NULL,
    Description NVARCHAR(MAX)
);
GO

PRINT 'PropertyInfo table created successfully.';
GO

-- 3. PropertyStatus Table
CREATE TABLE PropertyStatus (
    PropertyStatusId INT PRIMARY KEY IDENTITY(1,1),
    PropertyId INT NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT N'Активно',
    LastUpdatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_PropertyStatus_PropertyInfo FOREIGN KEY (PropertyId) REFERENCES PropertyInfo(PropertyId) ON DELETE CASCADE
);
GO

PRINT 'PropertyStatus table created successfully.';
GO

-- 4. PropertyOwner Table
CREATE TABLE PropertyOwner (
    PropertyOwnerId INT PRIMARY KEY IDENTITY(1,1),
    PropertyId INT NOT NULL,
    UserId INT NOT NULL,
    CONSTRAINT FK_PropertyOwner_PropertyInfo FOREIGN KEY (PropertyId) REFERENCES PropertyInfo(PropertyId) ON DELETE CASCADE,
    CONSTRAINT FK_PropertyOwner_Users FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
GO

PRINT 'PropertyOwner table created successfully.';
GO

-- 5. Messages Table
CREATE TABLE Messages (
    MessageId INT PRIMARY KEY IDENTITY(1,1),
    SenderId INT NOT NULL,
    ReceiverId INT NOT NULL,
    PropertyId INT NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    Timestamp DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Messages_Sender FOREIGN KEY (SenderId) REFERENCES Users(UserId),
    CONSTRAINT FK_Messages_Receiver FOREIGN KEY (ReceiverId) REFERENCES Users(UserId),
    CONSTRAINT FK_Messages_Property FOREIGN KEY (PropertyId) REFERENCES PropertyInfo(PropertyId) ON DELETE CASCADE
);
GO

PRINT 'Messages table created successfully.';
GO
