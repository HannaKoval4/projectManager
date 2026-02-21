using System.Windows;
using System.Windows.Controls;
using ProjectManager.Data;
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

        private void ProjectCard_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border?.DataContext is Models.Project project)
            {
                _viewModel.SelectedProject = project;
            }
        }
    }
}





