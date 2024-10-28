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

        private async void OnAdminLoginClicked(object sender, EventArgs e)
        {
            // Переход на страницу входа администратора
            await Navigation.PushAsync(new AdminLoginPage());
        }
    }
}