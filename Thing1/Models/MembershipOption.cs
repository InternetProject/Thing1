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
    using System.ComponentModel.DataAnnotations;

    public partial class MembershipOption
    {
        public int Id { get; set; }
        public int ClubId { get; set; }
        public int Option { get; set; }
        [DataType(DataType.Date)]
        public System.DateTime Expiration { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    
        public virtual Club Club { get; set; }
    }
}
