using System.Windows;
using ProjectManager.Views;

namespace ProjectManager.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Простая авторизация (в реальном приложении здесь должна быть проверка в БД)
            string username = LoginTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Открываем главное окно
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}





