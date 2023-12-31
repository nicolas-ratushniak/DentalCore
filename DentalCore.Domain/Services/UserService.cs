﻿using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using DentalCore.Data;
using DentalCore.Domain.Dto;
using DentalCore.Data.Models;
using DentalCore.Domain.Abstract;
using DentalCore.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DentalCore.Domain.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly HashAlgorithm _hashAlgorithm;

    public UserService(AppDbContext context)
    {
        _context = context;
        _hashAlgorithm = MD5.Create();
    }

    public async Task<UserDto> GetAsync(int id)
    {
        return await _context.Users
                   .Where(u => !u.IsDeleted)
                   .Select(u => new UserDto
                   {
                       Id = u.Id,
                       Role = u.Role,
                       Login = u.Login,
                       Name = u.Name,
                       Surname = u.Surname,
                       Phone = u.Phone
                   })
                   .SingleOrDefaultAsync(u => u.Id == id)
               ?? throw new EntityNotFoundException();
    }

    public async Task<UserDto> GetAsync(string login)
    {
        return await _context.Users
                   .Where(u => !u.IsDeleted)
                   .Select(u => new UserDto
                   {
                       Id = u.Id,
                       Role = u.Role,
                       Login = u.Login,
                       Name = u.Name,
                       Surname = u.Surname,
                       Phone = u.Phone
                   })
                   .SingleOrDefaultAsync(u => u.Login == login)
               ?? throw new EntityNotFoundException();
    }

    public async Task<UserDto> GetIncludeSoftDeletedAsync(int id)
    {
        return await _context.Users
                   .Select(u => new UserDto
                   {
                       Id = u.Id,
                       Role = u.Role,
                       Login = u.Login,
                       Name = u.Name,
                       Surname = u.Surname,
                       Phone = u.Phone
                   })
                   .SingleOrDefaultAsync(u => u.Id == id)
               ?? throw new EntityNotFoundException();
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        return await _context.Users
            .Where(u => !u.IsDeleted)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Role = u.Role,
                Login = u.Login,
                Name = u.Name,
                Surname = u.Surname,
                Phone = u.Phone
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<UserDto>> GetAllIncludeSoftDeletedAsync()
    {
        return await _context.Users
            .Select(u => new UserDto
            {
                Id = u.Id,
                Role = u.Role,
                Login = u.Login,
                Name = u.Name,
                Surname = u.Surname,
                Phone = u.Phone
            }).ToListAsync();
    }

    public async Task<int> AddAsync(UserCreateDto dto)
    {
        var createdOn = DateTime.Now;

        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        if (await _context.Users.AnyAsync(u => u.Login == dto.Login))
        {
            throw new ValidationException("У базі вже є користувач з таким логіном");
        }

        if (await _context.Users.AnyAsync(u =>
                u.Name == dto.Name &&
                u.Surname == dto.Surname))
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
            Phone = dto.Phone,
            CreatedOn = createdOn,
            PasswordHash = GetPasswordHash(dto.Password)
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return user.Id;
    }

    public async Task UpdateAsync(UserUpdateDto dto)
    {
        var updatedOn = DateTime.Now;

        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        if (await _context.Users.AnyAsync(u =>
                u.Login == dto.Login &&
                u.Id != dto.Id))
        {
            throw new ValidationException("У базі вже є користувач з таким логіном");
        }

        if (await _context.Users.AnyAsync(u =>
                u.Name == dto.Name &&
                u.Surname == dto.Surname &&
                u.Id != dto.Id))
        {
            throw new ValidationException("У базі вже є користувач з таким ПІБ");
        }

        var user = await GetEntityAsync(dto.Id);

        user.Login = dto.Login;
        user.PasswordHash = GetPasswordHash(dto.Password);
        user.Name = dto.Name;
        user.Surname = dto.Surname;
        user.Phone = dto.Phone;
        user.UpdatedOn = updatedOn;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var deletedOn = DateTime.Now;
        var user = await GetEntityAsync(id);

        user.IsDeleted = true;
        user.DeletedOn = deletedOn;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> CheckPasswordAsync(int id, string password)
    {
        var user = await GetEntityAsync(id);
        return GetPasswordHash(password) == user.PasswordHash;
    }

    private string GetPasswordHash(string password)
    {
        var data = _hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(password));
        var sb = new StringBuilder();

        foreach (var dataByte in data)
        {
            sb.Append(dataByte.ToString("x2"));
        }

        return sb.ToString();
    }
    
    private async Task<User> GetEntityAsync(int id)
    {
        return await _context.Users
                   .SingleOrDefaultAsync(u => 
                       u.Id == id &&
                       !u.IsDeleted)
               ?? throw new EntityNotFoundException();
    }
}