using System;
using System.IO;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using System.Timers;
using Lumia.Imaging;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage.Streams;

namespace CircularBuffer.Example
{
    public sealed partial class MainPage : Page
    {
        private MediaCapture _captureManager;
        private CircularStream _stream = new CircularStream(1000000);
        private Timer _timer;
        private StorageFolder _storageLocation; // C:\Users\jha\AppData\Local\Packages\4c38918e-ad40-4856-8a02-9550f2d38933_z8xdybqhpwm1j\LocalState

        public MainPage()
        {
            this.InitializeComponent();
            _captureManager = new MediaCapture();
            this.Loaded += MainPage_Loaded;
            _storageLocation = ApplicationData.Current.LocalFolder;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await _captureManager.InitializeAsync();
            VideoFeed.Source = _captureManager;
            await _captureManager.StartPreviewAsync();

            SetTimer();
        }

        private void SetTimer()
        {
            _timer = new Timer(500); // any faster than this seems to make it crash randomly
            _timer.Elapsed += OnElapsed;
            _timer.AutoReset = true;
            _timer.Start();
        }

        private async void OnElapsed(Object source, ElapsedEventArgs e)
        {
            var position = Convert.ToInt32(_stream.Position);
            await _captureManager.CapturePhotoToStreamAsync(ImageEncodingProperties.CreatePng(), _stream.AsRandomAccessStream());

            var data = new byte[_stream.Length];
            _stream.Read(data, position, data.Length);

            var sampleFile = await _storageLocation.CreateFileAsync(".png", CreationCollisionOption.GenerateUniqueName);

            await FileIO.WriteBytesAsync(sampleFile, data);
        }
      
        private async void DumpButton_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();  // stop the capture

            var imageSources = new List<IImageProvider>();
            IBuffer outBuffer;

            var images = Directory.GetFiles($@"{_storageLocation.Path}")  // get the names of each file in the directory
                .OfType<string>()
                .ToList();

            foreach (var filename in images)    // read each file and add them to the collection of images 
            {
                var image = await _storageLocation.GetFileAsync(filename.Replace($@"{_storageLocation.Path}", ""));
                imageSources.Add(new StorageFileImageSource(image));
            }

            using (var gifRenderer = new GifRenderer(imageSources))     // render the animated gif from the collection of files
            {
                gifRenderer.Duration = 250;
                outBuffer = await gifRenderer.RenderAsync();
            }

            var storageFile = await _storageLocation.CreateFileAsync("test-gif.gif", CreationCollisionOption.ReplaceExisting);  // create and write the output gif
            using (var stream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                await stream.WriteAsync(outBuffer);
            }
        }
    }
}