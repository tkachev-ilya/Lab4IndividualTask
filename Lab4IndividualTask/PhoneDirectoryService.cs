using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace Lab4IndividualTask
{
    public class PhoneDirectoryViewModel : INotifyPropertyChanged
    {
        private static PhoneDirectoryViewModel _instance;
        private static readonly object _lock = new object();
        public static PhoneDirectoryViewModel Instance
        {
            get
            {
                // ���������� ���������� ��� ������������� ������������
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new PhoneDirectoryViewModel();
                    }
                    return _instance;
                }
            }
        }
        private Room _selectedRoom;
        public Room SelectedRoom
        {
            get => _selectedRoom;
            set
            {
                _selectedRoom = value;
                OnPropertyChanged(nameof(SelectedRoom));
            }
        }
        public ObservableCollection<Room> PhoneDirectory { get; set; }
        public ICommand AddRoomCommand { get; }

        private string _newPhoneNumber;
        public string NewPhoneNumber
        {
            get => _newPhoneNumber;
            set
            {
                _newPhoneNumber = value;
                OnPropertyChanged(nameof(NewPhoneNumber));
            }
        }

        private string _newRoomNumber;
        public string NewRoomNumber
        {
            get => _newRoomNumber;
            set
            {
                _newRoomNumber = value;
                OnPropertyChanged(nameof(NewRoomNumber));
            }
        }

        private string _newEmployees;
        public string NewEmployees
        {
            get => _newEmployees;
            set
            {
                _newEmployees = value;
                OnPropertyChanged(nameof(NewEmployees));
            }
        }

        public PhoneDirectoryViewModel()
        {
            PhoneDirectory = new ObservableCollection<Room>
            {
                new Room { PhoneNumber = 12, RoomNumber = 101, Employees = new List<Employee> { new Employee { Name = "������" }, new Employee { Name = "������" } } },
                new Room { PhoneNumber = 34, RoomNumber = 102, Employees = new List<Employee> { new Employee { Name = "�������" } } }
            };

            AddRoomCommand = new Command(AddRoom);
        }

        private void AddRoom()
        {
            // ��������� ������ �����������
            var employeesList = NewEmployees.Split(',')
                .Select(name => new Employee { Name = name.Trim() })
                .ToList();

            // ���������, ��� ���������� ����������� �� ��������� 4
            if (employeesList.Count > 4)
            {
                // ������ ��������������, ��� �������� 4 ����������
                App.Current.MainPage.DisplayAlert("������", "�������� 4 ���������� ����� ���� � ����� �������.", "OK");
                return;
            }

            var newRoom = new Room
            {
                PhoneNumber = int.Parse(NewPhoneNumber),
                RoomNumber = int.Parse(NewRoomNumber),
                Employees = NewEmployees.Split(',').Select(name => new Employee { Name = name.Trim() }).ToList()
            };

            PhoneDirectory.Add(newRoom);
            ClearFields();
        }
        private void ClearFields()
        {
            NewPhoneNumber = string.Empty;
            NewRoomNumber = string.Empty;
            NewEmployees = string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void SaveDataToFile()
        {
            {
                try
                {
                    string filePath = "";

#if WINDOWS
                    // �������� ���� � �������� ����� ��� Windows
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string folderPath = Path.Combine(desktopPath, "PhoneDirectory");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    filePath = Path.Combine(folderPath, "rooms.txt");

#elif ANDROID || IOS
                // ���������� AppDataDirectory ��� Android/iOS
                string folderPath = FileSystem.AppDataDirectory;
                filePath = Path.Combine(folderPath, "rooms.txt");

#endif

                    // ����������� ������ � �����
                    StringBuilder dataToSave = new StringBuilder();
                    foreach (var room in PhoneDirectory)
                    {
                        dataToSave.AppendLine($"�������: {room.RoomNumber}, �������: {room.PhoneNumber}");
                        dataToSave.AppendLine("����������:");
                        foreach (var employee in room.Employees)
                        {
                            dataToSave.AppendLine($"- {employee.Name}");
                        }
                        dataToSave.AppendLine();
                    }

                    // ���������� ������ � ����
                    File.WriteAllText(filePath, dataToSave.ToString());

                    // ���������� ������������ �� �������� ����������
                    App.Current.MainPage.DisplayAlert("�����", $"������ ������� ��������� � {filePath}!", "OK");
                }
                catch (Exception ex)
                {
                    // ������� ��������� �� ������
                    App.Current.MainPage.DisplayAlert("������", $"�� ������� ��������� ������: {ex.Message}", "OK");
                }
            }
        }

        public void LoadDataFromFile()
        {
            try
            {
                string filePath = "";

#if WINDOWS
                // ���� � �������� ����� �� Windows
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string folderPath = Path.Combine(desktopPath, "PhoneDirectory");
                filePath = Path.Combine(folderPath, "rooms.txt");

#elif ANDROID || IOS
                // ���������� AppDataDirectory ��� Android/iOS
                string folderPath = FileSystem.AppDataDirectory;
                filePath = Path.Combine(folderPath, "rooms.txt");
#endif

                // ���������, ���������� �� ����
                if (File.Exists(filePath))
                {
                    // ������ ���������� �����
                    var fileContent = File.ReadAllText(filePath);
                    var lines = fileContent.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                    // ������� ������� ������ ������
                    PhoneDirectory.Clear();

                    // ������ ������ � ��������������� �������
                    Room currentRoom = null;
                    foreach (var line in lines)
                    {
                        if (line.StartsWith("�������:"))
                        {
                            // ���� ��� ����� �������, ������� ������ Room
                            if (currentRoom != null)
                            {
                                PhoneDirectory.Add(currentRoom);
                            }

                            currentRoom = new Room();
                            var roomData = line.Split(", ");
                            currentRoom.RoomNumber = int.Parse(roomData[0].Split(": ")[1]);
                            currentRoom.PhoneNumber = int.Parse(roomData[1].Split(": ")[1]);
                            currentRoom.Employees = new List<Employee>();
                        }
                        else if (line.StartsWith("- "))
                        {
                            // ��������� ���������� � ������� �������
                            var employeeName = line.Substring(2); // ������� "- " � ������ ������
                            currentRoom?.Employees.Add(new Employee { Name = employeeName });
                        }
                    }

                    // ��������� ��������� ������� (���� ��� �� ���� ���������)
                    if (currentRoom != null)
                    {
                        PhoneDirectory.Add(currentRoom);
                    }

                    // ���������� ������������ �� �������� ��������
                    App.Current.MainPage.DisplayAlert("�����", "������ ������� ���������!", "OK");
                }
                else
                {
                    // ���� ���� �� ������
                    App.Current.MainPage.DisplayAlert("������", "���� ������ �� ������", "OK");
                }
            }
            catch (Exception ex)
            {
                App.Current.MainPage.DisplayAlert("������", $"�� ������� ��������� ������: {ex.Message}", "OK");
            }
        }
    }
}