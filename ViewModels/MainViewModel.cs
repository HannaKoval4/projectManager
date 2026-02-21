using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ProjectManager.Data;
using ProjectManager.Models;
using ProjectManager.Views;

namespace ProjectManager.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ProjectManagerDbContext _context;
        private string _currentView = "Projects";
        private UserControl _currentViewContent;
        private string _currentViewTitle = "Проекты";
        private string _currentUser = "Администратор";

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

        public RelayCommand<string> NavigateCommand { get; set; }

        public MainViewModel()
        {
            _context = new ProjectManagerDbContext();
            NavigateCommand = new RelayCommand<string>(Navigate);
            UpdateView();
        }

        private void Navigate(string viewName)
        {
            CurrentView = viewName;
        }

        private void UpdateView()
        {
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
