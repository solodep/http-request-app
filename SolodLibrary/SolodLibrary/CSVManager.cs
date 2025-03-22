//Солод Алексей Александрович БПИ-248_2 Вариант-9 B-SIDE Библиотека классов
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Spectre.Console;

namespace SolodLibrary
{
    /// <summary>
    /// Класс для импорта и экспорта данных в формате CSV.
    /// Формат CSV: первая строка – заголовок (опционально), последующие – данные треков,
    /// разделенные запятыми.
    /// </summary>
    public static class CSVManager
    {
        /// <summary>
        /// Импортирует треки из CSV-файла.
        /// Если первая строка содержит заголовок ("Title"), она пропускается.
        /// </summary>
        /// <param name="csvPath">Путь к CSV-файлу.</param>
        /// <returns>Список импортированных треков.</returns>
        public static List<Track> ImportCSV(string csvPath)
        {
            List<Track> tracks = [];

            try
            {
                if (!File.Exists(csvPath))
                {
                    AnsiConsole.MarkupLine($"[red]CSV файл не найден: {csvPath}[/]");
                    return tracks;
                }

                string[] lines = File.ReadAllLines(csvPath);
                int startIndex = 0;

                // Если первая строка содержит заголовок, пропускаем её.
                if (lines.Length > 0 && lines[0].Contains("Title"))
                {
                    startIndex = 1;
                }

                for (int i = startIndex; i < lines.Length; i++)
                {
                    string[] cols = lines[i].Split(',');
                    if (cols.Length >= 5)
                    {
                        Track track = new()
                        {
                            Title = cols[0].Trim(),
                            Artist = cols[1].Trim(),
                            Album = cols[2].Trim(),
                            Year = int.TryParse(cols[3].Trim(), out int year) ? year : 0,
                            Genre = cols[4].Trim()
                        };

                        tracks.Add(track);
                    }
                }

                AnsiConsole.MarkupLine($"[green]Импортировано треков: {tracks.Count}[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Ошибка импорта CSV: {ex.Message}[/]");
            }

            return tracks;
        }

        /// <summary>
        /// Экспортирует список треков в CSV-файл.
        /// Добавляется строка заголовка.
        /// </summary>
        /// <param name="csvPath">Путь для сохранения CSV-файла.</param>
        /// <param name="tracks">Список треков для экспорта.</param>
        public static void ExportCSV(string csvPath, List<Track> tracks)
        {
            try
            {
                // Начинаем с заголовка
                List<string> lines = ["Title,Artist,Album,Year,Genre"];

                foreach (Track track in tracks)
                {
                    lines.Add($"{track.Title},{track.Artist},{track.Album},{track.Year},{track.Genre}");
                }

                File.WriteAllLines(csvPath, lines);
                AnsiConsole.MarkupLine($"[green]Экспортировано треков: {tracks.Count}[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Ошибка экспорта CSV: {ex.Message}[/]");
            }
        }
    }
}
