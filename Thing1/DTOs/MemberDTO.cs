using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Thing1.DTOs
{
    public class MemberDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Program { get; set; }
        public int RoleId { get; set; }
        public string Description { get; set; }
    }
}