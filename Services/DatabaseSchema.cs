using System;
using System.Data.Entity;
using ProjectManager.Data;

namespace ProjectManager.Services
{
    /// <summary>
    /// Идемпотентное обновление схемы БД для существующих установок (без EF Migrations).
    /// </summary>
    public static class DatabaseSchema
    {
        public static void EnsureLatest(ProjectManagerDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            // Отдельные вызовы: в SqlClient нет разделителя GO.
            context.Database.ExecuteSqlCommand(@"
IF COL_LENGTH('dbo.Projects', 'Notes') IS NULL
    ALTER TABLE dbo.Projects ADD Notes NVARCHAR(MAX) NULL;
");

            context.Database.ExecuteSqlCommand(@"
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
");

            context.Database.ExecuteSqlCommand(@"
IF COL_LENGTH('dbo.Tasks', 'DueDate') IS NULL
    ALTER TABLE dbo.Tasks ADD DueDate DATETIME NULL;
");

            context.Database.ExecuteSqlCommand(@"
IF COL_LENGTH('dbo.Employees', 'Skills') IS NULL
    ALTER TABLE dbo.Employees ADD Skills NVARCHAR(300) NULL;
");

            context.Database.ExecuteSqlCommand(@"
IF COL_LENGTH('dbo.Projects', 'StartDate') IS NULL
    ALTER TABLE dbo.Projects ADD StartDate DATETIME NULL;
IF COL_LENGTH('dbo.Projects', 'Client') IS NULL
    ALTER TABLE dbo.Projects ADD Client NVARCHAR(200) NULL;
IF COL_LENGTH('dbo.Projects', 'ProjectStatus') IS NULL
    ALTER TABLE dbo.Projects ADD ProjectStatus NVARCHAR(50) NULL;
IF COL_LENGTH('dbo.Projects', 'ProjectPriority') IS NULL
    ALTER TABLE dbo.Projects ADD ProjectPriority NVARCHAR(50) NULL;
IF COL_LENGTH('dbo.Projects', 'Budget') IS NULL
    ALTER TABLE dbo.Projects ADD Budget DECIMAL(18,2) NULL;
IF COL_LENGTH('dbo.Projects', 'Tags') IS NULL
    ALTER TABLE dbo.Projects ADD Tags NVARCHAR(500) NULL;
IF COL_LENGTH('dbo.Projects', 'ResponsibleEmployeeID') IS NULL
    ALTER TABLE dbo.Projects ADD ResponsibleEmployeeID INT NULL;
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Projects_ResponsibleEmployee')
BEGIN
    IF COL_LENGTH('dbo.Projects', 'ResponsibleEmployeeID') IS NOT NULL
        ALTER TABLE dbo.Projects ADD CONSTRAINT FK_Projects_ResponsibleEmployee
            FOREIGN KEY (ResponsibleEmployeeID) REFERENCES dbo.Employees(ID);
END
");
        }
    }
}
