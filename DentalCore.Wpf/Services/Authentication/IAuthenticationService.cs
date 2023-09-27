using System;
using DentalCore.Data.Models;

namespace DentalCore.Wpf.Services.Authentication;

public interface IAuthenticationService
{
    public event EventHandler<UserChangedEventArgs> CurrentUserChanged;
    public User? CurrentUser { get; set; }
    public bool IsLoggedIn { get; }
    public AuthenticationResult LogIn(string login, string password);
    public void LogOut();
}