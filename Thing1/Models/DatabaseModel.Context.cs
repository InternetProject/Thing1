﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class user_managementEntities : DbContext
    {
        public user_managementEntities()
            : base("name=user_managementEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<ClubMembership> ClubMemberships { get; set; }
        public virtual DbSet<Club> Clubs { get; set; }
        public virtual DbSet<Email> Emails { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<MembershipOption> MembershipOptions { get; set; }
        public virtual DbSet<payment> payments { get; set; }
        public virtual DbSet<TypesOfClub> TypesOfClubs { get; set; }
        public virtual DbSet<TypesOfMembershipOption> TypesOfMembershipOptions { get; set; }
        public virtual DbSet<TypesOfRecipient> TypesOfRecipients { get; set; }
        public virtual DbSet<TypesOfRole> TypesOfRoles { get; set; }
    }
}
