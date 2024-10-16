namespace Lab4IndividualTask;

public partial class AdminPanelPage : ContentPage
{
    public AdminPanelPage()
    {
        InitializeComponent();
        BindingContext = PhoneDirectoryViewModel.Instance;
    }

    private async void ReturnToMainPage(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }
}