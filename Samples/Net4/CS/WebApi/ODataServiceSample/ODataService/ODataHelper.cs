using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using System.Web.Http.Routing;

namespace ODataService
{
    public static class ODataHelper
    {
        public static TKey GetKeyValue<TKey>(this HttpConfiguration configuration, Uri uri)
        {
            IHttpRouteData data = configuration.Routes.GetRouteData(new HttpRequestMessage { RequestUri = uri });

            //get the path template Ex: ~/entityset/key/$links/navigation
            var path = data.Values[ODataRouteConstants.ODataPath] as string;
            var odataPath = configuration.GetODataPathHandler().Parse(path);
            var key = odataPath.Segments.OfType<KeyValuePathSegment>().First().Value;

            // TODO: this needs to use ODataLib to convert from OData literal form into the appropriate primitive type instance.
            return (TKey)Convert.ChangeType(key, typeof(TKey));
        }

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
