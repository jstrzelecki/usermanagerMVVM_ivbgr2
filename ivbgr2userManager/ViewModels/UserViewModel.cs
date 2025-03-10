using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dapper;
using ivbgr2userManager.Models;
using Npgsql;

namespace ivbgr2userManager.ViewModels;

public partial class UserViewModel : ViewModelBase
{
    private readonly string  _connectionString = "Host=localhost;Port=5432;Username=js;Password=postgres;Database=users_db";

    [ObservableProperty] private string _firstName; //FirstName
    [ObservableProperty] private string _lastName; // LastName
    [ObservableProperty] private string _email; // Email 

    public ObservableCollection<User> Users { get; } = new();
    
    public ICommand SaveCommand { get; }
    public ICommand LoadCommand { get; }

    public UserViewModel()
    {
        SaveCommand = new RelayCommand(SaveUser);
        LoadCommand = new RelayCommand(LoadUsers);
        InitDb();
        
    }

    private void LoadUsers()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        var users = connection.Query<User>("SELECT * FROM users");
        Users.Clear();
        foreach (var user in users)
        {
            Users.Add(user);
        }
    }

    private void SaveUser()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        var user = new User
        {
            FirstName = FirstName,
            LastName = LastName,
            Email = Email
        };

        try
        {
            connection.Execute(
                @"INSERT INTO users (first_name, last_name, email) 
                   VALUES (@FirstName, @LastName, @Email)", user);
            Users.Add(user);
        }
        catch (PostgresException ex) when (ex.SqlState == "23505")
        {
            Console.WriteLine("Użytkownik o takim emailu już istnieje");
        }
        catch (PostgresException ex) when (ex.SqlState == "23502")
        {
            Console.WriteLine("Wszystkie pola powinny być wypełnione");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        LoadUsers();
    }

    private void InitDb()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Execute(@"CREATE TABLE IF NOT EXISTS users(id SERIAL PRIMARY KEY, 
                                        first_name TEXT NOT NULL, 
                                        last_name TEXT NOT NULL, 
                                        email TEXT NOT NULL UNIQUE)"
            );
    }
}


