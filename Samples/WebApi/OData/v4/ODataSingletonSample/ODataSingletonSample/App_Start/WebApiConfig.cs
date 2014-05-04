using System.Web.Http;
using System.Web.OData.Extensions;

namespace WebStack.QA.Test.OData.Singleton
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            config.Routes.MapODataServiceRoute("odata", "odata", SingletonEdmModel.GetEdmModel()).MapODataRouteAttributes(config);
            config.EnsureInitialized();
        }
    }
}
