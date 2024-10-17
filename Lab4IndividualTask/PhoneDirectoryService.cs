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
            // ������������� ������
            SearchByPhoneCommand = new Command(async () => await SearchByPhoneAsync());
            SearchByRoomCommand = new Command(async () => await SearchByRoomAsync());
            SearchByLastNameCommand = new Command(async () => await SearchByLastNameAsync());
            SaveRoomCommand = new Command(async () => await SaveRoomAsync());
        }

        private async Task SearchByPhoneAsync()
{
            if (int.TryParse(PhoneEntry, out int phoneNumber))
            {
                // ���� ������� �� ������ ��������
                var room = await DatabaseService.GetRoomByPhoneAsync(phoneNumber);

                // ���� ������� �������, ���������� ������ � �������
                if (room != null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "���������� ������",
                        $"����� ���������: {room.RoomNumber}\n����������: {room.Employees}",
                        "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("������", "����� �������� �� ������", "OK");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("������", "������� ���������� ����� ��������", "OK");
            }
        }

private async Task SearchByRoomAsync()
{
            if (int.TryParse(RoomEntry, out int roomNumber))
            {
                // ���� ������� �� ������ ���������
                var room = await DatabaseService.GetRoomByRoomNumberAsync(roomNumber);

                // ���� ������� �������, ���������� ������ � �������
                if (room != null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "���������� ������",
                        $"����� ��������: {room.PhoneNumber}\n����������: {room.Employees}",
                        "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("������", "����� ��������� �� ������", "OK");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("������", "������� ���������� ����� ���������", "OK");
            }
        }

private async Task SearchByLastNameAsync()
{

            if (!string.IsNullOrEmpty(LastNameEntry))
            {
                // ���� ������� �� ������� ����������
                var room = await DatabaseService.GetRoomByEmployeeNameAsync(LastNameEntry);

                // ���� ������� �������, ���������� ������ � �������
                if (room != null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "���������� ������",
                        $"����� ���������: {room.RoomNumber}\n����� ��������: {room.PhoneNumber}\n����������: {room.Employees}",
                        "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("������", "�������� �� ������", "OK");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("������", "������� �������", "OK");
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
                await Application.Current.MainPage.DisplayAlert("�����", "������� ���������", "OK");

                // ������� ����� ����� ����������
                NewRoomNumber = string.Empty;
                NewPhoneNumber = string.Empty;
                NewEmployees = string.Empty;
                await LoadRoomsAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("������", "������� ���������� ������", "OK");
            }
        }

        // ���������� INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}