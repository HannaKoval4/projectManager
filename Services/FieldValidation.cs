using System;

namespace ProjectManager.Services
{
    /// <summary>Правила длины и обязательности полей (согласовано с моделями и AuthService).</summary>
    public static class FieldValidation
    {
        public const int UsernameMaxLength = 100;
        public const int PasswordMinLength = 6;
        public const int PasswordMaxLength = 128;

        public const int ProjectNameMaxLength = 200;
        public const int ProjectClientMaxLength = 200;
        public const int ProjectTagsMaxLength = 500;
        public const int ProjectStatusMaxLength = 50;
        public const int ProjectPriorityMaxLength = 50;

        public const int TaskTitleMaxLength = 200;
        public const int TaskStatusMaxLength = 50;
        public const int TaskPriorityMaxLength = 50;

        public const int EmployeeFullNameMaxLength = 200;
        public const int EmployeePositionMaxLength = 100;

        /// <summary>Возвращает текст ошибки или null, если значение допустимо.</summary>
        public static string ValidateUsername(string rawUsername)
        {
            if (string.IsNullOrWhiteSpace(rawUsername))
            {
                return "Введите логин.";
            }

            string t = rawUsername.Trim();
            if (t.Length > UsernameMaxLength)
            {
                return $"Логин — не более {UsernameMaxLength} символов.";
            }

            return null;
        }

        /// <summary>Пароль при входе: не пустой, разумная длина.</summary>
        public static string ValidatePasswordForLogin(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return "Введите пароль.";
            }

            if (password.Length > PasswordMaxLength)
            {
                return $"Пароль — не более {PasswordMaxLength} символов.";
            }

            return null;
        }

        /// <summary>Новый пароль (регистрация, сброс).</summary>
        public static string ValidateNewPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return "Введите пароль.";
            }

            if (password.Length < PasswordMinLength)
            {
                return $"Пароль — не короче {PasswordMinLength} символов.";
            }

            if (password.Length > PasswordMaxLength)
            {
                return $"Пароль — не более {PasswordMaxLength} символов.";
            }

            return null;
        }

        public static string ValidatePasswordConfirm(string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(confirmPassword))
            {
                return "Повторите пароль.";
            }

            if (!string.Equals(password ?? string.Empty, confirmPassword ?? string.Empty, StringComparison.Ordinal))
            {
                return "Пароли не совпадают.";
            }

            return null;
        }

        /// <summary>ФИО и должность: оба пустые или оба заполнены; длины по модели Employee.</summary>
        public static string ValidateEmployeeOptionalPair(string fullNameRaw, string positionRaw)
        {
            string fn = (fullNameRaw ?? string.Empty).Trim();
            string pos = (positionRaw ?? string.Empty).Trim();
            bool fnSet = fn.Length > 0;
            bool posSet = pos.Length > 0;

            if (fnSet != posSet)
            {
                return "Заполните и ФИО, и должность сотрудника, либо оставьте оба поля пустыми.";
            }

            if (!fnSet)
            {
                return null;
            }

            if (fn.Length > EmployeeFullNameMaxLength)
            {
                return $"ФИО — не более {EmployeeFullNameMaxLength} символов.";
            }

            if (pos.Length > EmployeePositionMaxLength)
            {
                return $"Должность — не более {EmployeePositionMaxLength} символов.";
            }

            return null;
        }

        public static string ValidateEmployeeNameRequired(string fullNameRaw)
        {
            string fn = (fullNameRaw ?? string.Empty).Trim();
            if (fn.Length == 0)
            {
                return "Введите ФИО.";
            }

            if (fn.Length > EmployeeFullNameMaxLength)
            {
                return $"ФИО — не более {EmployeeFullNameMaxLength} символов.";
            }

            return null;
        }

        public static string ValidateEmployeePositionRequired(string positionRaw)
        {
            string pos = (positionRaw ?? string.Empty).Trim();
            if (pos.Length == 0)
            {
                return "Введите должность.";
            }

            if (pos.Length > EmployeePositionMaxLength)
            {
                return $"Должность — не более {EmployeePositionMaxLength} символов.";
            }

            return null;
        }
    }
}
