using System.ComponentModel.DataAnnotations;
using DentalCore.Data;
using DentalCore.Data.Models;
using DentalCore.Domain.Dto;
using DentalCore.Domain.Exceptions;
using DentalCore.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace DentalCore.Domain.Tests;

public class PatientServiceTests
{
    // [Fact]
    // public void Add_ValidDto_AddsNewPatient()
    // {
    //     // arrange
    //     var options = new DbContextOptionsBuilder<AppDbContext>()
    //         .UseInMemoryDatabase(Guid.NewGuid().ToString())
    //         .Options;
    //
    //     using (var context = new AppDbContext(options))
    //     {
    //         context.Cities.Add(new City
    //         {
    //             Name = "Test City"
    //         });
    //
    //         context.SaveChanges();
    //     }
    //
    //     var testDto = GetDefaultCreateDto();
    //     var service = new PatientService(new AppDbContext(options));
    //
    //     // act
    //     service.Add(testDto);
    //
    //     // assert
    //     using (var context = new AppDbContext(options))
    //     {
    //         Assert.True(context.Patients.Any(p => p.Id == 1));
    //     }
    // }
    //
    // [Fact]
    // public void Add_CityNotFound_ThrowsEntityNotFoundException()
    // {
    //     // arrange
    //     var options = new DbContextOptionsBuilder<AppDbContext>()
    //         .UseInMemoryDatabase(Guid.NewGuid().ToString())
    //         .Options;
    //
    //     var testDto = GetDefaultCreateDto();
    //     testDto.CityId = 12; // no such in DB
    //
    //     var service = new PatientService(new AppDbContext(options));
    //
    //     // act
    //     Action throwsEx = () => service.Add(testDto);
    //
    //     // assert
    //     Assert.Throws<EntityNotFoundException>(throwsEx);
    // }
    //
    // [Theory]
    // [InlineData("0123")]
    // [InlineData("aTextPhone")]
    // [InlineData("01234567892")]
    // [InlineData("1111111111")]
    // public void Add_PhoneDoesntFollowRegex_ThrowsValidationException(string invalidPhone)
    // {
    //     // arrange
    //     var options = new DbContextOptionsBuilder<AppDbContext>()
    //         .UseInMemoryDatabase(Guid.NewGuid().ToString())
    //         .Options;
    //
    //     using (var context = new AppDbContext(options))
    //     {
    //         context.Cities.Add(new City
    //         {
    //             Name = "Test City"
    //         });
    //
    //         context.SaveChanges();
    //     }
    //
    //     var testDto = GetDefaultCreateDto();
    //     testDto.Phone = invalidPhone;
    //
    //     var service = new PatientService(new AppDbContext(options));
    //
    //     // act
    //     Action throwsEx = () => service.Add(testDto);
    //
    //     // assert
    //     Assert.Throws<ValidationException>(throwsEx);
    // }
    //
    // [Theory]
    // [InlineData("A")]
    // [InlineData("123456789 123456789 123456789 A")]
    // public void Add_NameHasInvalidLenght_ThrowsValidationException(string invalidName)
    // {
    //     // arrange
    //     var options = new DbContextOptionsBuilder<AppDbContext>()
    //         .UseInMemoryDatabase(Guid.NewGuid().ToString())
    //         .Options;
    //
    //     using (var context = new AppDbContext(options))
    //     {
    //         context.Cities.Add(new City
    //         {
    //             Name = "Test City"
    //         });
    //
    //         context.SaveChanges();
    //     }
    //
    //     var testDto = GetDefaultCreateDto();
    //     testDto.Name = invalidName;
    //
    //     var service = new PatientService(new AppDbContext(options));
    //
    //     // act
    //     Action throwsEx = () => service.Add(testDto);
    //
    //     // assert
    //     Assert.Throws<ValidationException>(throwsEx);
    // }
    //
    // [Theory]
    // [InlineData("A")]
    // [InlineData("123456789 123456789 123456789 A")]
    // public void Add_SurnameHasInvalidLenght_ThrowsValidationException(string invalidSurname)
    // {
    //     // arrange
    //     var options = new DbContextOptionsBuilder<AppDbContext>()
    //         .UseInMemoryDatabase(Guid.NewGuid().ToString())
    //         .Options;
    //
    //     using (var context = new AppDbContext(options))
    //     {
    //         context.Cities.Add(new City
    //         {
    //             Name = "Test City"
    //         });
    //
    //         context.SaveChanges();
    //     }
    //
    //     var testDto = GetDefaultCreateDto();
    //     testDto.Surname = invalidSurname;
    //
    //     var service = new PatientService(new AppDbContext(options));
    //
    //     // act
    //     Action throwsEx = () => service.Add(testDto);
    //
    //     // assert
    //     Assert.Throws<ValidationException>(throwsEx);
    // }
    //
    // [Theory]
    // [InlineData("A")]
    // [InlineData("123456789 123456789 123456789 A")]
    // public void Add_PatronymicHasInvalidLenght_ThrowsValidationException(string invalidPatronymic)
    // {
    //     // arrange
    //     var options = new DbContextOptionsBuilder<AppDbContext>()
    //         .UseInMemoryDatabase(Guid.NewGuid().ToString())
    //         .Options;
    //
    //     using (var context = new AppDbContext(options))
    //     {
    //         context.Cities.Add(new City
    //         {
    //             Name = "Test City"
    //         });
    //
    //         context.SaveChanges();
    //     }
    //
    //     var testDto = GetDefaultCreateDto();
    //     testDto.Patronymic = invalidPatronymic;
    //
    //     var service = new PatientService(new AppDbContext(options));
    //
    //     // act
    //     Action throwsEx = () => service.Add(testDto);
    //
    //     // assert
    //     Assert.Throws<ValidationException>(throwsEx);
    // }
    //
    // [Fact]
    // public void Add_BirthDateFromFuture_ThrowsValidationException()
    // {
    //     // arrange
    //     var options = new DbContextOptionsBuilder<AppDbContext>()
    //         .UseInMemoryDatabase(Guid.NewGuid().ToString())
    //         .Options;
    //
    //     using (var context = new AppDbContext(options))
    //     {
    //         context.Cities.Add(new City
    //         {
    //             Name = "Test City"
    //         });
    //
    //         context.SaveChanges();
    //     }
    //
    //     var testDto = GetDefaultCreateDto();
    //     testDto.BirthDate = DateTime.Now.AddYears(1);
    //
    //     var service = new PatientService(new AppDbContext(options));
    //
    //     // act
    //     Action throwsEx = () => service.Add(testDto);
    //
    //     // assert
    //     Assert.Throws<ValidationException>(throwsEx);
    // }
    //
    // [Fact]
    // public void Add_NameSurnamePatronymicDuplicate_ThrowsValidationException()
    // {
    //     // arrange
    //     var options = new DbContextOptionsBuilder<AppDbContext>()
    //         .UseInMemoryDatabase(Guid.NewGuid().ToString())
    //         .Options;
    //
    //     var duplicateName = "Double Name";
    //     var duplicateSurname = "Double Surname";
    //     var duplicatePatronymic = "Double Patronymic";
    //
    //     using (var context = new AppDbContext(options))
    //     {
    //         var city = new City
    //         {
    //             Name = "Test City"
    //         };
    //
    //         context.Patients.Add(new Patient
    //         {
    //             Gender = Gender.Male,
    //             Name = duplicateName,
    //             Surname = duplicateSurname,
    //             Patronymic = duplicatePatronymic,
    //             Phone = "0111111111",
    //             BirthDate = DateTime.Today,
    //             CreatedOn = DateTime.Today,
    //             City = city
    //         });
    //
    //         context.SaveChanges();
    //     }
    //
    //     var testDto = GetDefaultCreateDto();
    //     testDto.Name = duplicateName;
    //     testDto.Surname = duplicateSurname;
    //     testDto.Patronymic = duplicatePatronymic;
    //
    //     var service = new PatientService(new AppDbContext(options));
    //
    //     // act
    //     Action throwsEx = () => service.Add(testDto);
    //
    //     // assert
    //     Assert.Throws<ValidationException>(throwsEx);
    // }
    //
    // [Fact]
    // public void Update_ValidDto_UpdatesPatient()
    // {
    //     // arrange
    //     var options = new DbContextOptionsBuilder<AppDbContext>()
    //         .UseInMemoryDatabase(Guid.NewGuid().ToString())
    //         .Options;
    //
    //     using (var context = new AppDbContext(options))
    //     {
    //         var city = new City
    //         {
    //             Name = "Test City"
    //         };
    //
    //         context.Patients.Add(new Patient
    //         {
    //             Gender = Gender.Male,
    //             Name = "Name1",
    //             Surname = "Surname1",
    //             Patronymic = "Patronymic1",
    //             Phone = "0111111111",
    //             BirthDate = DateTime.Today,
    //             CreatedOn = DateTime.Today,
    //             City = city
    //         });
    //
    //         context.SaveChanges();
    //     }
    //
    //     var testDto = GetDefaultUpdateDto(1);
    //
    //     var service = new PatientService(new AppDbContext(options));
    //
    //     // act
    //     service.Update(testDto);
    //
    //     // assert
    //     using (var context = new AppDbContext(options))
    //     {
    //         var patient = context.Patients.Find(1);
    //
    //         if (patient is null)
    //         {
    //             Assert.Fail("No patient found");
    //         }
    //
    //         Assert.Equal(testDto.Name, patient.Name);
    //         Assert.Equal(testDto.Surname, patient.Surname);
    //     }
    // }
    //
    // [Fact]
    // public void Update_PatientNotFound_ThrowsEntityNotFoundException()
    // {
    //     // arrange
    //     var options = new DbContextOptionsBuilder<AppDbContext>()
    //         .UseInMemoryDatabase(Guid.NewGuid().ToString())
    //         .Options;
    //
    //     using (var context = new AppDbContext(options))
    //     {
    //         var city = new City
    //         {
    //             Name = "Test City"
    //         };
    //
    //         context.Cities.Add(city);
    //         context.SaveChanges();
    //     }
    //
    //     var testDto = GetDefaultUpdateDto(1);
    //     testDto.Id = 12; // no such in DB
    //
    //     var service = new PatientService(new AppDbContext(options));
    //
    //     // act
    //     var throwsEx = () => service.Update(testDto);
    //
    //     // assert
    //     Assert.Throws<EntityNotFoundException>(throwsEx);
    // }
    //
    // [Fact]
    // public void Update_CityNotFound_ThrowsEntityNotFoundException()
    // {
    //     // arrange
    //     var options = new DbContextOptionsBuilder<AppDbContext>()
    //         .UseInMemoryDatabase(Guid.NewGuid().ToString())
    //         .Options;
    //
    //     using (var context = new AppDbContext(options))
    //     {
    //         var city = new City
    //         {
    //             Name = "Test City"
    //         };
    //
    //         context.Patients.Add(new Patient
    //         {
    //             Gender = Gender.Male,
    //             Name = "Name1",
    //             Surname = "Surname1",
    //             Patronymic = "Patronymic1",
    //             Phone = "0111111111",
    //             BirthDate = DateTime.Today,
    //             CreatedOn = DateTime.Today,
    //             City = city
    //         });
    //
    //         context.SaveChanges();
    //     }
    //
    //     var testDto = GetDefaultUpdateDto(1);
    //     testDto.CityId = 12; // no such in DB
    //
    //     var service = new PatientService(new AppDbContext(options));
    //
    //     // act
    //     var throwsEx = () => service.Update(testDto);
    //
    //     // assert
    //     Assert.Throws<EntityNotFoundException>(throwsEx);
    // }
    //
    // [Theory]
    // [InlineData("0123")]
    // [InlineData("aTextPhone")]
    // [InlineData("01234567892345")]
    // [InlineData("1111111111")]
    // public void Update_PhoneDoesntFollowRegex_ThrowsValidationException(string invalidPhone)
    // {
    //     // arrange
    //     var options = new DbContextOptionsBuilder<AppDbContext>()
    //         .UseInMemoryDatabase(Guid.NewGuid().ToString())
    //         .Options;
    //
    //     using (var context = new AppDbContext(options))
    //     {
    //         var city = new City
    //         {
    //             Name = "Test City"
    //         };
    //
    //         context.Patients.Add(new Patient
    //         {
    //             Gender = Gender.Male,
    //             Name = "Name1",
    //             Surname = "Surname1",
    //             Patronymic = "Patronymic1",
    //             Phone = "0111111111",
    //             BirthDate = DateTime.Today,
    //             CreatedOn = DateTime.Today,
    //             City = city
    //         });
    //
    //         context.SaveChanges();
    //     }
    //
    //     var testDto = GetDefaultUpdateDto(1);
    //     testDto.Phone = invalidPhone;
    //
    //     var service = new PatientService(new AppDbContext(options));
    //
    //     // act
    //     var throwsEx = () => service.Update(testDto);
    //
    //     // assert
    //     Assert.Throws<ValidationException>(throwsEx);
    // }
    //
    // [Theory]
    // [InlineData("A")]
    // [InlineData("123456789 123456789 123456789 A")]
    // public void Update_NameHasInvalidLenght_ThrowsValidationException(string invalidName)
    // {
    //     // arrange
    //     var options = new DbContextOptionsBuilder<AppDbContext>()
    //         .UseInMemoryDatabase(Guid.NewGuid().ToString())
    //         .Options;
    //
    //     using (var context = new AppDbContext(options))
    //     {
    //         var city = new City
    //         {
    //             Name = "Test City"
    //         };
    //
    //         context.Patients.Add(new Patient
    //         {
    //             Gender = Gender.Male,
    //             Name = "Name1",
    //             Surname = "Surname1",
    //             Patronymic = "Patronymic1",
    //             Phone = "0111111111",
    //             BirthDate = DateTime.Today,
    //             CreatedOn = DateTime.Today,
    //             City = city
    //         });
    //
    //         context.SaveChanges();
    //     }
    //
    //     var testDto = GetDefaultUpdateDto(1);
    //     testDto.Name = invalidName;
    //
    //     var service = new PatientService(new AppDbContext(options));
    //
    //     // act
    //     var throwsEx = () => service.Update(testDto);
    //
    //     // assert
    //     Assert.Throws<ValidationException>(throwsEx);
    // }
    //
    // [Theory]
    // [InlineData("A")]
    // [InlineData("123456789 123456789 123456789 A")]
    // public void Update_SurnameHasInvalidLenght_ThrowsValidationException(string invalidSurname)
    // {
    //     // arrange
    //     var options = new DbContextOptionsBuilder<AppDbContext>()
    //         .UseInMemoryDatabase(Guid.NewGuid().ToString())
    //         .Options;
    //
    //     using (var context = new AppDbContext(options))
    //     {
    //         var city = new City
    //         {
    //             Name = "Test City"
    //         };
    //
    //         context.Patients.Add(new Patient
    //         {
    //             Gender = Gender.Male,
    //             Name = "Name1",
    //             Surname = "Surname1",
    //             Patronymic = "Patronymic1",
    //             Phone = "0111111111",
    //             BirthDate = DateTime.Today,
    //             CreatedOn = DateTime.Today,
    //             City = city
    //         });
    //
    //         context.SaveChanges();
    //     }
    //
    //     var testDto = GetDefaultUpdateDto(1);
    //     testDto.Surname = invalidSurname;
    //
    //     var service = new PatientService(new AppDbContext(options));
    //
    //     // act
    //     var throwsEx = () => service.Update(testDto);
    //
    //     // assert
    //     Assert.Throws<ValidationException>(throwsEx);
    // }
    //
    // [Theory]
    // [InlineData("A")]
    // [InlineData("123456789 123456789 123456789 A")]
    // public void Update_PatronymicHasInvalidLenght_ThrowsValidationException(string invalidPatronymic)
    // {
    //     // arrange
    //     var options = new DbContextOptionsBuilder<AppDbContext>()
    //         .UseInMemoryDatabase(Guid.NewGuid().ToString())
    //         .Options;
    //
    //     using (var context = new AppDbContext(options))
    //     {
    //         var city = new City
    //         {
    //             Name = "Test City"
    //         };
    //
    //         context.Patients.Add(new Patient
    //         {
    //             Gender = Gender.Male,
    //             Name = "Name1",
    //             Surname = "Surname1",
    //             Patronymic = "Patronymic1",
    //             Phone = "0111111111",
    //             BirthDate = DateTime.Today,
    //             CreatedOn = DateTime.Today,
    //             City = city
    //         });
    //
    //         context.SaveChanges();
    //     }
    //
    //     var testDto = GetDefaultUpdateDto(1);
    //     testDto.Patronymic = invalidPatronymic;
    //
    //     var service = new PatientService(new AppDbContext(options));
    //
    //     // act
    //     var throwsEx = () => service.Update(testDto);
    //
    //     // assert
    //     Assert.Throws<ValidationException>(throwsEx);
    // }
    //
    // [Fact]
    // public void Update_BirthDateFromFuture_ThrowsValidationException()
    // {
    //     // arrange
    //     var options = new DbContextOptionsBuilder<AppDbContext>()
    //         .UseInMemoryDatabase(Guid.NewGuid().ToString())
    //         .Options;
    //
    //     using (var context = new AppDbContext(options))
    //     {
    //         var city = new City
    //         {
    //             Name = "Test City"
    //         };
    //
    //         context.Patients.Add(new Patient
    //         {
    //             Gender = Gender.Male,
    //             Name = "Name1",
    //             Surname = "Surname1",
    //             Patronymic = "Patronymic1",
    //             Phone = "0111111111",
    //             BirthDate = DateTime.Today,
    //             CreatedOn = DateTime.Today,
    //             City = city
    //         });
    //
    //         context.SaveChanges();
    //     }
    //
    //
    //     var testDto = GetDefaultUpdateDto(1);
    //     testDto.BirthDate = DateTime.Now.AddYears(1);
    //
    //     var service = new PatientService(new AppDbContext(options));
    //
    //     // act
    //     var throwsEx = () => service.Update(testDto);
    //
    //     // assert
    //     Assert.Throws<ValidationException>(throwsEx);
    // }
    //
    // [Fact]
    // public void Update_NameSurnamePatronymicDuplicate_ThrowsValidationException()
    // {
    //     // arrange
    //     var options = new DbContextOptionsBuilder<AppDbContext>()
    //         .UseInMemoryDatabase(Guid.NewGuid().ToString())
    //         .Options;
    //
    //     var duplicateName = "Double Name";
    //     var duplicateSurname = "Double Surname";
    //     var duplicatePatronymic = "Double Patronymic";
    //
    //     using (var context = new AppDbContext(options))
    //     {
    //         var city = new City
    //         {
    //             Name = "Test City"
    //         };
    //
    //         context.Patients.AddRange(
    //             new Patient
    //             {
    //                 Gender = Gender.Male,
    //                 Name = "Name1",
    //                 Surname = "Surname1",
    //                 Patronymic = "Patronymic1",
    //                 Phone = "0111111111",
    //                 BirthDate = DateTime.Today,
    //                 CreatedOn = DateTime.Today,
    //                 City = city
    //             },
    //             new Patient
    //             {
    //                 Gender = Gender.Male,
    //                 Name = duplicateName,
    //                 Surname = duplicateSurname,
    //                 Patronymic = duplicatePatronymic,
    //                 Phone = "0111111111",
    //                 BirthDate = DateTime.Today,
    //                 CreatedOn = DateTime.Today,
    //                 City = city
    //             });
    //
    //         context.SaveChanges();
    //     }
    //
    //     var testDto = GetDefaultUpdateDto(1);
    //     testDto.Name = duplicateName;
    //     testDto.Surname = duplicateSurname;
    //     testDto.Patronymic = duplicatePatronymic;
    //
    //     var service = new PatientService(new AppDbContext(options));
    //
    //     // act
    //     var throwsEx = () => service.Update(testDto);
    //
    //     // assert
    //     Assert.Throws<ValidationException>(throwsEx);
    // }
    //
    // [Fact]
    // public void GetDebt_NoVisitsYet_ReturnsZero()
    // {
    //     // arrange
    //     var options = new DbContextOptionsBuilder<AppDbContext>()
    //         .UseInMemoryDatabase(Guid.NewGuid().ToString())
    //         .Options;
    //
    //     using (var context = new AppDbContext(options))
    //     {
    //         var city = new City
    //         {
    //             Name = "Test City"
    //         };
    //
    //         context.Patients.Add(
    //             new Patient
    //             {
    //                 Gender = Gender.Male,
    //                 Name = "Name1",
    //                 Surname = "Surname1",
    //                 Patronymic = "Patronymic1",
    //                 Phone = "0111111111",
    //                 BirthDate = DateTime.Today,
    //                 CreatedOn = DateTime.Today,
    //                 City = city
    //             });
    //
    //         context.SaveChanges();
    //     }
    //
    //     var service = new PatientService(new AppDbContext(options));
    //     var expectedDebt = 0;
    //
    //     // act
    //     var actualDebt = service.GetDebt(1);
    //
    //     // assert
    //     Assert.Equal(expectedDebt, actualDebt);
    // }
    //
    // [Fact]
    // public void GetDebt_AllVisitsArePayed_ReturnsZero()
    // {
    //     // arrange
    //     var options = new DbContextOptionsBuilder<AppDbContext>()
    //         .UseInMemoryDatabase(Guid.NewGuid().ToString())
    //         .Options;
    //
    //     using (var context = new AppDbContext(options))
    //     {
    //         var city = new City
    //         {
    //             Name = "Test City"
    //         };
    //
    //         var doctor = new User
    //         {
    //             Role = UserRole.Doctor,
    //             IsDeleted = false,
    //             Login = "login",
    //             PasswordHash = "sdfsdfk",
    //             Name = "Doctor Name",
    //             Surname = "Doctor Surname",
    //             Phone = "01111111111"
    //         };
    //
    //         var patient = new Patient
    //         {
    //             Gender = Gender.Male,
    //             Name = "Name1",
    //             Surname = "Surname1",
    //             Patronymic = "Patronymic1",
    //             Phone = "0111111111",
    //             BirthDate = DateTime.Today,
    //             CreatedOn = DateTime.Today,
    //             City = city
    //         };
    //
    //         var visit = new Visit
    //         {
    //             DiscountSum = 0,
    //             TotalPrice = 100,
    //             Diagnosis = "diagnosis",
    //             CreatedOn = DateTime.Today,
    //             Patient = patient,
    //             Doctor = doctor
    //         };
    //
    //         var payment = new Payment
    //         {
    //             VisitId = 1,
    //             CreatedOn = DateTime.Today,
    //             Sum = 100,
    //             Visit = visit
    //         };
    //
    //         context.Payments.Add(payment);
    //         context.SaveChanges();
    //     }
    //
    //     var service = new PatientService(new AppDbContext(options));
    //     var expectedDebt = 0;
    //
    //     // act
    //     var actualDebt = service.GetDebt(1);
    //
    //     // assert
    //     Assert.Equal(expectedDebt, actualDebt);
    // }
    //
    // [Fact]
    // public void GetDebt_HasDebt_ReturnsDebt()
    // {
    //     // arrange
    //     var options = new DbContextOptionsBuilder<AppDbContext>()
    //         .UseInMemoryDatabase(Guid.NewGuid().ToString())
    //         .Options;
    //
    //     using (var context = new AppDbContext(options))
    //     {
    //         var city = new City
    //         {
    //             Name = "Test City"
    //         };
    //
    //         var doctor = new User
    //         {
    //             Role = UserRole.Doctor,
    //             IsDeleted = false,
    //             Login = "login",
    //             PasswordHash = "sdfsdfk",
    //             Name = "Doctor Name",
    //             Surname = "Doctor Surname",
    //             Phone = "01111111111"
    //         };
    //
    //         var patient = new Patient
    //         {
    //             Gender = Gender.Male,
    //             Name = "Name1",
    //             Surname = "Surname1",
    //             Patronymic = "Patronymic1",
    //             Phone = "0111111111",
    //             BirthDate = DateTime.Today,
    //             CreatedOn = DateTime.Today,
    //             City = city
    //         };
    //
    //         var visit = new Visit
    //         {
    //             DiscountSum = 0,
    //             TotalPrice = 100,
    //             Diagnosis = "diagnosis",
    //             CreatedOn = DateTime.Today,
    //             Patient = patient,
    //             Doctor = doctor
    //         };
    //
    //         var payment1 = new Payment
    //         {
    //             CreatedOn = DateTime.Today,
    //             Sum = 20,
    //             Visit = visit
    //         };
    //
    //         var payment2 = new Payment
    //         {
    //             CreatedOn = DateTime.Today,
    //             Sum = 10,
    //             Visit = visit
    //         };
    //
    //         context.Payments.AddRange(payment1, payment2);
    //         context.SaveChanges();
    //     }
    //
    //     var service = new PatientService(new AppDbContext(options));
    //     var expectedDebt = 70;
    //
    //     // act
    //     var actualDebt = service.GetDebt(1);
    //
    //     // assert
    //     Assert.Equal(expectedDebt, actualDebt);
    // }
    //
    // [Fact]
    // public void GetDebt_PatientNotFound_ThrowsEntityNotFoundException()
    // {
    //     // arrange
    //     var options = new DbContextOptionsBuilder<AppDbContext>()
    //         .UseInMemoryDatabase(Guid.NewGuid().ToString())
    //         .Options;
    //
    //     var service = new PatientService(new AppDbContext(options));
    //
    //     // act
    //     Action throwsEx = () => service.GetDebt(1);
    //
    //     // assert
    //     Assert.Throws<EntityNotFoundException>(throwsEx);
    // }
    //
    // [Fact]
    // public void PayWholeDebt_PatientHasVisitsWithDebt_PaymentForSuchVisitIsCreated()
    // {
    //     // arrange
    //     var options = new DbContextOptionsBuilder<AppDbContext>()
    //         .UseInMemoryDatabase(Guid.NewGuid().ToString())
    //         .Options;
    //
    //     using (var context = new AppDbContext(options))
    //     {
    //         var city = new City
    //         {
    //             Name = "Test City"
    //         };
    //
    //         var doctor = new User
    //         {
    //             Role = UserRole.Doctor,
    //             IsDeleted = false,
    //             Login = "login",
    //             PasswordHash = "sdfsdfk",
    //             Name = "Doctor Name",
    //             Surname = "Doctor Surname",
    //             Phone = "01111111111"
    //         };
    //
    //         var patient = new Patient
    //         {
    //             Gender = Gender.Male,
    //             Name = "Name1",
    //             Surname = "Surname1",
    //             Patronymic = "Patronymic1",
    //             Phone = "0111111111",
    //             BirthDate = DateTime.Today,
    //             CreatedOn = DateTime.Today,
    //             City = city
    //         };
    //
    //         var visit1 = new Visit
    //         {
    //             DiscountSum = 0,
    //             TotalPrice = 100,
    //             Diagnosis = "diagnosis",
    //             CreatedOn = DateTime.Today,
    //             Patient = patient,
    //             Doctor = doctor
    //         };
    //
    //         var visit2 = new Visit
    //         {
    //             DiscountSum = 0,
    //             TotalPrice = 100,
    //             Diagnosis = "diagnosis",
    //             CreatedOn = DateTime.Today,
    //             Patient = patient,
    //             Doctor = doctor
    //         };
    //
    //         var payment1 = new Payment
    //         {
    //             CreatedOn = DateTime.Today,
    //             Sum = 20,
    //             Visit = visit1
    //         };
    //
    //         var payment2 = new Payment
    //         {
    //             CreatedOn = DateTime.Today,
    //             Sum = 40,
    //             Visit = visit2
    //         };
    //
    //         context.Payments.AddRange(payment1, payment2);
    //         context.SaveChanges();
    //     }
    //
    //     var service = new PatientService(new AppDbContext(options));
    //
    //     // act
    //     service.PayWholeDebt(1);
    //
    //     // assert
    //     using (var context = new AppDbContext(options))
    //     {
    //         Assert.Equal(4, context.Payments.Count());
    //         Assert.Equal(80, context.Payments.Find(3)?.Sum);
    //         Assert.Equal(60, context.Payments.Find(4)?.Sum);
    //         Assert.Equal(200, context.Payments.Sum(p => p.Sum));
    //     }
    // }
    //
    // [Fact]
    // public void PayWholeDebt_PatientHasNoDebt_NoPaymentIsCreated()
    // {
    //     // arrange
    //     var options = new DbContextOptionsBuilder<AppDbContext>()
    //         .UseInMemoryDatabase(Guid.NewGuid().ToString())
    //         .Options;
    //
    //     using (var context = new AppDbContext(options))
    //     {
    //         var city = new City
    //         {
    //             Name = "Test City"
    //         };
    //
    //         var doctor = new User
    //         {
    //             Role = UserRole.Doctor,
    //             IsDeleted = false,
    //             Login = "login",
    //             PasswordHash = "sdfsdfk",
    //             Name = "Doctor Name",
    //             Surname = "Doctor Surname",
    //             Phone = "01111111111"
    //         };
    //
    //         var patient = new Patient
    //         {
    //             Gender = Gender.Male,
    //             Name = "Name1",
    //             Surname = "Surname1",
    //             Patronymic = "Patronymic1",
    //             Phone = "0111111111",
    //             BirthDate = DateTime.Today,
    //             CreatedOn = DateTime.Today,
    //             City = city
    //         };
    //
    //         var visit = new Visit
    //         {
    //             DiscountSum = 0,
    //             TotalPrice = 100,
    //             Diagnosis = "diagnosis",
    //             CreatedOn = DateTime.Today,
    //             Patient = patient,
    //             Doctor = doctor
    //         };
    //
    //         var payment = new Payment
    //         {
    //             CreatedOn = DateTime.Today,
    //             Sum = 100,
    //             Visit = visit
    //         };
    //
    //         context.Payments.Add(payment);
    //
    //         context.SaveChanges();
    //     }
    //
    //     var service = new PatientService(new AppDbContext(options));
    //
    //     // act
    //     service.PayWholeDebt(1);
    //
    //     // assert
    //     using (var context = new AppDbContext(options))
    //     {
    //         Assert.Equal(1, context.Payments.Count());
    //     }
    // }
    //
    // [Fact]
    // public void PayWholeDebt_PatientNotFound_ThrowsEntityNotFoundException()
    // {
    //     // arrange
    //     var options = new DbContextOptionsBuilder<AppDbContext>()
    //         .UseInMemoryDatabase(Guid.NewGuid().ToString())
    //         .Options;
    //
    //     var service = new PatientService(new AppDbContext(options));
    //
    //     // act
    //     Action throwsEx = () => service.PayWholeDebt(1);
    //
    //     // assert
    //     Assert.Throws<EntityNotFoundException>(throwsEx);
    // }
    //
    // private PatientCreateDto GetDefaultCreateDto()
    // {
    //     return new PatientCreateDto
    //     {
    //         CityId = 1,
    //         Gender = Gender.Male,
    //         Name = "Name",
    //         Surname = "Surname",
    //         Patronymic = "Patronymic",
    //         Phone = "0000000000",
    //         BirthDate = new DateTime(2001, 1, 1)
    //     };
    // }
    //
    // private PatientUpdateDto GetDefaultUpdateDto(int id)
    // {
    //     return new PatientUpdateDto
    //     {
    //         Id = id,
    //         CityId = 1,
    //         Gender = Gender.Male,
    //         Name = "Updated Name",
    //         Surname = "Updated Surname",
    //         Patronymic = "Updated Patronymic",
    //         Phone = "0000000001",
    //         BirthDate = new DateTime(2001, 1, 1)
    //     };
    // }
}