namespace LexiconLMS.Migrations
{
    using LexiconLMS.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<LexiconLMS.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(LexiconLMS.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);


            var roleNames = new[] { "Teacher", "Student" };
            foreach (var roleName in roleNames)
            {
                if(!context.Roles.Any(r => r.Name ==roleName))
                {
                    //creates Roll
                    var role = new IdentityRole { Name = roleName };
 
                    var result = roleManager.Create(role);
                    if(!result.Succeeded)
                    {
                        throw new Exception(string.Join("\n", result.Errors));
                    }
                }
            }


            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            var emails = new[] { "user@lexicon.se", "teacher@lexicon.se", "student@lexicon.se" };
            foreach (var email in emails)
            {
              if(!context.Users.Any(u => u.UserName == email))
                {

                    //creating user
                    var user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        UserStartDate = DateTime.Now
                    };

                    var result = userManager.Create(user, "foobar");
                    if (!result.Succeeded)
                    {
                        throw new Exception(string.Join("\n", result.Errors));
                    }

                }
            }

            var teacherUser = userManager.FindByName("teacher@lexicon.se");
            userManager.AddToRole(teacherUser.Id, "Teacher");
           // teacherUser.UserFirstName.Replace(null, "Teacher");


            var studentUser = userManager.FindByName("student@lexicon.se");
            userManager.AddToRole(teacherUser.Id, "Student");
            //teacherUser.UserFirstName.Replace(null, "Student");

        }
    }
}
