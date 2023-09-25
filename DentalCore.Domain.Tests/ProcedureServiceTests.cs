using System.ComponentModel.DataAnnotations;
using DentalCore.Data;
using DentalCore.Data.Models;
using DentalCore.Domain.Dto;
using DentalCore.Domain.Exceptions;
using DentalCore.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace DentalCore.Domain.Tests;

public class ProcedureServiceTests
{
    [Fact]
    public void Add_ValidDto_AddsProcedure()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var testDto = GetDefaultCreateDto();

        var service = new ProcedureService(new AppDbContext(options));

        // act
        service.Add(testDto);

        // assert
        using (var context = new AppDbContext(options))
        {
            Assert.Equal(1, context.Procedures.Count());
        }
    }

    [Theory]
    [InlineData("A")]
    [InlineData("123456789 123456789 123456789 A")]
    public void Add_NameHasInvalidLenght_ThrowsValidationException(string invalidName)
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var testDto = GetDefaultCreateDto();
        testDto.Name = invalidName;

        var service = new ProcedureService(new AppDbContext(options));

        // act
        Action throwsEx = () => service.Add(testDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Add_PriceIsLessThan1_ThrowsValidationException(int invalidPrice)
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var testDto = GetDefaultCreateDto();
        testDto.Price = invalidPrice;

        var service = new ProcedureService(new AppDbContext(options));

        // act
        Action throwsEx = () => service.Add(testDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Fact]
    public void Add_NameDuplicate_ThrowsValidationException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var duplicateName = "Double Name";

        using (var context = new AppDbContext(options))
        {
            context.Procedures.Add(new Procedure
            {
                Name = duplicateName,
                Price = 10,
                IsDiscountValid = true
            });

            context.SaveChanges();
        }

        var testDto = GetDefaultCreateDto();
        testDto.Name = duplicateName;

        var service = new ProcedureService(new AppDbContext(options));

        // act
        Action throwsEx = () => service.Add(testDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Fact]
    public void Update_ValidDto_UpdatesProcedure()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            context.Procedures.Add(new Procedure
            {
                Name = "Name1",
                Price = 10,
                IsDiscountValid = true
            });

            context.SaveChanges();
        }

        var testDto = GetDefaultUpdateDto(1);

        var service = new ProcedureService(new AppDbContext(options));

        // act
        service.Update(testDto);

        // assert
        using (var context = new AppDbContext(options))
        {
            Assert.Equal(testDto.Name, context.Procedures.Find(1)?.Name);
        }
    }

    [Fact]
    public void Update_ProcedureNotFound_ThrowsEntityNotFoundException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var testDto = GetDefaultUpdateDto(1);
        var service = new ProcedureService(new AppDbContext(options));

        // act
        Action throwsEx = () => service.Update(testDto);

        // assert
        Assert.Throws<EntityNotFoundException>(throwsEx);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("123456789 123456789 123456789 A")]
    public void Update_NameHasInvalidLenght_ThrowsValidationException(string invalidName)
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            context.Procedures.Add(new Procedure
            {
                Name = "Name1",
                Price = 10,
                IsDiscountValid = true
            });

            context.SaveChanges();
        }

        var testDto = GetDefaultUpdateDto(1);
        testDto.Name = invalidName;

        var service = new ProcedureService(new AppDbContext(options));

        // act
        Action throwsEx = () => service.Update(testDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Fact]
    public void Update_NameDuplicate_ThrowsValidationException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var duplicateName = "Double Name";

        using (var context = new AppDbContext(options))
        {
            context.Procedures.AddRange(new Procedure
                {
                    Name = "Name1",
                    Price = 10,
                    IsDiscountValid = true
                },
                new Procedure
                {
                    Name = duplicateName,
                    Price = 10,
                    IsDiscountValid = true
                }
            );

            context.SaveChanges();
        }

        var testDto = GetDefaultUpdateDto(1);
        testDto.Name = duplicateName;

        var service = new ProcedureService(new AppDbContext(options));

        // act
        Action throwsEx = () => service.Update(testDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Fact]
    public void SoftDelete_ProcedureExists_ProcedureIsSoftDeleted()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            context.Procedures.Add(new Procedure
            {
                Name = "Name1",
                Price = 10,
                IsDiscountValid = true
            });

            context.SaveChanges();
        }

        var service = new ProcedureService(new AppDbContext(options));

        // act
        service.SoftDelete(1);

        // assert
        using (var context = new AppDbContext(options))
        {
            Assert.Equal(1, context.Procedures.Count(u => u.IsDeleted));
        }
    }

    [Fact]
    public void SoftDelete_ProcedureNotFound_ThrowsEntityNotFoundException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var service = new ProcedureService(new AppDbContext(options));

        // act
        Action throwsEx = () => service.SoftDelete(1);

        // assert
        Assert.Throws<EntityNotFoundException>(throwsEx);
    }

    [Fact]
    public void SoftDelete_ProcedureIsAlreadySoftDeleted_ThrowsEntityNotFoundException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            context.Procedures.Add(new Procedure
            {
                IsDeleted = true,
                Name = "Name1",
                Price = 10,
                IsDiscountValid = true
            });

            context.SaveChanges();
        }

        var service = new ProcedureService(new AppDbContext(options));

        // act
        Action throwsEx = () => service.SoftDelete(1);

        // assert
        Assert.Throws<EntityNotFoundException>(throwsEx);
    }

    private ProcedureCreateDto GetDefaultCreateDto()
    {
        return new ProcedureCreateDto
        {
            Name = "Name",
            Price = 1,
            IsDiscountValid = true
        };
    }

    private ProcedureUpdateDto GetDefaultUpdateDto(int id)
    {
        return new ProcedureUpdateDto
        {
            Id = id,
            Name = "New Name",
            Price = 2,
            IsDiscountValid = true
        };
    }
}