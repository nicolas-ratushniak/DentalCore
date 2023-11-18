﻿using DentalCore.Data.Models;
using DentalCore.Domain.Dto;

namespace DentalCore.Domain.Abstract;

public interface IUserService
{
    public Task<User> GetAsync(int id);
    public Task<User> GetAsync(string login);
    public Task<User> GetIncludeSoftDeletedAsync(int id);
    public Task<IEnumerable<User>> GetAllAsync();
    public Task<IEnumerable<User>> GetAllIncludeSoftDeletedAsync();
    public Task<int> AddAsync(UserCreateDto dto);
    public Task UpdateAsync(UserUpdateDto dto);
    public Task SoftDeleteAsync(int id);
    public Task<bool> CheckPasswordAsync(int id, string password);
}