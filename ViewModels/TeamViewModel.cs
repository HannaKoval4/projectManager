using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using ProjectManager.Data;
using ProjectManager.Models;
using ProjectManager.Services;

namespace ProjectManager.ViewModels
{
    public class TeamViewModel : INotifyPropertyChanged
    {
        private ProjectManagerDbContext _context;
        private Employee _selectedEmployee;

        public ObservableCollection<Employee> Employees { get; set; }

        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged(nameof(SelectedEmployee));
            }
        }

        public RelayCommand AddEmployeeCommand { get; set; }
        public RelayCommand EditEmployeeCommand { get; set; }
        public RelayCommand DeleteEmployeeCommand { get; set; }

        public TeamViewModel(ProjectManagerDbContext context)
        {
            _context = context;
            Employees = new ObservableCollection<Employee>();

            AddEmployeeCommand = new RelayCommand(AddEmployee);
            EditEmployeeCommand = new RelayCommand(EditEmployee, () => SelectedEmployee != null);
            DeleteEmployeeCommand = new RelayCommand(DeleteEmployee, () => SelectedEmployee != null);

            LoadEmployees();
        }

        private void LoadEmployees()
        {
            try
            {
                Employees.Clear();
                // Оптимизация: используем AsNoTracking для чтения
                var employees = _context.Employees
                    .Include("Tasks")
                    .AsNoTracking()
                    .ToList();
                foreach (var employee in employees)
                {
                    Employees.Add(employee);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddEmployee()
        {
            var dialog = new Views.EmployeeDialog();
            if (dialog.ShowDialog() == true)
            {
                var employee = new Employee
                {
                    FullName = dialog.EmployeeName,
                    Position = dialog.Position
                };

                if (!DatabaseService.ExecuteInTransaction(_context, () =>
                {
                    _context.Employees.Add(employee);
                }, out string error))
                {
                    MessageBox.Show(error, "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    LoadEmployees();
                }
            }
        }

        private void EditEmployee()
        {
            if (SelectedEmployee == null) return;

            var dialog = new Views.EmployeeDialog(SelectedEmployee);
            if (dialog.ShowDialog() == true)
            {
                var employeeToEdit = _context.Employees.Find(SelectedEmployee.ID);
                if (employeeToEdit == null) return;

                if (!DatabaseService.ExecuteInTransaction(_context, () =>
                {
                    employeeToEdit.FullName = dialog.EmployeeName;
                    employeeToEdit.Position = dialog.Position;
                }, out string error))
                {
                    MessageBox.Show(error, "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    LoadEmployees();
                }
            }
        }

        private void DeleteEmployee()
        {
            if (SelectedEmployee == null) return;

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить сотрудника '{SelectedEmployee.FullName}'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var employeeToDelete = _context.Employees.Find(SelectedEmployee.ID);
                if (employeeToDelete != null)
                {
                    if (!DatabaseService.ExecuteInTransaction(_context, () =>
                    {
                        _context.Employees.Remove(employeeToDelete);
                    }, out string error))
                    {
                        MessageBox.Show(error, "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        LoadEmployees();
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

