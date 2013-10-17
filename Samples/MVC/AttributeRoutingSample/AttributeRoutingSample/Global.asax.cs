using AttributeRoutingSample.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AttributeRoutingSample
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // seeding database with sample data
            Database.SetInitializer<SchoolContext>(new SchoolContextInitializer());

            // Web API configuration
            GlobalConfiguration.Configure(WebApiConfig.Register);

            // MVC attribute routes registration
            RouteTable.Routes.MapMvcAttributeRoutes();

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
