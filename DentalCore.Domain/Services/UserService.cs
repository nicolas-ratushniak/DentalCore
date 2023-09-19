﻿using System.ComponentModel.DataAnnotations;
using DentalCore.Data;
using DentalCore.Domain.Dto;
using DentalCore.Data.Models;
using DentalCore.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DentalCore.Domain.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _hasher;

    public UserService(AppDbContext context, IPasswordHasher<User> hasher)
    {
        _context = context;
        _hasher = hasher;
    }

    public User Get(int id)
    {
        return _context.Users.Find(id)
               ?? throw new EntityNotFoundException();
    }

    public IEnumerable<User> GetAll()
    {
        return _context.Users.ToList();
    }

    public IEnumerable<User> GetDoctors()
    {
        return _context.Users
            .Include(u => u.Role)
            .Where(u => u.Role.Name == "Doctor");
    }

    public void Add(UserCreateDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        if (_context.Users.Any(u => u.Login == dto.Login))
        {
            throw new ValidationException("У базі вже є користувач з таким логіном");
        }

        if (_context.Users.Any(u => u.Name == dto.Name && u.Surname == dto.Surname))
        {
            throw new ValidationException("У базі вже є користувач з таким ПІБ");
        }
        
        var user = new User
        {
            RoleId = dto.RoleId,
            IsEnabled = true,
            Login = dto.Login,
            Name = dto.Name,
            Surname = dto.Surname,
            Phone = dto.Phone
        };

        user.PasswordHash = _hasher.HashPassword(user, dto.Password);

        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public void Update(UserUpdateDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        if (_context.Users.Any(u => u.Login == dto.Login && u.Id != dto.Id))
        {
            throw new ValidationException("У базі вже є користувач з таким логіном");
        }

        if (_context.Users.Any(u => u.Name == dto.Name && u.Surname == dto.Surname && u.Id != dto.Id))
        {
            throw new ValidationException("У базі вже є користувач з таким ПІБ");
        }

        var user = Get(dto.Id);

        user.IsEnabled = dto.IsEnabled;
        user.Login = dto.Login;
        user.PasswordHash = _hasher.HashPassword(user, dto.Password);
        user.Name = dto.Name;
        user.Surname = dto.Surname;
        user.Phone = dto.Phone;

        _context.Users.Update(user);
        _context.SaveChanges();
    }
}