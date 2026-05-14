using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ProjectManager.ViewModels;

namespace ProjectManager.Views
{
    public partial class HelpView : UserControl
    {
        public HelpView()
        {
            InitializeComponent();
        }

        private static void ShowInfo(string text)
        {
            MessageBox.Show(text, "Справка", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DocButton_Click(object sender, RoutedEventArgs e)
        {
            ShowInfo("Документация будет доступна в следующей версии.");
        }

        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            ShowInfo("Отправка сообщения об ошибке пока не подключена.");
        }

        private void CloseFooter_Click(object sender, RoutedEventArgs e)
        {
            NavigateToProjects();
        }

        private void CloseHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            NavigateToProjects();
        }

        private static void NavigateToProjects()
        {
            if (Application.Current?.MainWindow?.DataContext is MainViewModel vm)
            {
                vm.NavigateCommand.Execute("Projects");
            }
        }
    }
}
