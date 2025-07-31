using System;
using System.Diagnostics;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Modloader.User_Controls;

public partial class Folder : UserControl
{
    private static readonly StyledProperty<string> FolderNameProperty = 
        AvaloniaProperty.Register<UserControl, string>(nameof(FolderName));

    public string FolderName
    {
        get => GetValue(FolderNameProperty);
        set => SetValue(FolderNameProperty, value);
    }
    
    public string FolderPath = String.Empty;
    
    private void OpenFolder(object sender, RoutedEventArgs e)
    {
        try
        {
            Process.Start("explorer.exe", @$"{FolderPath}/overrides/".Replace("/", @"\"));
        }
        catch
        {
            //ignored
        }
    }
    
    private void DeleteFolder(object sender, RoutedEventArgs e)
    {
        try
        {
            Directory.Delete(FolderPath,true);
        }
        catch
        {
            //ignored            
        }
    }
    
    public Folder()
    {
        DataContext = this;
        InitializeComponent();
    }
}