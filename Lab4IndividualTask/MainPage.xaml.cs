namespace Lab4IndividualTask
{
    public partial class MainPage : ContentPage
    {
        // Здесь можно хранить базу данных номеров, помещений и сотрудников
        public MainPage()
        {
            InitializeComponent();
            BindingContext = PhoneDirectoryViewModel.Instance;
        }

        private void OnPhoneSearchClicked(object sender, EventArgs e)
        {
            int phone;
            if (int.TryParse(PhoneEntry.Text, out phone))
            {
                var room = PhoneDirectoryViewModel.Instance.PhoneDirectory.FirstOrDefault(r => r.PhoneNumber == phone);
                if (room != null)
                {
                    DisplayAlert("Результаты", $"Номер помещения: {room.RoomNumber}\nСлужащие: {string.Join(", ", room.Employees.Select(emp => emp.Name))}", "OK");
                }
                else
                {
                    DisplayAlert("Ошибка", "Номер телефона не найден", "OK");
                }
            }
            else
            {
                DisplayAlert("Ошибка", "Введите корректный номер телефона", "OK");
            }
        }

        private void OnRoomSearchClicked(object sender, EventArgs e)
        {
            // Получаем выбранную комнату из Picker
            var selectedRoom = PhoneDirectoryViewModel.Instance.SelectedRoom;

            if (selectedRoom != null)
            {
                // Если комната выбрана, отображаем информацию о ней
                DisplayAlert("Результаты", $"Номер телефона: {selectedRoom.PhoneNumber}\nСлужащие: {string.Join(", ", selectedRoom.Employees.Select(emp => emp.Name))}", "OK");
            }
            else
            {
                // Если комната не выбрана, отображаем сообщение об ошибке
                DisplayAlert("Ошибка", "Выберите комнату для поиска", "OK");
            }
        }

        private void OnLastNameSearchClicked(object sender, EventArgs e)
        {
            var employeeName = LastNameEntry.Text;
            if (!string.IsNullOrEmpty(employeeName))
            {
                var room = PhoneDirectoryViewModel.Instance.PhoneDirectory.FirstOrDefault(r => r.Employees.Any(emp => emp.Name.Equals(employeeName, StringComparison.OrdinalIgnoreCase)));
                if (room != null)
                {
                    DisplayAlert("Результаты", $"Номер телефона: {room.PhoneNumber}\nНомер помещения: {room.RoomNumber}", "OK");
                }
                else
                {
                    DisplayAlert("Ошибка", "Служащий не найден", "OK");
                }
            }
            else
            {
                DisplayAlert("Ошибка", "Введите фамилию", "OK");
            }
        }

        private async void OnAdminLoginClicked(object sender, EventArgs e)
        {
            // Переход на страницу входа администратора
            await Navigation.PushAsync(new AdminLoginPage());
        }

        private void OnSaveData(object sender, EventArgs e)
        {
            PhoneDirectoryViewModel.Instance.SaveDataToFile();
        }
    }
}