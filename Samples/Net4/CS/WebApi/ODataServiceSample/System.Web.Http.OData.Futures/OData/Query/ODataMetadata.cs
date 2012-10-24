// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Data.Edm;

namespace System.Web.Http.OData.Query
{
    /// <summary>
    /// Metadata represents a $metadata found in an OData url.
    /// </summary>
    public class ODataMetadata : IEdmNamedElement
    {
        public const string Value = "$metadata";
        public static readonly ODataMetadata Singleton = new ODataMetadata();

        private ODataMetadata()
        {
        }

        public string Name
        {
            get { return ODataMetadata.Value; }
        }
    }
}
