using System.Windows.Controls;
using ProjectManager.Data;
using ProjectManager.ViewModels;

namespace ProjectManager.Views
{
    public partial class StatisticsView : UserControl
    {
        public StatisticsView(ProjectManagerDbContext context)
        {
            InitializeComponent();
            DataContext = new StatisticsViewModel(context);
        }
    }
}





