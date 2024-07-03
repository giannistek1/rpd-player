namespace RpdPlayerApp.Views;

public partial class BadgeView : ContentView
{
    public static readonly BindableProperty BadgeTextProperty =
        BindableProperty.Create(nameof(BadgeText), typeof(string), typeof(BadgeView), string.Empty, propertyChanged: OnBadgeTextChanged);

    public string BadgeText
    {
        get => (string)GetValue(BadgeTextProperty);
        set => SetValue(BadgeTextProperty, value);
    }

    public BadgeView()
    {
        InitializeComponent();
    }

    private static void OnBadgeTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var badgeView = (BadgeView)bindable;
        var newText = newValue as string;

        badgeView.BadgeLabel.Text = newText;
        badgeView.Badge.IsVisible = !string.IsNullOrEmpty(newText);
    }
}