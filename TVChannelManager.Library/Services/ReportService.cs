using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TVChannelManager.Library.Models;

namespace TVChannelManager.Library.Services
{
    /// <summary>
    /// Сервис генерации трёх видов отчётов о телеканалах.
    /// </summary>
    public static class ReportService
    {
        /// <summary>
        /// Формирует подробный отчёт об одном телеканале и сохраняет его в файл.
        /// </summary>
        /// <param name="channel">Объект телеканала.</param>
        /// <param name="outputPath">Путь к выходному файлу.</param>
        public static void GenerateSingleReport(TVChannel channel, string outputPath)
        {
            if (channel == null)
                throw new ArgumentNullException(nameof(channel), "Телеканал не может быть null.");

            var lines = new List<string>
            {
                "═══════════════════════════════════════════════════════",
                "            ОТЧЁТ О ТЕЛЕКАНАЛЕ",
                $"            Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm:ss}",
                "═══════════════════════════════════════════════════════",
                "",
                $"  Название:              {channel.Name}",
                $"  Рейтинг:               {channel.Rating}%",
                $"  Средний возраст:       {channel.MedianViewersAge} лет",
                $"  Дата основания:        {channel.FoundingDate:dd.MM.yyyy}",
                $"  Время начала вещания:  {channel.BroadcastStartTime:hh\\:mm}",
                $"  HD-вещание:            {(channel.IsHD ? "Да" : "Нет")}",
                $"  Жанр:                  {channel.Genre}",
                $"  Логотип:               {(string.IsNullOrWhiteSpace(channel.LogoImage) ? "Отсутствует" : "Загружен")}",
                "",
                "═══════════════════════════════════════════════════════"
            };

            File.WriteAllLines(outputPath, lines, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// Формирует отчёт о нескольких выбранных телеканалах.
        /// </summary>
        /// <param name="channels">Список выбранных каналов.</param>
        /// <param name="outputPath">Путь к выходному файлу.</param>
        public static void GenerateMultipleReport(List<TVChannel> channels, string outputPath)
        {
            if (channels == null || channels.Count == 0)
                throw new ArgumentException("Список телеканалов не может быть пустым.");

            var lines = new List<string>
            {
                "═══════════════════════════════════════════════════════",
                "       ОТЧЁТ ПО ВЫБРАННЫМ ТЕЛЕКАНАЛАМ",
                $"       Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm:ss}",
                $"       Количество каналов в отчёте: {channels.Count}",
                "═══════════════════════════════════════════════════════",
                ""
            };

            for (int i = 0; i < channels.Count; i++)
            {
                var ch = channels[i];
                lines.Add($"  [{i + 1}] {ch.Name}");
                lines.Add($"      Рейтинг:        {ch.Rating}%");
                lines.Add($"      Ср. возраст:    {ch.MedianViewersAge} лет");
                lines.Add($"      Дата осн.:      {ch.FoundingDate:dd.MM.yyyy}");
                lines.Add($"      Вещание с:      {ch.BroadcastStartTime:hh\\:mm}");
                lines.Add($"      HD:             {(ch.IsHD ? "Да" : "Нет")}");
                lines.Add($"      Жанр:           {ch.Genre}");
                lines.Add("      ───────────────────────────────────────────");
                lines.Add("");
            }

            lines.Add("═══════════════════════════════════════════════════════");

            File.WriteAllLines(outputPath, lines, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// Формирует отчёт с интегральными характеристиками коллекции каналов.
        /// </summary>
        /// <param name="channels">Полный список каналов пользователя.</param>
        /// <param name="outputPath">Путь к выходному файлу.</param>
        public static void GenerateIntegralReport(List<TVChannel> channels, string outputPath)
        {
            if (channels == null || channels.Count == 0)
                throw new ArgumentException("Нет данных для формирования интегрального отчёта.");

            double avgRating = channels.Average(c => c.Rating);
            double avgAge = channels.Average(c => c.MedianViewersAge);
            int hdCount = channels.Count(c => c.IsHD);
            double hdPercent = (double)hdCount / channels.Count * 100;
            TVChannel topRated = channels.OrderByDescending(c => c.Rating).First();
            TVChannel oldest = channels.OrderBy(c => c.FoundingDate).First();

            var genreGroups = channels
                .GroupBy(c => c.Genre)
                .OrderByDescending(g => g.Count())
                .Select(g => $"      {g.Key,-18}: {g.Count()} ({(double)g.Count() / channels.Count * 100:F1}%)");

            var lines = new List<string>
            {
                "═══════════════════════════════════════════════════════",
                "         ИНТЕГРАЛЬНЫЙ ОТЧЁТ ПО ТЕЛЕКАНАЛАМ",
                $"         Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm:ss}",
                "═══════════════════════════════════════════════════════",
                "",
                "  ОБЩАЯ СТАТИСТИКА",
                $"  Всего каналов:         {channels.Count}",
                $"  Средний рейтинг:       {avgRating:F2}%",
                $"  Средний возраст аудит: {avgAge:F1} лет",
                $"  Каналов с HD:          {hdCount} ({hdPercent:F1}%)",
                "",
                "  ЛИДЕРЫ",
                $"  Наивысший рейтинг:     {topRated.Name} ({topRated.Rating}%)",
                $"  Старейший канал:       {oldest.Name} (основан {oldest.FoundingDate:dd.MM.yyyy})",
                "",
                "  РАСПРЕДЕЛЕНИЕ ПО ЖАНРАМ",
            };

            lines.AddRange(genreGroups);
            lines.Add("");
            lines.Add("═══════════════════════════════════════════════════════");

            File.WriteAllLines(outputPath, lines, System.Text.Encoding.UTF8);
        }
    }
}