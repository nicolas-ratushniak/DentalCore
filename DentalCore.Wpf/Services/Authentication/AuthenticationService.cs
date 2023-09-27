using System;
using DentalCore.Data.Models;
using DentalCore.Domain.Exceptions;
using DentalCore.Domain.Services;

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

    public AuthenticationResult LogIn(string login, string password)
    {
        try
        {
            var user = _userService.Get(login);

            if (!_userService.CheckPassword(user.Id, password))
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