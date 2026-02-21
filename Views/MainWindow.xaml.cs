using System.Windows;
using ProjectManager.ViewModels;

namespace ProjectManager.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}






