using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using ProjectManager.Data;
using ProjectManager.Models;
using ProjectManager.Views;

namespace ProjectManager.ViewModels
{
    public class ProjectsViewModel : INotifyPropertyChanged
    {
        private ProjectManagerDbContext _context;
        private Project _selectedProject;

        public ObservableCollection<Project> Projects { get; set; }

        public Project SelectedProject
        {
            get => _selectedProject;
            set
            {
                _selectedProject = value;
                OnPropertyChanged(nameof(SelectedProject));
            }
        }

        public RelayCommand AddProjectCommand { get; set; }
        public RelayCommand EditProjectCommand { get; set; }
        public RelayCommand CompleteProjectCommand { get; set; }

        public ProjectsViewModel(ProjectManagerDbContext context)
        {
            _context = context;
            Projects = new ObservableCollection<Project>();

            AddProjectCommand = new RelayCommand(AddProject);
            EditProjectCommand = new RelayCommand(EditProject, () => SelectedProject != null);
            CompleteProjectCommand = new RelayCommand(CompleteProject, () => SelectedProject != null);

            LoadProjects();
        }

        private void LoadProjects()
        {
            try
            {
                Projects.Clear();
                // Оптимизация: загружаем только необходимые данные
                // Используем AsNoTracking только для чтения, так как мы не изменяем эти объекты
                var projects = _context.Projects
                    .Include("Tasks")
                    .Include("Tasks.Employee")
                    .AsNoTracking()
                    .ToList();
                
                foreach (var project in projects)
                {
                    Projects.Add(project);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddProject()
        {
            var dialog = new ProjectDialog();
            if (dialog.ShowDialog() == true)
            {
                LoadProjects();
            }
        }

        private void EditProject()
        {
            if (SelectedProject == null) return;

            var dialog = new ProjectDialog(SelectedProject);
            if (dialog.ShowDialog() == true)
            {
                LoadProjects();
            }
        }

        private void CompleteProject()
        {
            if (SelectedProject == null) return;

            var result = MessageBox.Show(
                $"Отметить проект '{SelectedProject.Name}' как завершенный?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Здесь можно добавить логику завершения проекта
                MessageBox.Show("Проект отмечен как завершенный", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadProjects();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

