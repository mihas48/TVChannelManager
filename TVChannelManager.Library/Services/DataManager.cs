using TVChannelManager.Library.Models;

namespace TVChannelManager.Library.Services
{
    public class DataManager
    {
        private List<TVChannel> _channels = new List<TVChannel>();

        public void Add(TVChannel channel)
        {
            int lowRating = 0;
            int highRating = 100;


            if (string.IsNullOrWhiteSpace(channel.Name))
                throw new ArgumentException("Ошибка! Название канала не может быть пустым!");
            if (channel.Rating < lowRating || channel.Rating > highRating)
                throw new ArgumentException($"Ошибка! Рейтинг канала должен быть в диапазоне от {lowRating} до {highRating}!");
            if (channel.MedianViewersAge < 0 || channel.MedianViewersAge > 199)
                throw new ArithmeticException("Ошибка! Средний возраст зрителей должен быть в диапазоне от 0 до 199!");

            _channels.Add(channel);
        }

        public void Remove(int index)
        {
            if (index < 1)
                throw new ArgumentException("Ошибка! Номер не может быть меньше 1!");
            if (index > _channels.Count)
                throw new ArgumentException($"Ошибка! Номер не может быть больше {_channels.Count}!");

            _channels.RemoveAt(index - 1);
        }

        public void Update(int index, TVChannel updated)
        {
            if (_channels.Count == 0)
                throw new Exception("Ошибка! Массив объектов пуст!");

            if (index < 1)
                throw new ArgumentException("Ошибка! Номер не может быть меньше 1!");
            if (index > _channels.Count)
                throw new ArgumentException($"Ошибка! Номер не может быть больше {_channels.Count}!");

            _channels[index - 1] = updated;
        }

        public void GetAll()
        {
            if (_channels.Count == 0)
                throw new Exception("Ошибка! Массив объектов пуст!");

            int number = 1;
            foreach (TVChannel channel in _channels)
            {
                Console.WriteLine($"{number}: Имя: \"{channel.Name}\"; Рейтинг: {channel.Rating}%; Средний возраст зрителей: {channel.MedianViewersAge}");
                number++;
            }
        }

        public List<TVChannel> GetData()
        {
            List<TVChannel> ch = new List<TVChannel>(_channels);
            return ch;
        }

        public void SetData(List<TVChannel> newChannels)
        {
            _channels = newChannels;
        }
    }
}
