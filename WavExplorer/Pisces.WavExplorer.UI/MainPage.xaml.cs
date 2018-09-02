using System;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using Pisces.WavExplorer.Core;
using Windows.Storage.Pickers;
using Microsoft.Win32.SafeHandles;

namespace Pisces.WavExplorer.UI
{
  public sealed partial class MainPage : Page
  {
    private WavFile _wv;

    public MainPage()
    {
      InitializeComponent();
      WavFilePicker.Click += HandleFileOpen;
      LogPath.Text = NLog.LogManager.Configuration.Variables["LogPath"] + "\\diag";
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
      LoadWavData(file.CreateSafeFileHandle(FileAccess.Read));
    }

    private async void LoadWavData(SafeFileHandle handle)
    {
      _wv?.Dispose();

      _wv = new WavFile();
      await _wv.OpenAsync(handle);
      WavFileInfo.Text = _wv.ToString();
    }
  }
}
