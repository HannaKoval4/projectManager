using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using ProjectManager.Data;
using ProjectManager.Models;

namespace ProjectManager.ViewModels
{
    public class EmployeeStatistic
    {
        public string EmployeeName { get; set; }
        public int CompletedTasks { get; set; }
        public int ActiveTasks { get; set; }
        public double Productivity { get; set; }
    }

    public class StatisticsViewModel : INotifyPropertyChanged
    {
        private ProjectManagerDbContext _context;
        private string _selectedPeriod = "Month";

        public int TotalProjects { get; set; }
        public int CompletedTasks { get; set; }
        public int ActiveTasks { get; set; }
        public int TotalEmployees { get; set; }

        public ObservableCollection<EmployeeStatistic> EmployeeStatistics { get; set; }

        public RelayCommand<string> FilterByPeriodCommand { get; set; }

        public StatisticsViewModel(ProjectManagerDbContext context)
        {
            _context = context;
            EmployeeStatistics = new ObservableCollection<EmployeeStatistic>();

            FilterByPeriodCommand = new RelayCommand<string>(FilterByPeriod);

            LoadStatistics();
        }

        private void FilterByPeriod(string period)
        {
            _selectedPeriod = period;
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            try
            {
                var now = DateTime.Now;
                DateTime startDate;

                switch (_selectedPeriod)
                {
                    case "Month":
                        startDate = new DateTime(now.Year, now.Month, 1);
                        break;
                    case "Quarter":
                        var quarter = (now.Month - 1) / 3;
                        startDate = new DateTime(now.Year, quarter * 3 + 1, 1);
                        break;
                    case "Year":
                        startDate = new DateTime(now.Year, 1, 1);
                        break;
                    default:
                        startDate = new DateTime(now.Year, now.Month, 1);
                        break;
                }

                TotalProjects = _context.Projects.Count();
                CompletedTasks = _context.Tasks.Count(t => t.Status == "Завершена");
                ActiveTasks = _context.Tasks.Count(t => t.Status != "Завершена");
                TotalEmployees = _context.Employees.Count();

                OnPropertyChanged(nameof(TotalProjects));
                OnPropertyChanged(nameof(CompletedTasks));
                OnPropertyChanged(nameof(ActiveTasks));
                OnPropertyChanged(nameof(TotalEmployees));

                EmployeeStatistics.Clear();
                // Оптимизация: используем AsNoTracking для чтения
                var employees = _context.Employees
                    .Include("Tasks")
                    .AsNoTracking()
                    .ToList();

                foreach (var employee in employees)
                {
                    var completed = employee.Tasks.Count(t => t.Status == "Завершена");
                    var active = employee.Tasks.Count(t => t.Status != "Завершена");
                    var total = completed + active;
                    var productivity = total > 0 ? (double)completed / total * 100 : 0;

                    EmployeeStatistics.Add(new EmployeeStatistic
                    {
                        EmployeeName = employee.FullName,
                        CompletedTasks = completed,
                        ActiveTasks = active,
                        Productivity = productivity
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки статистики: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

