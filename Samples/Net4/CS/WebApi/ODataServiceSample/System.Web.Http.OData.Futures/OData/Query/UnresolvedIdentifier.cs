// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Data.Edm;

namespace System.Web.Http.OData.Query
{
    /// <summary>
    /// UnresolvedIdentifier represents an OData identifier that can't be resolved
    /// </summary>
    public class UnresolvedIdentifier : IEdmNamedElement
    {
        /// <summary>
        /// Construct an UnresolvedIdentifier with the specified identifier value.
        /// </summary>
        public UnresolvedIdentifier(string name)
        {
            if (name == null)
            {
                throw Error.ArgumentNull("name");
            }
            Name = name;
        }

        /// <summary>
        /// We need a name only so we can be an IEdmNamedElement
        /// </summary>
        public string Name
        {
            get;
            private set;
        }
    }
}