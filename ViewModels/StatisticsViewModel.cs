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

        public int ActiveProjectsCount { get; private set; }
        public int OverdueTasksStat { get; private set; }
        public int AverageProgressRounded { get; private set; }
        public int TeamWorkloadRounded { get; private set; }
        public int TasksTotalStat { get; private set; }

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
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            try
            {
                var projects = _context.Projects.Include("Tasks").AsNoTracking().ToList();
                ActiveProjectsCount = projects.Count(p => p.Tasks != null && p.Tasks.Any() && !p.IsCompleted);
                OverdueTasksStat = _context.Tasks.AsNoTracking().Count(t =>
                    t.DueDate.HasValue
                    && t.DueDate.Value.Date < DateTime.Today.Date
                    && t.Status != "Завершена");

                var withTasks = projects.Where(p => p.Tasks != null && p.Tasks.Any()).ToList();
                AverageProgressRounded = (int)Math.Round(withTasks.Count == 0 ? 0 : withTasks.Average(p => p.CompletionPercentage));

                var employees = _context.Employees.Include("Tasks").AsNoTracking().ToList();
                TeamWorkloadRounded = employees.Count == 0
                    ? 0
                    : (int)Math.Round(employees.Average(e =>
                        Math.Min(100, e.Tasks.Count(t => t.Status != "Завершена") * 14.0)));

                TasksTotalStat = _context.Tasks.AsNoTracking().Count();

                OnPropertyChanged(nameof(ActiveProjectsCount));
                OnPropertyChanged(nameof(OverdueTasksStat));
                OnPropertyChanged(nameof(AverageProgressRounded));
                OnPropertyChanged(nameof(TeamWorkloadRounded));
                OnPropertyChanged(nameof(TasksTotalStat));

                EmployeeStatistics.Clear();
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
