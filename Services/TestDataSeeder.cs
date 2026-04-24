using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ProjectManager.Data;
using ProjectManager.Models;
using System.Windows;

namespace ProjectManager.Services
{
    public static class TestDataSeeder
    {
        private static readonly DateTime Today = new DateTime(2026, 4, 24);

        public static void SeedIfEmpty()
        {
            try
            {
                using (var context = new ProjectManagerDbContext())
                {
                    try
                    {
                        context.Database.CreateIfNotExists();
                    }
                    catch
                    {
                        return;
                    }

                    if (!context.Projects.Any() && !context.Employees.Any() && !context.Tasks.Any())
                    {
                        SeedAll(context);
                    }

                    EnsureDemoUser(context);
                }
            }
            catch
            {
            }
        }

        public static void ForceReseed()
        {
            try
            {
                using (var context = new ProjectManagerDbContext())
                {
                    context.Database.CreateIfNotExists();

                    DatabaseService.ExecuteInTransaction(context, () =>
                    {
                        context.Tasks.RemoveRange(context.Tasks);
                        context.Projects.RemoveRange(context.Projects);
                        context.Users.RemoveRange(context.Users);
                        context.Employees.RemoveRange(context.Employees);
                        context.SaveChanges();

                        SeedAll(context);
                        EnsureDemoUser(context);
                    });
                }
            }
            catch (Exception ex)
            {
                try
                {
                    MessageBox.Show("Не удалось загрузить демо-данные.\n\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch { }
            }
        }

        private static void SeedAll(ProjectManagerDbContext context)
        {
            DatabaseService.ExecuteInTransaction(context, () =>
            {
                var employees = SeedEmployees(context);
                var projects = SeedProjects(context);
                SeedTasks(context, projects, employees);
            });
        }

        private static List<Employee> SeedEmployees(ProjectManagerDbContext context)
        {
            var employees = new List<Employee>
            {
                new Employee { FullName = "Алина Сергеева", Position = "Разработчик" },
                new Employee { FullName = "Илья Миронов", Position = "Тестировщик" },
                new Employee { FullName = "Мария Кузнецова", Position = "Менеджер" },
                new Employee { FullName = "Ольга Федорова", Position = "Аналитик" },
                new Employee { FullName = "Денис Орлов", Position = "Разработчик" },
                new Employee { FullName = "Екатерина Волкова", Position = "Дизайнер" },
                new Employee { FullName = "Артём Климов", Position = "DevOps" },
                new Employee { FullName = "Наталья Белова", Position = "Руководитель проекта" }
            };

            context.Employees.AddRange(employees);
            context.SaveChanges();
            return employees;
        }

        private static List<Project> SeedProjects(ProjectManagerDbContext context)
        {
            var projects = new List<Project>
            {
                new Project
                {
                    Name = "Сайт корпоративного портала",
                    Description = "Авторизация, новости, база знаний, интеграция с задачами и ролями доступа.",
                    Deadline = Today.AddDays(36)
                },
                new Project
                {
                    Name = "Внедрение CRM",
                    Description = "Миграция данных, обучение отдела продаж, интеграция с почтой и телефонией.",
                    Deadline = Today.AddDays(68)
                },
                new Project
                {
                    Name = "Мобильное приложение",
                    Description = "MVP для iOS/Android, интеграция с корпоративным API и пуш-уведомлениями.",
                    Deadline = Today.AddDays(22)
                },
                new Project
                {
                    Name = "Рефакторинг отчётов",
                    Description = "Перенос отчётности на единый модуль, оптимизация запросов и кеширование.",
                    Deadline = Today.AddDays(14)
                },
                new Project
                {
                    Name = "Автоматизация HR-процессов",
                    Description = "Онбординг, заявки на отпуск, согласование документов, оргструктура.",
                    Deadline = Today.AddDays(52)
                },
                new Project
                {
                    Name = "Интеграция с 1С",
                    Description = "Синхронизация справочников, обмен заказами, мониторинг ошибок интеграции.",
                    Deadline = Today.AddDays(90)
                }
            };

            context.Projects.AddRange(projects);
            context.SaveChanges();
            return projects;
        }

        private static void SeedTasks(ProjectManagerDbContext context, List<Project> projects, List<Employee> employees)
        {
            var statuses = new[] { "Новая", "В работе", "На проверке", "Завершена" };
            var priorities = new[] { "Низкий", "Средний", "Высокий", "Критический" };

            var tasks = new List<Task>
            {
                NewTask(projects[0], employees[2], "Сверстать главную страницу портала", statuses[1], priorities[2]),
                NewTask(projects[0], employees[0], "Подключить SSO и роли доступа", statuses[2], priorities[3]),
                NewTask(projects[0], employees[5], "Собрать UI-кит для компонентов", statuses[3], priorities[1]),
                NewTask(projects[0], employees[3], "Согласовать требования к базе знаний", statuses[1], priorities[1]),

                NewTask(projects[1], employees[7], "Подготовить план миграции данных", statuses[1], priorities[2]),
                NewTask(projects[1], employees[4], "Импорт контактов и сделок", statuses[0], priorities[2]),
                NewTask(projects[1], employees[6], "Настроить мониторинг интеграции", statuses[2], priorities[1]),

                NewTask(projects[2], employees[0], "Экран авторизации + валидация", statuses[3], priorities[1]),
                NewTask(projects[2], employees[5], "Прототип навигации и экранов", statuses[2], priorities[2]),
                NewTask(projects[2], employees[6], "CI сборок и доставка тестерам", statuses[1], priorities[2]),

                NewTask(projects[3], employees[3], "Инвентаризация отчётов и источников", statuses[1], priorities[1]),
                NewTask(projects[3], employees[4], "Оптимизировать топ-5 медленных запросов", statuses[2], priorities[2]),
                NewTask(projects[3], employees[1], "Покрыть отчёты тестами", statuses[0], priorities[1]),

                NewTask(projects[4], employees[7], "Схема согласований и статусов", statuses[1], priorities[1]),
                NewTask(projects[4], employees[2], "Форма заявки на отпуск", statuses[2], priorities[1]),
                NewTask(projects[4], employees[5], "Дизайн карточки сотрудника", statuses[3], priorities[1]),

                NewTask(projects[5], employees[6], "Обмен справочниками (контрагенты)", statuses[1], priorities[3]),
                NewTask(projects[5], employees[4], "Логи и ретраи ошибок интеграции", statuses[2], priorities[2]),
                NewTask(projects[5], employees[1], "Тестовый прогон на песочнице", statuses[0], priorities[1]),
            };

            var extra = new[]
            {
                "Обновить документацию",
                "Проверить права доступа",
                "Согласовать макеты",
                "Подготовить релиз-ноты",
                "Ускорить загрузку списка",
                "Починить уведомления",
                "Провести демо заказчику",
                "Собрать обратную связь",
                "Настроить бэкапы",
                "Оптимизировать индексы",
            };

            for (int i = 0; i < 24; i++)
            {
                var p = projects[i % projects.Count];
                var e = employees[(i * 3) % employees.Count];
                var st = statuses[i % statuses.Length];
                var pr = priorities[(i + 1) % priorities.Length];
                tasks.Add(NewTask(p, e, extra[i % extra.Length] + $" #{i + 1}", st, pr));
            }

            context.Tasks.AddRange(tasks);
            context.SaveChanges();
        }

        private static void EnsureDemoUser(ProjectManagerDbContext context)
        {
            const string username = "annakoval002@gmail.com";
            const string password = "anna002koval";

            if (context.Users.Any(u => u.Username == username))
                return;

            var employee = context.Employees.FirstOrDefault(emp => emp.FullName == "Анна Коваль")
                           ?? context.Employees.FirstOrDefault();

            int? employeeId;
            if (employee == null)
            {
                employee = new Employee
                {
                    FullName = "Анна Коваль",
                    Position = "Менеджер проекта"
                };
                context.Employees.Add(employee);
                context.SaveChanges();
                employeeId = employee.ID;
            }
            else
            {
                employeeId = employee.ID;
            }

            AuthService.Register(username, password, employeeId, out _);
        }

        private static Task NewTask(Project project, Employee employee, string title, string status, string priority)
        {
            return new Task
            {
                ProjectID = project.ID,
                EmployeeID = employee?.ID,
                Title = title,
                Status = status,
                Priority = priority
            };
        }
    }
}
