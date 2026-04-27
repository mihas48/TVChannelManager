namespace TVChannelManager.Library.Models
{
    public enum ChannelGenre
    {
        Новостной,
        Развлекательный,
        Спортивный,
        Детский,
        Документальный,
        Музыкальный
    }

    public class TVChannel
    {
        private string? _name;


        public required string Name
        {
            get => _name ?? string.Empty;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Ошибка! Название канала не может быть пустым!");
                _name = value;
            }
        }

        public double Rating { get; set; }

        public double MedianViewersAge { get; set; }

        public DateTime FoundingDate { get; set; } = DateTime.Today;
        public TimeSpan BroadcastStartTime { get; set; } = TimeSpan.FromHours(6);

        public bool IsHD { get; set; } = false;

        public string? LogoBase64 { get; set; } = null;

        public ChannelGenre Genre { get; set; } = ChannelGenre.Развлекательный;
    }
}
