// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace System.Web.Http.OData.Properties
{
    public class SRResources
    {
        public const string ActionNotFound = "Action '{0}' was not found for RequestUri '{1}'.";
        public const string ActionRequiresPOST = "'{0}' identifies an OData Action. OData actions must be invoked with a POST request.";
        public const string ActionResolutionFailed = "Ambiguous request. Found multiple action overloads called '{0}' that bind to a '{1}'.";
        public const string IncompatibleEntitySetFound = "The EntitySet '{0}' already exists with an incompatible EntityType. The type '{1}' cannot be assigned to '{2}'";
        public const string InvalidNavigationDetected = "The URI segment '{0}' is invalid after the segment '{1}'.";
        public const string InvalidNavigationFromEntityCollectionDetected = "Invalid navigation detected, '{0}' is not an action that can bind to 'Collection({1})'";
        public const string InvalidNavigationFromPropertyDetected = "The URI segment '{0}' is not valid after property '{1}'. Only $value can come after properties.";
        public const string ParserRequiresExactlyOneEntityContainer = "Parse requires EdmModels with exactly 1 EntityContainer. {0} EntityContainers were found.";
        public const string UriIsNotRelative = "'{0}' is not a url based on '{1}'.";
        public const string UnsupportedSegmentKind = "There is no SegmentKind mapping for '{0}'.";
        public const string UnsupportedLinksMethod = "{0} is not a supported method for $links requests.";
    }
}
