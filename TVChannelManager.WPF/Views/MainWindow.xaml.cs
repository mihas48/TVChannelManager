using System;
using System.IO;
using System.Windows;
using TVChannelManager.Library;
using TVChannelManager.Library.Models;
using TVChannelManager.Library.Services;

namespace TVChannelManager.WPF.Views
{
    public partial class MainWindow : Window
    {
        private readonly DataManager _manager = new DataManager();
        private readonly string _dataFile;
        private readonly string _logFile;

        public MainWindow(string userName, string dataFile)
        {
            InitializeComponent();

            _dataFile = dataFile;
            _logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error_log.txt");

            UserLabel.Text = $"Пользователь: {userName}";

            if (File.Exists(_dataFile))
            {
                var fi = new FileInfo(_dataFile);
                if (fi.Length > 0)
                {
                    try { _manager.SetData(FileManager.LoadData(_dataFile)); }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка загрузки данных: {ex.Message}",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        File.AppendAllText(_logFile, $"[{DateTime.Now}] {ex.Message}\n");
                    }
                }
            }

            Closing += (_, _) => SaveData();
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            ChannelsGrid.ItemsSource = null;
            ChannelsGrid.ItemsSource = _manager.GetData();
        }

        private void SaveData()
        {
            try { FileManager.SaveData(_manager.GetData(), _dataFile); }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                File.AppendAllText(_logFile, $"[{DateTime.Now}] {ex.Message}\n");
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddEditWindow();
            if (dialog.ShowDialog() == true && dialog.Result != null)
            {
                try { _manager.Add(dialog.Result); SaveData(); RefreshGrid(); }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    File.AppendAllText(_logFile, $"[{DateTime.Now}] {ex.Message}\n");
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ChannelsGrid.SelectedItem is not TVChannel selected)
            {
                MessageBox.Show("Выберите канал для редактирования.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int index = ChannelsGrid.SelectedIndex + 1;
            var dialog = new AddEditWindow(selected);
            if (dialog.ShowDialog() == true && dialog.Result != null)
            {
                try { _manager.Update(index, dialog.Result); SaveData(); RefreshGrid(); }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    File.AppendAllText(_logFile, $"[{DateTime.Now}] {ex.Message}\n");
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ChannelsGrid.SelectedItem is not TVChannel selected)
            {
                MessageBox.Show("Выберите канал для удаления.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Удалить канал «{selected.Name}»?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm != MessageBoxResult.Yes) return;

            int index = ChannelsGrid.SelectedIndex + 1;
            try { _manager.Remove(index); SaveData(); RefreshGrid(); }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                File.AppendAllText(_logFile, $"[{DateTime.Now}] {ex.Message}\n");
            }
        }

        // ─── Отчёты ── НОВОЕ в ЛР4 ───────────────────────────────────

        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            var reportWindow = new ReportWindow(_manager.GetData()) { Owner = this };
            reportWindow.ShowDialog();
        }
    }
}
