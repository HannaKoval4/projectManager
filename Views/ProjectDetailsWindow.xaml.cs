using System;
using System.Windows;
using ProjectManager.Data;
using ProjectManager.ViewModels;

namespace ProjectManager.Views
{
    public partial class ProjectDetailsWindow : Window
    {
        private readonly ProjectManagerDbContext _context;

        public ProjectDetailsWindow(int projectId)
        {
            InitializeComponent();
            _context = new ProjectManagerDbContext();
            DataContext = new ProjectDetailsViewModel(_context, projectId, () => Close());
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            _context?.Dispose();
            base.OnClosed(e);
        }
    }
}
