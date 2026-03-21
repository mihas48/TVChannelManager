namespace TVChannelManager.Library.Models
{
    public class TVChannel
    {
        private string? _name;
        private double _rating;
        private double _medianViewersAge;

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
    }
}
