using System.Data.Entity;
using System.Web.Http;
using System.Web.OData.Extensions;
using System.Web.OData.Routing;
using System.Web.OData.Routing.Conventions;
using ODataSxSServiceV2.Extensions;
using ODataSxSServiceV2.Models;

namespace ODataSxSServiceV2
{
    public static class ODataApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            Database.SetInitializer(new DatabaseInitialize());

            var conventions = ODataRoutingConventions.CreateDefault();
            conventions.Insert(0, new EntitySetVersioningRoutingConvention("V2"));

            var odataRoute = config.Routes.MapODataServiceRoute(
                routeName: "odataV2",
                routePrefix: "odata",
                model: ModelBuilder.GetEdmModel(),
                pathHandler: new DefaultODataPathHandler(),
                routingConventions: conventions);

            var contraint = new ODataVersionRouteConstraint(new { v = "2" });
            odataRoute.Constraints.Add("VersionContraintV2", contraint);

            odataRoute.MapODataRouteAttributes(config);
        }
    }
}
