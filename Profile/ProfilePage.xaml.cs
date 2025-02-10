using ProfileApp.Models;
using System.Text.Json;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.IO;

namespace ProfileApp
{
    public partial class ProfilePage : ContentPage
    {
        private UserProfile _profile; // Changed from Profile to UserProfile
        private string _filePath;

        public ProfilePage()
        {
            InitializeComponent();
            _filePath = Path.Combine(FileSystem.AppDataDirectory, "profile.json");
            LoadProfile();
            BindingContext = _profile; // Binding the profile object to the page
        }

        private void LoadProfile()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                _profile = JsonSerializer.Deserialize<UserProfile>(json); // Use UserProfile here
            }
            else
            {
                _profile = new UserProfile(); // Initialize an empty profile if file does not exist
            }

            if (!string.IsNullOrEmpty(_profile.ProfilePicturePath))
            {
                ProfileImage.Source = ImageSource.FromFile(_profile.ProfilePicturePath); // Correct reference to ProfileImage
            }
        }

        private async void OnSaveProfileClicked(object sender, EventArgs e)
        {
            var json = JsonSerializer.Serialize(_profile); // Serialize the UserProfile
            File.WriteAllText(_filePath, json);
            await DisplayAlert("Success", "Profile saved successfully!", "OK");
        }

        private async void OnUploadPictureClicked(object sender, EventArgs e)
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select a profile picture",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                var filePath = Path.Combine(FileSystem.AppDataDirectory, result.FileName);
                using (var stream = await result.OpenReadAsync())
                using (var newStream = File.Create(filePath))
                {
                    await stream.CopyToAsync(newStream); // Copy the selected file to local directory
                }

                ProfileImage.Source = ImageSource.FromFile(filePath); // Set image to UI
                _profile.ProfilePicturePath = filePath; // Store the file path in the UserProfile object
            }
        }
    }
}
