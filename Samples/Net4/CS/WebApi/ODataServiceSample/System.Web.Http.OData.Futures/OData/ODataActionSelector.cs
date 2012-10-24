// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData.Properties;
using System.Web.Http.OData.Query;
using Microsoft.Data.Edm;

namespace System.Web.Http.OData
{
    /// <summary>
    /// This Action selector selects an appropriate action for OData routes that can be handled by convention.
    /// </summary>
    public class ODataActionSelector : ApiControllerActionSelector
    {
        public override HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
        {
            if (controllerContext.RouteData.Route == controllerContext.Configuration.Routes[ODataRouteNames2.PathInfo])
            {
                Select(controllerContext);
            }
            controllerContext.RouteData.Values.Remove(ODataRouteVariables.PathInfo);
            return base.SelectAction(controllerContext);
        }

        private static void Select(HttpControllerContext controllerContext)
        {
            if (controllerContext.ControllerDescriptor.ControllerName == ODataControllers.Metadata)
            {
                SelectMetadata(controllerContext);
                return;
            }

            ODataPathSegment segment = controllerContext.Request.Properties[ODataRouteVariables.ODataPathSegment] as ODataPathSegment;
            string template = segment.KindPath();

            if (template == "~/entityset")
            {
                SelectEntitySet(controllerContext);
            }
            else if (template == "~/entityset/key" || template == "~/entityset/key/cast")
            {
                SelectEntity(controllerContext, segment);
            }
            else if (template == "~/entityset/key/navigation" || template == "~/entityset/key/cast/navigation")
            {
                SelectNavigationProperty(controllerContext, segment);
            }
            else if (template == "~/entityset/key/action" || template == "~/entityset/key/cast/action")
            {
                SelectODataAction(controllerContext, segment);
            }
            else if (
                template == "~/entityset/key/$links/navigation" ||
                template == "~/entityset/key/cast/$links/navigation" ||
                template == "~/entityset/key/$links/navigation/key" ||
                template == "~/entityset/key/cast/$links/navigation/key")
            {
                SelectLinks(controllerContext, segment);
            }
            else
            {
                SelectWildcard(controllerContext, segment);
            }
        }

        private static void SelectEntitySet(HttpControllerContext controllerContext)
        {
            controllerContext.RouteData.Values[ODataRouteVariables.Action] = GetControllerActionPrefix(controllerContext.Request);
        }

        private static void SelectEntity(HttpControllerContext controllerContext, ODataPathSegment segment)
        {
            KeyValue key = segment.EdmElement as KeyValue;
            if (key == null)
            {
                key = segment.Previous.EdmElement as KeyValue;
                Contract.Assert(key != null);
            }
            controllerContext.RouteData.Values.Add(ODataRouteVariables.Id, key.Value);
            string actionName = GetControllerActionPrefix(controllerContext.Request);
            if (actionName == ODataActions.Get)
            {
                actionName = ODataActions.GetById;
            }
            controllerContext.RouteData.Values.Add(ODataRouteVariables.Action, actionName);
        }

        private static void SelectNavigationProperty(HttpControllerContext controllerContext, ODataPathSegment segment)
        {
            IEdmNavigationProperty navProp = segment.EdmElement as IEdmNavigationProperty;
            Contract.Assert(navProp != null);

            string parentId = GetLastKey(segment);
            string navigationProperty = navProp.Name;
            string actionName = string.Format(CultureInfo.InvariantCulture, "{0}{1}", GetControllerActionPrefix(controllerContext.Request), navigationProperty);
            string entityType = null;

            IEdmEntityType castType = segment.Previous.EdmElement as IEdmEntityType;
            if (castType != null)
            {
                entityType = castType.FullName();
                actionName = string.Format(CultureInfo.InvariantCulture, "{0}From{1}", actionName, castType.Name);
            }

            // update route data.
            controllerContext.RouteData.Values.Add(ODataRouteVariables.ParentId, parentId);
            controllerContext.RouteData.Values.Add(ODataRouteVariables.NavigationProperty, navigationProperty);
            controllerContext.RouteData.Values.Add(ODataRouteVariables.Action, actionName);
            if (entityType != null)
            {
                controllerContext.RouteData.Values.Add(ODataRouteVariables.EntityType, entityType);
            }
        }

        private static void SelectODataAction(HttpControllerContext controllerContext, ODataPathSegment segment)
        {
            if (controllerContext.Request.Method != HttpMethod.Post)
            {
                throw Error.InvalidOperation(SRResources.ActionRequiresPOST, controllerContext.Request.RequestUri.AbsolutePath);
            }
            IEdmFunctionImport action = segment.EdmElement as IEdmFunctionImport;

            Contract.Assert(action != null);

            string boundId = GetLastKey(segment);
            string odataAction = action.Name;
            string actionName = null;
            string entityType = null;

            IEdmEntityType castType = segment.Previous.EdmElement as IEdmEntityType;
            if (castType != null)
            {
                entityType = castType.FullName();
                actionName = string.Format(CultureInfo.InvariantCulture, "{0}On{1}", odataAction, castType.Name);
            }
            else
            {
                actionName = odataAction;
            }

            controllerContext.RouteData.Values.Add(ODataRouteVariables.BoundId, boundId);
            controllerContext.RouteData.Values.Add(ODataRouteVariables.ODataAction, odataAction);
            controllerContext.RouteData.Values.Add(ODataRouteVariables.Action, actionName);
            if (entityType != null)
            {
                controllerContext.RouteData.Values.Add(ODataRouteVariables.EntityType, entityType);
            }
            return;
        }

        private static void SelectMetadata(HttpControllerContext controllerContext)
        {
            string lastSegment = controllerContext.Request.RequestUri.Segments.LastOrDefault();
            if (lastSegment == ODataMetadata.Value)
            {
                controllerContext.RouteData.Values[ODataRouteVariables.Action] = ODataActions.GetMetadata;
            }
            else if (lastSegment == "/")
            {
                controllerContext.RouteData.Values[ODataRouteVariables.Action] = ODataActions.GetServiceDocument;
            }
        }

        private static void SelectWildcard(HttpControllerContext controllerContext, ODataPathSegment segment)
        {
            controllerContext.RouteData.Values.Add(ODataRouteVariables.Action, ODataActions.HandleUnmappedRequest);
        }

        private static void SelectLinks(HttpControllerContext controllerContext, ODataPathSegment segment)
        {
            string path = segment.KindPath();
            string method = GetControllerActionPrefix(controllerContext.Request);

            if (path.EndsWith("key"))
            {
                // handle the DELETE ~/entityset/key[/cast]/$links/navigation/key scenario.
                Contract.Assert(method == "Delete");
                string relatedId = (segment.EdmElement as KeyValue).Value;
                controllerContext.RouteData.Values.Add(ODataRouteVariables.RelatedId, relatedId);
                segment = segment.Previous;
            }

            string navigationProperty = (segment.EdmElement as IEdmNavigationProperty).Name;
            string id = null;

            ODataPathSegment keySegment = segment.Previous.Previous;
            Contract.Assert(keySegment != null);
            KeyValue key = keySegment.EdmElement as KeyValue;
            if (key == null)
            {
                keySegment = keySegment.Previous;
                Contract.Assert(keySegment != null);
                key = segment.EdmElement as KeyValue;
                Contract.Assert(key != null);
            }
            id = key.Value;

            if (method == ODataActions.Post || method == ODataActions.Put)
            {
                method = ODataActions.CreateLink;
            }
            else if (method == "Delete")
            {
                method = ODataActions.DeleteLink;
            }
            else
            {
                throw Error.InvalidOperation(SRResources.UnsupportedLinksMethod, method);
            }

            controllerContext.RouteData.Values[ODataRouteVariables.Id] = id;
            controllerContext.RouteData.Values[ODataRouteVariables.NavigationProperty] = navigationProperty;
            controllerContext.RouteData.Values[ODataRouteVariables.Action] = method;
        }

        private static string GetLastKey(ODataPathSegment segment)
        {
            KeyValue key = segment.PreviousSegments
                .Where(s => s.EdmElement is KeyValue)
                .Select(s => s.EdmElement as KeyValue)
                .FirstOrDefault();

            return key == null ? null : key.Value;
        }

        private static string GetControllerActionPrefix(HttpRequestMessage httpRequestMessage)
        {
            switch (httpRequestMessage.Method.Method.ToLower())
            {
                case "get":
                    return ODataActions.Get;
                case "post":
                    return ODataActions.Post;
                case "put":
                    return ODataActions.Put;
                case "patch":
                    return ODataActions.Patch;
                case "delete":
                    return ODataActions.Delete;

                default:
                    return httpRequestMessage.Method.Method;
            }
        }
    }
}
