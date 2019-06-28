using LabII.DTOs;
using LabII.Models;
using LabII.Services;
using LabII.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System.Linq;

namespace Tests
{
    public class UsersServiceTests
    {
        private IOptions<AppSettings> config;

        [SetUp]
        public void Setup()
        {
            config = Options.Create(new AppSettings
            {
                Secret = "THIS IS USED TO SIGN AND VERIFY JWT TOKENS, REPLACE IT WITH YOUR OWN SECRET, IT CAN BE ANY STRING"
            });
        }


        //[Test]
        //public void InvalidRegisterShouldReturnErrorsCollection()
        //{
        //    var options = new DbContextOptionsBuilder<ExpensesDbContext>()
        //                 .UseInMemoryDatabase(databaseName: nameof(InvalidRegisterShouldReturnErrorsCollection))
        //                 .Options;

        //    using (var context = new ExpensesDbContext(options))
        //    {
        //        var validator = new RegisterValidator();
        //        var crValidator = new CreateValidator();
        //        var usersService = new UsersService(context, validator, crValidator, null, config);
        //        var added = new LabII.DTOs.RegisterPostModel
        //        {
        //            FirstName = "firstName1",
        //            LastName = "lastName1",
        //            Username = "test_userName1",
        //            Email = "first@yahoo.com",
        //            Password = "111"    //invalid password should invalidate register
        //        };

        //        var result = usersService.Register(added);

        //        Assert.IsNotNull(result);
        //        Assert.AreEqual(1, result.ErrorMessages.Count());
        //    }
        //}

        [Test]
        public void AuthenticateShouldLoginTheRegisteredUser()
        {
            var options = new DbContextOptionsBuilder<ExpensesDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(AuthenticateShouldLoginTheRegisteredUser))
              .Options;

            using (var context = new ExpensesDbContext(options))
            {
                var validator = new RegisterValidator();
                var validatorUser = new UserRoleValidator();
                var crValidator = new CreateValidator();
                var userUserRoleService = new UserUserRoleService(validatorUser, context);
                var usersService = new UsersService(context, validator, crValidator, userUserRoleService, config);

                UserRole addUserRoleRegular = new UserRole
                {
                    Name = "Regular",
                    Description = "Created for test"
                };
                context.UserRoles.Add(addUserRoleRegular);
                context.SaveChanges();

                var added = new LabII.DTOs.RegisterPostModel
                {
                    FirstName = "Catalin",
                    LastName = "Albulescu",
                    Username = "albuc",
                    Email = "ac@aol.com",
                    Password = "12345678",
                };
                var result = usersService.Register(added);

                var authenticated = new LabII.DTOs.LoginPostModel
                {
                    Username = "albuc",
                    Password = "12345678"
                };
                //valid authentification
                var authresult = usersService.Authenticate(added.Username, added.Password);

                Assert.IsNotNull(authresult);
                Assert.AreEqual(1, authresult.Id);
                Assert.AreEqual(authenticated.Username, authresult.Username);

            }

        }
        

        [Test]
        public void ValidGetAllShouldReturnAllRegisteredUsers()
        {
            var options = new DbContextOptionsBuilder<ExpensesDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(ValidGetAllShouldReturnAllRegisteredUsers))
              .Options;

            using (var context = new ExpensesDbContext(options))
            {
                var regValidator = new RegisterValidator();
                var crValidator = new CreateValidator();
                var usersService = new UsersService(context, regValidator, crValidator, null, config);
                var added1 = new LabII.DTOs.RegisterPostModel
                {
                    FirstName = "Catalin",
                    LastName = "Albulescu",
                    Username = "albuc",
                    Email = "ac@aol.com",
                    Password = "12345678",
                };
                var added2 = new LabII.DTOs.RegisterPostModel
                {
                    FirstName = "testfname",
                    LastName = "testsname",
                    Username = "test_user",
                    Email = "test@yahoo.com",
                    Password = "1111111111"
                };
                usersService.Register(added1);
                usersService.Register(added2);

                int numberOfElements = usersService.GetAll().Count();

                Assert.NotZero(numberOfElements);
                Assert.AreEqual(2, numberOfElements);

            }
        }

        [Test]
        public void GetByIdShouldReturnAnValidUser()
        {
            var options = new DbContextOptionsBuilder<ExpensesDbContext>()
         .UseInMemoryDatabase(databaseName: nameof(GetByIdShouldReturnAnValidUser))
         .Options;

            using (var context = new ExpensesDbContext(options))
            {
                var regValidator = new RegisterValidator();
                var crValidator = new CreateValidator();
                var usersService = new UsersService(context, regValidator, crValidator, null, config);
                var added1 = new LabII.DTOs.RegisterPostModel
                {
                    FirstName = "firstName",
                    LastName = "lastName",
                    Username = "test_user1",
                    Email = "test1@yahoo.com",
                    Password = "111111111"
                };

                usersService.Register(added1);
                var userById = usersService.GetById(1);

                Assert.NotNull(userById);
                Assert.AreEqual("firstName", userById.FirstName);

            }
        }

        [Test]
        public void GetCurentUserShouldReturnAccesToKlaims()
        {
            var options = new DbContextOptionsBuilder<ExpensesDbContext>()
        .UseInMemoryDatabase(databaseName: nameof(GetCurentUserShouldReturnAccesToKlaims))
        .Options;

            using (var context = new ExpensesDbContext(options))
            {
                var regValidator = new RegisterValidator();
                var crValidator = new CreateValidator();
                var validatorUser = new UserRoleValidator();
                var userUserRoleService = new UserUserRoleService(validatorUser, context);
                var usersService = new UsersService(context, regValidator, crValidator, userUserRoleService, config);

                UserRole addUserRoleRegular = new UserRole
                {
                    Name = "Regular",
                    Description = "Created for test"
                };
                context.UserRoles.Add(addUserRoleRegular);
                context.SaveChanges();

                var added = new LabII.DTOs.RegisterPostModel
                {
                    FirstName = "firstName",
                    LastName = "lastName",
                    Username = "test_user1",
                    Email = "test@yahoo.com",
                    Password = "11111111"
                };
                var result = usersService.Register(added);

                var authenticated = new LabII.DTOs.LoginPostModel
                {
                    Username = "test_user1",
                    Password = "11111111"
                };
                var authresult = usersService.Authenticate(added.Username, added.Password);


                //usersService.GetCurentUser(httpContext);

                Assert.IsNotNull(authresult);
            }
        }

        [Test]
        public void CreateShouldReturnNullIfValidUserGetModel()
        {
            var options = new DbContextOptionsBuilder<ExpensesDbContext>()
            .UseInMemoryDatabase(databaseName: nameof(CreateShouldReturnNullIfValidUserGetModel))
            .Options;

            using (var context = new ExpensesDbContext(options))
            {
                var regValidator = new RegisterValidator();
                var crValidator = new CreateValidator();
                var usersService = new UsersService(context, regValidator, crValidator, null, config);

                UserRole addUserRoleRegular = new UserRole
                {
                    Name = "Regular",
                    Description = "Created for test"
                };
                context.UserRoles.Add(addUserRoleRegular);
                context.SaveChanges();

                var added1 = new LabII.DTOs.UserPostModel
                {
                    FirstName = "firstName",
                    LastName = "lastName",
                    UserName = "test_user",
                    Email = "test@yahoo.com",
                    Password = "11111111",
                    UserRole = "Regular",
                };

                var userCreated = usersService.Create(added1);

                Assert.IsNull(userCreated);
            }
        }

        //[Test]
        //public void ValidDeleteShouldRemoveTheUser()
        //{
        //    var options = new DbContextOptionsBuilder<ExpensesDbContext>()
        //    .UseInMemoryDatabase(databaseName: nameof(ValidDeleteShouldRemoveTheUser))
        //    .Options;

        //    using (var context = new ExpensesDbContext(options))
        //    {
        //        var validator = new RegisterValidator();
        //        var crValidator = new CreateValidator();
        //        var usersService = new UsersService(context, validator, crValidator, null, config);
        //        var added = new LabII.DTOs.RegisterPostModel
        //        {
        //            FirstName = "firstName1",
        //            LastName = "firstName1",
        //            Username = "test_userName1",
        //            Email = "first@yahoo.com",
        //            Password = "111111"
        //        };

        //        var userCreated = usersService.Register(added);

        //        Assert.NotNull(userCreated);

        //        //Assert.AreEqual(0, usersService.GetAll().Count());

        //        var userDeleted = usersService.Delete(1);

        //        Assert.Null(userDeleted);
        //        Assert.AreEqual(0, usersService.GetAll().Count());

        //    }
        //}


        [Test]
        public void ValidUpsertShouldModifyFieldsValues()
        {
            var options = new DbContextOptionsBuilder<ExpensesDbContext>()
            .UseInMemoryDatabase(databaseName: nameof(ValidUpsertShouldModifyFieldsValues))
            .Options;

            using (var context = new ExpensesDbContext(options))
            {
                var validator = new RegisterValidator();
                var crValidator = new CreateValidator();
                var usersService = new UsersService(context, validator, crValidator, null, config);
                var added = new LabII.DTOs.UserPostModel
                {
                    FirstName = "Nume",
                    LastName = "Prenume",
                    UserName = "userName",
                    Email = "user@yahoo.com",
                    Password = "333333"
                };

                usersService.Create(added);

                var updated = new LabII.Models.User
                {
                    FirstName = "Alin",
                    LastName = "Popescu",
                    Username = "popAlin",
                    Email = "pop@yahoo.com",
                    Password = "333333"
                };

                var userUpdated = usersService.Upsert(1, updated);

                Assert.NotNull(userUpdated);
                Assert.AreEqual("Alin", userUpdated.FirstName);
                Assert.AreEqual("Popescu", userUpdated.LastName);

            }
        }
    }   
}