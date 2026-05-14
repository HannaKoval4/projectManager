using ProjectManager.Data;
using ProjectManager.ViewModels;

namespace ProjectManager.Views
{
    public partial class TasksView : System.Windows.Controls.UserControl
    {
        public TasksView(ProjectManagerDbContext context)
        {
            InitializeComponent();
            DataContext = new TasksViewModel(context);
        }
    }
}
