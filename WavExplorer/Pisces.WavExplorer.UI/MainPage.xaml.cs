using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using Pisces.WavExplorer.Core;
using System.Threading.Tasks;
using Windows.Storage.Pickers;

namespace Pisces.WavExplorer.UI
{
  public sealed partial class MainPage : Page
  {
    public MainPage()
    {
      this.InitializeComponent();

      wavFilePicker.Click += HandleFileOpen;
    }

    private void HandleFileOpen(object sender, RoutedEventArgs e)
    {
      OpenWavFile();
    }

    private async void OpenWavFile()
    {
      var picker = new FileOpenPicker
      {
        ViewMode = PickerViewMode.List,
        SuggestedStartLocation = PickerLocationId.MusicLibrary
      };

      picker.FileTypeFilter.Add(".wav");

      var file = await picker.PickSingleFileAsync();
      LoadWavData(file.Path);
    }

    private async void LoadWavData(string filePath)
    {
      var wv = new WavFile(filePath);
      await wv.OpenAsync();
      wavFileInfo.Text = wv.ToString();
    }
  }
}
