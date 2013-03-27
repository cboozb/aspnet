using Microsoft.Data.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.OData.Routing;
using System.Web.Http.OData.Routing.Conventions;

namespace ODataActionsSample
{
    // Implements a routing convention for non-bindable actions.
    // The convention maps "MyAction" to Controller:MyAction() method, where the name of the controller 
    // is specified in the constructor.
    public class NonBindableActionRoutingConvention : IODataRoutingConvention
    {
        private string _controllerName;

        public NonBindableActionRoutingConvention(string controllerName)
        {
            _controllerName = controllerName;
        }

        // Route all non-bindable actions to a single controller.
        public string SelectController(ODataPath odataPath, System.Net.Http.HttpRequestMessage request)
        {
            if (odataPath.PathTemplate == "~/action")
            {
                return _controllerName;
            }
            return null;
        }

        // Route the action to a method with the same name as the action.
        public string SelectAction(ODataPath odataPath, System.Web.Http.Controllers.HttpControllerContext controllerContext, ILookup<string, System.Web.Http.Controllers.HttpActionDescriptor> actionMap)
        {
            if (controllerContext.Request.Method == HttpMethod.Post)
            {
                if (odataPath.PathTemplate == "~/action")
                {
                    ActionPathSegment actionSegment = odataPath.Segments.First() as ActionPathSegment;
                    IEdmFunctionImport action = actionSegment.Action;

                    if (!action.IsBindable && actionMap.Contains(action.Name))
                    {
                        return action.Name;
                    }
                }
            }
            return null;
        }
    }
}