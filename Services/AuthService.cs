using System;
using System.Linq;
using System.Security.Cryptography;
using System.Data.SqlClient;
using ProjectManager.Data;
using ProjectManager.Models;

namespace ProjectManager.Services
{
    public static class AuthService
    {
        private const int SaltSizeBytes = 16;
        private const int HashSizeBytes = 32;
        private const int Pbkdf2Iterations = 100_000;

        public static bool Register(string username, string password, int? employeeId, out string errorMessage)
        {
            errorMessage = null;

            username = (username ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(username))
            {
                errorMessage = "Логин не может быть пустым.";
                return false;
            }

            if (username.Length > 100)
            {
                errorMessage = "Логин слишком длинный (максимум 100 символов).";
                return false;
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                errorMessage = "Пароль должен быть не короче 6 символов.";
                return false;
            }

            byte[] salt = new byte[SaltSizeBytes];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            byte[] hash = DeriveHash(password, salt);

            var user = new User
            {
                Username = username,
                PasswordSalt = salt,
                PasswordHash = hash,
                CreatedAt = DateTime.UtcNow,
                EmployeeID = employeeId
            };

            using (var context = new ProjectManagerDbContext())
            {
                try
                {
                    bool exists = context.Users.Any(u => u.Username == username);
                    if (exists)
                    {
                        errorMessage = "Пользователь с таким логином уже существует.";
                        return false;
                    }

                    if (!DatabaseService.ExecuteInTransaction(context, () => context.Users.Add(user), out errorMessage))
                    {
                        return false;
                    }
                }
                catch (SqlException)
                {
                    errorMessage = "Не удалось подключиться к базе данных. Проверьте, что SQL Server/LocalDB установлен и база создана.";
                    return false;
                }
                catch (Exception ex)
                {
                    errorMessage = "Ошибка регистрации: " + ex.Message;
                    return false;
                }
            }

            return true;
        }

        public static bool ValidateLogin(string username, string password, out User user, out string errorMessage)
        {
            user = null;
            errorMessage = null;

            username = (username ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            {
                errorMessage = "Введите логин и пароль.";
                return false;
            }

            using (var context = new ProjectManagerDbContext())
            {
                try
                {
                    user = context.Users.FirstOrDefault(u => u.Username == username);
                }
                catch (SqlException)
                {
                    errorMessage = "Не удалось подключиться к базе данных. Проверьте, что SQL Server/LocalDB установлен и база создана.";
                    user = null;
                    return false;
                }
                catch (Exception ex)
                {
                    errorMessage = "Ошибка авторизации: " + ex.Message;
                    user = null;
                    return false;
                }
            }

            if (user == null)
            {
                errorMessage = "Неверный логин или пароль.";
                return false;
            }

            byte[] expectedHash = DeriveHash(password, user.PasswordSalt);
            bool ok = FixedTimeEquals(expectedHash, user.PasswordHash);
            if (!ok)
            {
                errorMessage = "Неверный логин или пароль.";
                user = null;
                return false;
            }

            return true;
        }

        private static byte[] DeriveHash(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Pbkdf2Iterations, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(HashSizeBytes);
            }
        }

        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }

            return diff == 0;
        }
    }
}

