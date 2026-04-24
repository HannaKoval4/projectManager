using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ProjectManager.Data;
using ProjectManager.Models;

namespace ProjectManager.ViewModels
{
    public class TaskDialogViewModel : INotifyPropertyChanged
    {
        private ProjectManagerDbContext _context;
        private ProjectManager.Models.Task _task;
        private Project _selectedProject;
        private Employee _selectedEmployee;
        private string _status;
        private string _priority;

        public ObservableCollection<Project> Projects { get; set; }
        public ObservableCollection<Employee> Employees { get; set; }
        public ObservableCollection<string> Statuses { get; set; }
        public ObservableCollection<string> Priorities { get; set; }

        public Project SelectedProject
        {
            get => _selectedProject;
            set
            {
                _selectedProject = value;
                if (_task != null && _selectedProject != null)
                {
                    _task.ProjectID = _selectedProject.ID;
                }
                OnPropertyChanged(nameof(SelectedProject));
            }
        }

        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged(nameof(SelectedEmployee));
            }
        }

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public string Priority
        {
            get => _priority;
            set
            {
                _priority = value;
                OnPropertyChanged(nameof(Priority));
            }
        }

        public TaskDialogViewModel(int projectId, ProjectManagerDbContext context, ProjectManager.Models.Task task = null)
        {
            _context = context;

            if (task != null)
            {
                _task = task;
                _title = task.Title;
                _status = task.Status;
                _priority = task.Priority;
                OnPropertyChanged(nameof(Title));
                OnPropertyChanged(nameof(Status));
                OnPropertyChanged(nameof(Priority));
                SelectedEmployee = task.Employee;
            }
            else
            {
                _task = new ProjectManager.Models.Task { ProjectID = projectId };
            }

            Projects = new ObservableCollection<Project>(_context.Projects.OrderBy(p => p.Name).ToList());
            Employees = new ObservableCollection<Employee>(_context.Employees.ToList());
            Statuses = new ObservableCollection<string> { "Новая", "В работе", "На проверке", "Завершена" };
            Priorities = new ObservableCollection<string> { "Низкий", "Средний", "Высокий", "Критический" };

            if (task == null)
            {
                Status = Statuses.First();
                Priority = Priorities[1];
            }

            if (projectId > 0)
            {
                SelectedProject = Projects.FirstOrDefault(p => p.ID == projectId);
            }

            if (SelectedProject == null)
            {
                SelectedProject = Projects.FirstOrDefault();
            }

            if (task != null)
            {
                SelectedProject = Projects.FirstOrDefault(p => p.ID == _task.ProjectID) ?? _context.Projects.Find(_task.ProjectID);
            }
        }

        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Title)) return false;
            if (SelectedProject == null) return false;
            if (string.IsNullOrWhiteSpace(Status)) return false;
            if (string.IsNullOrWhiteSpace(Priority)) return false;
            return true;
        }

        public void Save()
        {
            if (!Validate())
            {
                throw new InvalidOperationException("Заполните все обязательные поля");
            }

            _task.Title = Title;
            _task.Status = Status;
            _task.Priority = Priority;
            _task.ProjectID = SelectedProject.ID;
            _task.EmployeeID = SelectedEmployee?.ID;

            if (_task.ID == 0)
            {
                _context.Tasks.Add(_task);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
