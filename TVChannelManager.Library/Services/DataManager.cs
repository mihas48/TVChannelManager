using TVChannelManager.Library.Models;

namespace TVChannelManager.Library.Services
{
    public class DataManager
    {
        private List<TVChannel> tvChannels = new List<TVChannel>();

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

            tvChannels.Add(channel);
        }

        public void Remove(int id)
        {
            tvChannels.RemoveAt(id);
        }

        public void Update(int id, TVChannel updated)
        {
            tvChannels[id] = updated;
        }

        public void GetAll()
        {
            int id = 0;
            foreach (TVChannel channel in tvChannels)
            {
                Console.WriteLine($"{id}: Имя: \"{channel.Name}\"; Рейтинг: {channel.Rating}%; Средний возраст зрителей: {channel.MedianViewersAge}");
                id++;
            }
        }
    }
}
