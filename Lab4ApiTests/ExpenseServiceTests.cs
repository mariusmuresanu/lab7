using LabII.DTOs;
using LabII.Models;
using LabII.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lab4ApiTests
{
    class ExpenseServiceTests
    {
        [Test]
        public void GetAllShouldReturnCorrectNumberOfPagesForExpenses()
        {
            var options = new DbContextOptionsBuilder<ExpensesDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(GetAllShouldReturnCorrectNumberOfPagesForExpenses))
              .Options;

            using (var context = new ExpensesDbContext(options))
            {
                var expenseService = new ExpenseService(context);
                var added = expenseService.Create(new LabII.DTOs.ExpensePostModel

                {
                    Description = "Variable",
                    Type = "5",
                    Location = "Sibiu",
                    Date = Convert.ToDateTime("2019-05-05T11:11:11"),
                    Currency = "USD",
                    Sum = 555.77,
                    Comments = new List<Comment>()
                    {
                        new Comment
                        {
                            Important = true,
                            Text = "Very important expense",
                            Owner = null
                        }
                    },

                }, null);

                DateTime from = DateTime.Parse("2019-04-13T00:00:00");
                DateTime to = DateTime.Parse("2019-06-19T00:00:00");

                var allExpenses = expenseService.GetAll(1, from, to);
                Assert.AreEqual(1, allExpenses.Entries.Count);
                Assert.IsNotNull(allExpenses);
            }
        }



        [Test]
        public void CreateShouldAddAndReturnTheCreatedExpense()
        {
            var options = new DbContextOptionsBuilder<ExpensesDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(CreateShouldAddAndReturnTheCreatedExpense))
              .Options;

            using (var context = new ExpensesDbContext(options))
            {
                var taskService = new ExpenseService(context);
                var added = taskService.Create(new LabII.DTOs.ExpensePostModel

                {
                    Description = "Variable",
                    Type = "5",
                    Location = "Sibiu",
                    Date = Convert.ToDateTime("2019-05-05T11:11:11"),
                    Currency = "USD",
                    Sum = 555.77,
                    Comments = new List<Comment>()
                    {
                        new Comment
                        {
                            Important = true,
                            Text = "Very important expense",
                            Owner = null
                        }
                    },

                }, null);


                Assert.IsNotNull(added);
                Assert.AreEqual("Variable", added.Description);
                Assert.AreNotEqual("Fixed", added.Description);

            }
        }

        [Test]
        public void UpsertShouldModifyTheGivenExpense()
        {
            var options = new DbContextOptionsBuilder<ExpensesDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(UpsertShouldModifyTheGivenExpense))
              .Options;

            using (var context = new ExpensesDbContext(options))
            {
                var expenseService = new ExpenseService(context);
                var added = new ExpensePostModel()

                {
                    Description = "Variable",
                    Type = "5",
                    Location = "Sibiu",
                    Date = Convert.ToDateTime("2019-05-05T11:11:11"),
                    Currency = "USD",
                    Sum = 555.77,
                    Comments = new List<Comment>()
                    {
                        new Comment
                        {
                            Important = true,
                            Text = "Very important expense",
                            Owner = null
                        }
                    },

                };

                var toAdd = expenseService.Create(added, null);
                var update = new ExpensePostModel()
                {
                    Description = "Variable-Updated"
                };

                var toUp = expenseService.Create(update, null);
                var updateResult = expenseService.Upsert(toUp.Id, toUp);


                Assert.IsNotNull(updateResult);
                Assert.AreEqual(toUp.Description, updateResult.Description);

            }
        }

        [Test]
        public void DeleteExpenseWithCommentsShouldDeleteExpenseAndComments()
        {
            var options = new DbContextOptionsBuilder<ExpensesDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(DeleteExpenseWithCommentsShouldDeleteExpenseAndComments))
              .Options;

            using (var context = new ExpensesDbContext(options))
            {
                var expensesService = new ExpenseService(context);

                var expected = new ExpensePostModel()
                {
                    Description = "Variable",
                    Type = "5",
                    Location = "Sibiu",
                    Date = Convert.ToDateTime("2019-05-05T11:11:11"),
                    Currency = "USD",
                    Sum = 555.77,
                    Comments = new List<Comment>()
                    {
                        new Comment
                        {
                            Important = true,
                            Text = "Very important expense",
                            Owner = null
                        }
                    },

                };

                var actual = expensesService.Create(expected, null);
                var afterDelete = expensesService.Delete(actual.Id);
                int numberOfCommentsInDb = context.Comments.CountAsync().Result;
                var resultExpense = context.Expenses.Find(actual.Id);

                Assert.IsNotNull(afterDelete);
                Assert.IsNull(resultExpense);
                Assert.AreEqual(0, numberOfCommentsInDb);
            }
        }

        [Test]
        public void GetByIdShouldReturnExpenseWithCorrectId()
        {
            var options = new DbContextOptionsBuilder<ExpensesDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(GetByIdShouldReturnExpenseWithCorrectId))
              .Options;

            using (var context = new ExpensesDbContext(options))
            {
                var expenseService = new ExpenseService(context);
                var added = new ExpensePostModel()

                {
                    Description = "Variable",
                    Type = "5",
                    Location = "Sibiu",
                    Date = Convert.ToDateTime("2019-05-05T11:11:11"),
                    Currency = "USD",
                    Sum = 555.77,
                    Comments = new List<Comment>()
                    {
                        new Comment
                        {
                            Important = true,
                            Text = "Very important expense",
                            Owner = null
                        }
                    },

                };

                var current = expenseService.Create(added, null);
                var expected = expenseService.GetById(current.Id);

                Assert.IsNotNull(expected);
                Assert.AreEqual(expected.Description, current.Description);
                Assert.AreEqual(expected.Location, current.Location);
                Assert.AreEqual(expected.Sum, current.Sum);
                Assert.AreEqual(expected.Id, current.Id);

            }
        }
    }
}
