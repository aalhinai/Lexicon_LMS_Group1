using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LexiconLMS.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        //public int UserId { get; set; }
        //public string UserName { get; set; }
        [Display(Name = "First Name")]
        public string UserFirstName { get; set; }
        [Display(Name = "Last Name")]
        public string UserLastName { get; set; }
        public string UserFullName { get { return UserFirstName + " " + UserLastName; } }
        //public string UserEmail { get; set; }
        public DateTime UserStartDate { get; set; }
        public int? CourseId { get; set; }

        public virtual Course Course { get; set; }
        public virtual ICollection<Document> Documents { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("LexiconLMS", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Models.Activity> activities { get; set; }
        public DbSet<Models.Course> courses { get; set; }
        public DbSet<Models.Document> documents { get; set; }
        public DbSet<Models.Module> modules { get; set; }

        


    }

}