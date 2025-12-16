using RpdPlayerApp.ViewModels;

namespace RpdPlayerApp.Views;

public partial class OnboardingPage
{
    private readonly OnboardingViewModel _viewModel;


    public OnboardingPage()
    {
        InitializeComponent();
        _viewModel = new();
        BindingContext = _viewModel;
    }
}