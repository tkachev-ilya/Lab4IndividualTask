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
                // Используем блокировку для многопоточной безопасности
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
                new Room { PhoneNumber = 12, RoomNumber = 101, Employees = new List<Employee> { new Employee { Name = "Иванов" }, new Employee { Name = "Петров" } } },
                new Room { PhoneNumber = 34, RoomNumber = 102, Employees = new List<Employee> { new Employee { Name = "Сидоров" } } }
            };

            AddRoomCommand = new Command(AddRoom);
        }

        private void AddRoom()
        {
            // Разделяем строку сотрудников
            var employeesList = NewEmployees.Split(',')
                .Select(name => new Employee { Name = name.Trim() })
                .ToList();

            // Проверяем, что количество сотрудников не превышает 4
            if (employeesList.Count > 4)
            {
                // Выдаем предупреждение, что максимум 4 сотрудника
                App.Current.MainPage.DisplayAlert("Ошибка", "Максимум 4 сотрудника могут быть в одной комнате.", "OK");
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
                    // Получаем путь к рабочему столу для Windows
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string folderPath = Path.Combine(desktopPath, "PhoneDirectory");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    filePath = Path.Combine(folderPath, "rooms.txt");

#elif ANDROID || IOS
                // Используем AppDataDirectory для Android/iOS
                string folderPath = FileSystem.AppDataDirectory;
                filePath = Path.Combine(folderPath, "rooms.txt");

#endif

                    // Преобразуем данные в текст
                    StringBuilder dataToSave = new StringBuilder();
                    foreach (var room in PhoneDirectory)
                    {
                        dataToSave.AppendLine($"Комната: {room.RoomNumber}, Телефон: {room.PhoneNumber}");
                        dataToSave.AppendLine("Сотрудники:");
                        foreach (var employee in room.Employees)
                        {
                            dataToSave.AppendLine($"- {employee.Name}");
                        }
                        dataToSave.AppendLine();
                    }

                    // Записываем данные в файл
                    File.WriteAllText(filePath, dataToSave.ToString());

                    // Уведомляем пользователя об успешном сохранении
                    App.Current.MainPage.DisplayAlert("Успех", $"Данные успешно сохранены в {filePath}!", "OK");
                }
                catch (Exception ex)
                {
                    // Выводим сообщение об ошибке
                    App.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось сохранить данные: {ex.Message}", "OK");
                }
            }
        }

        public void LoadDataFromFile()
        {
            try
            {
                string filePath = "";

#if WINDOWS
                // Путь к рабочему столу на Windows
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string folderPath = Path.Combine(desktopPath, "PhoneDirectory");
                filePath = Path.Combine(folderPath, "rooms.txt");

#elif ANDROID || IOS
                // Используем AppDataDirectory для Android/iOS
                string folderPath = FileSystem.AppDataDirectory;
                filePath = Path.Combine(folderPath, "rooms.txt");
#endif

                // Проверяем, существует ли файл
                if (File.Exists(filePath))
                {
                    // Читаем содержимое файла
                    var fileContent = File.ReadAllText(filePath);
                    var lines = fileContent.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                    // Очищаем текущий список комнат
                    PhoneDirectory.Clear();

                    // Парсим данные и восстанавливаем комнаты
                    Room currentRoom = null;
                    foreach (var line in lines)
                    {
                        if (line.StartsWith("Комната:"))
                        {
                            // Если это новая комната, создаем объект Room
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
                            // Добавляем сотрудника в текущую комнату
                            var employeeName = line.Substring(2); // Удаляем "- " в начале строки
                            currentRoom?.Employees.Add(new Employee { Name = employeeName });
                        }
                    }

                    // Добавляем последнюю комнату (если она не была добавлена)
                    if (currentRoom != null)
                    {
                        PhoneDirectory.Add(currentRoom);
                    }

                    // Уведомляем пользователя об успешной загрузке
                    App.Current.MainPage.DisplayAlert("Успех", "Данные успешно загружены!", "OK");
                }
                else
                {
                    // Если файл не найден
                    App.Current.MainPage.DisplayAlert("Ошибка", "Файл данных не найден", "OK");
                }
            }
            catch (Exception ex)
            {
                App.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось загрузить данные: {ex.Message}", "OK");
            }
        }
    }
}