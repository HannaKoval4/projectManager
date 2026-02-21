using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using ProjectManager.Data;
using ProjectManager.Models;
using ProjectManager.Services;
using ProjectManager.Views;

namespace ProjectManager.ViewModels
{
    public class TasksViewModel : INotifyPropertyChanged
    {
        private ProjectManagerDbContext _context;
        private Task _selectedTask;
        private Project _selectedProjectFilter;
        private string _selectedStatusFilter;
        private string _selectedPriorityFilter;
        private Employee _selectedEmployeeFilter;

        public ObservableCollection<Task> Tasks { get; set; }
        public ObservableCollection<Task> FilteredTasks { get; set; }
        public ObservableCollection<Project> Projects { get; set; }
        public ObservableCollection<Employee> Employees { get; set; }
        public ObservableCollection<string> Statuses { get; set; }
        public ObservableCollection<string> Priorities { get; set; }

        public Task SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged(nameof(SelectedTask));
            }
        }

        public Project SelectedProjectFilter
        {
            get => _selectedProjectFilter;
            set
            {
                _selectedProjectFilter = value;
                OnPropertyChanged(nameof(SelectedProjectFilter));
                ApplyFilters();
            }
        }

        public string SelectedStatusFilter
        {
            get => _selectedStatusFilter;
            set
            {
                _selectedStatusFilter = value;
                OnPropertyChanged(nameof(SelectedStatusFilter));
                ApplyFilters();
            }
        }

        public string SelectedPriorityFilter
        {
            get => _selectedPriorityFilter;
            set
            {
                _selectedPriorityFilter = value;
                OnPropertyChanged(nameof(SelectedPriorityFilter));
                ApplyFilters();
            }
        }

        public Employee SelectedEmployeeFilter
        {
            get => _selectedEmployeeFilter;
            set
            {
                _selectedEmployeeFilter = value;
                OnPropertyChanged(nameof(SelectedEmployeeFilter));
                ApplyFilters();
            }
        }

        public RelayCommand AddTaskCommand { get; set; }
        public RelayCommand EditTaskCommand { get; set; }
        public RelayCommand CompleteTaskCommand { get; set; }
        public RelayCommand ResetFiltersCommand { get; set; }

        public void QuickChangeStatus(string newStatus)
        {
            if (SelectedTask == null) return;

            var taskToUpdate = _context.Tasks.Find(SelectedTask.ID);
            if (taskToUpdate == null) return;

            if (!DatabaseService.ExecuteInTransaction(_context, () =>
            {
                taskToUpdate.Status = newStatus;
            }, out string error))
            {
                MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                LoadData();
            }
        }

        public TasksViewModel(ProjectManagerDbContext context)
        {
            _context = context;
            Tasks = new ObservableCollection<Task>();
            FilteredTasks = new ObservableCollection<Task>();
            Projects = new ObservableCollection<Project>();
            Employees = new ObservableCollection<Employee>();
            Statuses = new ObservableCollection<string> { "Новая", "В работе", "На проверке", "Завершена" };
            Priorities = new ObservableCollection<string> { "Низкий", "Средний", "Высокий", "Критический" };

            AddTaskCommand = new RelayCommand(AddTask);
            EditTaskCommand = new RelayCommand(EditTask, () => SelectedTask != null);
            CompleteTaskCommand = new RelayCommand(CompleteTask, () => SelectedTask != null);
            ResetFiltersCommand = new RelayCommand(ResetFilters);

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                Tasks.Clear();
                Projects.Clear();
                Employees.Clear();

                // Оптимизация: используем AsNoTracking для чтения и загружаем только необходимые данные
                var tasks = _context.Tasks
                    .Include("Project")
                    .Include("Employee")
                    .AsNoTracking()
                    .ToList();
                var projects = _context.Projects.AsNoTracking().ToList();
                var employees = _context.Employees.AsNoTracking().ToList();

                foreach (var task in tasks)
                {
                    Tasks.Add(task);
                }

                foreach (var project in projects)
                {
                    Projects.Add(project);
                }

                foreach (var employee in employees)
                {
                    Employees.Add(employee);
                }

                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            FilteredTasks.Clear();
            var filtered = Tasks.AsEnumerable();

            if (SelectedProjectFilter != null)
            {
                filtered = filtered.Where(t => t.ProjectID == SelectedProjectFilter.ID);
            }

            if (!string.IsNullOrEmpty(SelectedStatusFilter))
            {
                filtered = filtered.Where(t => t.Status == SelectedStatusFilter);
            }

            if (!string.IsNullOrEmpty(SelectedPriorityFilter))
            {
                filtered = filtered.Where(t => t.Priority == SelectedPriorityFilter);
            }

            if (SelectedEmployeeFilter != null)
            {
                filtered = filtered.Where(t => t.EmployeeID == SelectedEmployeeFilter.ID);
            }

            foreach (var task in filtered)
            {
                FilteredTasks.Add(task);
            }
        }

        private void AddTask()
        {
            if (Projects.Count == 0)
            {
                MessageBox.Show("Сначала создайте проект", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new TaskDialog(Projects.First().ID, _context);
            if (dialog.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void EditTask()
        {
            if (SelectedTask == null) return;

            // Перезагружаем задачу из контекста для редактирования
            var taskToEdit = _context.Tasks.Find(SelectedTask.ID);
            if (taskToEdit == null) return;

            var dialog = new TaskDialog(taskToEdit.ProjectID, _context, taskToEdit);
            if (dialog.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void CompleteTask()
        {
            if (SelectedTask == null) return;

            var taskToComplete = _context.Tasks.Find(SelectedTask.ID);
            if (taskToComplete == null) return;

            if (!DatabaseService.ExecuteInTransaction(_context, () =>
            {
                taskToComplete.Status = "Завершена";
            }, out string error))
            {
                MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                LoadData();
            }
        }

        private void ResetFilters()
        {
            SelectedProjectFilter = null;
            SelectedStatusFilter = null;
            SelectedPriorityFilter = null;
            SelectedEmployeeFilter = null;
            ApplyFilters();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

