using System;
using System.IO;
using System.Windows;
using TVChannelManager.Library;
using TVChannelManager.Library.Services;

namespace TVChannelManager.WPF.Views
{
    public partial class LoginWindow : Window
    {
        private readonly AuthService _auth;
        private readonly string _usersFile;
        private readonly string _usersDataDir;
        private readonly string _logFile;

        public LoginWindow()
        {
            InitializeComponent();

            string baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TVChannelManager");
            Directory.CreateDirectory(baseDir);

            string userDir = Path.Combine(baseDir, "Users");
            Directory.CreateDirectory(userDir);

            _usersDataDir = Path.Combine(userDir, "Users Data");
            Directory.CreateDirectory(_usersDataDir);

            _usersFile = Path.Combine(userDir, "users.txt");
            if (!File.Exists(_usersFile))
                File.WriteAllText(_usersFile, "");

            _logFile = Path.Combine(baseDir, "error_log.txt");
            if (!File.Exists(_logFile))
                File.WriteAllText(_logFile, "");

            _auth = new AuthService(_usersFile);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login    = LoginBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (!_auth.Authenticate(login, password))
                {
                    MessageBox.Show("Неверный пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                File.AppendAllText(_logFile, $"[{DateTime.Now}] {ex.Message}\n");
                return;
            }

            string dataFile = Path.Combine(_usersDataDir, $"{login}.json");

            var mainWindow = new MainWindow(login, dataFile);
            mainWindow.Show();
            this.Close();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string login    = LoginBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Заполните имя пользователя и пароль.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _auth.AddUser(login, password);
                File.AppendAllText(_usersFile, $"{login}|{password}\n");
                MessageBox.Show($"Пользователь «{login}» зарегистрирован. Теперь войдите.",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                File.AppendAllText(_logFile, $"[{DateTime.Now}] {ex.Message}\n");
            }
        }
    }
}
