using System;
using System.Threading.Tasks;
using DentalCore.Data.Models;
using DentalCore.Wpf.Services.Authentication;

namespace DentalCore.Wpf.Abstract;

public interface IAuthenticationService
{
    public event EventHandler<UserChangedEventArgs> CurrentUserChanged;
    public User? CurrentUser { get; set; }
    public bool IsLoggedIn { get; }
    public Task<AuthenticationResult> LogInAsync(string login, string password);
    public void LogOut();
}