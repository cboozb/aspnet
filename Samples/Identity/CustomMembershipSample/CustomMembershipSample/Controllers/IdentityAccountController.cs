using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using CustomMembershipSample.Models;

namespace CustomMembershipSample.Controllers
{
    public class IdentityAccountController : Controller
    {
        //
        // GET: /IdentityAccount/

        public ActionResult Login()
        {
            return View();
        }

        //
        // POST: /IdentityAccount/LogOn

        [HttpPost]
        public ActionResult Login(LogOnModel model, string returnUrl)
        {
            var manager = HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            var user = manager.Find(model.UserName, model.Password);

            if (user != null)
            {

                HttpContext.GetOwinContext().Authentication.SignIn(manager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie));
                return RedirectToAction("Index", "Home");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                var manager = HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
                var user = new CustomMembershipSample.IdentityModels.AppUser() { Username = model.UserName, Email = model.Email };
                var result = manager.Create<CustomMembershipSample.IdentityModels.AppUser, int>(user, model.Password);

                if (result == IdentityResult.Success)
                {
                    HttpContext.GetOwinContext().Authentication.SignIn(manager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie));
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", result.Errors.FirstOrDefault());
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult LogOff()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }
    }
}
