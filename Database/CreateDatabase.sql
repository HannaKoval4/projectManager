USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'ProjectManagerDB')
    DROP DATABASE ProjectManagerDB;
GO

CREATE DATABASE ProjectManagerDB;
GO

USE ProjectManagerDB;
GO

CREATE TABLE Employees (
    ID INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(200) NOT NULL,
    Position NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE Projects (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    Deadline DATETIME,
    Notes NVARCHAR(MAX) NULL
);
GO

CREATE TABLE Tasks (
    ID INT PRIMARY KEY IDENTITY(1,1),
    ProjectID INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    Priority NVARCHAR(50) NOT NULL,
    EmployeeID INT,
    FOREIGN KEY (ProjectID) REFERENCES Projects(ID) ON DELETE CASCADE,
    FOREIGN KEY (EmployeeID) REFERENCES Employees(ID) ON DELETE SET NULL
);
GO

CREATE TABLE TaskComments (
    ID INT PRIMARY KEY IDENTITY(1,1),
    TaskID INT NOT NULL,
    Text NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    Author NVARCHAR(100) NULL,
    CONSTRAINT FK_TaskComments_Tasks FOREIGN KEY (TaskID) REFERENCES Tasks(ID) ON DELETE CASCADE
);
GO

CREATE TABLE Users (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(100) NOT NULL,
    PasswordHash VARBINARY(64) NOT NULL,
    PasswordSalt VARBINARY(16) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    EmployeeID INT NULL,
    CONSTRAINT UQ_Users_Username UNIQUE (Username),
    CONSTRAINT FK_Users_Employees FOREIGN KEY (EmployeeID) REFERENCES Employees(ID) ON DELETE SET NULL
);
GO

INSERT INTO Employees (FullName, Position) VALUES
(N'Иванов Иван Иванович', N'Разработчик'),
(N'Петрова Мария Сергеевна', N'Менеджер проектов'),
(N'Сидоров Петр Александрович', N'Тестировщик');
GO

INSERT INTO Projects (Name, Description, Deadline) VALUES
(N'Веб-сайт компании', N'Разработка корпоративного веб-сайта', '2024-12-31'),
(N'Мобильное приложение', N'Создание мобильного приложения для iOS и Android', '2025-03-15');
GO

INSERT INTO Tasks (ProjectID, Title, Status, Priority, EmployeeID) VALUES
(1, N'Дизайн главной страницы', N'В работе', N'Высокий', 1),
(1, N'Разработка API', N'Новая', N'Средний', 1),
(2, N'Прототипирование интерфейса', N'Завершена', N'Высокий', 2);
GO
