using System;
using DentalCore.Domain.Dto;

namespace DentalCore.Wpf.Services.Authentication;

public class UserChangedEventArgs : EventArgs
{
    public UserDto? NewUser { get; set; }

    public UserChangedEventArgs(UserDto? newUser)
    {
        NewUser = newUser;
    }
}