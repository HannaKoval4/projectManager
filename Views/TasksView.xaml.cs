using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ProjectManager.Data;
using ProjectManager.ViewModels;

namespace ProjectManager.Views
{
    public partial class TasksView : UserControl
    {
        private TasksViewModel _viewModel;

        public TasksView(ProjectManagerDbContext context)
        {
            InitializeComponent();
            _viewModel = new TasksViewModel(context);
            DataContext = _viewModel;
        }

        private void TaskCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border?.DataContext is Models.Task task)
            {
                _viewModel.SelectedTask = task;
            }
        }

        private void TaskCard_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var border = sender as Border;
            if (border?.DataContext is Models.Task task)
            {
                _viewModel.SelectedTask = task;
            }
        }

        private void QuickStatusChange_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.Tag is string newStatus)
            {
                if (_viewModel.SelectedTask != null)
                {
                    _viewModel.QuickChangeStatus(newStatus);
                }
            }
        }
    }
}

