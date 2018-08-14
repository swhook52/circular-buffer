using System;
using System.IO;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using System.Timers;
using Lumia.Imaging;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage.Streams;

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
        private Timer _timer;

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
            _timer.Stop();

            //var data = new byte[_stream.Length];
            //_stream.Read(data, 0, data.Length);

            var files = Directory.GetFiles(@"C:\Users\atrievel\AppData\Local\Packages\4c38918e-ad40-4856-8a02-9550f2d38933_z8xdybqhpwm1j\LocalState")
                .OfType<string>()
                .ToList();

            files.Sort();

            var imageSources = new List<IImageProvider>();
            IBuffer buffer;

            foreach(var filename in files)
            {
                var test = filename.Replace(@"C:\Users\atrievel\AppData\Local\Packages\4c38918e-ad40-4856-8a02-9550f2d38933_z8xdybqhpwm1j\LocalState", "");
                var image = await ApplicationData.Current.LocalFolder.GetFileAsync(test);
                imageSources.Add(new StorageFileImageSource(image));
            }

            using (var gifRenderer = new GifRenderer(imageSources))
            {
                gifRenderer.Duration = 100;
                buffer = await gifRenderer.RenderAsync();
            }

            var output = "test-gif.gif";
            var storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(output, CreationCollisionOption.ReplaceExisting);
            using (var stream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                await stream.WriteAsync(buffer);
            }
        }

        private void SetTimer()
        {
            _timer = new Timer(250); // 1 sec or 1000 milliseconds for testing. needs to be faster
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private async void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            var position = Convert.ToInt32(_stream.Position);
            await _captureManager.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), _stream.AsRandomAccessStream());


            var data = new byte[_stream.Length];
            _stream.Read(data, position, data.Length);

            var storageFolder = ApplicationData.Current.LocalFolder;
            var sampleFile = await storageFolder.CreateFileAsync(".png", CreationCollisionOption.GenerateUniqueName);

            await FileIO.WriteBytesAsync(sampleFile, data);
        }
    }
}