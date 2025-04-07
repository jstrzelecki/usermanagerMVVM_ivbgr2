using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ivbgr2userManager.Models;
using ivbgr2userManager.Services;
using Npgsql;

namespace ivbgr2userManager.ViewModels;

public partial class UserViewModel : ViewModelBase
{
    //private readonly UserRepository _repository;
    private readonly UserService _userService;
    
    [ObservableProperty] private string _firstName; //FirstName
    [ObservableProperty] private string _lastName; // LastName
    [ObservableProperty] private string _email; // Email 
    [ObservableProperty] private User? _selectedUser;

    // pola do obsługi radiobuttonów
    [ObservableProperty] private bool _isViaEmailSelected;
    [ObservableProperty] private bool _isViaSmsSelected;
    [ObservableProperty] private bool _isNoUpdatesSelected;

    [ObservableProperty] private bool _isChecked = true;

    [ObservableProperty] private string _selectedAccountType;
    public ObservableCollection<string> AccountTypes { get; } =
    [
        "Standard", "Premium", "Business"
    ];

    public ObservableCollection<User> Users { get; } = new();
    
    public ICommand SaveCommand { get; }
    public ICommand LoadCommand { get; }
    public ICommand DeleteCommand { get; }

    public UserViewModel(UserService usersService)
    {
        _userService = usersService;
        /*_repository = new UserRepository("""
                                            Host=localhost;
                                            Port=5432;
                                            Username=js;
                                            Password=postgres;
                                            Database=users_db
                                            """);*/
        
        SaveCommand = new AsyncRelayCommand(SaveUser);
        LoadCommand = new AsyncRelayCommand(LoadUsers);
        DeleteCommand = new AsyncRelayCommand(DeleteUser, CanDeleteUser);
        
        //_repository.InitDB();
        _ = LoadUsers();

        SelectedAccountType = AccountTypes[0];
        IsChecked = true;
    }

    private bool CanDeleteUser() => SelectedUser != null;
  

    private async Task DeleteUser()
    {
        if (SelectedUser == null) return;
        
        //_repository.DeleteUserById(SelectedUser.Id);
        try
        {
            await _userService.DeleteUserAsync(SelectedUser.Id);
            Users.Remove(SelectedUser);
            SelectedUser = null;
            (DeleteCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
            Console.WriteLine("User deleted successfully");

        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"User not exists error : {ex.Message} ");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
     
    }

    private async Task LoadUsers()
    {
        try
        {
            var usersFromDb = await _userService.GetAllUsersAsync();
            Users.Clear();
            Users.AddRange(usersFromDb);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
       
    }

    private async Task SaveUser()
    {   
        var user = new User
             {
                 FirstName = FirstName.Trim(),
                 LastName = LastName.Trim(),
                 Email = Email.Trim(),
                 NotificationPreference = IsViaEmailSelected ? "Email" : 
                                          IsViaSmsSelected ? "SMS" : "None",
                 AccountType = SelectedAccountType,
                 isTermsAccepted = IsChecked
             };
        try
        {
            await _userService.AddUserAsync(user);
            Users.Add(user);
            ClearFields();
        }
        catch (PostgresException ex) when (ex.SqlState == "23505") // kod błędu dla UNIQUE
        {
            Console.WriteLine("Użytkownik o takim emailu już istnieje");
        }
        catch (PostgresException ex) when (ex.SqlState == "23502") // kod błędu dla null
        {
            Console.WriteLine("Wszystkie pola powinny być wypełnione");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        

        
    }

    private void ClearFields()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
        IsViaEmailSelected = false;
        IsViaSmsSelected = false;
        IsNoUpdatesSelected = false;
        SelectedAccountType = AccountTypes[0];
        IsChecked = true;
    }

    partial void OnSelectedUserChanged(User? user)
    {
        (DeleteCommand as RelayCommand)?.NotifyCanExecuteChanged();
        
    }
}


