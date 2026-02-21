using System.Windows;
using ProjectManager.Models;

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
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) || string.IsNullOrWhiteSpace(PositionTextBox.Text))
            {
                MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            EmployeeName = NameTextBox.Text;
            Position = PositionTextBox.Text;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}





