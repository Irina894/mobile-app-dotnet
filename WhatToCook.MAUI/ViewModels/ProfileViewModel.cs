using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace WhatToCook.MAUI.ViewModels;

public class ProfileViewModel : INotifyPropertyChanged
{
    private string _fullName = "John Doe";
    private string _email = "john.doe@email.com";
    private string _phone = "+380 99 123 4567";

    public string FullName
    {
        get => _fullName;
        set { _fullName = value; OnPropertyChanged(); OnPropertyChanged(nameof(UserInitials)); }
    }

    public string Email
    {
        get => _email;
        set { _email = value; OnPropertyChanged(); }
    }

    public string Phone
    {
        get => _phone;
        set { _phone = value; OnPropertyChanged(); }
    }

    public string UserInitials => FullName.Length >= 2
        ? $"{FullName[0]}{FullName.Split(' ').LastOrDefault()?[0]}"
        : FullName[..1].ToUpper();

    public ICommand EditProfileCommand { get; }
    public ICommand EditDietCommand { get; }
    public ICommand EditAllergiesCommand { get; }
    public ICommand ChangePhotoCommand { get; }
    public ICommand SignOutCommand { get; }

    public ProfileViewModel()
    {
        EditProfileCommand = new Command(async () =>
        {
            // await Shell.Current.GoToAsync("editprofile");
        });

        EditDietCommand = new Command(async () =>
        {
            // await Shell.Current.GoToAsync("editdiet");
        });

        EditAllergiesCommand = new Command(async () =>
        {
            // await Shell.Current.GoToAsync("editallergies");
        });

        ChangePhotoCommand = new Command(async () =>
        {
            var photo = await MediaPicker.Default.PickPhotoAsync();
            // handle photo
        });

        SignOutCommand = new Command(async () =>
        {
            bool confirm = await Shell.Current.DisplayAlert(
                "Sign out",
                "Are you sure you want to sign out?",
                "Yes, sign out",
                "Cancel");

            if (confirm)
            {
                // clear session, navigate to login
                // await Shell.Current.GoToAsync("//login");
            }
        });
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}