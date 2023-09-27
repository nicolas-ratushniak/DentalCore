using DentalCore.Domain.Dto;
using DentalCore.Data.Models;

namespace DentalCore.Domain.Services;

public interface IUserService
{
    public User Get(int id);
    public User Get(string login);
    public bool CheckPassword(int id, string password);
    public User GetIncludeSoftDeleted(int id);
    public IEnumerable<User> GetAll();
    public IEnumerable<User> GetAllIncludeSoftDeleted();
    public void Add(UserCreateDto dto);
    public void Update(UserUpdateDto dto);
    public void SoftDelete(int id);
}