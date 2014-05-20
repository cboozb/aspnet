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

            var model = ModelBuilder.GetEdmModel();
            var conventions = ODataRoutingConventions.CreateDefaultWithAttributeRouting(config, model);
            conventions.Add(new EntitySetVersioningRoutingConvention("V2"));

            var odataRoute = config.MapODataServiceRoute(
                routeName: "odataV2",
                routePrefix: "odata",
                model: model,
                pathHandler: new DefaultODataPathHandler(),
                routingConventions: conventions);

            var constraint = new ODataVersionRouteConstraint(new { v = "2" });
            odataRoute.Constraints.Add("VersionConstraintV2", constraint);
        }
    }
}
