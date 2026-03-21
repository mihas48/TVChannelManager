using TVChannelManager.Library;
using TVChannelManager.Library.Models;
using TVChannelManager.Library.Services;

namespace TVChannelManager.App
{
    internal class Program
    {
        static void Main()
        {
			DataManager manager = new DataManager();
			try
			{
				var channel1 = new TVChannel
				{
					Name = "Россия 1",
					MedianViewersAge = 40,
					Rating = 11.5
				};

                var channel2 = new TVChannel
                {
                    Name = "ТНТ",
                    MedianViewersAge = 29,
                    Rating = -1
                };

                manager.Add(channel1);
                manager.Add(channel2);
                manager.GetAll();
			}
			catch (Exception ex)
			{
                Console.WriteLine(ex.Message);
				throw;
			}
        }
    }
}
