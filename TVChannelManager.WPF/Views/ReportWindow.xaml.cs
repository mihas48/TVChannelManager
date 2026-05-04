using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using TVChannelManager.Library.Models;
using TVChannelManager.Library.Services;

namespace TVChannelManager.WPF.Views
{
    public partial class ReportWindow : Window
    {
        private readonly List<TVChannel> _channels;
        private readonly string _logFile;

        public ReportWindow(List<TVChannel> channels)
        {
            InitializeComponent();

            _channels = channels ?? new List<TVChannel>();
            _logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error_log.txt");
        }


        private void ShowPanel(UIElement panel)
        {
            PanelButtons.Visibility = Visibility.Collapsed;
            PanelSingle.Visibility = Visibility.Collapsed;
            PanelMultiple.Visibility = Visibility.Collapsed;

            panel.Visibility = Visibility.Visible;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            ShowPanel(PanelButtons);
        }

        private void BtnSingleReport_Click(object sender, RoutedEventArgs e)
        {
            if (_channels.Count == 0)
            {
                MessageBox.Show("Нет данных для отчёта. Добавьте хотя бы один канал.",
                    "Нет данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ListSingle.ItemsSource = _channels;
            ListSingle.DisplayMemberPath = "Name";
            ListSingle.SelectedIndex = 0;

            ShowPanel(PanelSingle);
        }

        private void BtnGenerateSingle_Click(object sender, RoutedEventArgs e)
        {
            if (ListSingle.SelectedItem is not TVChannel selected)
            {
                MessageBox.Show("Выберите телеканал из списка.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string? path = ChooseSavePath($"Отчёт_{selected.Name}.html");
            if (path == null) return;

            try
            {
                ReportService.GenerateSingleReport(selected, path);
                MessageBox.Show($"Отчёт успешно сохранён:\n{path}",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка формирования отчёта: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                File.AppendAllText(_logFile, $"[{DateTime.Now}] ReportSingle: {ex.Message}\n");
            }
        }

        private void BtnMultipleReport_Click(object sender, RoutedEventArgs e)
        {
            if (_channels.Count == 0)
            {
                MessageBox.Show("Нет данных для отчёта. Добавьте хотя бы один канал.",
                    "Нет данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ListMultiple.ItemsSource = _channels;
            ShowPanel(PanelMultiple);
        }

        private void BtnGenerateMultiple_Click(object sender, RoutedEventArgs e)
        {
            var selected = ListMultiple.SelectedItems
                .Cast<TVChannel>()
                .ToList();

            if (selected.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы один канал (удерживайте Ctrl для множественного выбора).",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string? path = ChooseSavePath("Отчёт_по_выбранным.html");
            if (path == null) return;

            try
            {
                ReportService.GenerateMultipleReport(selected, path);
                MessageBox.Show($"Отчёт по {selected.Count} каналам успешно сохранён:\n{path}",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка формирования отчёта: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                File.AppendAllText(_logFile, $"[{DateTime.Now}] ReportMultiple: {ex.Message}\n");
            }
        }

        private void BtnIntegralReport_Click(object sender, RoutedEventArgs e)
        {
            if (_channels.Count == 0)
            {
                MessageBox.Show("Нет данных для интегрального отчёта. Добавьте хотя бы один канал.",
                    "Нет данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string? path = ChooseSavePath("Интегральный_отчёт.html");
            if (path == null) return;

            try
            {
                ReportService.GenerateIntegralReport(_channels, path);
                MessageBox.Show($"Интегральный отчёт успешно сохранён:\n{path}",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка формирования отчёта: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                File.AppendAllText(_logFile, $"[{DateTime.Now}] ReportIntegral: {ex.Message}\n");
            }
        }

        private static string? ChooseSavePath(string defaultName)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Сохранить отчёт",
                FileName = defaultName,
                DefaultExt = ".html",
                Filter = "HTML-файлы (*.html)|*.html|Все файлы (*.*)|*.*"
            };
            return dlg.ShowDialog() == true ? dlg.FileName : null;
        }
    }
}