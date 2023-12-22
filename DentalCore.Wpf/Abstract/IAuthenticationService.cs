using System;
using System.Threading.Tasks;
using DentalCore.Domain.Dto;
using DentalCore.Wpf.Services.Authentication;

namespace DentalCore.Wpf.Abstract;

public interface IAuthenticationService
{
    public event EventHandler<UserChangedEventArgs> CurrentUserChanged;
    public UserDto? CurrentUser { get; set; }
    public bool IsLoggedIn { get; }
    public Task<AuthenticationResult> LogInAsync(string login, string password);
    public void LogOut();
}