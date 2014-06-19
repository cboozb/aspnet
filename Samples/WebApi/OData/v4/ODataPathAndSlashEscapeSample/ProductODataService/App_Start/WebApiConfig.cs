using System.Web.Http;
using System.Web.OData.Extensions;
using System.Web.OData.Routing.Conventions;
using ProductODataService.Extensions;
using ProductODataService.Models;

namespace ProductODataService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapODataServiceRoute("odata", "odata", ProductsServiceEdmModel.GetEdmModel(), new PathAndSlashEscapeODataPathHandler(), ODataRoutingConventions.CreateDefault());
        }
    }
}
