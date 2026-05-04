using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TVChannelManager.Library.Models;

namespace TVChannelManager.Library.Services
{
    public static class ReportService
    {
        private static string HtmlPage(string title, string body) =>
            $@"<!DOCTYPE html>
<html lang='ru'>
<head><meta charset='utf-8'><title>{title}</title>
<style>
body {{ font-family: 'Segoe UI', sans-serif; margin: 2em; color: #1a237e; }}
h1 {{ color: #0d47a1; }}
table {{ border-collapse: collapse; width: 100%; margin-top: 1em; }}
th, td {{ border: 1px solid #90caf9; padding: 0.5em; text-align: left; }}
th {{ background-color: #1565c0; color: white; }}
tr:nth-child(even) {{ background-color: #e3f2fd; }}
img.logo {{ height: 40px; }}
</style></head><body>
<h1>{title}</h1>
{body}
</body></html>";

        // Отчёт по одному каналу
        public static void GenerateSingleReport(TVChannel channel, string outputPath)
        {
            if (channel == null)
                throw new ArgumentNullException(nameof(channel));

            string logoCell = string.IsNullOrWhiteSpace(channel.LogoBase64)
                ? "—"
                : $"<img class='logo' src='data:image/png;base64,{channel.LogoBase64}' />";

            var body = new StringBuilder();
            body.AppendLine("<table>");
            body.AppendLine($"<tr><td><b>Название</b></td><td>{channel.Name}</td></tr>");
            body.AppendLine($"<tr><td><b>Рейтинг</b></td><td>{channel.Rating}%</td></tr>");
            body.AppendLine($"<tr><td><b>Средний возраст</b></td><td>{channel.MedianViewersAge}</td></tr>");
            body.AppendLine($"<tr><td><b>Дата основания</b></td><td>{channel.FoundingDate:dd.MM.yyyy}</td></tr>");
            body.AppendLine($"<tr><td><b>Начало вещания</b></td><td>{channel.BroadcastStartTime:hh\\:mm}</td></tr>");
            body.AppendLine($"<tr><td><b>HD</b></td><td>{(channel.IsHD ? "Да" : "Нет")}</td></tr>");
            body.AppendLine($"<tr><td><b>Жанр</b></td><td>{channel.Genre}</td></tr>");
            body.AppendLine($"<tr><td><b>Логотип</b></td><td>{logoCell}</td></tr>");
            body.AppendLine("</table>");

            File.WriteAllText(outputPath, HtmlPage($"Отчёт: {channel.Name}", body.ToString()), Encoding.UTF8);
        }

        // Отчёт по нескольким каналам
        public static void GenerateMultipleReport(List<TVChannel> channels, string outputPath)
        {
            if (channels == null || channels.Count == 0)
                throw new ArgumentException("Список каналов пуст.");

            var body = new StringBuilder();
            body.AppendLine("<table>");
            body.AppendLine("<tr><th>Название</th><th>Рейтинг</th><th>Ср. возраст</th><th>Дата осн.</th><th>HD</th><th>Жанр</th></tr>");
            foreach (var ch in channels)
            {
                body.AppendLine($"<tr><td>{ch.Name}</td><td>{ch.Rating}%</td><td>{ch.MedianViewersAge}</td>"
                               + $"<td>{ch.FoundingDate:dd.MM.yyyy}</td><td>{(ch.IsHD ? "Да" : "Нет")}</td><td>{ch.Genre}</td></tr>");
            }
            body.AppendLine("</table>");
            File.WriteAllText(outputPath, HtmlPage($"Отчёт по {channels.Count} каналам", body.ToString()), Encoding.UTF8);
        }

        // Интегральный отчёт
        public static void GenerateIntegralReport(List<TVChannel> channels, string outputPath)
        {
            if (channels == null || channels.Count == 0)
                throw new ArgumentException("Нет данных.");

            double avgRating = channels.Average(c => c.Rating);
            double avgAge = channels.Average(c => c.MedianViewersAge);
            int hdCount = channels.Count(c => c.IsHD);
            double hdPerc = (double)hdCount / channels.Count * 100.0;
            var top = channels.OrderByDescending(c => c.Rating).First();
            var old = channels.OrderBy(c => c.FoundingDate).First();
            var genres = channels.GroupBy(c => c.Genre)
                                 .Select(g => $"<tr><td>{g.Key}</td><td>{g.Count()}</td></tr>");

            var body = new StringBuilder();
            body.AppendLine("<h2>Общая статистика</h2>");
            body.AppendLine("<table>");
            body.AppendLine($"<tr><td>Всего каналов</td><td>{channels.Count}</td></tr>");
            body.AppendLine($"<tr><td>Средний рейтинг</td><td>{avgRating:F2}%</td></tr>");
            body.AppendLine($"<tr><td>Средний возраст аудитории</td><td>{avgAge:F1}</td></tr>");
            body.AppendLine($"<tr><td>Доля HD</td><td>{hdPerc:F1}%</td></tr>");
            body.AppendLine($"<tr><td>Лидер по рейтингу</td><td>{top.Name} ({top.Rating}%)</td></tr>");
            body.AppendLine($"<tr><td>Старейший канал</td><td>{old.Name} (осн. {old.FoundingDate:dd.MM.yyyy})</td></tr>");
            body.AppendLine("</table>");
            body.AppendLine("<h2>Распределение по жанрам</h2>");
            body.AppendLine("<table><tr><th>Жанр</th><th>Количество</th></tr>");
            body.Append(string.Join("", genres));
            body.AppendLine("</table>");

            File.WriteAllText(outputPath, HtmlPage("Интегральный отчёт", body.ToString()), Encoding.UTF8);
        }
    }
}