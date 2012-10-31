// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Data.Edm;

namespace System.Web.Http.OData.Query
{
    /// <summary>
    /// ODataLinksSegment represents a $value found in an OData url.
    /// </summary>
    public class ODataValue : IEdmNamedElement
    {
        public const string Value = "$value";
        public static readonly ODataValue Singleton = new ODataValue();

        private ODataValue()
        {
        }

        public string Name
        {
            get { return ODataValue.Value; }
        }
    }
}