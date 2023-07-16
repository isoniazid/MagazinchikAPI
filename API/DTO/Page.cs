using System.Text.Json.Serialization;

namespace MagazinchikAPI.DTO
{
    public class Page<T>
    {
        public int Pages { get; set; }

        [JsonIgnore]
        public int CurrentOffset { get; set; }

        private List<T> _currentPage = new();

        [JsonPropertyName("rows")]
        public List<T> CurrentPage { get => _currentPage; set => (_currentPage, ElementsCount) = (value, value.Count); }

        [JsonPropertyName("count")]
        public int ElementsCount { get; set; }
    }
}