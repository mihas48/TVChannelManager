// TVChannelManager.WPF/Views/AddEditWindow.xaml.cs (исправлен)
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TVChannelManager.Library.Models;

namespace TVChannelManager.WPF.Views
{
    public partial class AddEditWindow : Window
    {
        private TVChannel _editingChannel;
        private string _selectedImageBase64;

        public AddEditWindow(TVChannel channel = null)
        {
            InitializeComponent();
            if (channel != null)
            {
                _editingChannel = channel;
                WindowTitleLabel.Text = "Редактировать канал";
                LoadChannelData();
            }
        }

        private void LoadChannelData()
        {
            NameBox.Text = _editingChannel.Name;
            RatingBox.Text = _editingChannel.Rating.ToString();
            AgeBox.Text = _editingChannel.MedianViewersAge.ToString();
            FoundingDatePicker.SelectedDate = _editingChannel.FoundingDate;
            BroadcastTimeBox.Text = _editingChannel.BroadcastStartTime.ToString(@"hh\:mm");
            IsHDCheckBox.IsChecked = _editingChannel.IsHD;
            if (!string.IsNullOrEmpty(_editingChannel.Genre))
            {
                foreach (ComboBoxItem item in GenreComboBox.Items)
                {
                    if (item.Tag?.ToString() == _editingChannel.Genre)
                    {
                        GenreComboBox.SelectedItem = item;
                        break;
                    }
                }
            }
            if (!string.IsNullOrEmpty(_editingChannel.LogoBase64))
            {
                _selectedImageBase64 = _editingChannel.LogoBase64;
                DisplayImageFromBase64(_selectedImageBase64);
                ImagePathLabel.Text = "Логотип загружен";
            }
        }

        private void ChooseImageButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp";
            if (dialog.ShowDialog() == true)
            {
                byte[] imageBytes = File.ReadAllBytes(dialog.FileName);
                _selectedImageBase64 = Convert.ToBase64String(imageBytes);
                DisplayImageFromBase64(_selectedImageBase64);
                ImagePathLabel.Text = System.IO.Path.GetFileName(dialog.FileName);
            }
        }

        private void DisplayImageFromBase64(string base64)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(base64);
                using var ms = new MemoryStream(bytes);
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                LogoPreview.Source = image;
            }
            catch { LogoPreview.Source = null; }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var channel = new TVChannel
                {
                    Name = NameBox.Text.Trim(),
                    Rating = double.Parse(RatingBox.Text),
                    MedianViewersAge = int.Parse(AgeBox.Text),
                    FoundingDate = FoundingDatePicker.SelectedDate ?? DateTime.Now,
                    BroadcastStartTime = TimeSpan.Parse(BroadcastTimeBox.Text),
                    IsHD = IsHDCheckBox.IsChecked ?? false,
                    Genre = (GenreComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "",
                    LogoBase64 = _selectedImageBase64
                };

                if (_editingChannel != null)
                {
                    channel.GetType().GetProperty("Id")?.SetValue(channel, _editingChannel.GetType().GetProperty("Id")?.GetValue(_editingChannel));
                }

                Tag = channel;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}