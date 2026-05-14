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
using TaskModel = ProjectManager.Models.Task;

namespace ProjectManager.ViewModels
{
    public class TasksViewModel : INotifyPropertyChanged
    {
        private readonly Project _allProjectsMarker = new Project { ID = -1, Name = "Все" };
        private readonly Employee _anyEmployeeMarker = new Employee { ID = -1, FullName = "Любой", Position = "-" };

        private ProjectManagerDbContext _context;
        private TaskModel _selectedTask;
        private Project _selectedProjectFilter;
        private Employee _selectedEmployeeFilter;
        private string _selectedPriorityFilter = "Любой";
        private string _statusChip = "Все";
        private string _searchQuery = string.Empty;

        public ObservableCollection<TaskModel> Tasks { get; set; }
        public ObservableCollection<TaskModel> FilteredTasks { get; set; }
        public ObservableCollection<Project> Projects { get; set; }
        public ObservableCollection<Employee> Employees { get; set; }
        public ObservableCollection<string> Statuses { get; set; }
        public ObservableCollection<string> Priorities { get; set; }

        public ObservableCollection<string> StatusChipOptions { get; }
        public ObservableCollection<string> PriorityFilterOptions { get; }

        public int TotalTasksCount { get; private set; }
        public int OverdueTasksCount { get; private set; }
        public int InProgressTasksCount { get; private set; }

        public string StatusChip
        {
            get => _statusChip;
            set
            {
                _statusChip = value;
                OnPropertyChanged(nameof(StatusChip));
                ApplyFilters();
            }
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value ?? string.Empty;
                OnPropertyChanged(nameof(SearchQuery));
                ApplyFilters();
            }
        }

        public TaskModel SelectedTask
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

        public string SelectedPriorityFilter
        {
            get => _selectedPriorityFilter;
            set
            {
                _selectedPriorityFilter = value ?? "Любой";
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
                DatabaseService.UnlinkIfEmployeeMissingOrDeleted(_context, taskToUpdate);
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
            StatusChipOptions = new ObservableCollection<string> { "Все", "Новые", "В работе", "На проверке", "Готово" };
            PriorityFilterOptions = new ObservableCollection<string> { "Любой", "Низкий", "Средний", "Высокий", "Критический" };

            Tasks = new ObservableCollection<TaskModel>();
            FilteredTasks = new ObservableCollection<TaskModel>();
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

                Projects.Add(_allProjectsMarker);
                foreach (var project in projects)
                {
                    Projects.Add(project);
                }

                Employees.Add(_anyEmployeeMarker);
                foreach (var employee in employees)
                {
                    Employees.Add(employee);
                }

                SelectedProjectFilter = _allProjectsMarker;
                SelectedEmployeeFilter = _anyEmployeeMarker;
                SelectedPriorityFilter = "Любой";
                StatusChip = "Все";
                SearchQuery = string.Empty;

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

            if (SelectedProjectFilter != null && SelectedProjectFilter.ID > 0)
            {
                filtered = filtered.Where(t => t.ProjectID == SelectedProjectFilter.ID);
            }

            if (!string.IsNullOrEmpty(StatusChip) && StatusChip != "Все")
            {
                var dbStatus = StatusChip == "Новые" ? "Новая"
                    : StatusChip == "Готово" ? "Завершена"
                    : StatusChip;
                filtered = filtered.Where(t => t.Status == dbStatus);
            }

            if (!string.IsNullOrWhiteSpace(SelectedPriorityFilter) && SelectedPriorityFilter != "Любой")
            {
                filtered = filtered.Where(t => t.Priority == SelectedPriorityFilter);
            }

            if (SelectedEmployeeFilter != null && SelectedEmployeeFilter.ID > 0)
            {
                filtered = filtered.Where(t => t.EmployeeID == SelectedEmployeeFilter.ID);
            }

            var q = SearchQuery.Trim();
            if (!string.IsNullOrEmpty(q))
            {
                filtered = filtered.Where(t =>
                    (t.Title != null && t.Title.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0));
            }

            foreach (var task in filtered)
            {
                FilteredTasks.Add(task);
            }

            UpdateSummary();
        }

        private void UpdateSummary()
        {
            TotalTasksCount = Tasks.Count;
            OverdueTasksCount = Tasks.Count(t =>
                t.DueDate.HasValue
                && t.DueDate.Value.Date < DateTime.Today.Date
                && t.Status != "Завершена");
            InProgressTasksCount = Tasks.Count(t => t.Status == "В работе");

            OnPropertyChanged(nameof(TotalTasksCount));
            OnPropertyChanged(nameof(OverdueTasksCount));
            OnPropertyChanged(nameof(InProgressTasksCount));
        }

        private void AddTask()
        {
            var proj = Projects.FirstOrDefault(p => p.ID > 0);
            if (proj == null)
            {
                MessageBox.Show("Сначала создайте проект", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new TaskDialog(proj.ID, _context);
            if (dialog.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void EditTask()
        {
            if (SelectedTask == null) return;

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
                DatabaseService.UnlinkIfEmployeeMissingOrDeleted(_context, taskToComplete);
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
            SelectedProjectFilter = _allProjectsMarker;
            SelectedEmployeeFilter = _anyEmployeeMarker;
            SelectedPriorityFilter = "Любой";
            StatusChip = "Все";
            SearchQuery = string.Empty;
            ApplyFilters();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
