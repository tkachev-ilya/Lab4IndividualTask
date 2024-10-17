using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
        public ICommand SaveRoomCommand { get; }

        private string _newRoomNumber;
        public string NewRoomNumber
        {
            get => _newRoomNumber;
            set
            {
                _newRoomNumber = value;
                OnPropertyChanged();
            }
        }

        private string _newPhoneNumber;
        public string NewPhoneNumber
        {
            get => _newPhoneNumber;
            set
            {
                _newPhoneNumber = value;
                OnPropertyChanged();
            }
        }

        private string _newEmployees;
        public string NewEmployees
        {
            get => _newEmployees;
            set
            {
                _newEmployees = value;
                OnPropertyChanged();
            }
        }

        public ICommand SearchByPhoneCommand { get; }
        public ICommand SearchByRoomCommand { get; }
        public ICommand SearchByLastNameCommand { get; }

        private string _searchResult;
        public string SearchResult
        {
            get => _searchResult;
            set
            {
                _searchResult = value;
                OnPropertyChanged();
            }
        }

        private string _phoneEntry;
        public string PhoneEntry
        {
            get => _phoneEntry;
            set
            {
                _phoneEntry = value;
                OnPropertyChanged();
            }
        }

        private string _roomEntry;
        public string RoomEntry
        {
            get => _roomEntry;
            set
            {
                _roomEntry = value;
                OnPropertyChanged();
            }
        }

        private string _lastNameEntry;
        public string LastNameEntry
        {
            get => _lastNameEntry;
            set
            {
                _lastNameEntry = value;
                OnPropertyChanged();
            }
        }

        public PhoneDirectoryViewModel()
        {
            // Инициализация команд
            SearchByPhoneCommand = new Command(async () => await SearchByPhoneAsync());
            SearchByRoomCommand = new Command(async () => await SearchByRoomAsync());
            SearchByLastNameCommand = new Command(async () => await SearchByLastNameAsync());
            SaveRoomCommand = new Command(async () => await SaveRoomAsync());
        }

        private async Task SearchByPhoneAsync()
{
            if (int.TryParse(PhoneEntry, out int phoneNumber))
            {
                // Ищем комнату по номеру телефона
                var room = await DatabaseService.GetRoomByPhoneAsync(phoneNumber);

                // Если комната найдена, показываем диалог с данными
                if (room != null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Результаты поиска",
                        $"Номер помещения: {room.RoomNumber}\nСотрудники: {room.Employees}",
                        "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка", "Номер телефона не найден", "OK");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Введите корректный номер телефона", "OK");
            }
        }

private async Task SearchByRoomAsync()
{
            if (int.TryParse(RoomEntry, out int roomNumber))
            {
                // Ищем комнату по номеру помещения
                var room = await DatabaseService.GetRoomByRoomNumberAsync(roomNumber);

                // Если комната найдена, показываем диалог с данными
                if (room != null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Результаты поиска",
                        $"Номер телефона: {room.PhoneNumber}\nСотрудники: {room.Employees}",
                        "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка", "Номер помещения не найден", "OK");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Введите корректный номер помещения", "OK");
            }
        }

private async Task SearchByLastNameAsync()
{

            if (!string.IsNullOrEmpty(LastNameEntry))
            {
                // Ищем комнату по фамилии сотрудника
                var room = await DatabaseService.GetRoomByEmployeeNameAsync(LastNameEntry);

                // Если комната найдена, показываем диалог с данными
                if (room != null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Результаты поиска",
                        $"Номер помещения: {room.RoomNumber}\nНомер телефона: {room.PhoneNumber}\nСотрудники: {room.Employees}",
                        "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка", "Служащий не найден", "OK");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Введите фамилию", "OK");
            }
        }

        private ObservableCollection<Room> _phoneDirectory;
        public ObservableCollection<Room> PhoneDirectory
        {
            get => _phoneDirectory;
            set
            {
                _phoneDirectory = value;
                OnPropertyChanged();
            }
        }

        public async Task LoadRoomsAsync()
        {
            var rooms = await DatabaseService.RetrieveRoomsAsync();
            PhoneDirectory = new ObservableCollection<Room>(rooms);
        }
        private async Task SaveRoomAsync()
        {
            if (int.TryParse(NewRoomNumber, out int roomNumber) && int.TryParse(NewPhoneNumber, out int phoneNumber))
            {
                var newRoom = new Room
                {
                    RoomNumber = roomNumber,
                    PhoneNumber = phoneNumber,
                    Employees = NewEmployees
                };
                await DatabaseService.SaveRoom(newRoom);
                await Application.Current.MainPage.DisplayAlert("Успех", "Комната сохранена", "OK");

                // Очистка полей после сохранения
                NewRoomNumber = string.Empty;
                NewPhoneNumber = string.Empty;
                NewEmployees = string.Empty;
                await LoadRoomsAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Введите корректные данные", "OK");
            }
        }

        // Реализация INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}