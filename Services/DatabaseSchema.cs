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
        }
    }
}
