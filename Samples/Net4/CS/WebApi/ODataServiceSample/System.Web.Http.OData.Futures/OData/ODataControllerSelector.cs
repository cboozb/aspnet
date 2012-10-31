// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

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
            //TODO: check that this matches the OData route first.
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
