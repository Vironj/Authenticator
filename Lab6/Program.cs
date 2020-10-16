using System;
using System.Text;
using RocksDbSharp;
using Lab2;
using System.Linq;

namespace Lab6
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1 - для регистрации");
            Console.WriteLine("2 - для аутентификация");
            int action = Convert.ToInt32(Console.ReadLine());
            Authenticator auth = new Authenticator();
            switch (action)
            {
                case 1:
                    auth.Registration();
                    break;
                case 2:
                    Console.WriteLine("Введите логин:");
                    string login = Console.ReadLine();
                    Console.WriteLine("Введите пароль:");
                    string password = Console.ReadLine();
                    byte[] passwordhash = SHA256.getHash(Encoding.Unicode.GetBytes(password));
                    auth.LogIn(login,passwordhash);
                    break;
            }
        }
    }
    
    public class Authenticator
    {
        public string LogIn(string login, byte[] passwordHash)
        {
            Database Users = new Database("path"); //путь до базы данных
            byte[] loginBytes = Encoding.Unicode.GetBytes(login);
            bool isExist = Users.Exists(loginBytes);
            if (isExist)
            {
                byte[] DBHash = Users.Get(loginBytes);
                Users.Close();
                if (passwordHash.SequenceEqual(DBHash))
                {
                    Console.WriteLine("Вы вошли в систему");
                    return "Вы вошли в систему";
                }
                else
                {
                    Console.WriteLine("Неверный пароль");
                    return "Неверный пароль";
                }
            }
            else
            {
                Console.WriteLine("Пользователя с логином {0} не существует", login);
                Users.Close();
                return "Пользователя с таким логином не существует";
            }
        }
        public void Registration()
        {
            Database Users = new Database("path"); //путь до базы данных
            Console.WriteLine("Введите логин:");
            string login = Console.ReadLine();
            byte[] loginBytes = Encoding.Unicode.GetBytes(login);//
            Console.WriteLine("Введите пароль:");
            string password = Console.ReadLine();
            byte[] passwordBytes = Encoding.Unicode.GetBytes(password);//
            byte[] hash = SHA256.getHash(passwordBytes);
            Users.Put(loginBytes, hash);
            Users.Close();
        }
    }

    public class Database
    {
        RocksDb db;
        public Database(string path)
        {
            db = RocksDb.Open(new DbOptions().SetCreateIfMissing(true).PrepareForBulkLoad(), path);
        }

        public byte[] Get(byte[] key)
        {
            byte[] output = db.Get(key);
            return output;
        }

        public void Put(byte[] key, byte[] value)
        {
            db.Put(key, value);
        }

         public bool Exists(byte[] key)
        {
            byte[] res = this.db.Get(key);
            if (res==null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void Close()
        {
            db.Dispose();
        }
    }
}
