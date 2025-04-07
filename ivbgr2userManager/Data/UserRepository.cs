using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using ivbgr2userManager.Models;
using Npgsql;

namespace ivbgr2userManager.Data;

public class UserRepository
{
    private readonly string _connectionString;

    public UserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task InitDB()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync("""
                                      CREATE TABLE IF NOT EXISTS users(
                                       id SERIAL PRIMARY KEY,
                                       first_name TEXT NOT NULL,
                                       last_name TEXT NOT NULL,
                                       email TEXT NOT NULL UNIQUE,
                                       notification_preference VARCHAR(50),
                                       account_type VARCHAR(50),
                                       is_terms_accepted VARCHAR(50)
                                       )
                           """);
        Console.WriteLine("Database initialized");
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        var users = await connection.QueryAsync<User>("SELECT * FROM users");
        return users.AsList();
    }

    public async Task<int> AddUserAsync(User user)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.ExecuteAsync("""
                                  INSERT INTO users (first_name, last_name, email, notification_preference, account_type, is_terms_accepted)
                                  VALUES (@FirstName, @LastName, @Email, @NotificationPreference, @AccountType, @IsTermsAccepted)""
                                  """, user);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM users WHERE email = @Email", email);
    }
    public async Task<int> DeleteUserByIdAsync(int userId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.ExecuteAsync("DELETE FROM users WHERE id = @Id", new { Id = userId });
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM users WHERE id = @Id", new { Id = userId });
    }
    
}