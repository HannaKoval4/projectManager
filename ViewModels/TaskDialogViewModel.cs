using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ProjectManager.Data;
using ProjectManager.Models;
using ProjectManager.Services;

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

        private DateTime? _dueDate;

        public DateTime? DueDate
        {
            get => _dueDate;
            set
            {
                _dueDate = value;
                OnPropertyChanged(nameof(DueDate));
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
                _dueDate = task.DueDate;
                OnPropertyChanged(nameof(DueDate));
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

        public IReadOnlyList<string> GetValidationErrorMessages()
        {
            var list = new List<string>();
            string titleTrim = Title?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(titleTrim))
            {
                list.Add("Укажите название задачи.");
            }
            else if (titleTrim.Length > FieldValidation.TaskTitleMaxLength)
            {
                list.Add($"Название — не более {FieldValidation.TaskTitleMaxLength} символов.");
            }

            if (SelectedProject == null)
            {
                list.Add("Выберите проект.");
            }

            string st = Status?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(st))
            {
                list.Add("Выберите статус задачи.");
            }
            else if (st.Length > FieldValidation.TaskStatusMaxLength)
            {
                list.Add($"Статус — не более {FieldValidation.TaskStatusMaxLength} символов.");
            }

            string pr = Priority?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(pr))
            {
                list.Add("Выберите приоритет.");
            }
            else if (pr.Length > FieldValidation.TaskPriorityMaxLength)
            {
                list.Add($"Приоритет — не более {FieldValidation.TaskPriorityMaxLength} символов.");
            }

            if (DueDate.HasValue && SelectedProject != null)
            {
                if (SelectedProject.Deadline.HasValue && DueDate.Value.Date > SelectedProject.Deadline.Value.Date)
                {
                    list.Add("Срок задачи не может быть позже дедлайна проекта.");
                }

                if (SelectedProject.StartDate.HasValue && DueDate.Value.Date < SelectedProject.StartDate.Value.Date)
                {
                    list.Add("Срок задачи не может быть раньше даты начала проекта.");
                }
            }

            return list;
        }

        public bool TryValidate(out IReadOnlyList<string> errors)
        {
            errors = GetValidationErrorMessages();
            return errors.Count == 0;
        }

        public void Save()
        {
            if (!TryValidate(out _))
            {
                throw new InvalidOperationException("Проверьте заполнение полей задачи.");
            }

            _task.Title = Title?.Trim();
            _task.Status = Status?.Trim();
            _task.Priority = Priority?.Trim();
            _task.ProjectID = SelectedProject.ID;
            _task.EmployeeID = SelectedEmployee?.ID;
            _task.DueDate = DueDate;

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
