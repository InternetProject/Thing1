using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Thing1.Models
{
    public class ClubMetadata
    {
        [Display(Name = "Club Name")]
        public string name;
    }
    public class MembershipOptionMetadata
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Expiration Date")]
        public System.DateTime ExpDate;
    }
    public class TypesOfMembershipOptionMetadata
    {
        [Display(Name = "Membership Type")]
        public string Description;
    }
}