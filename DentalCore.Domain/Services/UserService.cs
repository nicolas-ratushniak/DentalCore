using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using DentalCore.Data;
using DentalCore.Domain.Dto;
using DentalCore.Data.Models;
using DentalCore.Domain.Exceptions;

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

    public User Get(int id)
    {
        return _context.Users
            .Where(u => !u.IsDeleted)
            .SingleOrDefault(u => u.Id == id) ?? throw new EntityNotFoundException();
    }

    public User Get(string login)
    {
        return _context.Users
            .Where(u => !u.IsDeleted)
            .SingleOrDefault(u => u.Login == login) ?? throw new EntityNotFoundException();
    }

    public bool CheckPassword(int id, string password)
    {
        var user = Get(id);
        return GetPasswordHash(password) == user.PasswordHash;
    }

    public User GetIncludeSoftDeleted(int id)
    {
        return _context.Users.Find(id)
               ?? throw new EntityNotFoundException();
    }

    public IEnumerable<User> GetAll()
    {
        return _context.Users.Where(u => !u.IsDeleted);
    }

    public IEnumerable<User> GetAllIncludeSoftDeleted()
    {
        return _context.Users.ToList();
    }

    public int Add(UserCreateDto dto)
    {
        var createdOn = DateTime.Now;

        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        if (_context.Users.Any(u => u.Login == dto.Login))
        {
            throw new ValidationException("У базі вже є користувач з таким логіном");
        }

        if (_context.Users.Any(u =>
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
            CreatedOn = createdOn
        };

        user.PasswordHash = GetPasswordHash(dto.Password);

        _context.Users.Add(user);
        _context.SaveChanges();

        return user.Id;
    }

    public void Update(UserUpdateDto dto)
    {
        var updatedOn = DateTime.Now;

        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        if (_context.Users.Any(u =>
                u.Login == dto.Login &&
                u.Id != dto.Id))
        {
            throw new ValidationException("У базі вже є користувач з таким логіном");
        }

        if (_context.Users.Any(u =>
                u.Name == dto.Name &&
                u.Surname == dto.Surname &&
                u.Id != dto.Id))
        {
            throw new ValidationException("У базі вже є користувач з таким ПІБ");
        }

        var user = Get(dto.Id);

        user.Login = dto.Login;
        user.PasswordHash = GetPasswordHash(dto.Password);
        user.Name = dto.Name;
        user.Surname = dto.Surname;
        user.Phone = dto.Phone;
        user.UpdatedOn = updatedOn;

        _context.Users.Update(user);
        _context.SaveChanges();
    }

    public void SoftDelete(int id)
    {
        var deletedOn = DateTime.Now;
        var user = Get(id);

        user.IsDeleted = true;
        user.DeletedOn = deletedOn;

        _context.Users.Update(user);
        _context.SaveChanges();
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
}