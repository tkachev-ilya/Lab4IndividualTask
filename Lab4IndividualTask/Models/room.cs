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

        // ������ ����������� ����� ��������� ��� ������ � �������������
        public string Employees { get; set; }
    }
}