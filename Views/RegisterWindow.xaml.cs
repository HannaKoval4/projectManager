using System;
using System.Windows;
using System.Windows.Controls;
using ProjectManager.Models;
using ProjectManager.Services;
using ProjectManager.Data;
using Task = System.Threading.Tasks.Task;

namespace ProjectManager.Views
{
    public partial class RegisterWindow : Window
    {
        private bool _isSyncingPassword;

        public RegisterWindow()
        {
            InitializeComponent();
            Loaded += (_, __) => UsernameTextBox.Focus();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private async void CreateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            ClearErrors();
            HideToast();

            string username = UsernameTextBox.Text?.Trim();
            string password = GetPassword();
            string confirmPassword = GetConfirmPassword();
            string fullName = FullNameTextBox.Text?.Trim();
            string position = PositionTextBox.Text?.Trim();

            bool hasError = false;

            if (string.IsNullOrWhiteSpace(username))
            {
                ShowFieldError(UsernameErrorText, "Введите логин.");
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowFieldError(PasswordErrorText, "Введите пароль.");
                hasError = true;
            }

            if (password != confirmPassword)
            {
                ShowFieldError(ConfirmPasswordErrorText, "Пароли не совпадают.");
                hasError = true;
            }

            if (hasError)
            {
                return;
            }

            SetBusy(true, "Создаём аккаунт...");
            try
            {
                var result = await Task.Run(() => RegisterUser(username, password, fullName, position));
                if (!result.Success)
                {
                    ShowToastError(result.ErrorMessage);
                    return;
                }

                ShowToastSuccess("Аккаунт создан. Теперь можно войти.");
                await Task.Delay(900);
                DialogResult = true;
                Close();
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void ClearErrors()
        {
            UsernameErrorText.Visibility = Visibility.Collapsed;
            PasswordErrorText.Visibility = Visibility.Collapsed;
            ConfirmPasswordErrorText.Visibility = Visibility.Collapsed;
            GeneralErrorText.Visibility = Visibility.Collapsed;

            UsernameErrorText.Text = string.Empty;
            PasswordErrorText.Text = string.Empty;
            ConfirmPasswordErrorText.Text = string.Empty;
            GeneralErrorText.Text = string.Empty;
        }

        private static void ShowFieldError(System.Windows.Controls.TextBlock tb, string message)
        {
            tb.Text = message;
            tb.Visibility = Visibility.Visible;
        }

        private void ShowGeneralError(string message)
        {
            GeneralErrorText.Text = message;
            GeneralErrorText.Visibility = Visibility.Visible;
        }

        private (bool Success, string ErrorMessage) RegisterUser(string username, string password, string fullName, string position)
        {
            int? employeeId = null;

            if (!string.IsNullOrWhiteSpace(fullName) || !string.IsNullOrWhiteSpace(position))
            {
                if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(position))
                {
                    return (false, "Если заполняете сотрудника, заполните и ФИО, и должность.");
                }

                var employee = new Employee
                {
                    FullName = fullName,
                    Position = position
                };

                using (var context = new ProjectManagerDbContext())
                {
                    if (!DatabaseService.ExecuteInTransaction(context, () => context.Employees.Add(employee), out string error))
                    {
                        return (false, error);
                    }
                }

                employeeId = employee.ID;
            }

            if (!AuthService.Register(username, password, employeeId, out string registerError))
            {
                return (false, registerError);
            }

            return (true, null);
        }

        private void SetBusy(bool isBusy, string message = null)
        {
            CreateAccountButton.IsEnabled = !isBusy;
            BackButton.IsEnabled = !isBusy;
            UsernameTextBox.IsEnabled = !isBusy;
            PasswordBox.IsEnabled = !isBusy;
            ConfirmPasswordBox.IsEnabled = !isBusy;
            PasswordTextBox.IsEnabled = !isBusy;
            ConfirmPasswordTextBox.IsEnabled = !isBusy;
            ShowPasswordToggle.IsEnabled = !isBusy;
            ShowConfirmPasswordToggle.IsEnabled = !isBusy;
            FullNameTextBox.IsEnabled = !isBusy;
            PositionTextBox.IsEnabled = !isBusy;

            ProgressPanel.Visibility = isBusy ? Visibility.Visible : Visibility.Collapsed;
            if (!string.IsNullOrWhiteSpace(message))
            {
                ProgressText.Text = message;
            }
        }

        private void HideToast()
        {
            ToastBorder.Visibility = Visibility.Collapsed;
            ToastText.Text = string.Empty;
        }

        private void ShowToastSuccess(string message)
        {
            ToastBorder.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(34, 197, 94)); // green
            ToastText.Text = message;
            ToastBorder.Visibility = Visibility.Visible;
        }

        private void ShowToastError(string message)
        {
            ToastBorder.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(239, 68, 68)); // red
            ToastText.Text = message;
            ToastBorder.Visibility = Visibility.Visible;
        }

        private string GetPassword()
        {
            return PasswordTextBox.Visibility == Visibility.Visible
                ? (PasswordTextBox.Text ?? string.Empty)
                : (PasswordBox.Password ?? string.Empty);
        }

        private string GetConfirmPassword()
        {
            return ConfirmPasswordTextBox.Visibility == Visibility.Visible
                ? (ConfirmPasswordTextBox.Text ?? string.Empty)
                : (ConfirmPasswordBox.Password ?? string.Empty);
        }

        private void ShowPasswordToggle_Checked(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.Text = PasswordBox.Password;
            PasswordBox.Visibility = Visibility.Collapsed;
            PasswordTextBox.Visibility = Visibility.Visible;
            PasswordTextBox.Focus();
            PasswordTextBox.SelectionStart = PasswordTextBox.Text?.Length ?? 0;
            if (sender is System.Windows.Controls.Primitives.ToggleButton tb) tb.Content = "Скрыть";
        }

        private void ShowPasswordToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswordBox.Password = PasswordTextBox.Text ?? string.Empty;
            PasswordTextBox.Visibility = Visibility.Collapsed;
            PasswordBox.Visibility = Visibility.Visible;
            PasswordBox.Focus();
            if (sender is System.Windows.Controls.Primitives.ToggleButton tb) tb.Content = "Показать";
        }

        private void ShowConfirmPasswordToggle_Checked(object sender, RoutedEventArgs e)
        {
            ConfirmPasswordTextBox.Text = ConfirmPasswordBox.Password;
            ConfirmPasswordBox.Visibility = Visibility.Collapsed;
            ConfirmPasswordTextBox.Visibility = Visibility.Visible;
            ConfirmPasswordTextBox.Focus();
            ConfirmPasswordTextBox.SelectionStart = ConfirmPasswordTextBox.Text?.Length ?? 0;
            if (sender is System.Windows.Controls.Primitives.ToggleButton tb) tb.Content = "Скрыть";
        }

        private void ShowConfirmPasswordToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            ConfirmPasswordBox.Password = ConfirmPasswordTextBox.Text ?? string.Empty;
            ConfirmPasswordTextBox.Visibility = Visibility.Collapsed;
            ConfirmPasswordBox.Visibility = Visibility.Visible;
            ConfirmPasswordBox.Focus();
            if (sender is System.Windows.Controls.Primitives.ToggleButton tb) tb.Content = "Показать";
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_isSyncingPassword) return;
            if (PasswordTextBox.Visibility != Visibility.Visible) return;

            try
            {
                _isSyncingPassword = true;
                PasswordTextBox.Text = PasswordBox.Password;
                PasswordTextBox.SelectionStart = PasswordTextBox.Text.Length;
            }
            finally
            {
                _isSyncingPassword = false;
            }
        }

        private void PasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isSyncingPassword) return;
            if (PasswordBox.Visibility != Visibility.Visible) return;

            try
            {
                _isSyncingPassword = true;
                PasswordBox.Password = PasswordTextBox.Text ?? string.Empty;
            }
            finally
            {
                _isSyncingPassword = false;
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_isSyncingPassword) return;
            if (ConfirmPasswordTextBox.Visibility != Visibility.Visible) return;

            try
            {
                _isSyncingPassword = true;
                ConfirmPasswordTextBox.Text = ConfirmPasswordBox.Password;
                ConfirmPasswordTextBox.SelectionStart = ConfirmPasswordTextBox.Text.Length;
            }
            finally
            {
                _isSyncingPassword = false;
            }
        }

        private void ConfirmPasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isSyncingPassword) return;
            if (ConfirmPasswordBox.Visibility != Visibility.Visible) return;

            try
            {
                _isSyncingPassword = true;
                ConfirmPasswordBox.Password = ConfirmPasswordTextBox.Text ?? string.Empty;
            }
            finally
            {
                _isSyncingPassword = false;
            }
        }
    }
}

