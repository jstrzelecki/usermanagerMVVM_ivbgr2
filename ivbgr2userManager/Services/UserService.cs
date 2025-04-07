using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ivbgr2userManager.Data;
using ivbgr2userManager.Models;

namespace ivbgr2userManager.Services;

public class UserService
{
    private readonly UserRepository _userRepository;

    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task AddUserAsync(User user)
    {
        if( string.IsNullOrWhiteSpace(user.FirstName) ||
            string.IsNullOrWhiteSpace(user.LastName) ||
            string.IsNullOrWhiteSpace(user.Email) 
            )
        {
            throw new ArgumentException("First Name, Last Name, Email are required");
        }
        var existingUser = await _userRepository.GetUserByEmailAsync(user.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException($"User already exists (${user.Email})" );
        } ;

        try
        {
            int rowsAffected = await _userRepository.AddUserAsync(user);
            if (rowsAffected == 0)
            {
                throw new Exception("Could not add user");
            }
        }
        catch (Exception e)
        {
            throw new Exception("Could not add user", e);
        }
        
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var existingUser = _userRepository.GetUserByIdAsync(id);
        if (existingUser == null)
        {
            throw new KeyNotFoundException($"User with id {id} not found");
        }

        try
        {
            int rowsAffected = await _userRepository.DeleteUserByIdAsync(id);
            if (rowsAffected == 0)
            {
                throw new Exception("Could not delete user");
            }

            return true;
        }
        catch (Exception e)
        {
            throw new Exception($"Delete user with id {id} failed", e);
        }
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        try
        {
            return await _userRepository.GetAllUsersAsync();
        }
        catch (Exception e)
        {
            throw new Exception($"Get all users failed", e);
        }
    }
}