using StorageBenchmark.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace StorageBenchmark
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ResultViewModel model = new ResultViewModel();
        private static readonly string filename = "store_test.bin";
        private StorageFolder storageFolder;
        private StorageFile sampleFile;
        
        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = model;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            storageFolder = ApplicationData.Current.LocalFolder;
            sampleFile = await storageFolder.CreateFileAsync(filename,
                        CreationCollisionOption.ReplaceExisting);
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            double totalTime = 0;
            model.IsRunning = true;

            for (int i = 0; i < model.Times; i++)
            {
                totalTime += await WriteTest(20);
            }

            model.WriteSpeed = totalTime / model.Times;

            totalTime = 0;
            for (int i = 0; i < model.Times; i++)
            {
                totalTime += await ReadTest(20);
            }
            //await sampleFile.DeleteAsync();

            model.ReadSpeed = totalTime / model.Times;

            model.IsRunning = false;
        }

        private async Task<double> WriteTest(int sizeInMb)
        {
            var sw = new Stopwatch();
            var buffer = new byte[1024 * 1024 * sizeInMb];
            new Random().NextBytes(buffer);
            
            sw.Start();
            await FileIO.WriteBytesAsync(sampleFile, buffer);
            sw.Stop();

            return sizeInMb / sw.Elapsed.TotalSeconds;
        }

        private async Task<double> ReadTest(int sizeInMb)
        {
            var bytes = new byte[1024 * 1024 * sizeInMb];
            var sw = new Stopwatch();
            sw.Start();
            var buffer = await FileIO.ReadBufferAsync(sampleFile);
            using (var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buffer))
            {
                dataReader.ReadBytes(bytes);
            }
            sw.Stop();

            return sizeInMb / sw.Elapsed.TotalSeconds;
        }

        //private async void GetStorageFolder()
        //{
        //    StorageFolder storageFolder = null;

        //    if (sdCheckBox.IsChecked == true)
        //    {
        //        // Get the logical root folder for all external storage devices.
        //        StorageFolder externalDevices = KnownFolders.RemovableDevices;

        //        // Get the first child folder, which represents the SD card.
        //        StorageFolder sdCard = (await externalDevices.GetFoldersAsync()).FirstOrDefault();

        //        if (sdCard != null)
        //        {
        //            storageFolder = sdCard;
        //        }
        //        else
        //        {
        //            await new MessageDialog("SD card not found").ShowAsync();
        //        }
        //    }
        //    else
        //    {
        //        storageFolder = ApplicationData.Current.LocalFolder;
        //    }

        //    this.storageFolder = storageFolder;
        //    this.sampleFile = await storageFolder.CreateFileAsync(filename,
        //                CreationCollisionOption.ReplaceExisting);
        //}

        private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                e.Handled = true;
                StartButton_Click(sender, e);
            }
        }

        private async void SelectFileBox_Click(object sender, RoutedEventArgs e)
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "temp_file";

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                sampleFile = file;
            }
        }


    }
}
