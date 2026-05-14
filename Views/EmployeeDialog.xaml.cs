using System.Windows;
using ProjectManager.Models;
using ProjectManager.Services;

namespace ProjectManager.Views
{
    public partial class EmployeeDialog : Window
    {
        public string EmployeeName { get; private set; }
        public string Position { get; private set; }

        public EmployeeDialog()
        {
            InitializeComponent();
        }

        public EmployeeDialog(Employee employee) : this()
        {
            NameTextBox.Text = employee.FullName;
            PositionTextBox.Text = employee.Position;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ClearFieldErrors();

            var nameErr = FieldValidation.ValidateEmployeeNameRequired(NameTextBox.Text);
            if (nameErr != null)
            {
                ShowError(NameErrorText, nameErr);
            }

            var posErr = FieldValidation.ValidateEmployeePositionRequired(PositionTextBox.Text);
            if (posErr != null)
            {
                ShowError(PositionErrorText, posErr);
            }

            if (nameErr != null || posErr != null)
            {
                return;
            }

            EmployeeName = NameTextBox.Text.Trim();
            Position = PositionTextBox.Text.Trim();
            DialogResult = true;
            Close();
        }

        private static void ShowError(System.Windows.Controls.TextBlock block, string message)
        {
            block.Text = message;
            block.Visibility = Visibility.Visible;
        }

        private void ClearFieldErrors()
        {
            NameErrorText.Visibility = Visibility.Collapsed;
            PositionErrorText.Visibility = Visibility.Collapsed;
            NameErrorText.Text = string.Empty;
            PositionErrorText.Text = string.Empty;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}






