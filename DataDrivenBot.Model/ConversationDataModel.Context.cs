﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataDrivenBot.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ConversationDataModel : DbContext
    {
        public ConversationDataModel()
            : base("name=ConversationDataModel")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ConversationTemplate> ConversationTemplates { get; set; }
        public virtual DbSet<PayloadProperty> PayloadProperties { get; set; }
        public virtual DbSet<ResponseOption> ResponseOptions { get; set; }
        public virtual DbSet<ResponseType> ResponseTypes { get; set; }
        public virtual DbSet<Step> Steps { get; set; }
    }
}
