//Солод Алексей Александрович БПИ-248_2 Вариант-9 B-SIDE Библиотека классов
namespace SolodLibrary
{
    /// <summary>
    /// Класс для хранения дополнительной информации об альбоме.
    /// </summary>
    public class AlbumInfo
    {
        /// <summary>
        /// MBID альбома.
        /// </summary>
        public string AlbumMBID { get; set; } = string.Empty;

        /// <summary>
        /// Название альбома.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Лейбл альбома.
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Дата релиза альбома.
        /// </summary>
        public string ReleaseDate { get; set; } = string.Empty;

        /// <summary>
        /// Треклист альбома (перечисление треков через запятую).
        /// </summary>
        public string TrackList { get; set; } = string.Empty;

        /// <summary>
        /// Ссылка на страницу альбома на MusicBrainz.
        /// </summary>
        public string MusicBrainzURL => $"https://musicbrainz.org/release/{AlbumMBID}";
    }
}
