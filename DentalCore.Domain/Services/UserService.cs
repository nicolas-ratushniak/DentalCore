﻿using System.ComponentModel.DataAnnotations;
using DentalCore.Data;
using DentalCore.Domain.Dto;
using DentalCore.Data.Models;
using DentalCore.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;

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

    public User Get(int id, bool includeSoftDeleted)
    {
        return includeSoftDeleted
            ? _context.Users.Find(id) ?? throw new EntityNotFoundException()
            : _context.Users
                .Where(u => !u.IsDeleted)
                .SingleOrDefault(u => u.Id == id) ?? throw new EntityNotFoundException();
    }

    public IEnumerable<User> GetAll(bool includeSoftDeleted)
    {
        return includeSoftDeleted
            ? _context.Users.ToList()
            : _context.Users.Where(u => !u.IsDeleted);
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
            Role = dto.Role,
            IsDeleted = false,
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

        var user = Get(dto.Id, false);

        user.Login = dto.Login;
        user.PasswordHash = _hasher.HashPassword(user, dto.Password);
        user.Name = dto.Name;
        user.Surname = dto.Surname;
        user.Phone = dto.Phone;

        _context.Users.Update(user);
        _context.SaveChanges();
    }

    public void SoftDelete(int id)
    {
        var user = Get(id, false);

        user.IsDeleted = true;

        _context.Users.Update(user);
        _context.SaveChanges();
    }
}