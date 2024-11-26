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
                SelectedPathTextBlock.Text = $"선택된 경로: {selectedPath}";
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
                SelectedPhotoFolderTextBlock.Text = $"선택된 사진 폴더: {selectedPhotoFolderPath}";
            }
        }

        private async void CreateFolderButton_Click(object sender, RoutedEventArgs e)
        {
            string folderName = FolderNameTextBox.Text;

            if (string.IsNullOrWhiteSpace(folderName) || string.IsNullOrWhiteSpace(selectedPath))
            {
                await ShowMessageAsync("오류", "폴더 이름과 저장 경로를 모두 입력해주세요.");
                return;
            }

            try
            {
                string fullPath = Path.Combine(selectedPath, folderName);
                Directory.CreateDirectory(fullPath);
                await ShowMessageAsync("성공", $"폴더가 생성되었습니다: {fullPath}");

                // 선택된 사진 폴더가 있다면 사진을 새 폴더로 복사
                if (!string.IsNullOrWhiteSpace(selectedPhotoFolderPath))
                {
                    CopyPhotosToNewFolder(selectedPhotoFolderPath, fullPath);
                }
            }
            catch (Exception ex)
            {
                await ShowMessageAsync("오류", $"폴더 생성 중 오류 발생: {ex.Message}");
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
                CloseButtonText = "확인"
            };

            dialog.XamlRoot = this.Content.XamlRoot;
            await dialog.ShowAsync();
        }
    }
}