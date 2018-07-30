using System;
using System.IO;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CircularBuffer.Example
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MediaCapture _captureManager;
        private CircularStream _stream = new CircularStream(1000000);

        public MainPage()
        {
            this.InitializeComponent();
            _captureManager = new MediaCapture();
            this.Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await _captureManager.InitializeAsync();
            VideoFeed.Source = _captureManager;

            await _captureManager.StartPreviewAsync();
            await _captureManager.StartRecordToStreamAsync(MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Auto), _stream.AsRandomAccessStream());
        }

        private async void DumpButton_Click(object sender, RoutedEventArgs e)
        {
            await _captureManager.StopRecordAsync();

            var data = new byte[_stream.Length];
            await _stream.ReadAsync(data, 0, data.Length);

            var storageFolder = ApplicationData.Current.LocalFolder;
            var sampleFile = await storageFolder.CreateFileAsync("test.mp4", CreationCollisionOption.ReplaceExisting);

            await FileIO.WriteBytesAsync(sampleFile, data);
        }
    }
}
