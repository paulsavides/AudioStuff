using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Pisces.WavExplorer.Core;
using System.Threading.Tasks;
using Windows.Storage.Pickers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Pisces.WavExplorer.UI
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class MainPage : Page
  {
    public MainPage()
    {
      this.InitializeComponent();

      wavFilePicker.Click += new RoutedEventHandler(HandleFileOpen);

    }

    private void HandleFileOpen(object sender, RoutedEventArgs e)
    {
      Task.Run(new Action(() => OpenWavFile()));// OpenWav(wavFilePath.Text));
    }

    private async void OpenWavFile()
    {
      FileOpenPicker picker = new FileOpenPicker();
      picker.ViewMode = PickerViewMode.List;
      picker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
      picker.FileTypeFilter.Add(".wav");

      var file = await picker.PickSingleFileAsync();




      WavFile wv = new WavFile(file.Path);

      wavFileInfo.Text = wv.ToString();
    }
  }
}
