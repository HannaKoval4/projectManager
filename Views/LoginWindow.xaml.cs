using System;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ProjectManager.Services;

namespace ProjectManager.Views
{
    public partial class LoginWindow : Window
    {
        private bool _isSyncingPassword;

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoginTextBox.Focus();
            UpdateCapsLockHint();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ClearErrors();
            UpdateCapsLockHint();

            string username = LoginTextBox.Text?.Trim();
            string password = GetPassword();

            var loginErr = FieldValidation.ValidateUsername(LoginTextBox.Text);
            if (loginErr != null)
            {
                ShowFieldError(LoginErrorText, loginErr);
                return;
            }

            var passErr = FieldValidation.ValidatePasswordForLogin(password);
            if (passErr != null)
            {
                ShowFieldError(PasswordErrorText, passErr);
                return;
            }

            if (!AuthService.ValidateLogin(username, password, out var user, out string error))
            {
                ShowGeneralError(error);
                return;
            }

            SessionService.SetCurrentUser(user);

            var mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        private void ForgotPassword_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            string preset = LoginTextBox.Text?.Trim();
            var dialog = new ForgotPasswordWindow(preset)
            {
                Owner = this
            };
            if (dialog.ShowDialog() == true)
            {
                PasswordBox.Password = string.Empty;
                PasswordTextBox.Text = string.Empty;
                ClearErrors();
                PasswordBox.Focus();
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            OpenRegistrationDialog();
        }

        private void OpenRegistrationDialog()
        {
            ClearErrors();

            var dialog = new RegisterWindow
            {
                Owner = this
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            LoginTextBox.Focus();
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            UpdateCapsLockHint();
            if (e.Key == Key.Enter)
            {
                LoginButton_Click(sender, e);
            }
        }

        private void UpdateCapsLockHint()
        {
            CapsLockHintText.Visibility = Keyboard.IsKeyToggled(Key.CapsLock) ? Visibility.Visible : Visibility.Collapsed;
        }

        private string GetPassword()
        {
            return PasswordTextBox.Visibility == Visibility.Visible
                ? (PasswordTextBox.Text ?? string.Empty)
                : (PasswordBox.Password ?? string.Empty);
        }

        private void ShowPasswordToggle_Checked(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.Text = PasswordBox.Password;
            PasswordBox.Visibility = Visibility.Collapsed;
            PasswordTextBox.Visibility = Visibility.Visible;
            PasswordTextBox.Focus();
            PasswordTextBox.SelectionStart = PasswordTextBox.Text?.Length ?? 0;
            if (sender is ToggleButton tb) tb.Content = "Скрыть";
        }

        private void ShowPasswordToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswordBox.Password = PasswordTextBox.Text ?? string.Empty;
            PasswordTextBox.Visibility = Visibility.Collapsed;
            PasswordBox.Visibility = Visibility.Visible;
            PasswordBox.Focus();
            if (sender is ToggleButton tb) tb.Content = "Показать";
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

        private void ClearErrors()
        {
            LoginErrorText.Visibility = Visibility.Collapsed;
            PasswordErrorText.Visibility = Visibility.Collapsed;
            GeneralErrorText.Visibility = Visibility.Collapsed;

            LoginErrorText.Text = string.Empty;
            PasswordErrorText.Text = string.Empty;
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
    }
}






