using CentralisationV0.App_Start.IdentityConfig;
using CentralisationV0.Models.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Web;

[assembly: OwinStartup(typeof(CentralisationV0.App_Start.Startup))]

namespace CentralisationV0.App_Start
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            EnsureRolesExist();  // Assurez-vous que les rôles existent avant de créer des utilisateurs
            CreateRolesAndAdminUser();  // Créez les rôles et utilisateurs
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager, and signin manager to use a single instance per request
            app.CreatePerOwinContext(CentralisationContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Configure the sign-in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
        }

        private void CreateAdminUser()
        {
            using (var context = new CentralisationContext())
            {
                var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

                // Check if the admin user already exists
                var adminEmail = "admin@geomatic.ma";
                var adminUser = userManager.FindByEmail(adminEmail);

                if (adminUser == null)
                {
                    // Create the admin user
                    var admin = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        Nom = "Admin",
                        Prenom = "User",
                        Address = "Admin Address"
                    };

                    var result = userManager.Create(admin, "admin123");

                    if (result.Succeeded)
                    {
                        // Add the admin role to the user
                        userManager.AddToRole(admin.Id, "Admin");
                    }
                }
            }
        }

        private void CreateRolesAndAdminUser()
        {
            using (var context = new CentralisationContext())
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

                // Create Admin role if it does not exist
                string adminRoleName = "Admin";
                if (!roleManager.RoleExists(adminRoleName))
                {
                    var role = new IdentityRole(adminRoleName);
                    roleManager.Create(role);
                }

                // Create Admin user if it does not exist
                var adminEmail = "elharroufimohamed@gmail.com";
                var adminUser = userManager.FindByEmail(adminEmail);
                if (adminUser == null)
                {
                    var admin = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        Nom = "Mohammad",
                        Prenom = "El Ha",
                        Address = "Admin El Ha Address",
                        isActivated = true
                    };

                    var result = userManager.Create(admin, "mohamed_123");
                    if (result.Succeeded)
                    {
                        userManager.AddToRole(admin.Id, adminRoleName);
                    }
                }
            }
        }

        private void EnsureRolesExist()
        {
            using (var context = new CentralisationContext())
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                if (!roleManager.RoleExists("Admin"))
                {
                    roleManager.Create(new IdentityRole("Admin"));
                }

                if (!roleManager.RoleExists("User"))
                {
                    roleManager.Create(new IdentityRole("User"));
                }
            }
        }
    }
}
