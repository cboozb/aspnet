using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.OData.Routing.Conventions;
using System.Web.Http.OData.Routing;
using System.Web.Http.OData.Extensions;

namespace ODataSxSService.Extensions
{
    public class EntitySetVersioningRoutingConvention : IODataRoutingConvention
    {
        private string _versionSuffix;
        public EntitySetVersioningRoutingConvention(string versionSuffix)
        {
            _versionSuffix = versionSuffix;
        }
        private EntitySetRoutingConvention _entitySetRoutingConvention = new EntitySetRoutingConvention();
        public string SelectAction(System.Web.Http.OData.Routing.ODataPath odataPath, System.Web.Http.Controllers.HttpControllerContext controllerContext, ILookup<string, System.Web.Http.Controllers.HttpActionDescriptor> actionMap)
        {
            return _entitySetRoutingConvention.SelectAction(odataPath, controllerContext, actionMap);
        }

        public string SelectController(System.Web.Http.OData.Routing.ODataPath odataPath, System.Net.Http.HttpRequestMessage request)
        {
            var baseControllerName = _entitySetRoutingConvention.SelectController(odataPath, request);
            return baseControllerName == null ? null : string.Concat( baseControllerName, _versionSuffix);
        }
    }
}
