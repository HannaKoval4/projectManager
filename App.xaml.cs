using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using ProjectManager.Data;
using ProjectManager.Services;

namespace ProjectManager
{
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                using (var ctx = new ProjectManagerDbContext())
                {
                    if (ctx.Database.Exists())
                    {
                        DatabaseSchema.EnsureLatest(ctx);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось обновить структуру базы данных. Выполните скрипт Database\\AddProjectNotesAndTaskComments.sql вручную.\n\n"
                    + ex.Message,
                    "База данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            var flagPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "seed-demo-once.flag");
            if (File.Exists(flagPath))
            {
                try
                {
                    TestDataSeeder.ForceReseed();
                }
                finally
                {
                    try { File.Delete(flagPath); } catch { }
                }
            }
            else
            {
                TestDataSeeder.SeedIfEmpty();
            }
        }

        private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(
                "Произошла непредвиденная ошибка.\n\n" + e.Exception.Message,
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            e.Handled = true;
        }
    }
}
