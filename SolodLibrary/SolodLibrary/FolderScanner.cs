//Солод Алексей Александрович БПИ-248_2 Вариант-9 B-SIDE Библиотека классов
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Spectre.Console;
using TagLib;

namespace SolodLibrary
{
    /// <summary>
    /// Класс для автоматического сканирования папки и извлечения метаданных из аудиофайлов.
    /// Используется библиотека TagLibSharp для чтения тегов.
    /// Допустимые форматы файлов: .mp3, .flac, .ogg, .wav.
    /// </summary>
    public static class FolderScanner
    {
        // Массив допустимых расширений аудиофайлов.
        private static readonly string[] allowedExtensions = [".mp3", ".flac", ".ogg", ".wav"];

        /// <summary>
        /// Сканирует указанную папку (рекурсивно) и возвращает список треков.
        /// Для каждого файла извлекаются метаданные с помощью TagLibSharp.
        /// </summary>
        /// <param name="folderPath">Путь к папке с музыкой.</param>
        /// <returns>Список треков, извлеченных из файлов.</returns>
        public static List<Track> ScanFolder(string folderPath)
        {
            List<Track> tracks = [];

            try
            {
                if (!Directory.Exists(folderPath))
                {
                    AnsiConsole.MarkupLine($"[red]Папка не найдена: {folderPath}[/]");
                    return tracks;
                }

                // Получаем список файлов с заданными расширениями.
                List<string> files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories)
                    .Where(file => allowedExtensions.Contains(Path.GetExtension(file).ToLower()))
                    .ToList();

                AnsiConsole.MarkupLine($"[blue]Найдено файлов: {files.Count}[/]");

                foreach (string file in files)
                {
                    try
                    {
                        // Читаем теги аудиофайла с помощью TagLibSharp.
                        TagLib.File audioFile = TagLib.File.Create(file);

                        // Если тег Title пустой, используем имя файла без расширения.
                        string title = string.IsNullOrWhiteSpace(audioFile.Tag.Title)
                            ? Path.GetFileNameWithoutExtension(file)
                            : audioFile.Tag.Title;

                        // Извлекаем первого исполнителя (если их несколько).
                        string artist = audioFile.Tag.Performers.Length > 0
                            ? audioFile.Tag.Performers[0]
                            : "Unknown";
                        artist ??= "Unknown";

                        // Извлекаем название альбома.
                        string album = !string.IsNullOrWhiteSpace(audioFile.Tag.Album)
                            ? audioFile.Tag.Album
                            : "Unknown";

                        // Извлекаем год выпуска.
                        int year = (int)audioFile.Tag.Year;

                        // Извлекаем жанр (если имеется).
                        string genre = audioFile.Tag.Genres.Length > 0
                            ? audioFile.Tag.Genres[0]
                            : "Unknown";
                        genre ??= "Unknown";

                        // Создаем объект трека с извлеченными метаданными.
                        Track track = new()
                        {
                            Title = title,
                            Artist = artist,
                            Album = album,
                            Year = year,
                            Genre = genre,
                            Duration = audioFile.Properties.Duration
                        };

                        tracks.Add(track);
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.MarkupLine($"[yellow]Не удалось обработать файл {file}: {ex.Message}[/]");
                    }
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Ошибка сканирования папки: {ex.Message}[/]");
            }

            return tracks;
        }
    }
}
