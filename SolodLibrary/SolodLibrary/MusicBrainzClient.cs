//Солод Алексей Александрович БПИ-248_2 Вариант-9 B-SIDE Библиотека классов
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Spectre.Console;

namespace SolodLibrary
{
    /// <summary>
    /// Класс для работы с API MusicBrainz.
    /// Позволяет искать треки по названию и исполнителю, а также получать расширенную информацию.
    /// </summary>
    public static class MusicBrainzClient
    {
        private static readonly HttpClient client = new();

        // Конструктор для установки заголовка User-Agent (обязательное требование MusicBrainz)
        static MusicBrainzClient()
        {
            client.DefaultRequestHeaders.Add("User-Agent", "MusicOrganizerApp/1.0 (your-email@example.com)");
        }

        /// <summary>
        /// Ищет MBID трека по названию и исполнителю.
        /// Если найден – возвращает MBID, иначе возвращает пустую строку.
        /// </summary>
        public static async Task<string> SearchTrackAsync(string title, string artist)
        {
            try
            {
                // Формируем строку запроса для точного поиска.
                string query = $"recording:\"{title}\" AND artist:\"{artist}\"";
                string url = $"https://musicbrainz.org/ws/2/recording/?query={Uri.EscapeDataString(query)}&fmt=json";

                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    // Используем JsonDocument для парсинга ответа.
                    using JsonDocument doc = JsonDocument.Parse(json);
                    JsonElement root = doc.RootElement;

                    if (root.TryGetProperty("recordings", out JsonElement recordings) && recordings.GetArrayLength() > 0)
                    {
                        JsonElement firstRecording = recordings[0];
                        if (firstRecording.TryGetProperty("id", out JsonElement idElement))
                        {
                            string mbid = idElement.GetString() ?? string.Empty;
                            return mbid;
                        }
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Ошибка запроса к MusicBrainz: {ex.Message}[/]");
                return string.Empty;
            }
        }

        /// <summary>
        /// Обогащает информацию о треке, устанавливая его MBID.
        /// Если поиск не дал результата, предлагает пользователю ввести MBID вручную.
        /// </summary>
        /// <summary>
        /// Обогащает информацию о треке, устанавливая его MBID и заполняя недостающие поля (альбом, год, жанр)
        /// из данных, полученных с MusicBrainz.
        /// Если поиск не дал результата, предлагает пользователю ввести MBID вручную.
        /// </summary>
        public static async Task EnrichTrackInfoAsync(Track track)
        {
            // Получаем JSON-ответ, используя метод SearchTrackAsync (он может быть модифицирован, чтобы возвращать JSON).
            string jsonResult = await SearchTrackJsonAsync(track.Title, track.Artist);
            if (string.IsNullOrWhiteSpace(jsonResult))
            {
                AnsiConsole.MarkupLine($"[yellow]Информация из MusicBrainz не найдена для трека: {track.Title}[/]");
                AnsiConsole.Markup("Хотите ввести MBID вручную? (Y/N): ");
                string? response = Console.ReadLine();
                if (response != null && response.Trim().Equals("Y", StringComparison.CurrentCultureIgnoreCase))
                {
                    AnsiConsole.Markup("Введите MBID: ");
                    string? manualMBID = Console.ReadLine();
                    track.TrackMBID = manualMBID ?? string.Empty;
                }
                return;
            }

            using JsonDocument doc = JsonDocument.Parse(jsonResult);
            JsonElement root = doc.RootElement;

            if (root.TryGetProperty("recordings", out JsonElement recordings) && recordings.GetArrayLength() > 0)
            {
                JsonElement firstRecording = recordings[0];

                // Устанавливаем MBID трека
                if (firstRecording.TryGetProperty("id", out JsonElement idElement))
                {
                    string mbid = idElement.GetString() ?? string.Empty;
                    track.TrackMBID = mbid;
                }

                // Если данные по альбому или году отсутствуют, пытаемся заполнить их из первого релиза
                if ((string.IsNullOrWhiteSpace(track.Album) || track.Year == 0) &&
                    firstRecording.TryGetProperty("releases", out JsonElement releases) && releases.GetArrayLength() > 0)
                {
                    JsonElement firstRelease = releases[0];
                    if (string.IsNullOrWhiteSpace(track.Album) && firstRelease.TryGetProperty("title", out JsonElement albumEl))
                    {
                        track.Album = albumEl.GetString() ?? track.Album;
                    }
                    if (track.Year == 0 && firstRelease.TryGetProperty("date", out JsonElement dateEl))
                    {
                        string date = dateEl.GetString() ?? "";
                        if (!string.IsNullOrWhiteSpace(date))
                        {
                            // Предполагаем, что год – это первая часть даты (например, "1982-11-30")
                            string[] parts = date.Split('-');
                            if (parts.Length > 0 && int.TryParse(parts[0], out int releaseYear))
                            {
                                track.Year = releaseYear;
                            }
                        }
                    }
                }

                // Если жанр не заполнен, пытаемся заполнить его из тегов
                if (string.IsNullOrWhiteSpace(track.Genre) &&
                    firstRecording.TryGetProperty("tags", out JsonElement tags) &&
                    tags.ValueKind == JsonValueKind.Array && tags.GetArrayLength() > 0)
                {
                    JsonElement firstTag = tags[0];
                    if (firstTag.TryGetProperty("name", out JsonElement tagName))
                    {
                        track.Genre = tagName.GetString() ?? track.Genre;
                    }
                }

                AnsiConsole.MarkupLine($"[green]Информация из MusicBrainz найдена для трека: {track.Title}[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[yellow]Информация из MusicBrainz не найдена для трека: {track.Title}[/]");
                AnsiConsole.Markup("Хотите ввести MBID вручную? (Y/N): ");
                string? manualResponse = Console.ReadLine();
                if (manualResponse != null && manualResponse.Trim().Equals("Y", StringComparison.CurrentCultureIgnoreCase))
                {
                    AnsiConsole.Markup("Введите MBID: ");
                    string? manualMBID = Console.ReadLine();
                    track.TrackMBID = manualMBID ?? string.Empty;
                }
            }
        }

        /// <summary>
        /// Выполняет запрос к MusicBrainz и возвращает сырой JSON-ответ в виде строки.
        /// Этот метод можно использовать для более гибкого парсинга данных.
        /// </summary>
        private static async Task<string> SearchTrackJsonAsync(string title, string artist)
        {
            try
            {
                string query = $"recording:\"{title}\" AND artist:\"{artist}\"";
                string url = $"https://musicbrainz.org/ws/2/recording/?query={Uri.EscapeDataString(query)}&fmt=json";
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    return json;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Ошибка запроса к MusicBrainz: {ex.Message}[/]");
                return string.Empty;
            }
        }


        /// <summary>
        /// Получает расширенную информацию об исполнителе по его MBID.
        /// Возвращает объект типа ArtistInfo или null, если не удалось получить данные.
        /// </summary>
        public static async Task<ArtistInfo?> GetArtistInfoAsync(string artistMBID)
        {
            try
            {
                string url = $"https://musicbrainz.org/ws/2/artist/{artistMBID}?fmt=json";
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    using JsonDocument doc = JsonDocument.Parse(json);
                    JsonElement root = doc.RootElement;

                    // Создаем объект ArtistInfo, заполняя поля
                    ArtistInfo info = new()
                    {
                        ArtistMBID = artistMBID,
                        Name = root.GetProperty("name").GetString() ?? string.Empty,
                        Country = root.TryGetProperty("country", out JsonElement countryEl) ? countryEl.GetString() ?? string.Empty : string.Empty
                    };

                    if (root.TryGetProperty("life-span", out JsonElement lifeSpanEl))
                    {
                        string begin = lifeSpanEl.TryGetProperty("begin", out JsonElement beginEl) ? beginEl.GetString() ?? "" : "";
                        string end = lifeSpanEl.TryGetProperty("end", out JsonElement endEl) ? endEl.GetString() ?? "" : "";
                        info.LifeSpan = $"{begin} - {end}";
                    }

                    return info;
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Ошибка получения информации об исполнителе: {ex.Message}[/]");
            }
            return null;
        }

        /// <summary>
        /// Получает расширенную информацию об альбоме по его MBID.
        /// Возвращает объект типа AlbumInfo или null, если данные не получены.
        /// </summary>
        public static async Task<AlbumInfo?> GetAlbumInfoAsync(string albumMBID)
        {
            try
            {
                string url = $"https://musicbrainz.org/ws/2/release/{albumMBID}?fmt=json&inc=recordings+labels";
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    using JsonDocument doc = JsonDocument.Parse(json);
                    JsonElement root = doc.RootElement;

                    AlbumInfo info = new()
                    {
                        AlbumMBID = albumMBID,
                        Title = root.TryGetProperty("title", out JsonElement titleEl) ? titleEl.GetString() ?? string.Empty : string.Empty,
                        ReleaseDate = root.TryGetProperty("date", out JsonElement dateEl) ? dateEl.GetString() ?? string.Empty : string.Empty
                    };

                    if (root.TryGetProperty("label-info", out JsonElement labelInfo) && labelInfo.ValueKind == JsonValueKind.Array && labelInfo.GetArrayLength() > 0)
                    {
                        JsonElement firstLabel = labelInfo[0];
                        if (firstLabel.TryGetProperty("label", out JsonElement labelEl))
                        {
                            info.Label = labelEl.TryGetProperty("name", out JsonElement nameEl) ? nameEl.GetString() ?? string.Empty : string.Empty;
                        }
                    }

                    // Получаем треклист
                    if (root.TryGetProperty("media", out JsonElement mediaEl) && mediaEl.ValueKind == JsonValueKind.Array)
                    {
                        List<string> trackNames = [];
                        foreach (JsonElement medium in mediaEl.EnumerateArray())
                        {
                            if (medium.TryGetProperty("tracks", out JsonElement tracksEl) && tracksEl.ValueKind == JsonValueKind.Array)
                            {
                                foreach (JsonElement trackEl in tracksEl.EnumerateArray())
                                {
                                    if (trackEl.TryGetProperty("title", out JsonElement tEl))
                                    {
                                        trackNames.Add(tEl.GetString() ?? "");
                                    }
                                }
                            }
                        }
                        info.TrackList = string.Join(", ", trackNames);
                    }

                    return info;
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Ошибка получения информации об альбоме: {ex.Message}[/]");
            }
            return null;
        }
    }
}
