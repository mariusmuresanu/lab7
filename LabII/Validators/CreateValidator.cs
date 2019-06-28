using LabII.DTOs;
using LabII.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LabII.Validators
{
    public interface ICreateValidator
    {
        ErrorsCollection Validate(UserPostModel userPostModel, ExpensesDbContext context);
    }

    public class CreateValidator : ICreateValidator
    {
        public ErrorsCollection Validate(UserPostModel userPostModel, ExpensesDbContext context)
        {
            ErrorsCollection errorsCollection = new ErrorsCollection { Entity = nameof(UserPostModel) };
            User existing = context.Users.FirstOrDefault(u => u.Username == userPostModel.UserName);
            if (existing != null)
            {
                errorsCollection.ErrorMessages.Add($"The username {userPostModel.UserName} is already taken !");
            }
            if (userPostModel.Password.Length < 7)
            {
                errorsCollection.ErrorMessages.Add("The password cannot be shorter than 7 characters !");
            }
            if (errorsCollection.ErrorMessages.Count > 0)
            {
                return errorsCollection;
            }
            return null;
        }
    }
}
