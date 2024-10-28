using SQLite;

namespace Lab4IndividualTask
{
    static class DatabaseService
    {
        static SQLiteAsyncConnection db;
        static async Task Init()
        {
            if (db == null) 
            {
                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                var directoryPath = Path.Combine(desktopPath, "PhoneDirectory");

                // Создаем папку, если она не существует
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Путь к файлу базы данных
                var databasePath = Path.Combine(directoryPath, "MyData.db");

                // Инициализируем SQLiteAsyncConnection
                db = new SQLiteAsyncConnection(databasePath);

                // Создаём таблицу, если она не создана
                await db.CreateTableAsync<Room>();
            }
            return;
            
        }

        public static async Task SaveRoom(Room room)
        {
            await Init();
            await db.InsertAsync(room);
        }

        public static async Task DeleteRoom(Room room)
        {
            await Init();
            await db.DeleteAsync(room); // Асинхронное удаление
        }

        public static async Task<List<Room>> RetrieveRoomsAsync()
        {
            await Init();
            return await db.Table<Room>().ToListAsync(); // Асинхронное получение списка комнат
        }

        public static async Task<Room> GetRoomByPhoneAsync(int phoneNumber)
        {
            await Init();
            return await db.Table<Room>().FirstOrDefaultAsync(r => r.PhoneNumber == phoneNumber);
        }

        public static async Task<Room> GetRoomByRoomNumberAsync(int roomNumber)
        {
            await Init();
            return await db.Table<Room>().FirstOrDefaultAsync(r => r.RoomNumber == roomNumber);
        }

        public static async Task<Room> GetRoomByEmployeeNameAsync(string employeeName)
        {
            await Init();
            return await db.Table<Room>().FirstOrDefaultAsync(r => r.Employees.Contains(employeeName));
        }
        public static async Task<int> DeleteRoomAsync(int roomNumber)
        {
            await Init();  // Инициализация базы данных, если она еще не инициализирована

            // Получаем комнату с указанным номером
            var roomToDelete = await db.Table<Room>().FirstOrDefaultAsync(r => r.RoomNumber == roomNumber);

            // Если комната найдена, удаляем её
            if (roomToDelete != null)
            {
                return await db.DeleteAsync(roomToDelete);
            }

            return 0;
        }
    }
}