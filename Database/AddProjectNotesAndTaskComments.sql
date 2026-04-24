-- Выполните на существующей БД ProjectManagerDB (один раз).
USE ProjectManagerDB;
GO

IF COL_LENGTH('dbo.Projects', 'Notes') IS NULL
BEGIN
    ALTER TABLE dbo.Projects ADD Notes NVARCHAR(MAX) NULL;
END
GO

IF OBJECT_ID('dbo.TaskComments', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TaskComments (
        ID INT PRIMARY KEY IDENTITY(1,1),
        TaskID INT NOT NULL,
        Text NVARCHAR(MAX) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        Author NVARCHAR(100) NULL,
        CONSTRAINT FK_TaskComments_Tasks FOREIGN KEY (TaskID) REFERENCES dbo.Tasks(ID) ON DELETE CASCADE
    );
END
GO
