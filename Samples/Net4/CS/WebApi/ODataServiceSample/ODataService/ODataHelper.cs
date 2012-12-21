using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using System.Web.Http.Routing;
using Microsoft.Data.OData.Query;

namespace ODataService
{
    /// <summary>
    /// Helper class to facilitate building an odata service.
    /// </summary>
    public static class ODataHelper
    {
        private const string ODataRouteConfigurationKey = "MS_ODataRouteConfiguration";

        /// <summary>
        /// Helper method to get the odata path for an arbitrary odata uri.
        /// </summary>
        /// <param name="configuration">Http configuration object</param>
        /// <param name="uri">OData uri</param>
        /// <returns>The parsed odata path</returns>
        public static ODataPath GetODataPath(this HttpConfiguration configuration, Uri uri)
        {
            var newRequest = new HttpRequestMessage(HttpMethod.Get, uri);
            HttpConfiguration odataRouteConfig = null;
            object value;
            if (configuration.Properties.TryGetValue(ODataRouteConfigurationKey, out value))
            {
                odataRouteConfig = value as HttpConfiguration;
            }

            if (odataRouteConfig == null)
            {
                odataRouteConfig = new HttpConfiguration(new HttpRouteCollection(configuration.VirtualPathRoot));
                if (!configuration.Routes.ContainsKey(ODataRouteConstants.RouteName))
                {
                    throw new InvalidOperationException("You must enable odata route in global configuration first.");
                }
                var odataRoute = configuration.Routes[ODataRouteConstants.RouteName];
                odataRouteConfig.Routes.MapHttpRoute(
                    name: ODataRouteConstants.RouteName,
                    routeTemplate: odataRoute.RouteTemplate,
                    defaults: null,
                    constraints: odataRoute.Constraints);
                configuration.Properties.TryAdd(ODataRouteConfigurationKey, odataRouteConfig);
            }

            IHttpRouteData routeData = odataRouteConfig.Routes.GetRouteData(newRequest);
            if (routeData == null)
            {
                throw new InvalidOperationException("The link is not a valid odata link.");
            }

            //get the odata path Ex: ~/entityset/key/$links/navigation
            return newRequest.GetODataPath();
        }

        /// <summary>
        /// Helper method to get the key value from a uri.
        /// Usually used by $link action to extract the key value from the url in body.
        /// </summary>
        /// <typeparam name="TKey">The type of the key</typeparam>
        /// <param name="configuration">Http configuration object</param>
        /// <param name="uri">OData uri that contains the key value</param>
        /// <returns>The key value</returns>
        public static TKey GetKeyValue<TKey>(this HttpConfiguration configuration, Uri uri)
        {
            //get the odata path Ex: ~/entityset/key/$links/navigation
            var odataPath = configuration.GetODataPath(uri);
            var keySegment = odataPath.Segments.OfType<KeyValuePathSegment>().FirstOrDefault();
            if (keySegment == null)
            {
                throw new InvalidOperationException("The link does not contain a key.");
            }

            var value = ODataUriUtils.ConvertFromUriLiteral(keySegment.Value, Microsoft.Data.OData.ODataVersion.V3);
            return (TKey) value;
        }

        /// <summary>
        /// Convert model state errors into string value.
        /// </summary>
        /// <param name="modelState">Model state</param>
        /// <returns>String value which contains all model errors</returns>
        public static string GetModelStateErrorInformation(ModelStateDictionary modelState)
        {
            StringBuilder errorMessageBuilder = new StringBuilder();
            errorMessageBuilder.AppendLine("Invalid request received.");

            if (modelState != null)
            {
                foreach (var key in modelState.Keys)
                {
                    if (modelState[key].Errors.Count > 0)
                    {
                        errorMessageBuilder.AppendLine(key + ":" + ((modelState[key].Value != null) ? modelState[key].Value.RawValue : "null"));
                    }
                }
            }

            return errorMessageBuilder.ToString();
        }
    }
}
