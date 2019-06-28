using LabII.DTOs;
using LabII.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LabII.Validators
{
    public interface IUserRoleValidator
    {
        ErrorsCollection Validate(UserUserRolePostModel userUserRolePostModel, ExpensesDbContext context);
    }


    public class UserRoleValidator : IUserRoleValidator
    {
        public ErrorsCollection Validate(UserUserRolePostModel userUserRolePostModel, ExpensesDbContext context)
        {
            ErrorsCollection errorsCollection = new ErrorsCollection { Entity = nameof(UserUserRolePostModel) };

            List<string> userRoles = context
                .UserRoles
                .Select(userRole => userRole.Name)
                .ToList();

            if (!userRoles.Contains(userUserRolePostModel.UserRoleName))
            {
                errorsCollection.ErrorMessages.Add($"The UserRole {userUserRolePostModel.UserRoleName} does not exist!");
            }
            if (errorsCollection.ErrorMessages.Count > 0)
            {
                return errorsCollection;
            }
            return null;
        }
    }
}
