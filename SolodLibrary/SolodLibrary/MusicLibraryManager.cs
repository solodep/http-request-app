//Солод Алексей Александрович БПИ-248_2 Вариант-9 B-SIDE Библиотека классов
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

namespace SolodLibrary
{
    /// <summary>
    /// Класс, управляющий музыкальной библиотекой (коллекцией треков).
    /// Реализует операции просмотра, добавления, редактирования, удаления треков,
    /// а также эмуляцию воспроизведения.
    /// </summary>
    public class MusicLibraryManager
    {
        /// <summary>
        /// Коллекция треков, доступных в библиотеке.
        /// </summary>
        public List<Track> Tracks { get; set; } = [];

        /// <summary>
        /// Путь к файлу, в котором хранится список треков.
        /// </summary>
        public string DataFilePath { get; set; } = string.Empty;

        /// <summary>
        /// Отображает треки в детальном текстовом формате (каждый трек с отдельными строками).
        /// </summary>
        public void DisplayTracksDetailed()
        {
            if (Tracks.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]Нет треков для отображения.[/]");
                return;
            }

            foreach (Track track in Tracks)
            {
                AnsiConsole.MarkupLine("------------------------------------");
                AnsiConsole.MarkupLine(track.ToString());
            }
            AnsiConsole.MarkupLine("------------------------------------");
        }

        /// <summary>
        /// Отображает треки в виде таблицы (Spectre.Console Table).
        /// </summary>
        public void DisplayTracksTable()
        {
            if (Tracks.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]Нет треков для отображения.[/]");
                return;
            }

            // Создаем таблицу и цепочкой добавляем колонки
            Table table = new Table()
                .AddColumn("Название")
                .AddColumn("Исполнитель")
                .AddColumn("Альбом")
                .AddColumn("Год")
                .AddColumn("Жанр");

            // Добавляем строки (AddRow возвращает Table, поэтому присваиваем обратно)
            foreach (Track track in Tracks)
            {
                table = table.AddRow(
                    track.Title,
                    track.Artist,
                    track.Album,
                    track.Year.ToString(),
                    track.Genre
                );
            }

            // Выводим таблицу
            AnsiConsole.Write(table);
        }

        /// <summary>
        /// Отображает треки в виде древовидной структуры:
        /// Исполнитель -> Альбом -> Список треков.
        /// </summary>
        public void DisplayTracksTree()
        {
            if (Tracks.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]Нет треков для отображения.[/]");
                return;
            }

            // Корневой узел дерева
            Tree root = new("Музыкальная коллекция");

            // Группируем треки по исполнителю
            IEnumerable<IGrouping<string, Track>> artistGroups = Tracks.GroupBy(t => t.Artist);

            foreach (IGrouping<string, Track> artistGroup in artistGroups)
            {
                // Добавляем узел для исполнителя
                TreeNode artistNode = root.AddNode($"[yellow]{artistGroup.Key}[/]");

                // Группируем по альбому
                IEnumerable<IGrouping<string, Track>> albumGroups = artistGroup.GroupBy(t => t.Album);
                foreach (IGrouping<string, Track> albumGroup in albumGroups)
                {
                    // Узел для альбома
                    TreeNode albumNode = artistNode.AddNode($"[green]{albumGroup.Key}[/]");
                    foreach (Track track in albumGroup)
                    {
                        // Добавляем трек как листовой узел
                        TreeNode treeNode = albumNode.AddNode(track.Title);
                    }
                }
            }

            AnsiConsole.Write(root);
        }

        /// <summary>
        /// Асинхронное добавление нового трека. Запрашивает у пользователя поля
        /// и при желании обогащает информацию через MusicBrainz.
        /// </summary>
        public async Task AddTrackAsync()
        {
            AnsiConsole.MarkupLine("[blue]Добавление нового трека...[/]");
            AnsiConsole.Markup("Введите название: ");
            string title = Console.ReadLine() ?? string.Empty;
            AnsiConsole.Markup("Введите исполнителя: ");
            string artist = Console.ReadLine() ?? string.Empty;
            AnsiConsole.Markup("Введите альбом: ");
            string album = Console.ReadLine() ?? string.Empty;
            AnsiConsole.Markup("Введите год выпуска: ");
            string yearStr = Console.ReadLine() ?? "0";
            int year = int.TryParse(yearStr, out int y) ? y : 0;
            AnsiConsole.Markup("Введите жанр: ");
            string genre = Console.ReadLine() ?? string.Empty;

            // Создаем трек
            Track newTrack = new()
            {
                Title = title,
                Artist = artist,
                Album = album,
                Year = year,
                Genre = genre
            };

            // Обогащаем информацию через MusicBrainz
            await MusicBrainzClient.EnrichTrackInfoAsync(newTrack);

            // Добавляем в коллекцию
            Tracks.Add(newTrack);
            AnsiConsole.MarkupLine("[green]Новый трек добавлен.[/]");
        }

        /// <summary>
        /// Асинхронное редактирование существующего трека (по индексу).
        /// Запрашивает поля и позволяет пропустить изменения, оставив поле без изменений.
        /// </summary>
        public async Task EditTrackAsync()
        {
            if (Tracks.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]Нет треков для редактирования.[/]");
                return;
            }

            // Выводим список
            for (int i = 0; i < Tracks.Count; i++)
            {
                AnsiConsole.MarkupLine($"{i + 1}. {Tracks[i].Title} - {Tracks[i].Artist}");
            }

            // Запрашиваем номер
            AnsiConsole.Markup("Введите номер трека для редактирования: ");
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int index) && index > 0 && index <= Tracks.Count)
            {
                Track track = Tracks[index - 1];
                AnsiConsole.MarkupLine($"Редактирование трека: [cyan]{track.Title}[/]");

                // Новое название
                AnsiConsole.Markup("Новое название (Enter для пропуска): ");
                string? newTitle = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newTitle))
                {
                    track.Title = newTitle;
                }

                // Новый исполнитель
                AnsiConsole.Markup("Новый исполнитель (Enter для пропуска): ");
                string? newArtist = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newArtist))
                {
                    track.Artist = newArtist;
                }

                // Новый альбом
                AnsiConsole.Markup("Новый альбом (Enter для пропуска): ");
                string? newAlbum = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newAlbum))
                {
                    track.Album = newAlbum;
                }

                // Новый год
                AnsiConsole.Markup("Новый год выпуска (Enter для пропуска): ");
                string? newYearStr = Console.ReadLine();
                if (int.TryParse(newYearStr, out int newYear))
                {
                    track.Year = newYear;
                }

                // Новый жанр
                AnsiConsole.Markup("Новый жанр (Enter для пропуска): ");
                string? newGenre = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newGenre))
                {
                    track.Genre = newGenre;
                }

                // Повторное обогащение через MusicBrainz (вдруг название/исполнитель поменялись)
                await MusicBrainzClient.EnrichTrackInfoAsync(track);

                AnsiConsole.MarkupLine("[green]Трек обновлен.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Неверный номер трека.[/]");
            }
        }

        /// <summary>
        /// Удаляет выбранный пользователем трек из коллекции.
        /// </summary>
        public void DeleteTrack()
        {
            if (Tracks.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]Нет треков для удаления.[/]");
                return;
            }

            // Выводим список
            for (int i = 0; i < Tracks.Count; i++)
            {
                AnsiConsole.MarkupLine($"{i + 1}. {Tracks[i].Title} - {Tracks[i].Artist}");
            }

            // Запрашиваем номер
            AnsiConsole.Markup("Введите номер трека для удаления: ");
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int index) && index > 0 && index <= Tracks.Count)
            {
                Track track = Tracks[index - 1];
                Tracks.RemoveAt(index - 1);
                AnsiConsole.MarkupLine($"[green]Трек \"{track.Title}\" удален.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Неверный номер трека.[/]");
            }
        }
        /// <summary>
        /// Ищет трек по его MBID (MusicBrainz ID).
        /// Возвращает первый найденный трек или null, если трек не найден.
        /// </summary>
        public Track? SearchTrackByMBID(string mbid)
        {
            return Tracks.FirstOrDefault(t => t.TrackMBID.Equals(mbid, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Выводит подробную информацию о треке, включая ссылки на страницы MusicBrainz для трека, исполнителя и альбома.
        /// </summary>
        public void DisplayTrackDetails(Track track)
        {
            AnsiConsole.MarkupLine(track.ToString());
            if (!string.IsNullOrWhiteSpace(track.TrackMBID))
            {
                AnsiConsole.MarkupLine($"[blue]MusicBrainz (Track):[/] https://musicbrainz.org/recording/{track.TrackMBID}");
            }
            if (!string.IsNullOrWhiteSpace(track.ArtistMBID))
            {
                AnsiConsole.MarkupLine($"[blue]MusicBrainz (Artist):[/] https://musicbrainz.org/artist/{track.ArtistMBID}");
            }
            if (!string.IsNullOrWhiteSpace(track.AlbumMBID))
            {
                AnsiConsole.MarkupLine($"[blue]MusicBrainz (Album):[/] https://musicbrainz.org/release/{track.AlbumMBID}");
            }
        }


        /// <summary>
        /// Эмулирует воспроизведение выбранного трека с помощью прогресс-бара.
        /// Можно прервать воспроизведение, нажав S.
        /// </summary>
        public void PlayTrack()
        {
            if (Tracks.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]Нет треков для воспроизведения.[/]");
                return;
            }

            // Выводим список
            for (int i = 0; i < Tracks.Count; i++)
            {
                AnsiConsole.MarkupLine($"{i + 1}. {Tracks[i].Title} - {Tracks[i].Artist}");
            }

            // Запрашиваем номер
            AnsiConsole.Markup("Введите номер трека для воспроизведения: ");
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int index) && index > 0 && index <= Tracks.Count)
            {
                Track track = Tracks[index - 1];
                AnsiConsole.MarkupLine($"Воспроизведение трека: [cyan]{track.Title}[/]");
                bool userInterrupted = false;

                // Запускаем прогресс-бар
                AnsiConsole.Progress().Start(ctx =>
                {
                    // Создаем задачу
                    ProgressTask task = ctx.AddTask($"[cyan]Идет воспроизведение: {track.Title}[/]", maxValue: 100);

                    while (!ctx.IsFinished)
                    {
                        task.Increment(1);
                        Thread.Sleep(100);

                        // Если пользователь нажал S, прерываем
                        if (Console.KeyAvailable)
                        {
                            ConsoleKeyInfo key = Console.ReadKey(true);
                            if (key.Key == ConsoleKey.S)
                            {
                                userInterrupted = true;
                                break;
                            }
                        }
                    }
                });

                if (userInterrupted)
                {
                    AnsiConsole.MarkupLine("[yellow]Воспроизведение прервано пользователем.[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("[green]Воспроизведение завершено.[/]");
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Неверный номер трека.[/]");
            }
        }
    }
}
