namespace TVChannelManager.Library.Services
{
    public class AuthService
    {
        //Путь к файлу users.txt
        private string _path;

        private Dictionary<string, string> dic = new Dictionary<string, string>();

        public AuthService(string path)
        {
            _path = path;

            FileInfo fileInfo = new FileInfo(_path);

            if(fileInfo.Length != 0)
            {
                string[] usersData = File.ReadAllLines(_path);

                foreach (var str in usersData)
                {
                    string[] subStr = str.Split('|');
                    dic.Add(subStr[0], subStr[1]);
                }
            }    
        }

        public bool Authenticate(string login, string password)
        {
            if (dic.Count == 0)
                throw new ArgumentException("Ошибка! В базе нет пользователей!");

            string userPass = "";

            if (!dic.TryGetValue(login.Trim(), out userPass))
                throw new ArgumentException($"Ошибка! В базе нет пользователя с именем \"{login.Trim()}\"");

            return userPass == password;
        }

        public void AddUser(string login, string password)
        {
            if (dic.ContainsKey(login))
                throw new ArgumentException("Ошибка! Данное имя пользователя уже занято!");

            dic.Add(login, password);
        }
    }
}
