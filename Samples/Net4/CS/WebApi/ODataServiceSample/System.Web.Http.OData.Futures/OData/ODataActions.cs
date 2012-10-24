// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace System.Web.Http.OData
{
    /// <summary>
    /// This class makes it easy to refer to built-in actions provided by the built-in OData controllers.
    /// </summary>
    public class ODataActions
    {
        public const string GetMetadata = "GetMetadata";
        public const string GetServiceDocument = "GetServiceDocument";
        public const string HandleUnmappedRequest = "HandleUnmappedRequest";
        public const string Get = "Get";
        public const string GetById = "GetById";
        public const string Put = "Put";
        public const string Patch = "Patch";
        public const string Post = "Post";
        public const string Delete = "Delete";
        public const string CreateLink = "CreateLink";
        public const string DeleteLink = "DeleteLink";
    }
}
