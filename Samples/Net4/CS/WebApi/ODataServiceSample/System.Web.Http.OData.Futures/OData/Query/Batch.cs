// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Data.Edm;

namespace System.Web.Http.OData.Query
{
    /// <summary>
    /// This class represents a $batch in an OData url.
    /// </summary>
    public class Batch : IEdmNamedElement
    {
        public const string Value = "$batch";
        public static readonly Batch Singleton = new Batch();

        private Batch()
        {
        }

        public string Name
        {
            get { return Batch.Value; }
        }
    }
}
