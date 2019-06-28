﻿using LabII.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LabII.DTOs
{
    public class UserRoleGetModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }


        public static UserRoleGetModel FromUserRole(UserRole userRole)
        {
            return new UserRoleGetModel
            {
                Id = userRole.Id,
                Name = userRole.Name,
                Description = userRole.Description
            };
        }


    }
}
