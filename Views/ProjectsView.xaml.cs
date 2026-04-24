using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ProjectManager.Data;
using ProjectManager.Models;
using ProjectManager.ViewModels;

namespace ProjectManager.Views
{
    public partial class ProjectsView : UserControl
    {
        private ProjectsViewModel _viewModel;

        public ProjectsView(ProjectManagerDbContext context)
        {
            InitializeComponent();
            _viewModel = new ProjectsViewModel(context);
            DataContext = _viewModel;
        }

        private void ProjectCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border?.DataContext is Project project)
            {
                _viewModel.SelectedProject = project;
                if (e.ClickCount == 2)
                {
                    _viewModel.OpenProject(project);
                    e.Handled = true;
                }
            }
        }
    }
}






