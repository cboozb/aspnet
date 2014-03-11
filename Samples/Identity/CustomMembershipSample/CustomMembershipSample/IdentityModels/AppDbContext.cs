using CustomMembershipSample;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace CustomMembershipSample.IdentityModels
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, int, AppLogin, AppUserRole, AppClaim>
    {
        public static AppDbContext Create()
        {
            return new AppDbContext();
        }

        public AppDbContext()
            : base("AppDbConnection")
        {
            Database.Log = (str) => { Debug.WriteLine(str); };
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>().ToTable("AppUsers");

            var entity = modelBuilder.Entity<AppUser>();
            entity.HasKey(x => x.Id);
            entity.Ignore(x => x.Password);
            entity.Ignore(x => x.Username);
            entity.Ignore(x => x.EmailConfirmed);
            entity.Ignore(x => x.PhoneNumber);
            entity.Ignore(x => x.PhoneNumberConfirmed);
            entity.Ignore(x => x.TwoFactorEnabled);
            entity.Property(x => x.UserName).HasColumnName("Username");
            entity.Property(x => x.PasswordHash).HasColumnName("Password");

            entity.HasMany(u => u.Roles).WithRequired().HasForeignKey(ur => ur.UserId);
        }

        public DbSet<Address> Addresses { get; set; }
    }


}