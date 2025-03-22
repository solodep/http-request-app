//Солод Алексей Александрович БПИ-248_2 Вариант-9 B-SIDE Библиотека классов
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Spectre.Console;

namespace SolodLibrary
{
    /// <summary>
    /// Класс для чтения и записи списка треков в текстовый файл.
    /// Формат файла: каждая строка содержит поля, разделенные символом ';'
    /// (порядок: Title;Artist;Album;Year;Genre;ArtistMBID;AlbumMBID;TrackMBID).
    /// </summary>
    public static class FileManager
    {
        /// <summary>
        /// Считывает треки из файла по указанному пути.
        /// Если файла нет, выводит сообщение и возвращает пустой список.
        /// </summary>
        /// <param name="filePath">Путь к файлу с треками.</param>
        /// <returns>Список треков.</returns>
        public static List<Track> LoadTracks(string filePath)
        {
            // Создаем пустой список треков.
            List<Track> tracks = [];

            try
            {
                // Если файла не существует, выводим сообщение и возвращаем пустой список.
                if (!File.Exists(filePath))
                {
                    AnsiConsole.MarkupLine($"[red]Файл не найден: {filePath}[/]");
                    return tracks;
                }

                // Считываем все строки из файла.
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    // Пропускаем пустые строки.
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    // Разбиваем строку по символу ';'.
                    string[] parts = line.Split(';');
                    if (parts.Length >= 5)
                    {
                        // Создаем новый объект Track и заполняем его поля.
                        Track track = new()
                        {
                            Title = parts[0],
                            Artist = parts[1],
                            Album = parts[2],
                            Year = int.TryParse(parts[3], out int year) ? year : 0,
                            Genre = parts[4],
                            ArtistMBID = parts.Length > 5 ? parts[5] : string.Empty,
                            AlbumMBID = parts.Length > 6 ? parts[6] : string.Empty,
                            TrackMBID = parts.Length > 7 ? parts[7] : string.Empty
                        };

                        tracks.Add(track);
                    }
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Ошибка чтения файла: {ex.Message}[/]");
            }

            return tracks;
        }

        /// <summary>
        /// Сохраняет список треков в файл по указанному пути.
        /// </summary>
        /// <param name="filePath">Путь для сохранения файла.</param>
        /// <param name="tracks">Список треков.</param>
        public static void SaveTracks(string filePath, List<Track> tracks)
        {
            try
            {
                // Формируем строки для записи в файл.
                IEnumerable<string> lines = tracks.Select(t =>
                    $"{t.Title};{t.Artist};{t.Album};{t.Year};{t.Genre};{t.ArtistMBID};{t.AlbumMBID};{t.TrackMBID}"
                );

                File.WriteAllLines(filePath, lines);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Ошибка записи файла: {ex.Message}[/]");
            }
        }
    }
}
