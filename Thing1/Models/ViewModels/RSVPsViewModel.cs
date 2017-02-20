using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Thing1.Models.ViewModels
{
    public class RSVPsViewModel
    {
        public List<AspNetUser> membersGoing { get; set; }
        public List<AspNetUser> membersInterested { get; set; }
        public List<AspNetUser> membersNotGoing { get; set; }


        public RSVPsViewModel()
        {
            membersGoing = new List<AspNetUser>();
            membersInterested = new List<AspNetUser>();
            membersNotGoing = new List<AspNetUser>();
        }

    }
}