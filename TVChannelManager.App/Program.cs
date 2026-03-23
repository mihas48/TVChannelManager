using TVChannelManager.Library;
using TVChannelManager.Library.Models;
using TVChannelManager.Library.Services;

namespace TVChannelManager.App
{
    internal class Program
    {
        static void Main()
        {
            //Создание директории для хранения данных о пользователях
            string userDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users");
            if (!Directory.Exists(userDir))
            {
                Directory.CreateDirectory(userDir);
            }

            string usersDataDir = Path.Combine(userDir, "Users Data");
            if (!Directory.Exists(usersDataDir))
            {
                Directory.CreateDirectory(usersDataDir);
            }

            //Создание текстового файла для хранения данных для аутентификации пользователей
            string usersFile = Path.Combine(userDir, "users.txt");
            if (!File.Exists(usersFile))
            {
                File.WriteAllText(usersFile, "");
            }

            AuthService auth = new AuthService(usersFile);

            int choice;
            bool isContinue = true;
            while (isContinue)
            {
                Console.WriteLine("(1) Авторизация\n(2) Добавление нового пользователя");

                int.TryParse(Console.ReadLine(), out choice);
                switch (choice)
                {
                    case 1:
                        isContinue = false;
                        break;
                    case 2:
                        try
                        {
                            string? log = "", pass = "";
                            Console.Write("Введите имя нового пользователя: ");
                            log = Console.ReadLine();
                            Console.Write("\nВведите пароль для пользователя: ");
                            pass = Console.ReadLine();

                            if (string.IsNullOrWhiteSpace(log) || string.IsNullOrWhiteSpace(pass))
                                throw new ArgumentException("Ошибка! Данные для регистрации пользователя не могут быть пустыми!");

                            auth.AddUser(log, pass);

                            File.AppendAllText(usersFile, $"{log.Trim()}|{pass.Trim()}\n");

                            Console.Write("\n");
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;
                    default:
                        Console.WriteLine("Выберете правильный пункт меню");
                        break;
                }
            }


            
            try
            {
                //Аутентификация
                Console.WriteLine("Введите имя пользователя: ");
                string? authLogin = Console.ReadLine();

                Console.WriteLine($"Введите пароль для пользователя {authLogin}:");
                string? authPassword = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(authLogin) || string.IsNullOrWhiteSpace(authPassword))
                    throw new ArgumentException("Ошибка! Данные для входа не могут быть пустыми!");

                if (!auth.Authenticate(authLogin, authPassword))
                    throw new ArgumentException("Ошибка! Неверный пароль");

                string currentUserDataFile = Path.Combine(usersDataDir, $"{authLogin}.dat");

                //Работа с данными
                DataManager manager = new DataManager();

                if (!File.Exists(currentUserDataFile))
                {
                    File.Create(currentUserDataFile);
                }

                else
                {
                    FileInfo currentUserDataFileInfo = new FileInfo(currentUserDataFile);

                    if(currentUserDataFileInfo.Length != 0)
                        manager.SetData(FileManager.LoadData(currentUserDataFile));
                }

                isContinue = true;
                while (isContinue)
                {
                    Console.WriteLine("(1) Вывод всех объектов данных пользователя, включая содержательные поля\n" +
                        "(2) Добавление нового объекта\n" +
                        "(3) Удаление объекта из массива объектов пользователя\n" +
                        "(4) Редактирование выбранного объекта\n" +
                        "(5) Выход");

                    int.TryParse(Console.ReadLine(), out choice);
                    switch (choice)
                    {
                        case 1:
                            manager.GetAll();
                            break;
                        case 2:
                            Console.WriteLine("Введите название телеканала");
                            string? channelName = Console.ReadLine();

                            Console.WriteLine("Введите рейтинг телеканала");
                            double channelRating = 0.0;
                            double.TryParse(Console.ReadLine(), out channelRating);

                            Console.WriteLine("Введите средний возраст зрителей");
                            double channelMedianViewersAge = 0.0;
                            double.TryParse(Console.ReadLine(), out channelMedianViewersAge);

                            TVChannel newChannel = new TVChannel()
                            { Name = channelName, Rating = channelRating, MedianViewersAge = channelMedianViewersAge  }; 

                            manager.Add(newChannel);
                            break;

                        case 3:
                            Console.WriteLine("Введите номер телеканала");
                            int channelNumber = 0;
                            int.TryParse(Console.ReadLine(), out channelNumber);

                            manager.Remove(channelNumber);
                            break;

                        case 4:
                            Console.WriteLine("Введите номер телеканала");
                            int.TryParse(Console.ReadLine(), out channelNumber);

                            Console.WriteLine("Введите новое название телеканала");
                            channelName = Console.ReadLine();

                            Console.WriteLine("Введите новый рейтинг телеканала");
                            double.TryParse(Console.ReadLine(), out channelRating);

                            Console.WriteLine("Введите новый средний возраст зрителей");
                            double.TryParse(Console.ReadLine(), out channelMedianViewersAge);

                            TVChannel updatedChannel = new TVChannel()
                            { Name = channelName, Rating = channelRating, MedianViewersAge = channelMedianViewersAge };

                            manager.Update(channelNumber, updatedChannel);
                            break;
                        case 5:
                            return;
                        default:
                            Console.WriteLine("Выберете правильный пункт меню");
                            break;
                    }
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
