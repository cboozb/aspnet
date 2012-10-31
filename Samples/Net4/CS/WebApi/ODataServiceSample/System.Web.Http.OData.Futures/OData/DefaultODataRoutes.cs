// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Globalization;

namespace System.Web.Http.OData
{
    public class DefaultODataRoutes
    {
        public static readonly string PathInfo = string.Format(CultureInfo.InvariantCulture, "{{{0}}}", ODataRouteVariables.PathInfo);
        public static readonly string Links = string.Format(CultureInfo.InvariantCulture, "{{{0}}}({{{1}}})/$links/{{{2}}}", ODataRouteVariables.Controller, ODataRouteVariables.Id, ODataRouteVariables.NavigationProperty);
        public static readonly string GetById = string.Format(CultureInfo.InvariantCulture, "{{{0}}}({{{1}}})", ODataRouteVariables.Controller, ODataRouteVariables.Id);
        public static readonly string GetByIdWithCast = string.Format(CultureInfo.InvariantCulture, "{{{0}}}({{{1}}})/{{{2}}}", ODataRouteVariables.Controller, ODataRouteVariables.Id, ODataRouteVariables.EntityType);
        public static readonly string PropertyNavigation = string.Format(CultureInfo.InvariantCulture, "{{{0}}}({{{1}}})/{{{2}}}", ODataRouteVariables.Controller, ODataRouteVariables.ParentId, ODataRouteVariables.NavigationProperty);
        public static readonly string PropertyNavigationWithCast = string.Format(CultureInfo.InvariantCulture, "{{{0}}}({{{1}}})/{{{2}}}/{{{3}}}", ODataRouteVariables.Controller, ODataRouteVariables.ParentId, ODataRouteVariables.EntityType, ODataRouteVariables.NavigationProperty);
        public static readonly string InvokeBoundAction = string.Format(CultureInfo.InvariantCulture, "{{{0}}}({{{1}}})/{{{2}}}", ODataRouteVariables.Controller, ODataRouteVariables.BoundId, ODataRouteVariables.ODataAction);
        public static readonly string InvokeBoundActionWithCast = string.Format(CultureInfo.InvariantCulture, "{{{0}}}({{{1}}})/{{{2}}}/{{{3}}}", ODataRouteVariables.Controller, ODataRouteVariables.BoundId, ODataRouteVariables.EntityType, ODataRouteVariables.ODataAction);
    }
}
