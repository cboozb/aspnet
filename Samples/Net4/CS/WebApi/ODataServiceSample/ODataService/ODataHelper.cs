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
    /// Helper class to facilitate building odata service.
    /// </summary>
    public static class ODataHelper
    {
        /// <summary>
        /// Helper method to get the key value from a uri.
        /// Usually used by $link action to extract the key value from the url in body.
        /// </summary>
        /// <typeparam name="TKey">The type of the key</typeparam>
        /// <param name="configuration">Http configuration object</param>
        /// <param name="currentRequest">The current request</param>
        /// <param name="uri">OData uri that contains the key value</param>
        /// <returns>The key value</returns>
        public static TKey GetKeyValue<TKey>(this HttpConfiguration configuration, HttpRequestMessage currentRequest, Uri uri)
        {
            var newRequest = new HttpRequestMessage(HttpMethod.Get, uri);
            foreach (var key in currentRequest.Properties.Keys)
            {
                newRequest.Properties.Add(key, currentRequest.Properties[key]);
            }
            IHttpRouteData data = configuration.Routes.GetRouteData(newRequest);

            //get the path template Ex: ~/entityset/key/$links/navigation
            var path = data.Values[ODataRouteConstants.ODataPath] as string;
            var odataPath = configuration.GetODataPathHandler().Parse(path);
            var keySegment = odataPath.Segments.OfType<KeyValuePathSegment>().First().Value;

            var value = ODataUriUtils.ConvertFromUriLiteral(keySegment, Microsoft.Data.OData.ODataVersion.V3);
            return (TKey) value;
        }

        /// <summary>
        /// Convert model state errors into string value.
        /// </summary>
        /// <param name="modelState">Model states</param>
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
