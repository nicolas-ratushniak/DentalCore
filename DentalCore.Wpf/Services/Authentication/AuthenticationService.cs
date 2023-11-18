using System;
using System.Threading.Tasks;
using DentalCore.Data.Models;
using DentalCore.Domain.Abstract;
using DentalCore.Domain.Exceptions;
using DentalCore.Domain.Services;
using DentalCore.Wpf.Abstract;

namespace DentalCore.Wpf.Services.Authentication;

public class AuthenticationService : IAuthenticationService
{
    public event EventHandler<UserChangedEventArgs>? CurrentUserChanged;

    private readonly IUserService _userService;

    public User? CurrentUser { get; set; }
    public bool IsLoggedIn => CurrentUser is not null;

    public AuthenticationService(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<AuthenticationResult> LogInAsync(string login, string password)
    {
        try
        {
            var user = await _userService.GetAsync(login);

            if (!await _userService.CheckPasswordAsync(user.Id, password))
            {
                return AuthenticationResult.InvalidPassword;
            }

            CurrentUser = user;
            CurrentUserChanged?.Invoke(this, new UserChangedEventArgs(CurrentUser));

            return AuthenticationResult.Success;
        }
        catch (EntityNotFoundException)
        {
            return AuthenticationResult.UserNotFound;
        }
    }

    public void LogOut()
    {
        CurrentUser = null;
        CurrentUserChanged?.Invoke(this, new UserChangedEventArgs(CurrentUser));
    }
}

public enum AuthenticationResult
{
    Success,
    UserNotFound,
    InvalidPassword
}