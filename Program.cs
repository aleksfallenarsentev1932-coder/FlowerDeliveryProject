using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace FlowerDeliveryApp
{
    class Program
    {

        static string dbServer = @"sql.bsite.net\MSSQL2016";
        static string dbUser = "osipov_";
        static string dbName = "osipov_";
        static string dbPass = "osipow188";

        static string connString = $@"Server={dbServer};Database={dbName};User Id={dbUser};Password={dbPass};TrustServerCertificate=True;";

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("     FLOWERS DELIVERY SYSTEM (CRUD)    ");
                Console.WriteLine("========================================");
                Console.WriteLine("1. Просмотр всех цветов    5. Список курьеров");
                Console.WriteLine("2. Добавить новый цветок   6. Добавить курьера");
                Console.WriteLine("3. Обновить цветок (ID)    7. Оформить заказ");
                Console.WriteLine("4. Удалить цветок (ID)     8. Список всех заказов");
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("9. ИНИЦИАЛИЗАЦИЯ И ЗАПОЛНЕНИЕ БАЗЫ");
                Console.WriteLine("10. Найти цветок по ID");
                Console.WriteLine("0. Выход");
                Console.Write("\nВыберите действие: ");

                string choice = Console.ReadLine();
                if (choice == "0") return;

                try
                {
                    switch (choice)
                    {
                        case "1": ReadAllFlowers(); break;
                        case "2": CreateFlower(); break;
                        case "3": UpdateFlower(); break;
                        case "4": DeleteFlower(); break;
                        case "5": ShowCouriers(); break;
                        case "6": AddCourier(); break;
                        case "7": CreateOrder(); break;
                        case "8": ShowOrders(); break;
                        case "9": InitializeAndSeed(); break;
                        case "10": ReadFlowerById(); break;
                    }
                }
                catch (Exception ex) { Console.WriteLine($"\nОШИБКА: {ex.Message}"); }

                Console.WriteLine("\nНажмите любую клавишу...");
                Console.ReadKey();
            }
        }

        static void ReadAllFlowers()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Flowers", conn);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    Console.WriteLine("\nID | Название           | Цена");
                    while (r.Read()) Console.WriteLine($"{r[0],-2} | {r[1],-18} | {r[2]} руб.");
                }
            }
        }
        
        static void CreateFlower()
        {
            Console.Write("Название: "); string n = Console.ReadLine();
            Console.Write("Цена: "); decimal p = decimal.Parse(Console.ReadLine());
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Flowers (Name, Price) VALUES (@n, @p)", conn);
                cmd.Parameters.AddWithValue("@n", n); cmd.Parameters.AddWithValue("@p", p);
                cmd.ExecuteNonQuery();
                Console.WriteLine("Цветок добавлен!");
            }
        }

        static void UpdateFlower()
        {
            Console.Write("ID цветка для изменения: "); int id = int.Parse(Console.ReadLine());
            Console.Write("Новое название: "); string n = Console.ReadLine();
            Console.Write("Новая цена: "); decimal p = decimal.Parse(Console.ReadLine());
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Flowers SET Name = @n, Price = @p WHERE FlowerID = @id", conn);
                cmd.Parameters.AddWithValue("@id", id); cmd.Parameters.AddWithValue("@n", n); cmd.Parameters.AddWithValue("@p", p);
                Console.WriteLine(cmd.ExecuteNonQuery() > 0 ? "Обновлено!" : "Не найден.");
            }
        }

        static void DeleteFlower()
        {
            Console.Write("ID для удаления: "); int id = int.Parse(Console.ReadLine());
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Flowers WHERE FlowerID = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                Console.WriteLine(cmd.ExecuteNonQuery() > 0 ? "Удалено." : "Не найден.");
            }
        }

        static void ReadFlowerById()
        {
            Console.Write("Введите ID: "); int id = int.Parse(Console.ReadLine());
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Name, Price FROM Flowers WHERE FlowerID = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read()) Console.WriteLine($"\nНайдено: {r[0]} - {r[1]} руб.");
                    else Console.WriteLine("Ничего не найдено.");
                }
            }
        }

        static void InitializeAndSeed()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string sql = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'Flowers')
                    CREATE TABLE Flowers (FlowerID INT PRIMARY KEY IDENTITY, Name NVARCHAR(100), Price DECIMAL(18,2));

                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'Couriers')
                    CREATE TABLE Couriers (CourierID INT PRIMARY KEY IDENTITY, Name NVARCHAR(100), Phone NVARCHAR(20));

                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'Orders')
                    CREATE TABLE Orders (OrderID INT PRIMARY KEY IDENTITY, FlowerID INT FOREIGN KEY REFERENCES Flowers(FlowerID), CourierID INT FOREIGN KEY REFERENCES Couriers(CourierID), Status NVARCHAR(50) DEFAULT N'Новый');

                    IF ((SELECT COUNT(*) FROM Flowers) = 0)
                    INSERT INTO Flowers (Name, Price) VALUES (N'Роза', 150), (N'Тюльпан', 80), (N'Лилия', 250);

                    IF ((SELECT COUNT(*) FROM Couriers) = 0)
                    INSERT INTO Couriers (Name, Phone) VALUES (N'Иван Петров', '+79001112233'), (N'Алексей Сидоров', '+79004445566');";

                new SqlCommand(sql, conn).ExecuteNonQuery();
                Console.WriteLine("База данных готова и заполнена!");
            }
        }

        static void ShowCouriers() { }
        static void AddCourier() { }
        static void CreateOrder() {  }
        static void ShowOrders() { }
    }
}
