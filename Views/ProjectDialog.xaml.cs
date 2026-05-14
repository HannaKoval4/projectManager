using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using ProjectManager.Data;
using ProjectManager.Models;
using ProjectManager.Services;
using ProjectManager.ViewModels;

namespace ProjectManager.Views
{
    public partial class ProjectDialog : Window
    {
        private ProjectDialogViewModel _viewModel;
        private ProjectManagerDbContext _context;

        public Project Project { get; private set; }

        public ProjectDialog()
        {
            InitializeComponent();
            _context = new ProjectManagerDbContext();
            _viewModel = new ProjectDialogViewModel(_context);
            DataContext = _viewModel;
        }

        public ProjectDialog(Project project)
        {
            InitializeComponent();
            _context = new ProjectManagerDbContext();
            var projectFromDb = _context.Projects.Include("Tasks").FirstOrDefault(p => p.ID == project.ID)
                               ?? _context.Projects.Find(project.ID);
            _viewModel = projectFromDb != null
                ? new ProjectDialogViewModel(_context, projectFromDb)
                : new ProjectDialogViewModel(_context);
            DataContext = _viewModel;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_viewModel.TryValidate(out IReadOnlyList<string> validationErrors))
            {
                MessageBox.Show(
                    string.Join(Environment.NewLine, validationErrors),
                    "Проверка данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            try
            {
                _viewModel.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Project = _viewModel.Project;

            if (!DatabaseService.ExecuteInTransaction(_context, () =>
            {
                if (Project.ID == 0)
                {
                    _context.Projects.Add(Project);
                }
                else
                {
                    var existingProject = _context.Projects.Find(Project.ID);
                    if (existingProject != null)
                    {
                        existingProject.Name = Project.Name;
                        existingProject.Description = Project.Description;
                        existingProject.Deadline = Project.Deadline;
                        existingProject.StartDate = Project.StartDate;
                        existingProject.Client = Project.Client;
                        existingProject.ProjectStatus = Project.ProjectStatus;
                        existingProject.ProjectPriority = Project.ProjectPriority;
                        existingProject.Budget = Project.Budget;
                        existingProject.Tags = Project.Tags;
                        existingProject.ResponsibleEmployeeID = Project.ResponsibleEmployeeID;
                        existingProject.Notes = Project.Notes;
                    }
                }
            }, out string error))
            {
                MessageBox.Show(error, "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
