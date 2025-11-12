using System.Windows;
using AliNedvizhimostApp.Services;
using AliNedvizhimostApp.ViewModels;
using AliNedvizhimostApp.Views;
using System.Configuration;
using System.Threading; // Добавлено
using System.Globalization; // Добавлено

namespace AliNedvizhimostApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Установка русской локали для всего приложения
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");

            // Create services and main view model
            // Чтение строки подключения из App.config
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            var databaseService = new DatabaseService(connectionString);
            var appViewModel = new ApplicationViewModel(databaseService);

            // Create the main shell window
            var shell = new ShellWindow
            {
                DataContext = appViewModel
            };

            shell.Show();
        }
    }
}