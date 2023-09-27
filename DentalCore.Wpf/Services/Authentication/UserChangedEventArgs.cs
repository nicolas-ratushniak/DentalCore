using System;
using DentalCore.Data.Models;

namespace DentalCore.Wpf.Services.Authentication;

public class UserChangedEventArgs : EventArgs
{
    public User? NewUser { get; set; }

    public UserChangedEventArgs(User? newUser)
    {
        NewUser = newUser;
    }
}