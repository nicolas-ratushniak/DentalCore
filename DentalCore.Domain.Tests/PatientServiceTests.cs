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
    [Fact]
    public void Add_ValidDto_AddsNewPatient()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            context.Cities.Add(new City
            {
                Id = 1,
                Name = "Test City"
            });

            context.SaveChanges();
        }

        var testDto = new PatientCreateDto
        {
            CityId = 1,
            IsMale = false,
            Name = "Name1",
            Surname = "Surname1",
            Patronymic = "Patronymic1",
            Phone = "0111111111",
            BirthDate = new DateTime(2001, 1, 1)
        };

        var service = new PatientService(new AppDbContext(options));

        // act
        service.Add(testDto);

        // assert
        using (var context = new AppDbContext(options))
        {
            Assert.True(context.Patients.Any(p => p.Id == 1));
            Assert.Equal("Patronymic1", context.Patients.Find(1)?.Patronymic);
        }
    }

    [Fact]
    public void Add_CityNotFound_ThrowsEntityNotFoundException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var invalidDto = new PatientCreateDto
        {
            CityId = 1, // no such city in DB
            IsMale = false,
            Name = "Name1",
            Surname = "Surname1",
            Patronymic = "Patronymic1",
            Phone = "0111111111",
            BirthDate = new DateTime(2001, 1, 1)
        };

        var service = new PatientService(new AppDbContext(options));

        // act
        var throwsEx = () => service.Add(invalidDto);

        // assert
        Assert.Throws<EntityNotFoundException>(throwsEx);
    }

    [Theory]
    [InlineData("0123")]
    [InlineData("01234567892345")]
    [InlineData("1111111111")]
    public void Add_InvalidPhone_ThrowsValidationException(string invalidPhone)
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            context.Cities.Add(new City
            {
                Id = 1,
                Name = "Test City"
            });

            context.SaveChanges();
        }

        var invalidDto = new PatientCreateDto
        {
            CityId = 1,
            IsMale = false,
            Name = "Name1",
            Surname = "Surname1",
            Patronymic = "Patronymic1",
            Phone = invalidPhone,
            BirthDate = new DateTime(2001, 1, 1)
        };

        var service = new PatientService(new AppDbContext(options));

        // act
        var throwsEx = () => service.Add(invalidDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("Too long naaaaaaaaame")]
    public void Add_InvalidNameLenght_ThrowsValidationException(string invalidName)
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            context.Cities.Add(new City
            {
                Id = 1,
                Name = "Test City"
            });

            context.SaveChanges();
        }

        var invalidDto = new PatientCreateDto
        {
            CityId = 1,
            IsMale = false,
            Name = invalidName,
            Surname = "Surname1",
            Patronymic = "Patronymic1",
            Phone = "0111111111",
            BirthDate = new DateTime(2001, 1, 1)
        };

        var service = new PatientService(new AppDbContext(options));

        // act
        var throwsEx = () => service.Add(invalidDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("Too long surnaaaaaaame")]
    public void Add_InvalidSurnameLenght_ThrowsValidationException(string invalidSurname)
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            context.Cities.Add(new City
            {
                Id = 1,
                Name = "Test City"
            });

            context.SaveChanges();
        }

        var invalidDto = new PatientCreateDto
        {
            CityId = 1,
            IsMale = false,
            Name = "Name1",
            Surname = invalidSurname,
            Patronymic = "Patronymic1",
            Phone = "0111111111",
            BirthDate = new DateTime(2001, 1, 1)
        };

        var service = new PatientService(new AppDbContext(options));

        // act
        var throwsEx = () => service.Add(invalidDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("Too long patronymiiic")]
    public void Add_InvalidPatronymicLenght_ThrowsValidationException(string invalidPatronymic)
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            context.Cities.Add(new City
            {
                Id = 1,
                Name = "Test City"
            });

            context.SaveChanges();
        }

        var invalidDto = new PatientCreateDto
        {
            CityId = 1,
            IsMale = false,
            Name = "Name1",
            Surname = "Surname1",
            Patronymic = invalidPatronymic,
            Phone = "0111111111",
            BirthDate = new DateTime(2001, 1, 1)
        };

        var service = new PatientService(new AppDbContext(options));

        // act
        var throwsEx = () => service.Add(invalidDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Fact]
    public void Add_BirthDateFromFuture_ThrowsValidationException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            context.Cities.Add(new City
            {
                Id = 1,
                Name = "Test City"
            });

            context.SaveChanges();
        }

        var invalidDto = new PatientCreateDto
        {
            CityId = 1,
            IsMale = false,
            Name = "Name1",
            Surname = "Surname1",
            Patronymic = "Patronymic1",
            Phone = "0111111111",
            BirthDate = DateTime.Now.AddDays(1)
        };

        var service = new PatientService(new AppDbContext(options));

        // act
        var throwsEx = () => service.Add(invalidDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Fact]
    public void Add_NameSurnamePatronymicDuplicate_ThrowsValidationException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Id = 1,
                Name = "Test City"
            };

            context.Cities.Add(city);

            context.Patients.Add(new Patient
            {
                CityId = 1,
                Gender = Gender.Male,
                Name = "Name1",
                Surname = "Surname1",
                Patronymic = "Patronymic1",
                Phone = "1111111111",
                BirthDate = DateTime.Today,
                DateCreated = DateTime.Today,
                City = city
            });

            context.SaveChanges();
        }

        var testDto = new PatientCreateDto
        {
            CityId = 1,
            IsMale = false,
            Name = "Name1",
            Surname = "Surname1",
            Patronymic = "Patronymic1",
            Phone = "2222222222",
            BirthDate = new DateTime(2001, 1, 1)
        };

        var service = new PatientService(new AppDbContext(options));

        // act
        var throwsEx = () => service.Add(testDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Fact]
    public void Update_ValidDto_UpdatesPatient()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Id = 1,
                Name = "Test City"
            };

            context.Cities.Add(city);

            context.Patients.Add(new Patient
            {
                CityId = 1,
                Gender = Gender.Male,
                Name = "Name1",
                Surname = "Surname1",
                Patronymic = "Patronymic1",
                Phone = "0111111111",
                BirthDate = DateTime.Today,
                DateCreated = DateTime.Today,
                City = city
            });

            context.SaveChanges();
        }

        var testDto = new PatientUpdateDto
        {
            Id = 1,
            CityId = 1,
            IsMale = false,
            Name = "Updated Name1",
            Surname = "Updated Surname1",
            Patronymic = "Updated Patronymic1",
            Phone = "0222222222",
            BirthDate = new DateTime(2001, 1, 1)
        };

        var service = new PatientService(new AppDbContext(options));

        // act
        service.Update(testDto);

        // assert
        using (var context = new AppDbContext(options))
        {
            var patient = context.Patients.Find(1);

            if (patient is null)
            {
                Assert.Fail("No patient found");
            }

            Assert.Equal("Updated Patronymic1", patient.Patronymic);
        }
    }

    [Fact]
    public void Update_PatientNotFound_ThrowsEntityNotFoundException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Id = 1,
                Name = "Test City"
            };

            context.Cities.Add(city);
            context.SaveChanges();
        }

        var invalidDto = new PatientUpdateDto()
        {
            Id = 12, // no such patient in db
            CityId = 1,
            IsMale = false,
            Name = "Name1",
            Surname = "Surname1",
            Patronymic = "Patronymic1",
            Phone = "0111111111",
            BirthDate = new DateTime(2001, 1, 1)
        };

        var service = new PatientService(new AppDbContext(options));

        // act
        var throwsEx = () => service.Update(invalidDto);

        // assert
        Assert.Throws<EntityNotFoundException>(throwsEx);
    }

    [Fact]
    public void Update_CityNotFound_ThrowsEntityNotFoundException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Id = 1,
                Name = "Test City"
            };

            context.Patients.Add(new Patient
            {
                CityId = 1,
                Gender = Gender.Male,
                Name = "Name1",
                Surname = "Surname1",
                Patronymic = "Patronymic1",
                Phone = "0111111111",
                BirthDate = DateTime.Today,
                DateCreated = DateTime.Today,
                City = city
            });

            context.SaveChanges();
        }

        var invalidDto = new PatientUpdateDto()
        {
            Id = 1,
            CityId = 12, // no such city in DB
            IsMale = false,
            Name = "Name1",
            Surname = "Surname1",
            Patronymic = "Patronymic1",
            Phone = "0111111111",
            BirthDate = new DateTime(2001, 1, 1)
        };

        var service = new PatientService(new AppDbContext(options));

        // act
        var throwsEx = () => service.Update(invalidDto);

        // assert
        Assert.Throws<EntityNotFoundException>(throwsEx);
    }

    [Theory]
    [InlineData("0123")] // too short
    [InlineData("01234567892345")] // too long
    [InlineData("1111111111")] // starts not with zero
    public void Update_InvalidPhone_ThrowsValidationException(string invalidPhone)
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Id = 1,
                Name = "Test City"
            };

            context.Patients.Add(new Patient
            {
                CityId = 1,
                Gender = Gender.Male,
                Name = "Name1",
                Surname = "Surname1",
                Patronymic = "Patronymic1",
                Phone = "0111111111",
                BirthDate = DateTime.Today,
                DateCreated = DateTime.Today,
                City = city
            });

            context.SaveChanges();
        }

        var invalidDto = new PatientUpdateDto
        {
            Id = 1,
            CityId = 1,
            IsMale = false,
            Name = "Name1",
            Surname = "Surname1",
            Patronymic = "Patronymic1",
            Phone = invalidPhone,
            BirthDate = new DateTime(2001, 1, 1)
        };

        var service = new PatientService(new AppDbContext(options));

        // act
        var throwsEx = () => service.Update(invalidDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("Too long naaaaaaaaame")]
    public void Update_InvalidNameLenght_ThrowsValidationException(string invalidName)
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Id = 1,
                Name = "Test City"
            };

            context.Patients.Add(new Patient
            {
                CityId = 1,
                Gender = Gender.Male,
                Name = "Name1",
                Surname = "Surname1",
                Patronymic = "Patronymic1",
                Phone = "0111111111",
                BirthDate = DateTime.Today,
                DateCreated = DateTime.Today,
                City = city
            });

            context.SaveChanges();
        }

        var invalidDto = new PatientUpdateDto
        {
            Id = 1,
            CityId = 1,
            IsMale = false,
            Name = invalidName,
            Surname = "Surname1",
            Patronymic = "Patronymic1",
            Phone = "0111111111",
            BirthDate = new DateTime(2001, 1, 1)
        };

        var service = new PatientService(new AppDbContext(options));

        // act
        var throwsEx = () => service.Update(invalidDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("Too long surnaaaaaaame")]
    public void Update_InvalidSurnameLenght_ThrowsValidationException(string invalidSurname)
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Id = 1,
                Name = "Test City"
            };

            context.Cities.Add(city);

            context.Patients.Add(new Patient
            {
                CityId = 1,
                Gender = Gender.Male,
                Name = "Name1",
                Surname = "Surname1",
                Patronymic = "Patronymic1",
                Phone = "0111111111",
                BirthDate = DateTime.Today,
                DateCreated = DateTime.Today,
                City = city
            });

            context.SaveChanges();
        }

        var invalidDto = new PatientUpdateDto
        {
            Id = 1,
            CityId = 1,
            IsMale = false,
            Name = "Name1",
            Surname = invalidSurname,
            Patronymic = "Patronymic1",
            Phone = "0111111111",
            BirthDate = new DateTime(2001, 1, 1)
        };

        var service = new PatientService(new AppDbContext(options));

        // act
        var throwsEx = () => service.Update(invalidDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("Too long patronymiiic")]
    public void Update_InvalidPatronymicLenght_ThrowsValidationException(string invalidPatronymic)
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Id = 1,
                Name = "Test City"
            };

            context.Cities.Add(city);

            context.Patients.Add(new Patient
            {
                CityId = 1,
                Gender = Gender.Male,
                Name = "Name1",
                Surname = "Surname1",
                Patronymic = "Patronymic1",
                Phone = "0111111111",
                BirthDate = DateTime.Today,
                DateCreated = DateTime.Today,
                City = city
            });

            context.SaveChanges();
        }

        var invalidDto = new PatientUpdateDto
        {
            Id = 1,
            CityId = 1,
            IsMale = false,
            Name = "Name1",
            Surname = "Surname1",
            Patronymic = invalidPatronymic,
            Phone = "0111111111",
            BirthDate = new DateTime(2001, 1, 1)
        };

        var service = new PatientService(new AppDbContext(options));

        // act
        var throwsEx = () => service.Update(invalidDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Fact]
    public void Update_BirthDateFromFuture_ThrowsValidationException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Id = 1,
                Name = "Test City"
            };

            context.Cities.Add(city);

            context.Patients.Add(new Patient
            {
                CityId = 1,
                Gender = Gender.Male,
                Name = "Name1",
                Surname = "Surname1",
                Patronymic = "Patronymic1",
                Phone = "0111111111",
                BirthDate = DateTime.Today,
                DateCreated = DateTime.Today,
                City = city
            });

            context.SaveChanges();
        }

        var invalidDto = new PatientUpdateDto
        {
            Id = 1,
            CityId = 1,
            IsMale = false,
            Name = "Name1",
            Surname = "Surname1",
            Patronymic = "Patronymic1",
            Phone = "0111111111",
            BirthDate = DateTime.Now.AddDays(1)
        };

        var service = new PatientService(new AppDbContext(options));

        // act
        var throwsEx = () => service.Update(invalidDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Fact]
    public void Update_NameSurnamePatronymicDuplicate_ThrowsValidationException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Id = 1,
                Name = "Test City"
            };

            context.Cities.Add(city);

            context.Patients.AddRange(
                new Patient
                {
                    CityId = 1,
                    Gender = Gender.Male,
                    Name = "Name1",
                    Surname = "Surname1",
                    Patronymic = "Patronymic1",
                    Phone = "0111111111",
                    BirthDate = DateTime.Today,
                    DateCreated = DateTime.Today,
                    City = city
                },
                new Patient
                {
                    CityId = 1,
                    Gender = Gender.Male,
                    Name = "Name2",
                    Surname = "Surname2",
                    Patronymic = "Patronymic2",
                    Phone = "0111111111",
                    BirthDate = DateTime.Today,
                    DateCreated = DateTime.Today,
                    City = city
                });

            context.SaveChanges();
        }

        var testDto = new PatientUpdateDto
        {
            Id = 2,
            CityId = 1,
            IsMale = false,
            Name = "Name1",
            Surname = "Surname1",
            Patronymic = "Patronymic1",
            Phone = "0222222222",
            BirthDate = new DateTime(2001, 1, 1)
        };

        var service = new PatientService(new AppDbContext(options));

        // act
        var throwsEx = () => service.Update(testDto);

        // assert
        Assert.Throws<ValidationException>(throwsEx);
    }

    [Fact]
    public void GetDebt_NoVisitsYet_ReturnsZero()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Id = 1,
                Name = "Test City"
            };

            context.Cities.Add(city);

            context.Patients.Add(
                new Patient
                {
                    CityId = 1,
                    Gender = Gender.Male,
                    Name = "Name1",
                    Surname = "Surname1",
                    Patronymic = "Patronymic1",
                    Phone = "0111111111",
                    BirthDate = DateTime.Today,
                    DateCreated = DateTime.Today,
                    City = city
                });

            context.SaveChanges();
        }

        var service = new PatientService(new AppDbContext(options));
        var expectedDebt = 0;

        // act
        var actualDebt = service.GetDebt(1);

        // assert
        Assert.Equal(expectedDebt, actualDebt);
    }

    [Fact]
    public void GetDebt_AllVisitsArePayed_ReturnsZero()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Id = 1,
                Name = "Test City"
            };

            var doctor = new User
            {
                Id = 0,
                Role = UserRole.Doctor,
                IsDeleted = false,
                Login = "login",
                PasswordHash = "sdfsdfk",
                Name = "Doctor Name",
                Surname = "Doctor Surname",
                Phone = "01111111111"
            };

            var patient = new Patient
            {
                CityId = 1,
                Gender = Gender.Male,
                Name = "Name1",
                Surname = "Surname1",
                Patronymic = "Patronymic1",
                Phone = "0111111111",
                BirthDate = DateTime.Today,
                DateCreated = DateTime.Today,
                City = city
            };

            var visit = new Visit
            {
                Id = 0,
                PatientId = 1,
                DoctorId = 1,
                DiscountPercent = 0,
                TotalPrice = 100,
                Diagnosis = "diagnosis",
                Date = DateTime.Today,
                Patient = patient,
                Doctor = doctor
            };

            var payment = new Payment
            {
                VisitId = 1,
                DateTime = DateTime.Today,
                Sum = 100,
                Visit = visit
            };

            context.Add(city);
            context.Add(doctor);
            context.Add(patient);
            context.Add(visit);
            context.Add(payment);

            context.SaveChanges();
        }

        var service = new PatientService(new AppDbContext(options));
        var expectedDebt = 0;

        // act
        var actualDebt = service.GetDebt(1);

        // assert
        Assert.Equal(expectedDebt, actualDebt);
    }

    [Fact]
    public void GetDebt_HasDebt_ReturnsDebt()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Id = 1,
                Name = "Test City"
            };

            var doctor = new User
            {
                Id = 0,
                Role = UserRole.Doctor,
                IsDeleted = false,
                Login = "login",
                PasswordHash = "sdfsdfk",
                Name = "Doctor Name",
                Surname = "Doctor Surname",
                Phone = "01111111111"
            };

            var patient = new Patient
            {
                CityId = 1,
                Gender = Gender.Male,
                Name = "Name1",
                Surname = "Surname1",
                Patronymic = "Patronymic1",
                Phone = "0111111111",
                BirthDate = DateTime.Today,
                DateCreated = DateTime.Today,
                City = city
            };

            var visit = new Visit
            {
                Id = 0,
                PatientId = 1,
                DoctorId = 1,
                DiscountPercent = 0,
                TotalPrice = 100,
                Diagnosis = "diagnosis",
                Date = DateTime.Today,
                Patient = patient,
                Doctor = doctor
            };

            var payment1 = new Payment
            {
                VisitId = 1,
                DateTime = DateTime.Today,
                Sum = 20,
                Visit = visit
            };

            var payment2 = new Payment
            {
                VisitId = 1,
                DateTime = DateTime.Today,
                Sum = 10,
                Visit = visit
            };

            context.Add(city);
            context.Add(doctor);
            context.Add(patient);
            context.Add(visit);
            context.AddRange(payment1, payment2);

            context.SaveChanges();
        }

        var service = new PatientService(new AppDbContext(options));
        var expectedDebt = 70;

        // act
        var actualDebt = service.GetDebt(1);

        // assert
        Assert.Equal(expectedDebt, actualDebt);
    }

    [Fact]
    public void GetDebt_PatientNotFound_ThrowsEntityNotFoundException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var service = new PatientService(new AppDbContext(options));

        // act
        Action throwsEx = () => service.GetDebt(1);

        // assert
        Assert.Throws<EntityNotFoundException>(throwsEx);
    }

    [Fact]
    public void PayWholeDebt_PatientHasVisitsWithDebt_PaymentForSuchVisitIsCreated()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Id = 1,
                Name = "Test City"
            };

            var doctor = new User
            {
                Id = 0,
                Role = UserRole.Doctor,
                IsDeleted = false,
                Login = "login",
                PasswordHash = "sdfsdfk",
                Name = "Doctor Name",
                Surname = "Doctor Surname",
                Phone = "01111111111"
            };

            var patient = new Patient
            {
                CityId = 1,
                Gender = Gender.Male,
                Name = "Name1",
                Surname = "Surname1",
                Patronymic = "Patronymic1",
                Phone = "0111111111",
                BirthDate = DateTime.Today,
                DateCreated = DateTime.Today,
                City = city
            };

            var visit1 = new Visit
            {
                Id = 0,
                PatientId = 1,
                DoctorId = 1,
                DiscountPercent = 0,
                TotalPrice = 100,
                Diagnosis = "diagnosis",
                Date = DateTime.Today,
                Patient = patient,
                Doctor = doctor
            };

            var visit2 = new Visit
            {
                Id = 0,
                PatientId = 1,
                DoctorId = 1,
                DiscountPercent = 0,
                TotalPrice = 100,
                Diagnosis = "diagnosis",
                Date = DateTime.Today,
                Patient = patient,
                Doctor = doctor
            };

            var payment1 = new Payment
            {
                VisitId = 1,
                DateTime = DateTime.Today,
                Sum = 20,
                Visit = visit1
            };
            
            var payment2 = new Payment
            {
                VisitId = 1,
                DateTime = DateTime.Today,
                Sum = 40,
                Visit = visit2
            };

            context.Add(city);
            context.Add(doctor);
            context.Add(patient);
            context.AddRange(visit1, visit2);
            context.AddRange(payment1, payment2);

            context.SaveChanges();
        }

        var service = new PatientService(new AppDbContext(options));

        // act
        service.PayWholeDebt(1);

        // assert
        using (var context = new AppDbContext(options))
        {
            Assert.Equal(4, context.Payments.Count());
            Assert.Equal(80, context.Payments.Find(3)?.Sum);
            Assert.Equal(60, context.Payments.Find(4)?.Sum);
            Assert.Equal(200, context.Payments.Sum(p => p.Sum));
        }
    }
    
    [Fact]
    public void PayWholeDebt_PatientHasNoDebt_NoPaymentIsCreated()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var context = new AppDbContext(options))
        {
            var city = new City
            {
                Id = 1,
                Name = "Test City"
            };

            var doctor = new User
            {
                Id = 0,
                Role = UserRole.Doctor,
                IsDeleted = false,
                Login = "login",
                PasswordHash = "sdfsdfk",
                Name = "Doctor Name",
                Surname = "Doctor Surname",
                Phone = "01111111111"
            };

            var patient = new Patient
            {
                CityId = 1,
                Gender = Gender.Male,
                Name = "Name1",
                Surname = "Surname1",
                Patronymic = "Patronymic1",
                Phone = "0111111111",
                BirthDate = DateTime.Today,
                DateCreated = DateTime.Today,
                City = city
            };

            var visit = new Visit
            {
                Id = 0,
                PatientId = 1,
                DoctorId = 1,
                DiscountPercent = 0,
                TotalPrice = 100,
                Diagnosis = "diagnosis",
                Date = DateTime.Today,
                Patient = patient,
                Doctor = doctor
            };

            var payment = new Payment
            {
                VisitId = 1,
                DateTime = DateTime.Today,
                Sum = 100,
                Visit = visit
            };

            context.Add(city);
            context.Add(doctor);
            context.Add(patient);
            context.Add(visit);
            context.Add(payment);

            context.SaveChanges();
        }

        var service = new PatientService(new AppDbContext(options));

        // act
        service.PayWholeDebt(1);

        // assert
        using (var context = new AppDbContext(options))
        {
            Assert.Equal(1, context.Payments.Count());
        }
    }
    
    [Fact]
    public void PayWholeDebt_PatientNotFound_ThrowsEntityNotFoundException()
    {
        // arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var service = new PatientService(new AppDbContext(options));

        // act
        Action throwsEx = () => service.PayWholeDebt(1);

        // assert
        Assert.Throws<EntityNotFoundException>(throwsEx);
    }
}