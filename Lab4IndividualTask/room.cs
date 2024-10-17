using System.Data.SqlTypes;
using SQLite;


namespace Lab4IndividualTask
{

    public class Room
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int RoomNumber { get; set; }
        public int PhoneNumber { get; set; }

        // Список сотрудников будет храниться как строка с разделителями
        public string Employees { get; set; }
    }
}