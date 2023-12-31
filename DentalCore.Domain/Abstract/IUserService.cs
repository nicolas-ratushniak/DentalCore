using DentalCore.Domain.Dto;

namespace DentalCore.Domain.Abstract;

public interface IUserService
{
    public Task<UserDto> GetAsync(int id);
    public Task<UserDto> GetAsync(string login);
    public Task<UserDto> GetIncludeSoftDeletedAsync(int id);
    public Task<IEnumerable<UserDto>> GetAllAsync();
    public Task<IEnumerable<UserDto>> GetAllIncludeSoftDeletedAsync();
    public Task<int> AddAsync(UserCreateDto dto);
    public Task UpdateAsync(UserUpdateDto dto);
    public Task SoftDeleteAsync(int id);
    public Task<bool> CheckPasswordAsync(int id, string password);
}