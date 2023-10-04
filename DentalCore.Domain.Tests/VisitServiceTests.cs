using System.ComponentModel.DataAnnotations;
using DentalCore.Data;
using DentalCore.Data.Models;
using DentalCore.Domain.Dto;
using DentalCore.Domain.Exceptions;
using DentalCore.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace DentalCore.Domain.Tests;

public class VisitServiceTests
{
    [Fact]
    public void Add_ValidDtoAndPaymentIsZero_AddsVisitAndNoPaymentCreated()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Name = "Test City"
            };

            var procedure = new Procedure
            {
                Name = "Procedure 1",
                Price = 100,
                IsDiscountAllowed = true
            };

            var patient = new Patient
            {
                Gender = Gender.Male,
                Name = "Name",
                Surname = "Surname",
                Patronymic = "Patronymic",
                Phone = "0000000000",
                BirthDate = DateTime.Today,
                DateAdded = DateTime.Today,
                City = city
            };

            var doctor = new User
            {
                Role = UserRole.Doctor,
                IsDeleted = false,
                Login = "login",
                PasswordHash = "ds34-234ds-332",
                Name = "Doctor Name",
                Surname = "Doctor Surname",
                Phone = "01111111111"
            };

            context.Add(city);
            context.Add(procedure);
            context.Add(patient);
            context.Add(doctor);

            context.SaveChanges();
        }

        var testDto = GetDefaultCreateDto();
        var service = new VisitService(new AppDbContext(options));

        // act
        service.Add(testDto);

        // assert
        using (var context = new AppDbContext(options))
        {
            Assert.Equal(1, context.Visits.Count());
            Assert.Equal(0, context.Payments.Count());
        }
    }

    [Fact]
    public void Add_ValidDtoAndPaymentIsNotZero_AddsVisitAndPayment()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Name = "Test City"
            };

            var procedure = new Procedure
            {
                Name = "Procedure 1",
                Price = 100,
                IsDiscountAllowed = true
            };

            var patient = new Patient
            {
                Gender = Gender.Male,
                Name = "Name",
                Surname = "Surname",
                Patronymic = "Patronymic",
                Phone = "0000000000",
                BirthDate = DateTime.Today,
                DateAdded = DateTime.Today,
                City = city
            };

            var doctor = new User
            {
                Role = UserRole.Doctor,
                IsDeleted = false,
                Login = "login",
                PasswordHash = "ds34-234ds-332",
                Name = "Doctor Name",
                Surname = "Doctor Surname",
                Phone = "01111111111"
            };

            context.Add(city);
            context.Add(procedure);
            context.Add(patient);
            context.Add(doctor);

            context.SaveChanges();
        }

        var testDto = GetDefaultCreateDto();
        testDto.FirstPayment = 20;

        var service = new VisitService(new AppDbContext(options));

        // act
        service.Add(testDto);

        // assert
        using (var context = new AppDbContext(options))
        {
            var payment = context.Payments.Find(1);

            if (payment is null)
            {
                Assert.Fail("Payment was not created");
            }

            Assert.Equal(1, context.Visits.Count());
            Assert.Equal(1, context.Payments.Count());
            Assert.Equal(20, payment.Sum);
        }
    }

    [Theory]
    [InlineData(20, 4, 10, 50)]
    [InlineData(150, 3, 16, 400)]
    [InlineData(150, 3, 17, 350)]
    public void Add_DiscountIsAllowed_AddsVisitWithCorrectTotalPrice(
        int priceNoDiscount,
        int quantity,
        int discountPercent,
        int expectedTotal)
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Name = "Test City"
            };

            var procedure = new Procedure
            {
                Name = "Procedure 1",
                Price = priceNoDiscount,
                IsDiscountAllowed = true
            };

            var patient = new Patient
            {
                Gender = Gender.Male,
                Name = "Name",
                Surname = "Surname",
                Patronymic = "Patronymic",
                Phone = "0000000000",
                BirthDate = DateTime.Today,
                DateAdded = DateTime.Today,
                City = city
            };

            var doctor = new User
            {
                Role = UserRole.Doctor,
                IsDeleted = false,
                Login = "login",
                PasswordHash = "ds34-234ds-332",
                Name = "Doctor Name",
                Surname = "Doctor Surname",
                Phone = "01111111111"
            };

            context.Add(city);
            context.Add(procedure);
            context.Add(patient);
            context.Add(doctor);

            context.SaveChanges();
        }

        var testDto = GetDefaultCreateDto();

        testDto.DiscountPercent = discountPercent;
        testDto.TreatmentItems = new List<TreatmentItemDto>
        {
            new() { ProcedureId = 1, Quantity = quantity }
        };

        var service = new VisitService(new AppDbContext(options));

        // act
        service.Add(testDto);

        // assert
        using (var context = new AppDbContext(options))
        {
            Assert.Equal(expectedTotal, context.Visits.Find(1)?.TotalPrice);
        }
    }

    [Theory]
    [InlineData(20, 4, 10, 100)]
    [InlineData(150, 3, 16, 450)]
    [InlineData(150, 3, 17, 450)]
    public void Add_DiscountIsNotAllowed_AddsVisitWithCorrectTotalPrice(
        int priceNoDiscount,
        int quantity,
        int discountPercent,
        int expectedTotal)
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Name = "Test City"
            };

            var procedure = new Procedure
            {
                Name = "Procedure 1",
                Price = priceNoDiscount,
                IsDiscountAllowed = false
            };

            var patient = new Patient
            {
                Gender = Gender.Male,
                Name = "Name",
                Surname = "Surname",
                Patronymic = "Patronymic",
                Phone = "0000000000",
                BirthDate = DateTime.Today,
                DateAdded = DateTime.Today,
                City = city
            };

            var doctor = new User
            {
                Role = UserRole.Doctor,
                IsDeleted = false,
                Login = "login",
                PasswordHash = "ds34-234ds-332",
                Name = "Doctor Name",
                Surname = "Doctor Surname",
                Phone = "01111111111"
            };

            context.Add(city);
            context.Add(procedure);
            context.Add(patient);
            context.Add(doctor);

            context.SaveChanges();
        }

        var testDto = GetDefaultCreateDto();

        testDto.DiscountPercent = discountPercent;
        testDto.TreatmentItems = new List<TreatmentItemDto>
        {
            new() { ProcedureId = 1, Quantity = quantity }
        };

        var service = new VisitService(new AppDbContext(options));

        // act
        service.Add(testDto);

        // assert
        using (var context = new AppDbContext(options))
        {
            Assert.Equal(expectedTotal, context.Visits.Find(1)?.TotalPrice);
        }
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Add_DiscountOutOfBounds_ThrowsEntityNotFoundException(int invalidDiscount)
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Name = "Test City"
            };

            var procedure = new Procedure
            {
                Name = "Procedure 1",
                Price = 100,
                IsDiscountAllowed = true
            };

            var patient = new Patient
            {
                Gender = Gender.Male,
                Name = "Name",
                Surname = "Surname",
                Patronymic = "Patronymic",
                Phone = "0000000000",
                BirthDate = DateTime.Today,
                DateAdded = DateTime.Today,
                City = city
            };

            var doctor = new User
            {
                Role = UserRole.Doctor,
                IsDeleted = false,
                Login = "login",
                PasswordHash = "ds34-234ds-332",
                Name = "Doctor Name",
                Surname = "Doctor Surname",
                Phone = "01111111111"
            };

            context.Add(city);
            context.Add(procedure);
            context.Add(patient);
            context.Add(doctor);

            context.SaveChanges();
        }

        var testDto = GetDefaultCreateDto();
        testDto.DiscountPercent = invalidDiscount;

        var service = new VisitService(new AppDbContext(options));

        // act
        Action throwsEx = () => service.Add(testDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Fact]
    public void Add_PatientNotFound_ThrowsEntityNotFoundException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Name = "Test City"
            };

            var procedure = new Procedure
            {
                Name = "Procedure 1",
                Price = 100,
                IsDiscountAllowed = true
            };

            var doctor = new User
            {
                Role = UserRole.Doctor,
                IsDeleted = false,
                Login = "login",
                PasswordHash = "ds34-234ds-332",
                Name = "Doctor Name",
                Surname = "Doctor Surname",
                Phone = "01111111111"
            };

            context.Add(city);
            context.Add(procedure);
            context.Add(doctor);

            context.SaveChanges();
        }

        var testDto = GetDefaultCreateDto();
        testDto.PatientId = 12; // no such in DB

        var service = new VisitService(new AppDbContext(options));

        // act
        Action throwsEx = () => service.Add(testDto);

        // assert
        Assert.Throws<EntityNotFoundException>(throwsEx);
    }

    [Fact]
    public void Add_DoctorNotFound_ThrowsEntityNotFoundException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Name = "Test City"
            };

            var procedure = new Procedure
            {
                Name = "Procedure 1",
                Price = 100,
                IsDiscountAllowed = true
            };

            var patient = new Patient
            {
                Gender = Gender.Male,
                Name = "Name",
                Surname = "Surname",
                Patronymic = "Patronymic",
                Phone = "0000000000",
                BirthDate = DateTime.Today,
                DateAdded = DateTime.Today,
                City = city
            };

            context.Add(city);
            context.Add(patient);
            context.Add(procedure);

            context.SaveChanges();
        }

        var testDto = GetDefaultCreateDto();
        testDto.DoctorId = 12; // no such in DB

        var service = new VisitService(new AppDbContext(options));

        // act
        Action throwsEx = () => service.Add(testDto);

        // assert
        Assert.Throws<EntityNotFoundException>(throwsEx);
    }

    [Fact]
    public void Add_FirstPaymentLessThanZero_ThrowsEntityNotFoundException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Name = "Test City"
            };

            var procedure = new Procedure
            {
                Name = "Procedure 1",
                Price = 100,
                IsDiscountAllowed = true
            };

            var patient = new Patient
            {
                Gender = Gender.Male,
                Name = "Name",
                Surname = "Surname",
                Patronymic = "Patronymic",
                Phone = "0000000000",
                BirthDate = DateTime.Today,
                DateAdded = DateTime.Today,
                City = city
            };

            var doctor = new User
            {
                Role = UserRole.Doctor,
                IsDeleted = false,
                Login = "login",
                PasswordHash = "ds34-234ds-332",
                Name = "Doctor Name",
                Surname = "Doctor Surname",
                Phone = "01111111111"
            };

            context.Add(city);
            context.Add(procedure);
            context.Add(patient);
            context.Add(doctor);

            context.SaveChanges();
        }

        var testDto = GetDefaultCreateDto();
        testDto.FirstPayment = -1;

        var service = new VisitService(new AppDbContext(options));

        // act
        Action throwsEx = () => service.Add(testDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Fact]
    public void Add_FirstPaymentIsGreaterThanTotalSum_ThrowsValidationException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var shouldHavePayed = 100;
        var actuallyPayed = 120;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Name = "Test City"
            };

            var procedure = new Procedure
            {
                Name = "Procedure 1",
                Price = shouldHavePayed,
                IsDiscountAllowed = true
            };

            var patient = new Patient
            {
                Gender = Gender.Male,
                Name = "Name",
                Surname = "Surname",
                Patronymic = "Patronymic",
                Phone = "0000000000",
                BirthDate = DateTime.Today,
                DateAdded = DateTime.Today,
                City = city
            };

            var doctor = new User
            {
                Role = UserRole.Doctor,
                IsDeleted = false,
                Login = "login",
                PasswordHash = "ds34-234ds-332",
                Name = "Doctor Name",
                Surname = "Doctor Surname",
                Phone = "01111111111"
            };

            context.Add(city);
            context.Add(procedure);
            context.Add(patient);
            context.Add(doctor);

            context.SaveChanges();
        }

        var testDto = GetDefaultCreateDto();
        testDto.FirstPayment = actuallyPayed;

        var service = new VisitService(new AppDbContext(options));

        // act
        Action throwsEx = () => service.Add(testDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Fact]
    public void Add_DiagnosisHasInvalidLenght_ThrowsValidationException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Name = "Test City"
            };

            var procedure = new Procedure
            {
                Name = "Procedure 1",
                Price = 100,
                IsDiscountAllowed = true
            };

            var patient = new Patient
            {
                Gender = Gender.Male,
                Name = "Name",
                Surname = "Surname",
                Patronymic = "Patronymic",
                Phone = "0000000000",
                BirthDate = DateTime.Today,
                DateAdded = DateTime.Today,
                City = city
            };

            var doctor = new User
            {
                Role = UserRole.Doctor,
                IsDeleted = false,
                Login = "login",
                PasswordHash = "ds34-234ds-332",
                Name = "Doctor Name",
                Surname = "Doctor Surname",
                Phone = "01111111111"
            };

            context.Add(city);
            context.Add(procedure);
            context.Add(patient);
            context.Add(doctor);

            context.SaveChanges();
        }

        var testDto = GetDefaultCreateDto();
        testDto.Diagnosis = "123456789 123456789 123456789 123456789 123456789 " +
                            "123456789 123456789 123456789 123456789 123456789 A";

        var service = new VisitService(new AppDbContext(options));

        // act
        Action throwsEx = () => service.Add(testDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Fact]
    public void Add_TreatmentItemsIsNull_ThrowsValidationException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Name = "Test City"
            };

            var procedure = new Procedure
            {
                Name = "Procedure 1",
                Price = 100,
                IsDiscountAllowed = true
            };

            var patient = new Patient
            {
                Gender = Gender.Male,
                Name = "Name",
                Surname = "Surname",
                Patronymic = "Patronymic",
                Phone = "0000000000",
                BirthDate = DateTime.Today,
                DateAdded = DateTime.Today,
                City = city
            };

            var doctor = new User
            {
                Role = UserRole.Doctor,
                IsDeleted = false,
                Login = "login",
                PasswordHash = "ds34-234ds-332",
                Name = "Doctor Name",
                Surname = "Doctor Surname",
                Phone = "01111111111"
            };

            context.Add(city);
            context.Add(procedure);
            context.Add(patient);
            context.Add(doctor);

            context.SaveChanges();
        }

        var testDto = GetDefaultCreateDto();
        testDto.TreatmentItems = null;

        var service = new VisitService(new AppDbContext(options));

        // act
        Action throwsEx = () => service.Add(testDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Fact]
    public void Add_TreatmentItemsZeroItems_ThrowsValidationException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Name = "Test City"
            };

            var procedure = new Procedure
            {
                Name = "Procedure 1",
                Price = 100,
                IsDiscountAllowed = true
            };

            var patient = new Patient
            {
                Gender = Gender.Male,
                Name = "Name",
                Surname = "Surname",
                Patronymic = "Patronymic",
                Phone = "0000000000",
                BirthDate = DateTime.Today,
                DateAdded = DateTime.Today,
                City = city
            };

            var doctor = new User
            {
                Role = UserRole.Doctor,
                IsDeleted = false,
                Login = "login",
                PasswordHash = "ds34-234ds-332",
                Name = "Doctor Name",
                Surname = "Doctor Surname",
                Phone = "01111111111"
            };

            context.Add(city);
            context.Add(procedure);
            context.Add(patient);
            context.Add(doctor);

            context.SaveChanges();
        }

        var testDto = GetDefaultCreateDto();
        testDto.TreatmentItems = new List<TreatmentItemDto>();

        var service = new VisitService(new AppDbContext(options));

        // act
        Action throwsEx = () => service.Add(testDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Fact]
    public void Add_TreatmentItemsProcedureNotFound_ThrowsEntityNotFoundException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Name = "Test City"
            };

            var patient = new Patient
            {
                Gender = Gender.Male,
                Name = "Name",
                Surname = "Surname",
                Patronymic = "Patronymic",
                Phone = "0000000000",
                BirthDate = DateTime.Today,
                DateAdded = DateTime.Today,
                City = city
            };

            var doctor = new User
            {
                Role = UserRole.Doctor,
                IsDeleted = false,
                Login = "login",
                PasswordHash = "ds34-234ds-332",
                Name = "Doctor Name",
                Surname = "Doctor Surname",
                Phone = "01111111111"
            };

            context.Add(city);
            context.Add(patient);
            context.Add(doctor);

            context.SaveChanges();
        }

        var testDto = GetDefaultCreateDto();
        testDto.TreatmentItems = new List<TreatmentItemDto>
        {
            new() { ProcedureId = 12, Quantity = 1 } // no such in DB
        };

        var service = new VisitService(new AppDbContext(options));

        // act
        Action throwsEx = () => service.Add(testDto);

        // assert
        Assert.Throws<EntityNotFoundException>(throwsEx);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Add_TreatmentItemWithInvalidQuantity_ThrowsValidationException(int invalidQuantity)
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Name = "Test City"
            };

            var procedure = new Procedure
            {
                Name = "Procedure 1",
                Price = 100,
                IsDiscountAllowed = true
            };

            var patient = new Patient
            {
                Gender = Gender.Male,
                Name = "Name",
                Surname = "Surname",
                Patronymic = "Patronymic",
                Phone = "0000000000",
                BirthDate = DateTime.Today,
                DateAdded = DateTime.Today,
                City = city
            };

            var doctor = new User
            {
                Role = UserRole.Doctor,
                IsDeleted = false,
                Login = "login",
                PasswordHash = "ds34-234ds-332",
                Name = "Doctor Name",
                Surname = "Doctor Surname",
                Phone = "01111111111"
            };

            context.Add(city);
            context.Add(procedure);
            context.Add(patient);
            context.Add(doctor);

            context.SaveChanges();
        }

        var testDto = GetDefaultCreateDto();
        testDto.TreatmentItems = new List<TreatmentItemDto>
        {
            new() { ProcedureId = 1, Quantity = invalidQuantity }
        };

        var service = new VisitService(new AppDbContext(options));

        // act
        Action throwsEx = () => service.Add(testDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }
    
    [Fact]
    public void Add_TreatmentItemsHaveDuplicates_DuplicatesAreMergedIntoOneItem()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Name = "Test City"
            };

            var procedure1 = new Procedure
            {
                Name = "Procedure",
                Price = 100,
                IsDiscountAllowed = true
            };
            
            var procedure2 = new Procedure
            {
                Name = "Procedure2",
                Price = 50,
                IsDiscountAllowed = true
            };

            var patient = new Patient
            {
                Gender = Gender.Male,
                Name = "Name",
                Surname = "Surname",
                Patronymic = "Patronymic",
                Phone = "0000000000",
                BirthDate = DateTime.Today,
                DateAdded = DateTime.Today,
                City = city
            };

            var doctor = new User
            {
                Role = UserRole.Doctor,
                IsDeleted = false,
                Login = "login",
                PasswordHash = "ds34-234ds-332",
                Name = "Doctor Name",
                Surname = "Doctor Surname",
                Phone = "01111111111"
            };

            context.Add(city);
            context.AddRange(procedure1, procedure2);
            context.Add(patient);
            context.Add(doctor);

            context.SaveChanges();
        }

        var testDto = GetDefaultCreateDto();
        testDto.TreatmentItems = new List<TreatmentItemDto>
        {
            new() { ProcedureId = 1, Quantity = 2 },
            new() { ProcedureId = 1, Quantity = 3 },
            new() { ProcedureId = 2, Quantity = 1 }
        };

        var service = new VisitService(new AppDbContext(options));

        // act
        service.Add(testDto);

        // assert
        using (var context = new AppDbContext(options))
        {
            Assert.Equal(1, context.Visits.Count());
            Assert.Equal(2, context.TreatmentItems.Count());
            Assert.Equal(5, context.TreatmentItems.Find(1, 1)?.Quantity);
        }
    }

    [Fact]
    public void AddPayment_ValidSum_AddsPayment()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var totalSum = 100;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Name = "Test City"
            };

            var procedure = new Procedure
            {
                Name = "Procedure 1",
                Price = totalSum,
                IsDiscountAllowed = true
            };

            var patient = new Patient
            {
                Gender = Gender.Male,
                Name = "Name",
                Surname = "Surname",
                Patronymic = "Patronymic",
                Phone = "0000000000",
                BirthDate = DateTime.Today,
                DateAdded = DateTime.Today,
                City = city
            };

            var doctor = new User
            {
                Role = UserRole.Doctor,
                IsDeleted = false,
                Login = "login",
                PasswordHash = "ds34-234ds-332",
                Name = "Doctor Name",
                Surname = "Doctor Surname",
                Phone = "01111111111"
            };

            var visit = new Visit
            {
                DiscountPercent = 0,
                TotalPrice = totalSum,
                DateAdded = DateTime.Today,
                Patient = patient,
                Doctor = doctor,
                TreatmentItems = new List<TreatmentItem>
                {
                    new() { ProcedureId = 1, Quantity = 1 }
                }
            };

            context.Add(procedure);
            context.Add(visit);
            context.SaveChanges();
        }

        var service = new VisitService(new AppDbContext(options));

        // act
        service.AddPayment(1, 20);

        // assert
        using (var context = new AppDbContext(options))
        {
            var payment = context.Payments.Find(1);

            if (payment is null)
            {
                Assert.Fail("Payment was not created");
            }

            Assert.Equal(20, payment.Sum);
        }
    }

    [Fact]
    public void AddPayment_SumLessThanZero_ThrowsArgumentOutOfRangeException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var totalSum = 100;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Name = "Test City"
            };

            var procedure = new Procedure
            {
                Name = "Procedure 1",
                Price = totalSum,
                IsDiscountAllowed = true
            };

            var patient = new Patient
            {
                Gender = Gender.Male,
                Name = "Name",
                Surname = "Surname",
                Patronymic = "Patronymic",
                Phone = "0000000000",
                BirthDate = DateTime.Today,
                DateAdded = DateTime.Today,
                City = city
            };

            var doctor = new User
            {
                Role = UserRole.Doctor,
                IsDeleted = false,
                Login = "login",
                PasswordHash = "ds34-234ds-332",
                Name = "Doctor Name",
                Surname = "Doctor Surname",
                Phone = "01111111111"
            };

            var visit = new Visit
            {
                DiscountPercent = 0,
                TotalPrice = totalSum,
                DateAdded = DateTime.Today,
                Patient = patient,
                Doctor = doctor,
                TreatmentItems = new List<TreatmentItem>
                {
                    new() { ProcedureId = 1, Quantity = 1 }
                }
            };

            context.Add(procedure);
            context.Add(visit);
            context.SaveChanges();
        }

        var service = new VisitService(new AppDbContext(options));

        // act
        var throwsEx = () => service.AddPayment(1, -20);

        // assert
        Assert.Throws<ArgumentOutOfRangeException>(throwsEx);
    }

    [Fact]
    public void AddPayment_SumIsOverpayment_ThrowsArgumentOutOfRangeException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var totalSum = 100;
        var alreadyPayed = 40;
        var newPayment = 61;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Name = "Test City"
            };

            var procedure = new Procedure
            {
                Name = "Procedure 1",
                Price = totalSum,
                IsDiscountAllowed = true
            };

            var patient = new Patient
            {
                Gender = Gender.Male,
                Name = "Name",
                Surname = "Surname",
                Patronymic = "Patronymic",
                Phone = "0000000000",
                BirthDate = DateTime.Today,
                DateAdded = DateTime.Today,
                City = city
            };

            var doctor = new User
            {
                Role = UserRole.Doctor,
                IsDeleted = false,
                Login = "login",
                PasswordHash = "ds34-234ds-332",
                Name = "Doctor Name",
                Surname = "Doctor Surname",
                Phone = "01111111111"
            };

            var visit = new Visit
            {
                DiscountPercent = 0,
                TotalPrice = totalSum,
                DateAdded = DateTime.Today,
                Patient = patient,
                Doctor = doctor,
                Payments = new List<Payment>(),
                TreatmentItems = new List<TreatmentItem>
                {
                    new() { ProcedureId = 1, Quantity = 1 }
                }
            };

            var payment = new Payment
            {
                DateTime = DateTime.Today,
                Sum = alreadyPayed,
                Visit = visit
            };

            visit.Payments.Add(payment);

            context.Add(procedure);
            context.Add(visit);
            context.SaveChanges();
        }

        var service = new VisitService(new AppDbContext(options));

        // act
        var throwsEx = () => service.AddPayment(1, newPayment);

        // assert
        Assert.Throws<ArgumentOutOfRangeException>(throwsEx);
    }

    [Fact]
    public void AddPayment_VisitNotFound_ThrowsEntityNotFoundException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var service = new VisitService(new AppDbContext(options));

        // act
        var throwsEx = () => service.AddPayment(12, 1);

        // assert
        Assert.Throws<EntityNotFoundException>(throwsEx);
    }

    [Fact]
    public void GetDebt_HasDebt_ReturnsDebt()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var totalSum = 100;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Name = "Test City"
            };

            var procedure = new Procedure
            {
                Name = "Procedure 1",
                Price = totalSum,
                IsDiscountAllowed = true
            };

            var patient = new Patient
            {
                Gender = Gender.Male,
                Name = "Name",
                Surname = "Surname",
                Patronymic = "Patronymic",
                Phone = "0000000000",
                BirthDate = DateTime.Today,
                DateAdded = DateTime.Today,
                City = city
            };

            var doctor = new User
            {
                Role = UserRole.Doctor,
                IsDeleted = false,
                Login = "login",
                PasswordHash = "ds34-234ds-332",
                Name = "Doctor Name",
                Surname = "Doctor Surname",
                Phone = "01111111111"
            };

            var visit = new Visit
            {
                DiscountPercent = 0,
                TotalPrice = totalSum,
                DateAdded = DateTime.Today,
                Patient = patient,
                Doctor = doctor,
                TreatmentItems = new List<TreatmentItem>
                {
                    new() { ProcedureId = 1, Quantity = 1 }
                }
            };

            var payment1 = new Payment
            {
                DateTime = DateTime.Today,
                Sum = 20,
                Visit = visit
            };

            var payment2 = new Payment
            {
                DateTime = DateTime.Today,
                Sum = 30,
                Visit = visit
            };

            context.Add(procedure);
            context.Add(visit);
            context.AddRange(payment1, payment2);
            context.SaveChanges();
        }

        var service = new VisitService(new AppDbContext(options));
        var expectedDebt = 50;

        // act
        var actualDebt = service.GetDebt(1);

        // assert
        Assert.Equal(expectedDebt, actualDebt);
    }

    [Fact]
    public void GetDebt_NoPaymentsYet_ReturnsTotalSum()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var totalSum = 100;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Name = "Test City"
            };

            var procedure = new Procedure
            {
                Name = "Procedure 1",
                Price = totalSum,
                IsDiscountAllowed = true
            };

            var patient = new Patient
            {
                Gender = Gender.Male,
                Name = "Name",
                Surname = "Surname",
                Patronymic = "Patronymic",
                Phone = "0000000000",
                BirthDate = DateTime.Today,
                DateAdded = DateTime.Today,
                City = city
            };

            var doctor = new User
            {
                Role = UserRole.Doctor,
                IsDeleted = false,
                Login = "login",
                PasswordHash = "ds34-234ds-332",
                Name = "Doctor Name",
                Surname = "Doctor Surname",
                Phone = "01111111111"
            };

            var visit = new Visit
            {
                DiscountPercent = 0,
                TotalPrice = totalSum,
                DateAdded = DateTime.Today,
                Patient = patient,
                Doctor = doctor,
                TreatmentItems = new List<TreatmentItem>
                {
                    new() { ProcedureId = 1, Quantity = 1 }
                }
            };

            context.Add(procedure);
            context.Add(visit);
            context.SaveChanges();
        }

        var service = new VisitService(new AppDbContext(options));

        // act
        var actualDebt = service.GetDebt(1);

        // assert
        Assert.Equal(totalSum, actualDebt);
    }

    [Fact]
    public void GetDebt_NoDebt_ReturnsZero()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Name = "Test City"
            };

            var procedure = new Procedure
            {
                Name = "Procedure 1",
                Price = 100,
                IsDiscountAllowed = true
            };

            var patient = new Patient
            {
                Gender = Gender.Male,
                Name = "Name",
                Surname = "Surname",
                Patronymic = "Patronymic",
                Phone = "0000000000",
                BirthDate = DateTime.Today,
                DateAdded = DateTime.Today,
                City = city
            };

            var doctor = new User
            {
                Role = UserRole.Doctor,
                IsDeleted = false,
                Login = "login",
                PasswordHash = "ds34-234ds-332",
                Name = "Doctor Name",
                Surname = "Doctor Surname",
                Phone = "01111111111"
            };

            var visit = new Visit
            {
                DiscountPercent = 0,
                TotalPrice = 100,
                DateAdded = DateTime.Today,
                Patient = patient,
                Doctor = doctor,
                TreatmentItems = new List<TreatmentItem>
                {
                    new() { ProcedureId = 1, Quantity = 1 }
                }
            };

            var payment = new Payment
            {
                DateTime = DateTime.Today,
                Sum = 100,
                Visit = visit
            };

            context.Add(procedure);
            context.Add(visit);
            context.Add(payment);
            context.SaveChanges();
        }

        var service = new VisitService(new AppDbContext(options));

        // act
        var actualDebt = service.GetDebt(1);

        // assert
        Assert.Equal(0, actualDebt);
    }

    [Fact]
    public void GetDebt_VisitNotFound_ThrowsEntityNotFoundException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var service = new VisitService(new AppDbContext(options));

        // act
        var throwsEx = () => service.AddPayment(12, 1);

        // assert
        Assert.Throws<EntityNotFoundException>(throwsEx);
    }

    [Fact]
    public void GetMoneyPayed_HasPayments_ReturnsSumOfPayments()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Name = "Test City"
            };

            var procedure = new Procedure
            {
                Name = "Procedure 1",
                Price = 100,
                IsDiscountAllowed = true
            };

            var patient = new Patient
            {
                Gender = Gender.Male,
                Name = "Name",
                Surname = "Surname",
                Patronymic = "Patronymic",
                Phone = "0000000000",
                BirthDate = DateTime.Today,
                DateAdded = DateTime.Today,
                City = city
            };

            var doctor = new User
            {
                Role = UserRole.Doctor,
                IsDeleted = false,
                Login = "login",
                PasswordHash = "ds34-234ds-332",
                Name = "Doctor Name",
                Surname = "Doctor Surname",
                Phone = "01111111111"
            };

            var visit = new Visit
            {
                DiscountPercent = 0,
                TotalPrice = 100,
                DateAdded = DateTime.Today,
                Patient = patient,
                Doctor = doctor,
                TreatmentItems = new List<TreatmentItem>
                {
                    new() { ProcedureId = 1, Quantity = 1 }
                }
            };

            var payment1 = new Payment
            {
                DateTime = DateTime.Today,
                Sum = 20,
                Visit = visit
            };

            var payment2 = new Payment
            {
                DateTime = DateTime.Today,
                Sum = 35,
                Visit = visit
            };

            context.Add(procedure);
            context.Add(visit);
            context.AddRange(payment1, payment2);
            context.SaveChanges();
        }

        var service = new VisitService(new AppDbContext(options));
        var expected = 55;

        // act
        var actual = service.GetMoneyPayed(1);

        // assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetMoneyPayed_VisitNotFound_ThrowsEntityNotFoundException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var service = new VisitService(new AppDbContext(options));

        // act
        Action throwsEx = () => service.GetMoneyPayed(1);

        // assert
        Assert.Throws<EntityNotFoundException>(throwsEx);
    }

    [Fact]
    public void CalculateTotalPrice_DiscountIsZero_ReturnsSumMultipliedQuantity()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            context.Procedures.AddRange(
                new Procedure
                {
                    Name = "Procedure 1",
                    Price = 20,
                    IsDiscountAllowed = true
                },
                new Procedure
                {
                    Name = "Procedure 2",
                    Price = 30,
                    IsDiscountAllowed = true
                }
            );

            context.SaveChanges();
        }

        var service = new VisitService(new AppDbContext(options));

        var items = new List<TreatmentItemDto>()
        {
            new()
            {
                ProcedureId = 1,
                Quantity = 20
            },
            new()
            {
                ProcedureId = 2,
                Quantity = 3
            }
        };

        var expected = 500; // 20 * 20 + 30 * 3 = 490 => rounds to 500

        // act
        var actual = service.CalculateTotalPrice(items, 0);

        // assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(20, 4, 10, 50)]
    [InlineData(150, 3, 16, 400)]
    [InlineData(150, 3, 17, 350)]
    public void CalculateTotalPrice_DiscountIsAllowed_ReturnsSumWithDiscount(
        int priceNoDiscount,
        int quantity,
        int discountPercent,
        int expectedTotal)
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            context.Procedures.Add(new Procedure
            {
                Name = "Procedure",
                Price = priceNoDiscount,
                IsDiscountAllowed = true
            });

            context.SaveChanges();
        }

        var service = new VisitService(new AppDbContext(options));

        var items = new List<TreatmentItemDto>()
        {
            new()
            {
                ProcedureId = 1,
                Quantity = quantity
            }
        };

        // act
        var actualTotal = service.CalculateTotalPrice(items, discountPercent);

        // assert
        Assert.Equal(expectedTotal, actualTotal);
    }

    [Theory]
    [InlineData(20, 4, 10, 100)]
    [InlineData(150, 3, 16, 450)]
    [InlineData(150, 3, 17, 450)]
    public void CalculateTotalPrice_DiscountIsNotAllowed_ReturnsSumWithDiscount(
        int priceNoDiscount,
        int quantity,
        int discountPercent,
        int expectedTotal)
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            context.Procedures.Add(new Procedure
            {
                Name = "Procedure",
                Price = priceNoDiscount,
                IsDiscountAllowed = false
            });

            context.SaveChanges();
        }

        var service = new VisitService(new AppDbContext(options));

        var items = new List<TreatmentItemDto>()
        {
            new()
            {
                ProcedureId = 1,
                Quantity = quantity
            }
        };

        // act
        var actualTotal = service.CalculateTotalPrice(items, discountPercent);

        // assert
        Assert.Equal(expectedTotal, actualTotal);
    }
    
    [Fact]
    public void CalculateTotalSum_TreatmentItemsHaveDuplicates_ThrowsArgumentException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var procedure1 = new Procedure
            {
                Name = "Procedure",
                Price = 100,
                IsDiscountAllowed = true
            };
            
            var procedure2 = new Procedure
            {
                Name = "Procedure2",
                Price = 50,
                IsDiscountAllowed = true
            };

            context.AddRange(procedure1, procedure2);
            context.SaveChanges();
        }

        var service = new VisitService(new AppDbContext(options));

        var items = new List<TreatmentItemDto>()
        {
            new() { ProcedureId = 1, Quantity = 2 },
            new() { ProcedureId = 1, Quantity = 3 },
            new() { ProcedureId = 2, Quantity = 1 }
        };

        // act
        Action throwsEx = () => service.CalculateTotalPrice(items, 0);

        // assert
        Assert.Throws<ArgumentException>(throwsEx);
    }

    private VisitCreateDto GetDefaultCreateDto()
    {
        return new VisitCreateDto
        {
            PatientId = 1,
            DoctorId = 1,
            DiscountPercent = 0,
            FirstPayment = 0,
            Diagnosis = null,
            Date = DateTime.Today,
            TreatmentItems = new List<TreatmentItemDto>
            {
                new() { ProcedureId = 1, Quantity = 1 }
            }
        };
    }
}