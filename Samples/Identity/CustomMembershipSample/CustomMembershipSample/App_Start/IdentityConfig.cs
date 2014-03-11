using CustomMembershipSample.IdentityModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomMembershipSample
{
    public class AppUserManager : UserManager<AppUser, int>
    {
        public AppUserManager(IUserStore<AppUser, int> store)
            : base(store)
        {

        }

        public static AppUserManager Create(IdentityFactoryOptions<AppUserManager> options, IOwinContext context)
        {
            var manager = new AppUserManager(new AppUserStore(context.Get<AppDbContext>()));

            manager.PasswordValidator = new PasswordValidator()
            {
                RequiredLength = 6
            };

            manager.PasswordHasher = new AppPasswordHasher() { DbContext = context.Get<AppDbContext>() };

            return manager;
        }
    }
}