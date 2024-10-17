namespace Lab4IndividualTask;

public partial class AdminPanelPage : ContentPage
{
    public AdminPanelPage()
    {
        InitializeComponent();
        BindingContext = PhoneDirectoryViewModel.Instance;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Загружаем данные
        var viewModel = BindingContext as PhoneDirectoryViewModel;
        if (viewModel != null)
        {
            await viewModel.LoadRoomsAsync();
        }
    }

    private async void ReturnToMainPage(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }
}