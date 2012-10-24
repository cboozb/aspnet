// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Data.Edm;

namespace System.Web.Http.OData.Query
{
    /// <summary>
    /// KeyValue represents a keyValue found in an OData url.
    /// </summary>
    public class KeyValue : IEdmNamedElement
    {
        /// <summary>
        /// The IEdmNamedElement.Name of the KeyValue
        /// </summary>
        public const string KeyIdentifier = "$key";

        /// <summary>
        /// Construct an ODataKeyValue with the specified value.
        /// </summary>
        public KeyValue(string name)
        {
            if (name == null)
            {
                throw Error.ArgumentNull("name");
            }
            Name = name;
            Value = name.Substring(1, name.Length - 2);
        }

        protected KeyValue()
        {
        }

        /// <summary>
        /// We need a name only so we can be an IEdmNamedElement
        /// </summary>
        public string Name
        {
            get;
            protected set;
        }

        /// <summary>
        /// The inner value of the key.
        /// </summary>
        public string Value
        {
            get;
            protected set;
        }
    }
}