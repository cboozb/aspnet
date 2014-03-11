using CustomMembershipSample.IdentityModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomMembershipSample
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext<AppDbContext>(AppDbContext.Create);
            app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/IdentityAccount/Login"),
                Provider = new CookieAuthenticationProvider()
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<AppUserManager, AppUser, int>(
                         validateInterval: TimeSpan.FromMinutes(20),
                         regenerateIdentityCallback: (manager, user) => user.GenerateUserIdentity(manager),
                         getUserIdCallback: (id) => (Int32.Parse(id.GetUserId())))
                }
            });
        }

    }
}

