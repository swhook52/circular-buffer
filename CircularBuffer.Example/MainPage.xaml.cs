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
using Windows.UI.Core;

namespace CircularBuffer.Example
{
    public sealed partial class MainPage : Page
    {
        private readonly string DEFAULT_FILE_EXT = "png";

        private MediaCapture _captureManager;
        private CircularStream _stream = new CircularStream(1000000);
        private Timer _timer;
        private StorageFolder _storageLocation; // C:\Users\jha\AppData\Local\Packages\4c38918e-ad40-4856-8a02-9550f2d38933_z8xdybqhpwm1j\LocalState
        private int _fileCount = 0;

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
            var nextFileName = CreateUniqueSortedFileName();

            try
            {
                var position = Convert.ToInt32(_stream.Position);
                await _captureManager.CapturePhotoToStreamAsync(ImageEncodingProperties.CreatePng(), _stream.AsRandomAccessStream());   // capture an image and write it to the stream

                var data = new byte[_stream.Length];
                _stream.Read(data, position, data.Length);

                var sampleFile = await _storageLocation.CreateFileAsync(nextFileName, CreationCollisionOption.ReplaceExisting);

                await FileIO.WriteBytesAsync(sampleFile, data);     // dump the captured image to a file
            }
            catch (Exception ex)
            {
                if (File.Exists(_storageLocation.Path + nextFileName))  // delete the corrupted file if an error occurs
                {
                    File.Delete(_storageLocation.Path + nextFileName);
                }

                DumpButton_Click(null, null);   // stream has failed, create a replay for what we have so far
            }
        }

        private string CreateUniqueSortedFileName()
        {
            _fileCount = _fileCount == _stream.Length ? 0 : ++_fileCount;

            return _fileCount.ToString().PadLeft(_stream.Length.ToString().Length, '0') + $".{DEFAULT_FILE_EXT}";
        }
      
        private async void DumpButton_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();  // stop the capture

            var imageSources = new List<IImageProvider>();
            IBuffer outBuffer;

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { DumpButton.Content = "Creating GIF now. Please wait... "; });

            var images = Directory.GetFiles($@"{_storageLocation.Path}", $"*.{DEFAULT_FILE_EXT}")  // get the names of each file in the directory
                .OfType<string>()
                .ToList();

            foreach (var filename in images)    // read each file and add them to the collection of images 
            {
                var image = await _storageLocation.GetFileAsync(filename.Replace($@"{_storageLocation.Path}", ""));
                imageSources.Add(new StorageFileImageSource(image));             
            }

            using (var gifRenderer = new GifRenderer(imageSources))     // render the animated gif from the collection of files
            {
                gifRenderer.Duration = 300;
                outBuffer = await gifRenderer.RenderAsync();
            }

            var storageFile = await _storageLocation.CreateFileAsync("replay.gif", CreationCollisionOption.ReplaceExisting);  // create and write the output gif
            using (var stream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                await stream.WriteAsync(outBuffer);
            }
           
            Array.ForEach(Directory.GetFiles($@"{_storageLocation.Path}", $"*.{DEFAULT_FILE_EXT}"), File.Delete);   // cleanup old png files from buffer

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { DumpButton.Content = "Done! :)"; });
        }
    }
}