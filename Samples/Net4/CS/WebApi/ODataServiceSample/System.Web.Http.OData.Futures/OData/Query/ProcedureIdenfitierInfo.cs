// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Microsoft.Data.Edm;

namespace System.Web.Http.OData
{
    /// <summary>
    /// A ProcedureIdentifierFilter filters procedures based on the identifier used to address the procedure.
    /// The identifier, can be simply a name, or it can be qualified, with the ContainerName, and sometime even the Namespace of the Container.
    /// </summary>
    internal class ProcedureIdentifierFilter
    {
        /// <summary>
        /// Creates a ProcedureIdentifierFilter based on the single name found
        /// </summary>
        internal ProcedureIdentifierFilter(string name)
        {
            if (name == null)
            {
                throw Error.ArgumentNull("name");
            }

            string[] nameParts = name.Split('.');
            Contract.Assert(nameParts.Length != 0);

            if (nameParts.Length == 1)
            {
                Name = nameParts[0];
            }
            else if (nameParts.Length == 2)
            {
                Name = nameParts[nameParts.Length - 1];
                Container = nameParts[nameParts.Length - 2];
            }
            else if (nameParts.Length > 2)
            {
                Name = nameParts[nameParts.Length - 1];
                Container = nameParts[nameParts.Length - 2];
                Namespace = String.Join(".", nameParts.Take(nameParts.Length - 2));
            }
        }

        /// <summary>
        /// The name of the Procedure
        /// </summary>
        internal string Name { get; private set; }

        /// <summary>
        /// The name of the Container that declares the Procedure
        /// </summary>
        internal string Container { get; private set; }

        /// <summary>
        /// The Namespace of the Container that declares the Procedure
        /// </summary>
        internal string Namespace { get; private set; }

        /// <summary>
        /// Apply this filter to a list of procedures. 
        /// </summary>
        /// <param name="procedures">The sequence of procedures to filter</param>
        /// <returns>The filtered sequence of procedures</returns>
        internal IEnumerable<IEdmFunctionImport> Apply(IEnumerable<IEdmFunctionImport> procedures)
        {
            IEnumerable<IEdmFunctionImport> filtered = procedures.Where(p => p.Name == Name);

            if (Container != null)
            {
                filtered = filtered.Where(p => p.Container.Name == Container);
            }

            if (Namespace != null)
            {
                filtered = filtered.Where(p => p.Container.Namespace == Namespace);
            }

            return filtered;
        }
    }
}