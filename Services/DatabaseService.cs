using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using ProjectManager.Data;
using TaskModel = ProjectManager.Models.Task;

namespace ProjectManager.Services
{
    /// <summary>
    /// Сервис для безопасной работы с базой данных с использованием транзакций
    /// </summary>
    public class DatabaseService
    {
        /// <summary>
        /// Выполняет операцию в транзакции с обработкой ошибок
        /// </summary>
        public static bool ExecuteInTransaction(ProjectManagerDbContext context, Action operation, out string errorMessage)
        {
            errorMessage = null;
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    operation();
                    context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (DbEntityValidationException ex)
                {
                    transaction.Rollback();
                    errorMessage = "Ошибка валидации данных: " + string.Join("; ", 
                        ex.EntityValidationErrors.SelectMany(e => e.ValidationErrors.Select(v => v.ErrorMessage)));
                    return false;
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    transaction.Rollback();
                    errorMessage = "Ошибка базы данных: " + ex.Message;
                    return false;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    errorMessage = "Ошибка: " + ex.Message;
                    return false;
                }
            }
        }

        /// <summary>
        /// Выполняет операцию в транзакции без возврата сообщения об ошибке
        /// </summary>
        public static bool ExecuteInTransaction(ProjectManagerDbContext context, Action operation)
        {
            return ExecuteInTransaction(context, operation, out _);
        }

        /// <summary>
        /// Снимает ссылку на сотрудника, если сущность удалена в контексте или отсутствует.
        /// Иначе обновление задачи при SaveChanges падает с ошибкой о «deleted principal».
        /// </summary>
        public static void UnlinkIfEmployeeMissingOrDeleted(ProjectManagerDbContext context, TaskModel task)
        {
            if (task == null || !task.EmployeeID.HasValue) return;
            var id = task.EmployeeID.Value;
            var employee = context.Employees.Find(id);
            if (employee == null)
            {
                task.Employee = null;
                task.EmployeeID = null;
                return;
            }
            if (context.Entry(employee).State == EntityState.Deleted)
            {
                task.Employee = null;
                task.EmployeeID = null;
            }
        }
    }
}






