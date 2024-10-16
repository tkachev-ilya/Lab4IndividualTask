namespace Lab4IndividualTask
{
    public class Room
    {
        public int PhoneNumber { get; set; }
        public int RoomNumber { get; set; }
        public List<Employee> Employees { get; set; }

        // ¬ычисл€емое свойство дл€ отображени€ сотрудников в виде строки
        public string EmployeesString
        {
            get
            {
                return Employees != null && Employees.Count > 0
                    ? string.Join(", ", Employees.Select(e => e.Name))
                    : "Ќет сотрудников";
            }
        }
    }

    public class Employee
    {
        public string Name { get; set; }
    }
}