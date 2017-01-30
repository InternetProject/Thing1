//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Thing1.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Event
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Event()
        {
            this.Clubs = new HashSet<Club>();
            this.EventsRSVPs = new HashSet<EventsRSVP>();
        }
    
        public int Id { get; set; }
        public System.DateTime StartsAt { get; set; }
        public System.DateTime EndsAt { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string TargetAudience { get; set; }
        public bool IsPublic { get; set; }
        public string Food { get; set; }
        public string Contact { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string Title { get; set; }
        public Nullable<int> ClubId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Club> Clubs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EventsRSVP> EventsRSVPs { get; set; }
        public virtual Club Club { get; set; }
    }
}
