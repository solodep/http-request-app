//Солод Алексей Александрович БПИ-248_2 Вариант-9 B-SIDE Библиотека классов
using System;

namespace SolodLibrary
{
    /// <summary>
    /// Класс модели музыкального трека.
    /// Содержит свойства для хранения основной информации о треке.
    /// Все строковые свойства инициализируются значением string.Empty,
    /// а числовые – нулём.
    /// </summary>
    public class Track
    {
        /// <summary>
        /// Название трека.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Исполнитель трека.
        /// </summary>
        public string Artist { get; set; } = string.Empty;

        /// <summary>
        /// Альбом, в котором содержится трек.
        /// </summary>
        public string Album { get; set; } = string.Empty;

        /// <summary>
        /// Год выпуска трека.
        /// </summary>
        public int Year { get; set; } = 0;

        /// <summary>
        /// Жанр трека (например, Rock, Pop и т.д.).
        /// </summary>
        public string Genre { get; set; } = string.Empty;

        /// <summary>
        /// MBID исполнителя из MusicBrainz.
        /// </summary>
        public string ArtistMBID { get; set; } = string.Empty;

        /// <summary>
        /// MBID альбома из MusicBrainz.
        /// </summary>
        public string AlbumMBID { get; set; } = string.Empty;

        /// <summary>
        /// MBID трека из MusicBrainz.
        /// </summary>
        public string TrackMBID { get; set; } = string.Empty;

        /// <summary>
        /// Длительность трека (получается при сканировании аудиофайла).
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Переопределенный метод для получения строкового представления трека.
        /// </summary>
        public override string ToString()
        {
            return $"Название: {Title}\n" +
                   $"Исполнитель: {Artist}\n" +
                   $"Альбом: {Album}\n" +
                   $"Год выпуска: {Year}\n" +
                   $"Жанр: {Genre}";
        }
    }
}
