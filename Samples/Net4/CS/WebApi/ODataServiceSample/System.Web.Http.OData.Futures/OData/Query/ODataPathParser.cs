// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web.Http.OData.Properties;
using Microsoft.Data.Edm;

namespace System.Web.Http.OData.Query
{
    /// <summary>
    /// The ODataPathParser is used to parse a Uri into OData/Edm building block.
    /// </summary>
    public class ODataPathParser
    {
        /// <summary>
        /// Parse the uri relative to the baseUri.
        /// </summary>
        /// <returns>
        /// The ODataPathSegment that represents the uri. If the Uri does not match the model this method returns null.
        /// </returns>
        public virtual ODataPathSegment Parse(Uri uri, Uri baseUri, IEdmModel model)
        {
            if (uri == null)
            {
                throw Error.ArgumentNull("uri");
            }
            if (baseUri == null)
            {
                throw Error.ArgumentNull("baseUri");
            }
            if (model == null)
            {
                throw Error.ArgumentNull("model");
            }
            if (!baseUri.IsBaseOf(uri))
            {
                throw Error.InvalidOperation(SRResources.UriIsNotRelative, uri.AbsoluteUri, baseUri.AbsoluteUri);
            }

            Uri relative = baseUri.MakeRelativeUri(uri);
            string path = relative.OriginalString;
            Uri fake = new Uri("http://server/" + path);
            ODataPathSegment root = new ODataPathSegment(model, new ServiceBase(baseUri));

            IEdmEntityContainer[] containers = model.EntityContainers().ToArray();
            if (containers.Length != 1)
            {
                throw Error.InvalidOperation(SRResources.ParserRequiresExactlyOneEntityContainer, containers.Length);
            }

            IEdmEntityContainer container = containers[0];
            string[] segments = fake.Segments.SelectMany(s => Convert(s)).Where(s => s.Length != 0).ToArray();
            if (segments.Length == 0)
            {
                return root;
            }

            IEdmNamedElement entryPoint = FindElement(container, segments[0]);
            if (entryPoint == null)
            {
                return null;
            }

            IEdmEntitySet entitySet = entryPoint as IEdmEntitySet;
            if (entitySet != null)
            {
                root = new ODataPathSegment(root, entitySet);
            }
            else
            {
                IEdmFunctionImport function = entryPoint as IEdmFunctionImport;
                if (function != null)
                {
                    root = new ODataPathSegment(root, function);
                }
                else
                {
                    ODataMetadata metadata = entryPoint as ODataMetadata;
                    if (metadata != null)
                    {
                        root = new ODataPathSegment(root, metadata);
                    }
                    else
                    {
                        Batch batch = entryPoint as Batch;
                        Contract.Assert(batch != null);
                        root = new ODataPathSegment(root, batch);
                    }
                }
            }

            for (int i = 1; i < segments.Length; i++)
            {
                root = Move(root, segments[i]);
            }
            return root;
        }

        /// <summary>
        /// Tries to find a root level elemtn with a matching name.
        /// </summary>
        protected virtual IEdmNamedElement FindElement(IEdmEntityContainer container, string segment)
        {
            IEdmNamedElement element = null;

            if (segment == ODataMetadata.Value)
            {
                element = ODataMetadata.Singleton;
            }
            else if (segment == Batch.Value)
            {
                element = Batch.Singleton;
            }

            // try match entityset
            if (element == null)
            {
                element = container.FindEntitySet(segment);
            }
            if (element == null)
            {
                // TODO: this needs to support fully qualified Action/Function/ServiceOp names.
                element = container.FunctionImports().SingleOrDefault(fi => fi.Name == segment && fi.IsBindable == false);
            }
            return element;
        }

        /// <summary>
        /// Moves from one segment to the next segment
        /// </summary>
        protected virtual ODataPathSegment Move(ODataPathSegment previous, string segment)
        {
            Contract.Assert(previous != null);
            Contract.Assert(segment != null);

            if (previous.EdmType == null)
            {
                throw Error.InvalidOperation(SRResources.InvalidNavigationDetected, segment, previous.Text);
            }

            switch (previous.EdmType.TypeKind)
            {
                case EdmTypeKind.Collection:
                    return MoveFromCollection(previous, segment);

                case EdmTypeKind.Entity:
                    return MoveFromEntity(previous, segment);

                case EdmTypeKind.Complex:
                    return MoveFromComplex(previous, segment);

                case EdmTypeKind.Primitive:
                    return MoveFromProperty(previous, segment);

                default:
                    throw Error.InvalidOperation(SRResources.InvalidNavigationDetected, segment, previous.Text);
            }
        }

        /// <summary>
        /// Move from a ComplexType segment to the next segment
        /// </summary>
        protected virtual ODataPathSegment MoveFromComplex(ODataPathSegment previous, string segment)
        {
            Contract.Assert(previous != null);
            Contract.Assert(segment != null);

            IEdmComplexType previousType = previous.EdmType as IEdmComplexType;
            Contract.Assert(previousType != null);

            // look for properties
            IEdmProperty property = previousType.Properties().SingleOrDefault(p => p.Name == segment);
            if (property != null)
            {
                return new ODataPathSegment(previous, property);
            }

            // Treating as an Open Property (even though we currently don't support Open Types).
            return new ODataPathSegment(previous, new UnresolvedIdentifier(segment));
        }

        protected virtual ODataPathSegment MoveFromCollection(ODataPathSegment previous, string segment)
        {
            Contract.Assert(previous != null);
            Contract.Assert(segment != null);
            Contract.Assert(previous.EdmType.TypeKind == EdmTypeKind.Collection);

            IEdmCollectionType collection = previous.EdmType as IEdmCollectionType;
            switch (collection.ElementType.Definition.TypeKind)
            {
                case EdmTypeKind.Primitive:
                    return MoveFromProperty(previous, segment);

                case EdmTypeKind.Entity:
                    return MoveFromEntityCollection(previous, segment);

                default:
                    throw Error.InvalidOperation(SRResources.InvalidNavigationDetected, segment, previous.Text);
            }
        }

        protected virtual ODataPathSegment MoveFromEntityCollection(ODataPathSegment previous, string segment)
        {
            Contract.Assert(previous != null);
            Contract.Assert(segment != null);
            Contract.Assert(previous.EdmType.TypeKind == EdmTypeKind.Collection);

            // look for keys first.
            if (segment.StartsWith("(", StringComparison.Ordinal))
            {
                return new ODataPathSegment(previous, new KeyValue(segment));
            }
            // get the collection type
            IEdmCollectionType collectionType = previous.EdmType as IEdmCollectionType;
            IEdmEntityType elementType = collectionType.ElementType.Definition as IEdmEntityType;
            Contract.Assert(elementType != null);

            // next look for casts
            IEdmEntityType castType = previous.Model.FindDeclaredType(segment) as IEdmEntityType;
            if (castType != null)
            {
                return new ODataPathSegment(previous, castType);
            }

            // now look for bindable actions          
            IEdmFunctionImport procedure = previous.Container.FunctionImports().FindBindableProcedure(collectionType, segment);
            if (procedure != null)
            {
                return new ODataPathSegment(previous, procedure);
            }

            throw Error.InvalidOperation(SRResources.InvalidNavigationFromEntityCollectionDetected, segment, collectionType.ElementType.FullName());
        }

        protected virtual ODataPathSegment MoveFromProperty(ODataPathSegment previous, string segment)
        {
            Contract.Assert(previous != null);
            Contract.Assert(segment != null);
            Contract.Assert((previous.EdmElement as IEdmProperty) != null);
            if (segment == ODataValue.Value)
            {
                return new ODataPathSegment(previous, ODataValue.Singleton);
            }
            throw Error.InvalidOperation(SRResources.InvalidNavigationFromPropertyDetected, segment, (previous.EdmElement as IEdmProperty).Name);
        }

        protected virtual ODataPathSegment MoveFromEntity(ODataPathSegment previous, string segment)
        {
            Contract.Assert(previous != null);
            Contract.Assert(segment != null);
            Contract.Assert(previous.EntitySet != null);
            IEdmEntityType previousType = previous.EdmType as IEdmEntityType;
            Contract.Assert(previousType != null);

            if (segment == Links.Value)
            {
                return new ODataPathSegment(previous, Links.Singleton);
            }

            // first look for navigation properties
            IEdmNavigationProperty navigation = previousType.NavigationProperties().SingleOrDefault(np => np.Name == segment);
            if (navigation != null)
            {
                return new ODataPathSegment(previous, navigation);
            }
            // next look for properties
            IEdmProperty property = previousType.Properties().SingleOrDefault(p => p.Name == segment);
            if (property != null)
            {
                return new ODataPathSegment(previous, property);
            }

            // next look for type casts
            IEdmEntityType cast = previous.Model.FindDeclaredType(segment) as IEdmEntityType;
            if (cast != null)
            {
                return new ODataPathSegment(previous, cast);
            }

            // finally look for bindable procedures
            IEdmFunctionImport procedure = previous.Container.FunctionImports().FindBindableProcedure(previousType, segment);
            if (procedure != null)
            {
                return new ODataPathSegment(previous, procedure);
            }

            // Treating as an Open Property (even though we currently don't support Open Types).
            return new ODataPathSegment(previous, new UnresolvedIdentifier(segment));
        }

        /// <summary>
        /// Reads individual segments and splits if necessary
        /// </summary>
        protected virtual IEnumerable<string> Convert(string segment)
        {
            string segmentValue = segment.Replace("/", String.Empty);

            // if there is a key in the segment, go from this:
            // name(key) -> name & key
            int i = segmentValue.IndexOf('(');
            if (i > -1)
            {
                yield return segmentValue.Remove(i);
                int j = segmentValue.IndexOf(')');
                if (j != i + 1)
                {
                    yield return segmentValue.Substring(i, (j + 1) - i);
                }
            }
            else
            {
                yield return segmentValue;
            }
        }
    }
}