using Avalonia;
using Avalonia.Controls;

namespace Modloader.User_Controls;

public partial class SearchResult : UserControl
{
    private static readonly StyledProperty<string?> TitleProperty = 
        AvaloniaProperty.Register<UserControl, string?>(nameof(Title));
    
    private static readonly StyledProperty<string?> LogoUrlProperty = 
        AvaloniaProperty.Register<UserControl, string?>(nameof(LogoUrl));

    private static readonly StyledProperty<int> DownloadCountProperty = 
        AvaloniaProperty.Register<UserControl, int>(nameof(DownloadsCount));

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    
    public string? LogoUrl 
    {
        get => GetValue(LogoUrlProperty);
        set => SetValue(LogoUrlProperty, value);
    }
    
    public int? DownloadsCount 
    {
        get => GetValue(DownloadCountProperty);
        set => SetValue(DownloadCountProperty, value);
    }

    public int? ModpackId;

    public SearchResult()
    {
        DataContext = this;
        InitializeComponent();
    }
}