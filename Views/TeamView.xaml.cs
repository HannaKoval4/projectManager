using System.Windows.Controls;
using ProjectManager.Data;
using ProjectManager.ViewModels;

namespace ProjectManager.Views
{
    public partial class TeamView : UserControl
    {
        public TeamView(ProjectManagerDbContext context)
        {
            InitializeComponent();
            DataContext = new TeamViewModel(context);
        }
    }
}





