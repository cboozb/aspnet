
using System.Linq;
using System.Net.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.OData.Query;

namespace System.Web.Http.OData
{
    public class ODataControllerSelector : DefaultHttpControllerSelector
    {
        ODataPathParser _parser = new ODataPathParser();

        public ODataControllerSelector(HttpConfiguration configuration)
            : base(configuration)
        {
        }

        public override string GetControllerName(HttpRequestMessage request)
        {
            Uri baseUri = new Uri(request.RequestUri, request.GetConfiguration().VirtualPathRoot);

            ODataPathSegment segment = _parser.Parse(request.RequestUri, baseUri, request.GetConfiguration().GetEdmModel());
            if (segment == null)
            {
                return base.GetControllerName(request);
            }
            request.Properties.Add(ODataRouteVariables.ODataPathSegment, segment);
            if (segment.EdmElement is ServiceBase || segment.EdmElement is Query.ODataMetadata)
            {
                return ODataControllers.Metadata;
            }
            else
            {
                ODataPathSegment entryPoint = segment.Segments.Skip(1).FirstOrDefault();
                return entryPoint.Text;
            }

        }
    }
}
