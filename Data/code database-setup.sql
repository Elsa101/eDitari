CREATE TABLE Students (
    StudentId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100),
    Surname NVARCHAR(100),
    DateOfBirth DATE,
    Email NVARCHAR(100),
    Phone NVARCHAR(50),
    Address NVARCHAR(255)
);

CREATE TABLE Grades (
    GradeId INT PRIMARY KEY IDENTITY,
    StudentId INT,
    Subject NVARCHAR(100),
    GradeValue DECIMAL(5,2),
    Date DATE,
    FOREIGN KEY (StudentId) REFERENCES Students(StudentId)
);

CREATE TABLE Attendance (
    AttendanceId INT PRIMARY KEY IDENTITY,
    StudentId INT,
    Date DATE,
    Status NVARCHAR(50),
    FOREIGN KEY (StudentId) REFERENCES Students(StudentId)
);
CREATE TABLE Subjects (
    SubjectId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100),
    TeacherId INT,
    FOREIGN KEY (TeacherId) REFERENCES Teachers(TeacherId)
);
CREATE TABLE Teachers (
    TeacherId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100),
    Surname NVARCHAR(100),
    Email NVARCHAR(100),
    Phone NVARCHAR(50)
);

