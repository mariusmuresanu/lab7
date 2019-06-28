using LabII.DTOs;
using LabII.Models;
using LabII.Services;
using LabII.Validators;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lab4ApiTests
{
    class UserUserRoleServiceTests
    {
        [Test]
        public void GetByIdShouldReturnUserRole()
        {
            var options = new DbContextOptionsBuilder<ExpensesDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(GetByIdShouldReturnUserRole))
              .Options;

            using (var context = new ExpensesDbContext(options))
            {
                var userUserRoleService = new UserUserRoleService(null, context);

                User userToAdd = new User
                {
                    FirstName = "Ana",
                    LastName = "Marcus",
                    Username = "amarcus",
                    Email = "ana@yahoo.com",
                    Password = "1234567",
                    CreatedAt = DateTime.Now,
                    //Expenses = new List<Expense>(),
                    UserUserRoles = new List<UserUserRole>()
                };
                context.Users.Add(userToAdd);

                UserRole addUserRole = new UserRole
                {
                    Name = "Newbegginer",
                    Description = "A role for a new guy..."
                };
                context.UserRoles.Add(addUserRole);
                context.SaveChanges();

                context.UserUserRoles.Add(new UserUserRole
                {
                    User = userToAdd,
                    UserRole = addUserRole,
                    StartTime = DateTime.Now,
                    EndTime = null
                });
                context.SaveChanges();

                var userUserRoleGet = userUserRoleService.GetById(1);
                Assert.IsNotNull(userUserRoleGet.FirstOrDefaultAsync(uurole => uurole.EndTime == null));

            }
        }

        

        [Test]
        public void CreateShouldAddTheUserUserRole()
        {
            var options = new DbContextOptionsBuilder<ExpensesDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(CreateShouldAddTheUserUserRole))
              .Options;

            using (var context = new ExpensesDbContext(options))
            {
                var validator = new UserRoleValidator();
                var userUserRolesService = new UserUserRoleService(validator, context);

                User userToAdd = new User
                {
                    Email = "user@yahoo.com",
                    LastName = "John",
                    FirstName = "Kennedy",
                    Password = "000000000",
                    CreatedAt = DateTime.Now,
                    UserUserRoles = new List<UserUserRole>()
                };
                context.Users.Add(userToAdd);

                UserRole addUserRoleRegular = new UserRole
                {
                    Name = "RegularTest",
                    Description = "Regular - Created for testing"
                };
                UserRole addUserRoleAdmin = new UserRole
                {
                    Name = "AdminTest",
                    Description = "Admin - Created for testing"
                };
                context.UserRoles.Add(addUserRoleRegular);
                context.UserRoles.Add(addUserRoleAdmin);
                context.SaveChanges();

                context.UserUserRoles.Add(new UserUserRole
                {
                    User = userToAdd,
                    UserRole = addUserRoleRegular,
                    StartTime = DateTime.Parse("2019-06-10T00:00:00"),
                    EndTime = null
                });
                context.SaveChanges();

                //sectiunea de schimbare valori invalidata de catre UserRoleValidator
                var uurpost = new UserUserRolePostModel
                {
                    UserId = userToAdd.Id,
                    UserRoleName = "Admin"
                };
                var result = userUserRolesService.Create(uurpost);
                Assert.IsNotNull(result);   //User role nu exista in baza de date dupa validare, ==> exista erori la validare

                //sectiunea de schimbare valori validata de catre UserRoleValidator
                var uurpm1 = new UserUserRolePostModel
                {
                    UserId = userToAdd.Id,
                    UserRoleName = "AdminTest"
                };
                var result1 = userUserRolesService.Create(uurpm1);
                Assert.IsNull(result1);   //User role exista si se face upsert
            }
        }



        [Test]
        public void GetUserRoleNameByIdShouldReturnUserRoleName()
        {
            var options = new DbContextOptionsBuilder<ExpensesDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(GetUserRoleNameByIdShouldReturnUserRoleName))
              .Options;

            using (var context = new ExpensesDbContext(options))
            {
                var userUserRoleService = new UserUserRoleService(null, context);

                User userToAdd = new User
                {
                    FirstName = "Ana",
                    LastName = "Marcus",
                    Username = "amarcus",
                    Email = "ana@yahoo.com",
                    Password = "1234567",
                    CreatedAt = DateTime.Now,
                    UserUserRoles = new List<UserUserRole>()
                };
                context.Users.Add(userToAdd);

                UserRole addUserRole = new UserRole
                {
                    Name = "Newbegginer",
                    Description = "A role for a new guy..."
                };
                context.UserRoles.Add(addUserRole);
                context.SaveChanges();

                context.UserUserRoles.Add(new UserUserRole
                {
                    User = userToAdd,
                    UserRole = addUserRole,
                    StartTime = DateTime.Now,
                    EndTime = null
                });
                context.SaveChanges();

                string userRoleName = userUserRoleService.GetUserRoleNameById(userToAdd.Id);
                Assert.AreEqual("Newbegginer", userRoleName);
                Assert.AreEqual("Ana", userToAdd.FirstName);

            }
        }
    }
}
