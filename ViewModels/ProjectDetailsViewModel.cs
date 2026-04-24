using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using ProjectManager.Data;
using ProjectManager.Models;
using ProjectManager.Services;
using TaskModel = ProjectManager.Models.Task;

namespace ProjectManager.ViewModels
{
    public class ProjectDetailsViewModel : INotifyPropertyChanged
    {
        private readonly ProjectManagerDbContext _context;
        private readonly Action _close;
        private int _projectId;
        private string _name;
        private string _description;
        private DateTime? _deadline;
        private string _notes;
        private double _completionPercent;
        private bool _isCompleted;
        private int _taskCount;
        private string _progressSummary;

        public int ProjectId => _projectId;

        public string ProgressSummary
        {
            get => _progressSummary;
            private set { _progressSummary = value; OnPropertyChanged(nameof(ProgressSummary)); }
        }

        public string Name
        {
            get => _name;
            private set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        public string Description
        {
            get => _description;
            private set { _description = value; OnPropertyChanged(nameof(Description)); }
        }

        public DateTime? Deadline
        {
            get => _deadline;
            private set { _deadline = value; OnPropertyChanged(nameof(Deadline)); }
        }

        public string Notes
        {
            get => _notes;
            set
            {
                _notes = value;
                OnPropertyChanged(nameof(Notes));
            }
        }

        public double CompletionPercent
        {
            get => _completionPercent;
            private set { _completionPercent = value; OnPropertyChanged(nameof(CompletionPercent)); }
        }

        public bool IsCompleted
        {
            get => _isCompleted;
            private set { _isCompleted = value; OnPropertyChanged(nameof(IsCompleted)); }
        }

        public int TaskCount
        {
            get => _taskCount;
            private set { _taskCount = value; OnPropertyChanged(nameof(TaskCount)); OnPropertyChanged(nameof(HasNoTasks)); }
        }

        public bool HasNoTasks => TaskCount == 0;

        public ObservableCollection<ProjectTaskRowViewModel> TaskRows { get; } = new ObservableCollection<ProjectTaskRowViewModel>();

        public RelayCommand SaveNotesCommand { get; }
        public RelayCommand CloseCommand { get; }
        public RelayCommand ReloadCommand { get; }

        public ProjectDetailsViewModel(ProjectManagerDbContext context, int projectId, Action close)
        {
            _context = context;
            _close = close;
            _projectId = projectId;

            SaveNotesCommand = new RelayCommand(SaveNotes);
            CloseCommand = new RelayCommand(() => _close?.Invoke());
            ReloadCommand = new RelayCommand(() => LoadProject(_projectId));

            LoadProject(projectId);
        }

        private void LoadProject(int projectId)
        {
            TaskRows.Clear();

            var project = _context.Projects
                .Include("Tasks")
                .Include("Tasks.Employee")
                .Include("Tasks.Comments")
                .FirstOrDefault(p => p.ID == projectId);

            if (project == null)
            {
                MessageBox.Show("Проект не найден.", "Проекты", MessageBoxButton.OK, MessageBoxImage.Information);
                _close?.Invoke();
                return;
            }

            _projectId = project.ID;
            Name = project.Name;
            Description = project.Description ?? string.Empty;
            Deadline = project.Deadline;
            Notes = project.Notes ?? string.Empty;

            var tasks = project.Tasks == null
                ? new List<TaskModel>()
                : project.Tasks.ToList();
            TaskCount = tasks.Count;
            if (tasks.Count == 0)
            {
                CompletionPercent = 0;
                IsCompleted = false;
                ProgressSummary = "Нет задач";
            }
            else
            {
                var done = tasks.Count(t => t.Status == "Завершена");
                CompletionPercent = (double)done / tasks.Count * 100.0;
                IsCompleted = done == tasks.Count;
                ProgressSummary = $"Завершено: {CompletionPercent:F0}%, задач: {tasks.Count}";
            }

            foreach (var t in tasks.OrderBy(t => t.Title))
            {
                TaskRows.Add(new ProjectTaskRowViewModel(_context, t));
            }
        }

        private void SaveNotes()
        {
            if (!DatabaseService.ExecuteInTransaction(_context, () =>
            {
                var p = _context.Projects.Find(_projectId);
                if (p == null) return;
                p.Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim();
            }, out string error))
            {
                MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Заметки сохранены.", "Проекты", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
