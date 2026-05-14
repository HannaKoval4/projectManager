using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using ProjectManager.Data;
using ProjectManager.Models;

namespace ProjectManager.ViewModels
{
    public class ProjectDialogViewModel : INotifyPropertyChanged
    {
        private readonly ProjectManagerDbContext _context;
        private string _name;
        private string _description;
        private DateTime? _deadline;
        private DateTime? _startDate;
        private string _client;
        private string _projectStatus;
        private string _projectPriority;
        private decimal? _budget;
        private string _tags;
        private Employee _responsible;
        private string _notes;
        private bool _nameInvalid;

        public Project Project { get; set; }

        public ObservableCollection<Employee> Employees { get; }
        public ObservableCollection<string> StatusOptions { get; }
        public ObservableCollection<string> PriorityOptions { get; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
                NameInvalid = string.IsNullOrWhiteSpace(value);
            }
        }

        public bool NameInvalid
        {
            get => _nameInvalid;
            set
            {
                _nameInvalid = value;
                OnPropertyChanged(nameof(NameInvalid));
            }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(nameof(Description)); }
        }

        public DateTime? Deadline
        {
            get => _deadline;
            set { _deadline = value; OnPropertyChanged(nameof(Deadline)); }
        }

        public DateTime? StartDate
        {
            get => _startDate;
            set { _startDate = value; OnPropertyChanged(nameof(StartDate)); }
        }

        public string Client
        {
            get => _client;
            set { _client = value; OnPropertyChanged(nameof(Client)); }
        }

        public string ProjectStatus
        {
            get => _projectStatus;
            set { _projectStatus = value; OnPropertyChanged(nameof(ProjectStatus)); }
        }

        public string ProjectPriority
        {
            get => _projectPriority;
            set { _projectPriority = value; OnPropertyChanged(nameof(ProjectPriority)); }
        }

        public decimal? Budget
        {
            get => _budget;
            set { _budget = value; OnPropertyChanged(nameof(Budget)); }
        }

        public string Tags
        {
            get => _tags;
            set { _tags = value; OnPropertyChanged(nameof(Tags)); }
        }

        public Employee Responsible
        {
            get => _responsible;
            set { _responsible = value; OnPropertyChanged(nameof(Responsible)); }
        }

        public string Notes
        {
            get => _notes;
            set { _notes = value; OnPropertyChanged(nameof(Notes)); }
        }

        public double ProgressPercent => Project?.CompletionPercentage ?? 0;

        public ProjectDialogViewModel(ProjectManagerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Project = new Project();
            Employees = new ObservableCollection<Employee>(_context.Employees.AsNoTracking().OrderBy(e => e.FullName).ToList());
            StatusOptions = new ObservableCollection<string> { "Активен", "Приостановлен", "Завершён" };
            PriorityOptions = new ObservableCollection<string> { "Низкий", "Средний", "Высокий", "Критический" };
            ProjectStatus = "Активен";
            ProjectPriority = "Высокий";
        }

        public ProjectDialogViewModel(ProjectManagerDbContext context, Project project)
            : this(context)
        {
            Project = project ?? throw new ArgumentNullException(nameof(project));
            Name = project.Name;
            Description = project.Description;
            Deadline = project.Deadline;
            StartDate = project.StartDate;
            Client = project.Client;
            ProjectStatus = string.IsNullOrWhiteSpace(project.ProjectStatus) ? "Активен" : project.ProjectStatus;
            ProjectPriority = string.IsNullOrWhiteSpace(project.ProjectPriority) ? "Средний" : project.ProjectPriority;
            Budget = project.Budget;
            Tags = project.Tags;
            Notes = project.Notes;
            Responsible = project.ResponsibleEmployeeID.HasValue
                ? Employees.FirstOrDefault(e => e.ID == project.ResponsibleEmployeeID.Value)
                : null;
            NameInvalid = string.IsNullOrWhiteSpace(Name);
            OnPropertyChanged(nameof(ProgressPercent));
        }

        public bool Validate()
        {
            NameInvalid = string.IsNullOrWhiteSpace(Name);
            return !NameInvalid;
        }

        public void Save()
        {
            if (!Validate())
            {
                throw new InvalidOperationException("Название обязательно");
            }

            Project.Name = Name?.Trim();
            Project.Description = Description;
            Project.Deadline = Deadline;
            Project.StartDate = StartDate;
            Project.Client = string.IsNullOrWhiteSpace(Client) ? null : Client.Trim();
            Project.ProjectStatus = ProjectStatus;
            Project.ProjectPriority = ProjectPriority;
            Project.Budget = Budget;
            Project.Tags = string.IsNullOrWhiteSpace(Tags) ? null : Tags.Trim();
            Project.Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim();
            Project.ResponsibleEmployeeID = Responsible?.ID;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
