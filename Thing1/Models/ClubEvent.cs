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
    
    public partial class ClubEvent
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int ClubId { get; set; }
    
        public virtual Club Club { get; set; }
        public virtual Event Event { get; set; }
    }
}
