using Owin;
using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using Microsoft.OData.Edm;
using ODataQueryableSample.Models;

namespace ODataQueryableSample
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            // Set up server configuration
            var config = new HttpConfiguration();
            config.Routes.MapODataServiceRoute(routeName: "OData", routePrefix: "odata", model: GetEdmModel());
            appBuilder.UseWebApi(config);
        }

        private IEdmModel GetEdmModel()
        {
            var modelBuilder = new ODataConventionModelBuilder();
            modelBuilder.EntitySet<Customer>("customer");
            modelBuilder.EntitySet<Order>("order");
            modelBuilder.EntitySet<Customer>("response");
            return modelBuilder.GetEdmModel();
        }
    }
}
