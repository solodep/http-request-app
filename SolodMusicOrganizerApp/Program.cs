//Солод Алексей Александрович БПИ-248_2 Вариант-9 B-SIDE Консольное приложение
//Перед началом работы необходимо включить впнчик))) 
using Spectre.Console;
using SolodLibrary;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace SolodMusicOrganizerApp
{
    /// <summary>
    /// Точка входа консольного приложения.
    /// Здесь реализовано главное меню, которое вызывает функции из MusicLibraryManager.
    /// </summary>
    public class Program
    {
        public static async Task Main()
        {
            AnsiConsole.MarkupLine("[bold green]Добро пожаловать в Музыкальный Органайзер![/]");

            // Создаем экземпляр менеджера библиотеки.
            MusicLibraryManager manager = new();

            // Запрашиваем путь к файлу с треками.
            AnsiConsole.Markup("Введите путь к файлу с треками: ");
            string? dataPath = Console.ReadLine();
            manager.DataFilePath = dataPath ?? string.Empty;

            // Загружаем треки из файла.
            manager.Tracks = FileManager.LoadTracks(manager.DataFilePath);
            AnsiConsole.MarkupLine($"[green]Загружено треков: {manager.Tracks.Count}[/]");

            bool exit = false;
            while (!exit)
            {
                // Выводим меню.
                AnsiConsole.MarkupLine("\n[bold blue]Меню:[/]");
                AnsiConsole.MarkupLine("1. Просмотр треков (детальный список)");
                AnsiConsole.MarkupLine("2. Просмотр треков (таблица)");
                AnsiConsole.MarkupLine("3. Просмотр треков (дерево)");
                AnsiConsole.MarkupLine("4. Добавить трек");
                AnsiConsole.MarkupLine("5. Редактировать трек");
                AnsiConsole.MarkupLine("6. Удалить трек");
                AnsiConsole.MarkupLine("7. Импорт CSV");
                AnsiConsole.MarkupLine("8. Экспорт CSV");
                AnsiConsole.MarkupLine("9. Сканировать папку с музыкой");
                AnsiConsole.MarkupLine("10. Воспроизвести трек");
                AnsiConsole.MarkupLine("11. Поиск трека по MBID");
                AnsiConsole.MarkupLine("12. Получить информацию об исполнителе по MBID");
                AnsiConsole.MarkupLine("13. Получить информацию об альбоме по MBID");
                AnsiConsole.MarkupLine("0. Выход");
                AnsiConsole.Markup("Выберите опцию: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        manager.DisplayTracksDetailed();
                        break;
                    case "2":
                        manager.DisplayTracksTable();
                        break;
                    case "3":
                        manager.DisplayTracksTree();
                        break;
                    case "4":
                        await manager.AddTrackAsync();
                        break;
                    case "5":
                        await manager.EditTrackAsync();
                        break;
                    case "6":
                        manager.DeleteTrack();
                        break;
                    case "7":
                        AnsiConsole.Markup("Введите путь к CSV файлу для импорта: ");
                        string? importPath = Console.ReadLine();
                        List<Track> importedTracks = CSVManager.ImportCSV(importPath ?? string.Empty);
                        manager.Tracks.AddRange(importedTracks);
                        break;
                    case "8":
                        AnsiConsole.Markup("Введите путь для экспорта CSV: ");
                        string? exportPath = Console.ReadLine();
                        CSVManager.ExportCSV(exportPath ?? string.Empty, manager.Tracks);
                        break;
                    case "9":
                        AnsiConsole.Markup("Введите путь к папке с музыкой: ");
                        string? folderPath = Console.ReadLine();
                        List<Track> scannedTracks = FolderScanner.ScanFolder(folderPath ?? string.Empty);
                        manager.Tracks.AddRange(scannedTracks);
                        break;
                    case "10":
                        manager.PlayTrack();
                        break;
                    case "11":
                        AnsiConsole.Markup("Введите MBID трека: ");
                        string? mbid = Console.ReadLine();
                        Track? foundTrack = manager.SearchTrackByMBID(mbid ?? string.Empty);
                        if (foundTrack != null)
                        {
                            manager.DisplayTrackDetails(foundTrack);
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]Трек с данным MBID не найден.[/]");
                        }
                        break;
                    case "12":
                        AnsiConsole.Markup("Введите MBID исполнителя: ");
                        string? artistMBID = Console.ReadLine();
                        ArtistInfo? artistInfo = await MusicBrainzClient.GetArtistInfoAsync(artistMBID ?? string.Empty);
                        if (artistInfo != null)
                        {
                            AnsiConsole.MarkupLine($"[green]Исполнитель: {artistInfo.Name}[/]");
                            AnsiConsole.MarkupLine($"Страна: {artistInfo.Country}");
                            AnsiConsole.MarkupLine($"Период активности: {artistInfo.LifeSpan}");
                            AnsiConsole.MarkupLine($"Ссылка: {artistInfo.MusicBrainzURL}");
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]Информация об исполнителе не найдена.[/]");
                        }
                        break;
                    case "13":
                        AnsiConsole.Markup("Введите MBID альбома: ");
                        string? albumMBID = Console.ReadLine();
                        AlbumInfo? albumInfo = await MusicBrainzClient.GetAlbumInfoAsync(albumMBID ?? string.Empty);
                        if (albumInfo != null)
                        {
                            AnsiConsole.MarkupLine($"[green]Альбом: {albumInfo.Title}[/]");
                            AnsiConsole.MarkupLine($"Лейбл: {albumInfo.Label}");
                            AnsiConsole.MarkupLine($"Дата релиза: {albumInfo.ReleaseDate}");
                            AnsiConsole.MarkupLine($"Треклист: {albumInfo.TrackList}");
                            AnsiConsole.MarkupLine($"Ссылка: {albumInfo.MusicBrainzURL}");
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]Информация об альбоме не найдена.[/]");
                        }
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        AnsiConsole.MarkupLine("[red]Неверный выбор. Попробуйте снова.[/]");
                        break;
                }
            }

            // Сохраняем изменения перед выходом.
            FileManager.SaveTracks(manager.DataFilePath, manager.Tracks);
            AnsiConsole.MarkupLine("[green]Данные сохранены. До свидания![/]");
        }
    }
}
