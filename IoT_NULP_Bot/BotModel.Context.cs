﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IoT_NULP_Bot
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class IoT_BotDbEntities : DbContext
    {
        public IoT_BotDbEntities()
            : base("name=IoT_BotDbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Intent> Intents { get; set; }
        public virtual DbSet<Photo> Photos { get; set; }
        public virtual DbSet<Respons> Responses { get; set; }
    }
}