using System;
using System.Collections.Generic;
using System.Windows;
using ProjectManager.Data;
using ProjectManager.Services;
using ProjectManager.ViewModels;

namespace ProjectManager.Views
{
    public partial class TaskDialog : Window
    {
        private TaskDialogViewModel _viewModel;
        private ProjectManagerDbContext _context;

        public TaskDialog(int projectId, ProjectManagerDbContext context, Models.Task task = null)
        {
            InitializeComponent();
            _context = context;
            _viewModel = new TaskDialogViewModel(projectId, context, task);
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
                
                // Сохраняем в транзакции
                if (!DatabaseService.ExecuteInTransaction(_context, () => { }, out string error))
                {
                    MessageBox.Show(error, "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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


