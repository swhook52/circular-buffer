using System;
using System.IO;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using System.Timers;

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
        private System.Timers.Timer aTimer;
        private int _position;

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
            SetTimer();
        }

        //C:\Users\jha\AppData\Local\Packages\4c38918e-ad40-4856-8a02-9550f2d38933_z8xdybqhpwm1j\LocalState
        private async void DumpButton_Click(object sender, RoutedEventArgs e)
        {
            //await _captureManager.StopRecordAsync();

            //Get the position of the new photo to be saved
            //_position = Convert.ToInt32(_stream.Position);
            //await _captureManager.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), _stream.AsRandomAccessStream());


            //var data = new byte[_stream.Length];
            //_stream.Read(data, _position, data.Length);

            //var storageFolder = ApplicationData.Current.LocalFolder;
            //var sampleFile = await storageFolder.CreateFileAsync("test.jpg", CreationCollisionOption.GenerateUniqueName);

            //await FileIO.WriteBytesAsync(sampleFile, data);


        }

        private void SetTimer()
        {
            aTimer = new System.Timers.Timer(500); // 1 sec or 1000 milliseconds for testing. needs to be faster
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private async void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            _position = Convert.ToInt32(_stream.Position);
            await _captureManager.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), _stream.AsRandomAccessStream());

            var data = new byte[_stream.Length];
            _stream.Read(data, _position, data.Length);

            var storageFolder = ApplicationData.Current.LocalFolder;
            var sampleFile = await storageFolder.CreateFileAsync("test.jpg", CreationCollisionOption.GenerateUniqueName);

            await FileIO.WriteBytesAsync(sampleFile, data);

        }
    }
}