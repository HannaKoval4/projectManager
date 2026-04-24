using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using ProjectManager.Data;
using ProjectManager.Views;
using ProjectManager.Services;

namespace ProjectManager.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ProjectManagerDbContext _context;
        private string _currentView = "Projects";
        private UserControl _currentViewContent;
        private string _currentViewTitle = "Проекты";
        private string _currentUser = "Гость";
        private int _projectsCount;
        private int _tasksCount;
        private int _employeesCount;

        public string CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
                UpdateView();
            }
        }

        public UserControl CurrentViewContent
        {
            get => _currentViewContent;
            set
            {
                _currentViewContent = value;
                OnPropertyChanged(nameof(CurrentViewContent));
            }
        }

        public string CurrentViewTitle
        {
            get => _currentViewTitle;
            set
            {
                _currentViewTitle = value;
                OnPropertyChanged(nameof(CurrentViewTitle));
            }
        }

        public string CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged(nameof(CurrentUser));
            }
        }

        public int ProjectsCount
        {
            get => _projectsCount;
            private set
            {
                _projectsCount = value;
                OnPropertyChanged(nameof(ProjectsCount));
            }
        }

        public int TasksCount
        {
            get => _tasksCount;
            private set
            {
                _tasksCount = value;
                OnPropertyChanged(nameof(TasksCount));
            }
        }

        public int EmployeesCount
        {
            get => _employeesCount;
            private set
            {
                _employeesCount = value;
                OnPropertyChanged(nameof(EmployeesCount));
            }
        }

        public RelayCommand<string> NavigateCommand { get; set; }
        public RelayCommand LogoutCommand { get; set; }

        public MainViewModel()
        {
            _context = new ProjectManagerDbContext();
            NavigateCommand = new RelayCommand<string>(Navigate);
            LogoutCommand = new RelayCommand(Logout);
            CurrentUser = SessionService.CurrentUser?.Username ?? "Гость";
            RefreshCounts();
            UpdateView();
        }

        private void Navigate(string viewName)
        {
            CurrentView = viewName;
        }

        private void UpdateView()
        {
            RefreshCounts();
            switch (_currentView)
            {
                case "Projects":
                    CurrentViewTitle = "Проекты";
                    CurrentViewContent = new ProjectsView(_context);
                    break;
                case "Tasks":
                    CurrentViewTitle = "Задачи";
                    CurrentViewContent = new TasksView(_context);
                    break;
                case "Team":
                    CurrentViewTitle = "Команда";
                    CurrentViewContent = new TeamView(_context);
                    break;
                case "Statistics":
                    CurrentViewTitle = "Статистика";
                    CurrentViewContent = new StatisticsView(_context);
                    break;
                default:
                    CurrentViewTitle = "Проекты";
                    CurrentViewContent = new ProjectsView(_context);
                    break;
            }
        }

        private void RefreshCounts()
        {
            try
            {
                ProjectsCount = _context.Projects.Count();
                TasksCount = _context.Tasks.Count();
                EmployeesCount = _context.Employees.Count();
            }
            catch
            {
                // если БД недоступна на старте, не роняем UI
                ProjectsCount = 0;
                TasksCount = 0;
                EmployeesCount = 0;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Logout()
        {
            SessionService.Clear();

            var login = new LoginWindow();
            login.Show();

            foreach (var w in System.Windows.Application.Current.Windows)
            {
                if (w is MainWindow main)
                {
                    main.Close();
                    break;
                }
            }
        }
    }
}
