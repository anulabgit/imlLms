using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.Threading.Tasks;
using Microsoft.UI;

namespace App1
{
    public sealed partial class MainWindow : Window
    {
        private string selectedPath = string.Empty;
        private string selectedPhotoFolderPath = string.Empty;

        public MainWindow()
        {
            this.InitializeComponent();
        }
        private async void SelectPathButton_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add("*");

            // Initialize with the current window's HWND
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                selectedPath = folder.Path;
                SelectedPathTextBlock.Text = $"���õ� ���: {selectedPath}";
            }
        }

        private async void SelectPhotoFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            folderPicker.FileTypeFilter.Add("*");

            // Initialize with the current window's HWND
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                selectedPhotoFolderPath = folder.Path;
                SelectedPhotoFolderTextBlock.Text = $"���õ� ���� ����: {selectedPhotoFolderPath}";
            }
        }

        private async void CreateFolderButton_Click(object sender, RoutedEventArgs e)
        {
            string folderName = FolderNameTextBox.Text;

            if (string.IsNullOrWhiteSpace(folderName) || string.IsNullOrWhiteSpace(selectedPath))
            {
                await ShowMessageAsync("����", "���� �̸��� ���� ��θ� ��� �Է����ּ���.");
                return;
            }

            try
            {
                string fullPath = Path.Combine(selectedPath, folderName);
                Directory.CreateDirectory(fullPath);
                await ShowMessageAsync("����", $"������ �����Ǿ����ϴ�: {fullPath}");

                // ���õ� ���� ������ �ִٸ� ������ �� ������ ����
                if (!string.IsNullOrWhiteSpace(selectedPhotoFolderPath))
                {
                    CopyPhotosToNewFolder(selectedPhotoFolderPath, fullPath);
                }
            }
            catch (Exception ex)
            {
                await ShowMessageAsync("����", $"���� ���� �� ���� �߻�: {ex.Message}");
            }
        }

        private void CopyPhotosToNewFolder(string sourcePath, string destinationPath)
        {
            string[] photoExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
            foreach (string file in Directory.GetFiles(sourcePath))
            {
                string extension = Path.GetExtension(file).ToLower();
                if (Array.IndexOf(photoExtensions, extension) != -1)
                {
                    string destFile = Path.Combine(destinationPath, Path.GetFileName(file));
                    File.Copy(file, destFile, true);
                }
            }
        }

        private async Task ShowMessageAsync(string title, string content)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "Ȯ��"
            };

            dialog.XamlRoot = this.Content.XamlRoot;
            await dialog.ShowAsync();
        }
    }
}