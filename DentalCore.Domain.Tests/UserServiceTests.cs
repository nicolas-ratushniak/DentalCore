using System.ComponentModel.DataAnnotations;
using DentalCore.Data;
using DentalCore.Data.Models;
using DentalCore.Domain.Dto;
using DentalCore.Domain.Exceptions;
using DentalCore.Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DentalCore.Domain.Tests;

public class UserServiceTests
{
    [Fact]
    public void Add_ValidDto_AddsUser()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var testDto = GetDefaultCreateDto();

        var service = new UserService(new AppDbContext(options), new PasswordHasher<User>());

        // act
        service.Add(testDto);

        // assert
        using (var context = new AppDbContext(options))
        {
            Assert.Equal(1, context.Users.Count());
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData("2startsWithDigit")]
    [InlineData("space space")]
    [InlineData("Specials&*(*^")]
    public void Add_LoginDoesntFollowRegex_ThrowsValidationException(string invalidLogin)
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var testDto = GetDefaultCreateDto();
        testDto.Login = invalidLogin;

        var service = new UserService(new AppDbContext(options), new PasswordHasher<User>());

        // act
        Action throwsEx = () => service.Add(testDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Theory]
    [InlineData("")]
    [InlineData("123456789 123456789 A")]
    public void Add_PasswordHasInvalidLenght_ThrowsValidationException(string invalidPassword)
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var testDto = GetDefaultCreateDto();
        testDto.Password = invalidPassword;

        var service = new UserService(new AppDbContext(options), new PasswordHasher<User>());

        // act
        Action throwsEx = () => service.Add(testDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("123456789 123456789 A")]
    public void Add_NameHasInvalidLenght_ThrowsValidationException(string invalidName)
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var testDto = GetDefaultCreateDto();
        testDto.Name = invalidName;

        var service = new UserService(new AppDbContext(options), new PasswordHasher<User>());

        // act
        Action throwsEx = () => service.Add(testDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("123456789 123456789 A")]
    public void Add_SurnameHasInvalidLenght_ThrowsValidationException(string invalidSurname)
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var testDto = GetDefaultCreateDto();
        testDto.Surname = invalidSurname;

        var service = new UserService(new AppDbContext(options), new PasswordHasher<User>());

        // act
        Action throwsEx = () => service.Add(testDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Theory]
    [InlineData("0123")]
    [InlineData("aTextPhone")]
    [InlineData("01234567892345")]
    [InlineData("1111111111")]
    public void Add_PhoneDoesntFollowRegex_ThrowsValidationException(string invalidPhone)
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var testDto = GetDefaultCreateDto();
        testDto.Phone = invalidPhone;

        var service = new UserService(new AppDbContext(options), new PasswordHasher<User>());

        // act
        Action throwsEx = () => service.Add(testDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Fact]
    public void Add_LoginDuplicate_ThrowsValidationException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var duplicateLogin = "Double_Login";

        using (var context = new AppDbContext(options))
        {
            context.Users.Add(new User
            {
                Role = UserRole.Admin,
                Login = duplicateLogin,
                PasswordHash = "123",
                Name = "some name",
                Surname = "some surname",
                Phone = "0123123123"
            });

            context.SaveChanges();
        }

        var testDto = GetDefaultCreateDto();
        testDto.Login = duplicateLogin;

        var service = new UserService(new AppDbContext(options), new PasswordHasher<User>());

        // act
        Action throwsEx = () => service.Add(testDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Fact]
    public void Add_NameSurnameDuplicate_ThrowsValidationException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var duplicateName = "Double Name";
        var duplicateSurname = "Double Surname";

        using (var context = new AppDbContext(options))
        {
            context.Users.Add(new User
            {
                Role = UserRole.Admin,
                Login = "some Login",
                PasswordHash = "123",
                Name = duplicateName,
                Surname = duplicateSurname,
                Phone = "0123123123"
            });

            context.SaveChanges();
        }

        var testDto = GetDefaultCreateDto();
        testDto.Name = duplicateName;
        testDto.Surname = duplicateSurname;

        var service = new UserService(new AppDbContext(options), new PasswordHasher<User>());

        // act
        Action throwsEx = () => service.Add(testDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Fact]
    public void Update_ValidDto_UpdatesUser()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            context.Users.Add(new User
            {
                Role = UserRole.Admin,
                Login = "Login",
                PasswordHash = "123",
                Name = "Name",
                Surname = "Surname",
                Phone = "0000000000"
            });

            context.SaveChanges();
        }

        var testDto = GetDefaultUpdateDto(1);

        var service = new UserService(new AppDbContext(options), new PasswordHasher<User>());

        // act
        service.Update(testDto);

        // assert
        using (var context = new AppDbContext(options))
        {
            Assert.Equal(testDto.Login, context.Users.Find(1)?.Login);
        }
    }

    [Fact]
    public void Update_UserNotFound_ThrowsEntityNotFoundException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var testDto = GetDefaultUpdateDto(1);

        var service = new UserService(new AppDbContext(options), new PasswordHasher<User>());

        // act
        Action throwsEx = () => service.Update(testDto);

        // assert
        Assert.Throws<EntityNotFoundException>(throwsEx);
    }

    [Theory]
    [InlineData("")]
    [InlineData("2startsWithDigit")]
    [InlineData("space space")]
    [InlineData("Specials&*(*^")]
    public void Update_LoginDoesntFollowRegex_ThrowsValidationException(string invalidLogin)
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            context.Users.Add(new User
            {
                Role = UserRole.Admin,
                Login = "Login",
                PasswordHash = "123",
                Name = "Name",
                Surname = "Surname",
                Phone = "0000000000"
            });

            context.SaveChanges();
        }

        var testDto = GetDefaultUpdateDto(1);
        testDto.Login = invalidLogin;

        var service = new UserService(new AppDbContext(options), new PasswordHasher<User>());

        // act
        Action throwsEx = () => service.Update(testDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Theory]
    [InlineData("")]
    [InlineData("123456789 123456789 A")]
    public void Update_PasswordHasInvalidLenght_ThrowsValidationException(string invalidPassword)
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            context.Users.Add(new User
            {
                Role = UserRole.Admin,
                Login = "Login",
                PasswordHash = "123",
                Name = "Name",
                Surname = "Surname",
                Phone = "0000000000"
            });

            context.SaveChanges();
        }

        var testDto = GetDefaultUpdateDto(1);
        testDto.Password = invalidPassword;

        var service = new UserService(new AppDbContext(options), new PasswordHasher<User>());

        // act
        Action throwsEx = () => service.Update(testDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("123456789 123456789 A")]
    public void Update_NameHasInvalidLenght_ThrowsValidationException(string invalidName)
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            context.Users.Add(new User
            {
                Role = UserRole.Admin,
                Login = "Login",
                PasswordHash = "123",
                Name = "Name",
                Surname = "Surname",
                Phone = "0000000000"
            });

            context.SaveChanges();
        }

        var testDto = GetDefaultUpdateDto(1);
        testDto.Name = invalidName;

        var service = new UserService(new AppDbContext(options), new PasswordHasher<User>());

        // act
        Action throwsEx = () => service.Update(testDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("123456789 123456789 A")]
    public void Update_SurnameHasInvalidLenght_ThrowsValidationException(string invalidSurname)
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            context.Users.Add(new User
            {
                Role = UserRole.Admin,
                Login = "Login",
                PasswordHash = "123",
                Name = "Name",
                Surname = "Surname",
                Phone = "0000000000"
            });

            context.SaveChanges();
        }

        var testDto = GetDefaultUpdateDto(1);
        testDto.Surname = invalidSurname;

        var service = new UserService(new AppDbContext(options), new PasswordHasher<User>());

        // act
        Action throwsEx = () => service.Update(testDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Theory]
    [InlineData("0123")]
    [InlineData("aTextPhone")]
    [InlineData("01234567892345")]
    [InlineData("1111111111")]
    public void Update_PhoneDoesntFollowRegex_ThrowsValidationException(string invalidPhone)
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            context.Users.Add(new User
            {
                Role = UserRole.Admin,
                Login = "Login",
                PasswordHash = "123",
                Name = "Name",
                Surname = "Surname",
                Phone = "0000000000"
            });

            context.SaveChanges();
        }

        var testDto = GetDefaultUpdateDto(1);
        testDto.Phone = invalidPhone;

        var service = new UserService(new AppDbContext(options), new PasswordHasher<User>());

        // act
        Action throwsEx = () => service.Update(testDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Fact]
    public void Update_LoginDuplicate_ThrowsValidationException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var duplicateLogin = "Double_Login";

        using (var context = new AppDbContext(options))
        {
            context.Users.AddRange(new User
                {
                    Role = UserRole.Admin,
                    Login = "some login",
                    PasswordHash = "123",
                    Name = "some name",
                    Surname = "some surname",
                    Phone = "0123123123"
                },
                new User
                {
                    Role = UserRole.Admin,
                    Login = duplicateLogin,
                    PasswordHash = "123",
                    Name = "Name2",
                    Surname = "Surname2",
                    Phone = "0123123123"
                }
            );

            context.SaveChanges();
        }

        var testDto = GetDefaultUpdateDto(1);
        testDto.Login = duplicateLogin;

        var service = new UserService(new AppDbContext(options), new PasswordHasher<User>());

        // act
        Action throwsEx = () => service.Update(testDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Fact]
    public void Update_NameSurnameDuplicate_ThrowsValidationException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var duplicateName = "Double Name";
        var duplicateSurname = "Double Surname";

        using (var context = new AppDbContext(options))
        {
            context.Users.AddRange(new User
                {
                    Role = UserRole.Admin,
                    Login = "some login",
                    PasswordHash = "123",
                    Name = "some name",
                    Surname = "some surname",
                    Phone = "0123123123"
                },
                new User
                {
                    Role = UserRole.Admin,
                    Login = "login2",
                    PasswordHash = "123",
                    Name = duplicateName,
                    Surname = duplicateSurname,
                    Phone = "0123123123"
                }
            );

            context.SaveChanges();
        }

        var testDto = GetDefaultUpdateDto(1);
        testDto.Name = duplicateName;
        testDto.Surname = duplicateSurname;

        var service = new UserService(new AppDbContext(options), new PasswordHasher<User>());

        // act
        Action throwsEx = () => service.Update(testDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Fact]
    public void SoftDelete_UserExists_UserIsSoftDeleted()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            context.Users.Add(new User
            {
                Role = UserRole.Admin,
                Login = "Login",
                PasswordHash = "123",
                Name = "Name",
                Surname = "Surname",
                Phone = "0000000000"
            });

            context.SaveChanges();
        }

        var service = new UserService(new AppDbContext(options), new PasswordHasher<User>());

        // act
        service.SoftDelete(1);

        // assert
        using (var context = new AppDbContext(options))
        {
            Assert.Equal(1, context.Users.Count(u => u.IsDeleted));
        }
    }

    [Fact]
    public void SoftDelete_UserNotFound_ThrowsEntityNotFoundException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var service = new UserService(new AppDbContext(options), new PasswordHasher<User>());

        // act
        Action throwsEx = () => service.SoftDelete(1);

        // assert
        Assert.Throws<EntityNotFoundException>(throwsEx);
    }

    [Fact]
    public void SoftDelete_UserIsAlreadySoftDeleted_ThrowsEntityNotFoundException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            context.Users.Add(new User
            {
                IsDeleted = true,
                Role = UserRole.Admin,
                Login = "Login",
                PasswordHash = "123",
                Name = "Name",
                Surname = "Surname",
                Phone = "0000000000"
            });

            context.SaveChanges();
        }

        var service = new UserService(new AppDbContext(options), new PasswordHasher<User>());

        // act
        Action throwsEx = () => service.SoftDelete(1);

        // assert
        Assert.Throws<EntityNotFoundException>(throwsEx);
    }

    private UserCreateDto GetDefaultCreateDto()
    {
        return new UserCreateDto
        {
            Role = UserRole.Doctor,
            Login = "Login",
            Password = "Pass",
            Name = "Name",
            Surname = "Surname",
            Phone = "0000000000"
        };
    }

    private UserUpdateDto GetDefaultUpdateDto(int id)
    {
        return new UserUpdateDto
        {
            Id = id,
            Login = "Updated_Login",
            Password = "New Password",
            Name = "New Name",
            Surname = "New Surname",
            Phone = "0000000001"
        };
    }
}