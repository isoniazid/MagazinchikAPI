using System.Text.Json.Serialization;

namespace MagazinchikAPI.Model
{
    public abstract class Traceable
    {
        private DateTime UpdatedAt_;

        private DateTime CreatedAt_;
        [JsonIgnore]
        public DateTime UpdatedAt { get => UpdatedAt_; set => UpdatedAt_ = value; }

        [JsonIgnore]
        public DateTime CreatedAt { get => CreatedAt_; init => (CreatedAt_, UpdatedAt_) = (DateTime.UtcNow, DateTime.UtcNow); }

        public void UpdateTime()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}