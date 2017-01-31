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

    public class EventMetadata
    {
        [Display(Name = "Event is Public")]
        public bool IsPublic;
        [Display(Name = "Event Starts at")]
        public System.DateTime StartsAt { get; set; }
        [Display(Name = "Event Ends at")]
        public System.DateTime EndsAt { get; set; }
        [Display(Name = "Event Target Audience")]
        public string TargetAudience { get; set; }
        [Display(Name = "Catered Food From")]
        public string Food { get; set; }
        [Display(Name = "Main Contact for Questions")]
        public string Contact { get; set; }
        [Display(Name = "Price for Entry")]
        public Nullable<decimal> Price { get; set; }
        [Display(Name = "Event Name")]
        public string Title { get; set; }
    }
}