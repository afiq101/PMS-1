﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PMS.Models.Database
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class photog_recoveryEntities : DbContext
    {
        public photog_recoveryEntities()
            : base("name=photog_recoveryEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
    
        public virtual int BACKUP_AZURE_BLOB()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("BACKUP_AZURE_BLOB");
        }
    
        public virtual int RESTORE_AZURE_BLOB(string url)
        {
            var urlParameter = url != null ?
                new ObjectParameter("url", url) :
                new ObjectParameter("url", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("RESTORE_AZURE_BLOB", urlParameter);
        }
    }
}
