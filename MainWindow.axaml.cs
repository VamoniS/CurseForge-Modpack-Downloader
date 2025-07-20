using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Modloader.DataStructures;
using Modloader.User_Controls;
using HttpRequestMessage = System.Net.Http.HttpRequestMessage;

namespace Modloader;

public partial class MainWindow : Window
{
    private int Progress
    {
        set
        {
            if (!Dispatcher.UIThread.CheckAccess())
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    ProgressBar.Value = value;
                });
            }
            else
                ProgressBar.Value = value;
        }
    }

    private static readonly int PageSize = 10;
    private static readonly HttpClient Client = new();
    private static readonly string ApiKey = "$2a$10$KvgZMrQ2Ms21V10Z2jbV2.N.9WvK3zd1gFJ4ejIxFB2.6ke6yW6NS";
    
    private static async Task<SearchResponse> SearchModpack(string? query)
    {
        HttpRequestMessage request;
        if (query is null || query == String.Empty)
        {
            request = new HttpRequestMessage(HttpMethod.Get, 
                $"https://api.curseforge.com/v1/mods/search" +
                $"?gameId=432&sortField=2&sortOrder=desc&pageSize={PageSize}&classId=4471");
        }
        else
        {
            request = new HttpRequestMessage(HttpMethod.Get, 
                $"https://api.curseforge.com/v1/mods/search" +
                $"?gameId=432&sortField=2&sortOrder=desc&pageSize={PageSize}&searchFilter={query}&classId=4471");
        }
        
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("x-api-key", ApiKey);
        
        var response = await Client.SendAsync(request);
        
        response.EnsureSuccessStatusCode();

        var content = JsonSerializer.Deserialize(await response.Content.ReadAsStringAsync(),
            DataStructuresContext.Default.SearchResponse);

        return content;
    }
    
    private static async Task<GetFilesResponse> GetFiles(int modId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, 
            $"https://api.curseforge.com/v1/mods/{modId}/files");
        
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("x-api-key", ApiKey);
        
        var response = await Client.SendAsync(request);
        
        response.EnsureSuccessStatusCode();
        
        var content = JsonSerializer.Deserialize(await response.Content.ReadAsStringAsync(),
            DataStructuresContext.Default.GetFilesResponse);

        return content;
        
    }
    
    private static async Task DownloadFile(int? id, string? filename, string path)
    {
        string? sId = Convert.ToString(id);
        
        if (sId is null)
            return;
        
        string url = $"https://mediafilez.forgecdn.net/files/" +
                     $"{sId.Substring(0, 4).TrimStart('0')}/" +
                     $"{sId.Substring(sId.Length - 3).TrimStart('0')}/{filename}"
            .Replace("+", "%2B")
            .Replace(" ", "%20");
            
        var content = await Client.GetStreamAsync(url);

        try
        {
            var file = File.Create(Path.Combine(path, filename ?? "Error"));
        
            byte[] buffer = new byte[4096];
            int bytesRead;
        
            while ((bytesRead = await content.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                file.Write(buffer, 0, bytesRead);
            }
            await file.DisposeAsync();
        }
        catch 
        {
                //ignored
        }
    }
    
    private static async Task<GetFileResponse> GetFile(int? modId, int? fileId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get,
            $"https://api.curseforge.com/v1/mods/{modId}/files/{fileId}");
        
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("x-api-key", ApiKey);
        
        var response = await Client.SendAsync(request);
        
        response.EnsureSuccessStatusCode();
        
        var content = JsonSerializer.Deserialize(await response.Content.ReadAsStringAsync(),
            DataStructuresContext.Default.GetFileResponse);

        return content;
    }

    private void CheckFolders(string path)
    {
        var folders = new[] { "mods", "shaderpacks", "resourcepacks"};

        foreach (var folder in  folders)
        {
            if (!Directory.Exists($"{Path.GetDirectoryName(path)}/overrides/{folder}"))
            {
                Directory.CreateDirectory($"{Path.GetDirectoryName(path)}/overrides/{folder}");
            }
        }
    }
    
    private static string GetDirectoryToDownload(GetFileResponse? fileInfo)
    {
        if (fileInfo?.Data.FileName is null)
            return String.Empty;
        
        if (fileInfo.Value.Data.FileName.EndsWith(".zip"))
        {
            if (fileInfo.Value.Data.Modules.Any(x => x.Name == "shaders"))
            {
                return "shaderpacks";
            }

            return "resourcepacks";
        }

        return "mods";
    }

    private async Task CheckFiles(string path, List<int> downloadedFiles)
    {
        
        var content = await File.ReadAllTextAsync(path);

        var json = JsonSerializer.Deserialize(content,
            DataStructuresContext.Default.ManifestStructure);
        
        if(downloadedFiles.Count == json.Files.Length)
            return;
        
        int downloadedCount = 0;
        
        foreach (var file in json.Files)
        {
            if (downloadedFiles.Contains(file.Fileid))
            {
                downloadedCount++;
            
                Progress = downloadedCount * 100 / json.Files.Length;
                continue;
            }

            var fileInfo = GetFile(file.Projectid, file.Fileid).GetAwaiter().GetResult();

            await DownloadFile(file.Fileid, fileInfo.Data.FileName, 
                $"{Path.GetDirectoryName(path)}/overrides/{GetDirectoryToDownload(fileInfo)}");

            downloadedCount++;
            
            Progress = downloadedCount * 100 / json.Files.Length;
        }
    }
    
    private List<int> DownloadFilesFromManifest(string path)
    {
        var content = File.ReadAllText(path);

        var json = JsonSerializer.Deserialize(content,
            DataStructuresContext.Default.ManifestStructure);

        List<int> downloadedFiles = new List<int>();
        
        Parallel.ForEach(json.Files, (file) =>
        {
            try
            {
                var fileInfo = GetFile(file.Projectid, file.Fileid).GetAwaiter().GetResult();

                DownloadFile(file.Fileid, fileInfo.Data.FileName, 
                        $"{Path.GetDirectoryName(path)}/overrides/{GetDirectoryToDownload(fileInfo)}")
                    .GetAwaiter();

                downloadedFiles.Add(file.Fileid);

                Progress = downloadedFiles.Count * 100 / json.Files.Length;
            }
            catch
            {
                // ignored
            }
        });

        return downloadedFiles;
    }
    
    private static void UnZipArchive(string path)
    {
        string? directoryName = Path.GetDirectoryName(path);

        if (directoryName is null)
            return;
        
        ZipFile.ExtractToDirectory(path, directoryName);
    }
    
    private async void SearchPanel_OnInitialized(object? sender, EventArgs e)
    {
        var searchResponse = await SearchModpack(null);
        
        SearchPanel.Items.Clear();

        foreach (var modpack in searchResponse.Data)
        {
            var searchResult = new SearchResult()
            {
                Title = modpack.Name,
                ModpackId = modpack.Id,
                DownloadsCount = modpack.DownloadCount
            };
            SearchPanel.Items.Add(searchResult);
        }
    }
    private async void SearchBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        string? query = (sender as TextBox)?.Text;
        
        var searchResponse = await SearchModpack(query);
        
        SearchPanel.Items.Clear();

        foreach (var modpack in searchResponse.Data)
        {
            var searchResult = new SearchResult()
            {
                Title = modpack.Name,
                ModpackId = modpack.Id,
                DownloadsCount = modpack.DownloadCount
            };
            SearchPanel.Items.Add(searchResult);
        }
    }

    private void AddVersionsToModpackFileVersionsComboBox(GetFilesResponse response)
    {
        FileVersionsComboBox.Items.Clear();

        foreach (var file in response.Data)
        {
            var textBlock = new TextBlock()
            {
                Classes = { "ComboBox" },
                Text = file.DisplayName,
                Tag = new ModpackFileTag
                {
                    FileId = file.Id,
                    ModId = file.ModId,
                    Filename = file.FileName,
                    DisplayName = file.DisplayName
                }
            };
            FileVersionsComboBox.Items.Add(textBlock);
        }

        FileVersionsComboBox.SelectedItem = FileVersionsComboBox.Items[0];
    }
    
    private async void ModpackSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var listBox = sender as ListBox;
        var selectedItem = listBox?.SelectedItem as SearchResult;
        
        if(selectedItem is null || selectedItem.ModpackId is null)
            return;

        var files = await GetFiles((int)selectedItem.ModpackId);
        
        AddVersionsToModpackFileVersionsComboBox(files);
    }

    private void SetStatus(string status)
    {
        
        if (!Dispatcher.UIThread.CheckAccess())
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                StatusTextBlock.Text = status;
            });
        }
        else
        {
            StatusTextBlock.Text = status;
        }
    }

    private async void DownloadButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var textBlock = FileVersionsComboBox.SelectedItem as TextBlock;

        var fileInfo = (ModpackFileTag)textBlock?.Tag!;
        
        DownloadButton.IsEnabled = false;
        Progress = 0;

        string path = $"{Environment.CurrentDirectory}/ModloaderData/{fileInfo.DisplayName}";
        
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
        
        Directory.CreateDirectory(path);

        await Task.Run(async () =>
        {
            SetStatus("Downloading manifest.json");
            await DownloadFile(fileInfo.FileId, fileInfo.Filename, path);

            SetStatus("Unzipping archive");
            UnZipArchive($"{path}/{fileInfo.Filename}");
    
            CheckFolders($"{path}/manifest.json");

            SetStatus("Downloading mods");
            var downloadedFiles = DownloadFilesFromManifest($"{path}/manifest.json");

            SetStatus("Checking mods");
            await CheckFiles($"{path}/manifest.json", downloadedFiles);
            SetStatus("Done");
        });
        
        DownloadButton.IsEnabled = true;
    }
    
    public MainWindow()
    {
        InitializeComponent();
    }
}