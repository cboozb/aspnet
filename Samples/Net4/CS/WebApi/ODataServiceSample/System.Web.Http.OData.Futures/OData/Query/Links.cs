// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Data.Edm;

namespace System.Web.Http.OData.Query
{
    /// <summary>
    /// Links represents a $links found in an OData url.
    /// </summary>
    public class Links : IEdmNamedElement
    {
        public const string Value = "$links";
        public static readonly Links Singleton = new Links();

        private Links()
        {
        }

        public string Name
        {
            get { return Links.Value; }
        }
    }
}
