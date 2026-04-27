using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using TVChannelManager.Library.Models;

namespace TVChannelManager.WPF.Views
{
    public partial class AddEditWindow : Window
    {
        // Результат диалога — канал для сохранения
        public TVChannel? Result { get; private set; }

        private string? _logoBase64 = null;

        // ─── Конструктор: режим добавления ───────────────────────────────────
        public AddEditWindow()
        {
            InitializeComponent();
            WindowTitleLabel.Text   = "Новый телеканал";
            FoundingDatePicker.SelectedDate = DateTime.Today;
            BroadcastTimeBox.Text   = "06:00";
            IsHDCheckBox.IsChecked  = false;
            GenreComboBox.SelectedIndex = 1; // Развлекательный
        }

        // ─── Конструктор: режим редактирования ───────────────────────────────
        public AddEditWindow(TVChannel existing) : this()
        {
            WindowTitleLabel.Text = "Редактировать канал";

            NameBox.Text   = existing.Name;
            RatingBox.Text = existing.Rating.ToString();
            AgeBox.Text    = existing.MedianViewersAge.ToString();

            FoundingDatePicker.SelectedDate = existing.FoundingDate;
            BroadcastTimeBox.Text           = existing.BroadcastStartTime.ToString(@"hh\:mm");
            IsHDCheckBox.IsChecked          = existing.IsHD;

            // Жанр
            foreach (var item in GenreComboBox.Items)
            {
                if (item is System.Windows.Controls.ComboBoxItem cbi &&
                    cbi.Tag?.ToString() == existing.Genre.ToString())
                {
                    GenreComboBox.SelectedItem = cbi;
                    break;
                }
            }

            // Изображение
            if (!string.IsNullOrWhiteSpace(existing.LogoBase64))
            {
                _logoBase64 = existing.LogoBase64;
                ImagePathLabel.Text = "Изображение загружено";
            }
        }

        // ─── Выбор изображения ───────────────────────────────────────────────
        private void ChooseImageButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title  = "Выберите изображение",
                Filter = "Изображения (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp"
            };

            if (dialog.ShowDialog() != true) return;

            try
            {
                byte[] bytes = File.ReadAllBytes(dialog.FileName);
                _logoBase64 = Convert.ToBase64String(bytes);

                // Превью
                var img = new BitmapImage();
                img.BeginInit();
                img.UriSource = new Uri(dialog.FileName);
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.EndInit();

                LogoPreview.Source  = img;
                ImagePathLabel.Text = Path.GetFileName(dialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ─── Сохранение ──────────────────────────────────────────────────────
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Валидация обязательных полей
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите название канала.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(RatingBox.Text, out double rating) || rating < 0 || rating > 100)
            {
                MessageBox.Show("Рейтинг должен быть числом от 0 до 100.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(AgeBox.Text, out double age) || age < 0 || age > 199)
            {
                MessageBox.Show("Средний возраст должен быть числом от 0 до 199.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (FoundingDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Укажите дату основания канала.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!TimeSpan.TryParse(BroadcastTimeBox.Text, out TimeSpan broadcastTime))
            {
                MessageBox.Show("Время начала вещания должно быть в формате ЧЧ:ММ (например, 06:00).", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Жанр
            ChannelGenre genre = ChannelGenre.Развлекательный;
            if (GenreComboBox.SelectedItem is System.Windows.Controls.ComboBoxItem selectedItem)
            {
                Enum.TryParse(selectedItem.Tag?.ToString(), out genre);
            }

            Result = new TVChannel
            {
                Name               = NameBox.Text.Trim(),
                Rating             = rating,
                MedianViewersAge   = age,
                FoundingDate       = FoundingDatePicker.SelectedDate.Value,
                BroadcastStartTime = broadcastTime,
                IsHD               = IsHDCheckBox.IsChecked == true,
                Genre              = genre,
                LogoBase64         = _logoBase64
            };

            DialogResult = true;
        }

        // ─── Отмена ──────────────────────────────────────────────────────────
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
