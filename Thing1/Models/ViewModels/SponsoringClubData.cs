using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//View Model to match clubs with names and their sponsoring status for events
namespace Thing1.Models.ViewModels
{
    public class SponsoringClubData
    {
        public int ClubID { get; set; }
        public string Name { get; set; }
        public bool Sponsoring { get; set; }

    }
}