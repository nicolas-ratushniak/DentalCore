using DentalCore.Domain.Dto;
using DentalCore.Data.Models;

namespace DentalCore.Domain.Services;

public interface IUserService
{
    public User Get(int id);
    public IEnumerable<User> GetAll();
    public void Add(UserCreateDto dto);
    public void Update(UserUpdateDto dto);
    public void SoftDelete(int id);
}