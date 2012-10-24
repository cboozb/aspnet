// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.Data.Edm;

namespace System.Web.Http.OData.Query
{
    /// <summary>
    /// Represents the ServiceBase of an OData Service
    /// </summary>
    public class ServiceBase : IEdmNamedElement
    {
        private Uri _uri;
        private string _name;

        /// <summary>
        /// Constructs a new ServiceBase bound to the serviceRoot provided.
        /// </summary>
        public ServiceBase(Uri uri)
        {
            if (uri == null)
            {
                throw Error.ArgumentNull("uri");
            }
            _uri = uri;
            _name = ExtractName(uri);
        }

        /// <summary>
        ///  The name of this element
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// The uri associated with this Service.
        /// </summary>
        public Uri Uri
        {
            get { return _uri; }
        }

        private static string ExtractName(Uri uri)
        {
            if (uri.Segments.Length > 0)
            {
                return uri.Segments.Last();
            }
            else
            {
                UriBuilder builder = new UriBuilder();
                builder.Scheme = uri.Scheme;
                builder.Port = uri.Port;
                builder.Host = uri.Host;
                return builder.Uri.AbsoluteUri;
            }
        }
    }
}
