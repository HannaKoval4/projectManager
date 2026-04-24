using System;
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
    public class ProjectTaskRowViewModel : INotifyPropertyChanged
    {
        private readonly ProjectManagerDbContext _context;
        private readonly TaskModel _task;
        private string _newCommentText;

        public int TaskId => _task.ID;

        public string Title => _task.Title;

        public string Status => _task.Status;

        public string Priority => _task.Priority;

        public string EmployeeName => _task.Employee?.FullName ?? "Не назначен";

        public ObservableCollection<TaskComment> Comments { get; } = new ObservableCollection<TaskComment>();

        public string NewCommentText
        {
            get => _newCommentText;
            set
            {
                _newCommentText = value;
                OnPropertyChanged(nameof(NewCommentText));
            }
        }

        public RelayCommand AddCommentCommand { get; }

        public ProjectTaskRowViewModel(ProjectManagerDbContext context, TaskModel task)
        {
            _context = context;
            _task = task;
            ReloadCommentsFromModel();
            AddCommentCommand = new RelayCommand(AddComment);
        }

        private void ReloadCommentsFromModel()
        {
            Comments.Clear();
            if (_task.Comments == null) return;
            foreach (var c in _task.Comments.OrderByDescending(x => x.CreatedAt))
            {
                Comments.Add(c);
            }
        }

        private void ReloadCommentsFromDb()
        {
            var list = _context.TaskComments
                .AsNoTracking()
                .Where(c => c.TaskID == _task.ID)
                .OrderByDescending(c => c.CreatedAt)
                .ToList();
            Comments.Clear();
            foreach (var c in list)
            {
                Comments.Add(c);
            }
        }

        private void AddComment()
        {
            var text = NewCommentText?.Trim();
            if (string.IsNullOrEmpty(text)) return;

            var author = string.IsNullOrEmpty(SessionService.CurrentUser?.Username)
                ? "Пользователь"
                : SessionService.CurrentUser.Username;

            if (!DatabaseService.ExecuteInTransaction(_context, () =>
            {
                _context.TaskComments.Add(new TaskComment
                {
                    TaskID = _task.ID,
                    Text = text,
                    CreatedAt = DateTime.Now,
                    Author = author
                });
            }, out string error))
            {
                MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            NewCommentText = string.Empty;
            ReloadCommentsFromDb();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
