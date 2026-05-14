using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ProjectManager.Data;
using ProjectManager.Models;
using ProjectManager.ViewModels;

namespace ProjectManager.Views
{
    public partial class TeamView : UserControl
    {
        private readonly TeamViewModel _viewModel;

        public TeamView(ProjectManagerDbContext context)
        {
            InitializeComponent();
            _viewModel = new TeamViewModel(context);
            DataContext = _viewModel;
        }

        private void EmployeeCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is Employee emp)
            {
                _viewModel.SelectedEmployee = emp;
            }
        }
    }
}
