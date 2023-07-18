using System.Text.Json.Serialization;

namespace MagazinchikAPI.DTO
{
    public class Page<T>
    {
        public int Pages { get; set; }

        [JsonIgnore]
        public int CurrentOffset { get; set; }


        [JsonPropertyName("rows")]
        public List<T> CurrentPage { get; set;} = new();

        [JsonPropertyName("count")]
        public int ElementsCount { get; set; }
    }

    public class Page //For static methods
    {
        
        public static int CalculatePagesAmount(int totalCount, int limit)
        {
            return (int)Math.Ceiling((float)totalCount / (float)limit);
        }

        public static bool OffsetIsOk(int offset, int pages)
        {
            if (offset > pages - 1 || offset < 0) return false;
            return true;
        }
    }
}