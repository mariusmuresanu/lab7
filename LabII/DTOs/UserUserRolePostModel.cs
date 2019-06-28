using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LabII.DTOs
{
    public class UserUserRolePostModel
    {
        public int UserId { get; set; }
        public string UserRoleName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }


    }
}
