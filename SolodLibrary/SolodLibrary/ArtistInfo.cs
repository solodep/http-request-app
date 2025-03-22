//Солод Алексей Александрович БПИ-248_2 Вариант-9 B-SIDE Библиотека классов
namespace SolodLibrary
{
    /// <summary>
    /// Класс для хранения дополнительной информации об исполнителе.
    /// </summary>
    public class ArtistInfo
    {
        /// <summary>
        /// MBID исполнителя.
        /// </summary>
        public string ArtistMBID { get; set; } = string.Empty;

        /// <summary>
        /// Название исполнителя.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Страна происхождения исполнителя.
        /// </summary>
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Период активности (например, "1980 - 2000").
        /// </summary>
        public string LifeSpan { get; set; } = string.Empty;

        /// <summary>
        /// Ссылка на страницу исполнителя на MusicBrainz.
        /// </summary>
        public string MusicBrainzURL => $"https://musicbrainz.org/artist/{ArtistMBID}";
    }
}
