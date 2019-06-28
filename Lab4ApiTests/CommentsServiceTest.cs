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
    class CommentsServiceTest
    {
        [Test]
        public void GetAllShouldReturnCorrectNumberOfPagesForComments()
        {
            var options = new DbContextOptionsBuilder<ExpensesDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(GetAllShouldReturnCorrectNumberOfPagesForComments))
              .Options;

            using (var context = new ExpensesDbContext(options))
            {
                var taskService = new ExpenseService(context);
                var commentService = new CommentService(context);
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
                            Text = "An important expense",
                            Owner = null
                        }
                    },

                };

                var current = taskService.Create(added, null);

                var allComments = commentService.GetAll(string.Empty, 1);
                Assert.AreEqual(1, allComments.NumberOfPages);

            }
        }

        [Test]
        public void CreateShouldAddAndReturnTheCreatedComment()
        {
            var options = new DbContextOptionsBuilder<ExpensesDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(CreateShouldAddAndReturnTheCreatedComment))
              .Options;

            using (var context = new ExpensesDbContext(options))
            {
                var commentsService = new CommentService(context);
                var toAdd = new CommentPostModel()

                {

                    Important = true,
                    Text = "An important expense",

                };

                var added = commentsService.Create(toAdd, null);


                Assert.IsNotNull(added);
                Assert.AreEqual("An important expense", added.Text);
                Assert.True(added.Important);
            }
        }

        [Test]
        public void UpsertShouldModifyTheGivenComment()
        {
            var options = new DbContextOptionsBuilder<ExpensesDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(UpsertShouldModifyTheGivenComment))
              .Options;

            using (var context = new ExpensesDbContext(options))
            {
                var commentsService = new CommentService(context);
                var toAdd = new CommentPostModel()

                {

                    Important = true,
                    Text = "An important expense",

                };

                var added = commentsService.Create(toAdd, null);
                var update = new CommentPostModel()
                {
                    Important = false
                };

                var toUp = commentsService.Create(update, null);
                var updateResult = commentsService.Upsert(added.Id, added);
                Assert.IsNotNull(updateResult);
                Assert.False(toUp.Important);

            }
        }

        [Test]
        public void DeleteShouldDeleteAGivenComment()
        {
            var options = new DbContextOptionsBuilder<ExpensesDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(DeleteShouldDeleteAGivenComment))
              .Options;

            using (var context = new ExpensesDbContext(options))
            {
                var commentsService = new CommentService(context);
                var toAdd = new CommentPostModel()

                {

                    Important = true,
                    Text = "An important expense",

                };


                var actual = commentsService.Create(toAdd, null);
                var afterDelete = commentsService.Delete(actual.Id);
                int numberOfCommentsInDb = context.Comments.CountAsync().Result;
                var resultComment = context.Comments.Find(actual.Id);


                Assert.IsNotNull(afterDelete);
                Assert.IsNull(resultComment);
                Assert.AreEqual(0, numberOfCommentsInDb);
            }
        }

        [Test]
        public void GetByIdShouldReturnCommentWithCorrectId()
        {
            var options = new DbContextOptionsBuilder<ExpensesDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(GetByIdShouldReturnCommentWithCorrectId))
              .Options;

            using (var context = new ExpensesDbContext(options))
            {
                var commentsService = new CommentService(context);
                var toAdd = new CommentPostModel()

                {

                    Important = true,
                    Text = "An important expense",

                };


                var current = commentsService.Create(toAdd, null);
                var expected = commentsService.GetById(current.Id);



                Assert.IsNotNull(expected);
                Assert.AreEqual(expected.Text, current.Text);
                Assert.AreEqual(expected.Id, current.Id);
            }
        }

    }
}
