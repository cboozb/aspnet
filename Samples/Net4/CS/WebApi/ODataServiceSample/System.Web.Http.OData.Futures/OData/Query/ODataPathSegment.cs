// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web.Http.OData.Properties;
using Microsoft.Data.Edm;

namespace System.Web.Http.OData.Query
{
    /// <summary>
    /// ODataUriSegmentInfo information about the current path segment.
    /// Including the EdmType, EntitySet, Text, EdmElement.
    /// </summary>
    public class ODataPathSegment
    {
        private ODataPathSegment _previous;
        private string _text;

        /// <summary>
        /// Create a root ODataPathSegment.
        /// </summary>
        public ODataPathSegment(IEdmModel model, ServiceBase serviceBase)
        {
            if (model == null)
            {
                throw Error.ArgumentNull("model");
            }
            if (serviceBase == null)
            {
                throw Error.ArgumentNull("serviceBase");
            }
            Model = model;
            Container = model.EntityContainers().Single();
            EdmElement = serviceBase;
            Text = serviceBase.Uri.AbsoluteUri;
        }

        /// <summary>
        /// Create an ODataPathSegment that navigates from the previous segment via the specified entity set.
        /// </summary>
        public ODataPathSegment(ODataPathSegment previous, IEdmEntitySet segmentSet)
        {
            if (previous == null)
            {
                throw Error.ArgumentNull("previous");
            }
            if (segmentSet == null)
            {
                throw Error.ArgumentNull("segmentSet");
            }
            Previous = previous;
            EdmElement = segmentSet;
            EdmType = segmentSet.ElementType.GetCollection();
            EntitySet = segmentSet;
        }

        /// <summary>
        /// Create an ODataPathSegment that navigates from the previous segment via the specified procedure.
        /// </summary>
        public ODataPathSegment(ODataPathSegment previous, IEdmFunctionImport procedure)
        {
            if (previous == null)
            {
                throw Error.ArgumentNull("previous");
            }
            if (procedure == null)
            {
                throw Error.ArgumentNull("procedure");
            }
            Previous = previous;
            EdmElement = procedure;
            EdmType = procedure.ReturnType.SafelyGetDefinition();

            // TODO: should take the fullname of the procedure to avoid ambiguity.
            // but only if the procedure is bound.
            IEdmEntitySet functionEntitySet = null;
            if (procedure.TryGetStaticEntitySet(out functionEntitySet))
            {
                EntitySet = functionEntitySet;
            }
        }

        /// <summary>
        /// Create an ODataPathSegment that navigates from the previous segment via the unresolved identifier.
        /// </summary>
        public ODataPathSegment(ODataPathSegment previous, UnresolvedIdentifier identifier)
        {
            if (previous == null)
            {
                throw Error.ArgumentNull("previous");
            }
            if (identifier == null)
            {
                throw Error.ArgumentNull("identifier");
            }
            Previous = previous;
            EdmElement = identifier;
        }

        /// <summary>
        /// Create an ODataPathSegment that navigates from the previous segment via the specified property;
        /// </summary>
        public ODataPathSegment(ODataPathSegment previous, IEdmProperty property)
        {
            if (previous == null)
            {
                throw Error.ArgumentNull("previous");
            }
            if (property == null)
            {
                throw Error.ArgumentNull("property");
            }
            Previous = previous;
            EdmElement = property;
            EdmType = property.Type.Definition;
        }

        /// <summary>
        /// Create an ODataPathSegment that navigates from the previous segment via the specified key.
        /// </summary>
        public ODataPathSegment(ODataPathSegment previous, KeyValue key)
        {
            if (previous == null)
            {
                throw Error.ArgumentNull("previous");
            }
            if (key == null)
            {
                throw Error.ArgumentNull("key");
            }
            Previous = previous;
            EdmElement = key;
            EntitySet = previous.EntitySet;
            EdmType = (previous.EdmType as IEdmCollectionType).ElementType.Definition;
        }

        /// <summary>
        /// Create an ODataPathSegment that navigates from the previous segment via the specified cast.
        /// </summary>
        public ODataPathSegment(ODataPathSegment previous, IEdmEntityType cast)
        {
            if (previous == null)
            {
                throw Error.ArgumentNull("previous");
            }
            if (cast == null)
            {
                throw Error.ArgumentNull("cast");
            }
            Previous = previous;
            EdmElement = cast;
            EntitySet = Previous.EntitySet;
            EdmType = Previous.EdmType.CastToTypeOrCollectionOfType(cast);
            Text = cast.FullName(); // the name of the type is insufficient.
        }

        /// <summary>
        /// Create an ODataPathSegment that navigates from the previous segment via the specified navigation property.
        /// </summary>
        public ODataPathSegment(ODataPathSegment previous, IEdmNavigationProperty navigation)
        {
            if (previous == null)
            {
                throw Error.ArgumentNull("previous");
            }
            if (navigation == null)
            {
                throw Error.ArgumentNull("navigation");
            }
            Previous = previous;
            EdmType = navigation.GetTargetType();
            EntitySet = previous.EntitySet.FindNavigationTarget(navigation);
            EdmElement = navigation;
        }

        /// <summary>
        /// Create an ODataPathSegment that navigates from the previous segment via the built-in $metadata command.
        /// </summary>
        public ODataPathSegment(ODataPathSegment previous, ODataMetadata metadata)
        {
            if (previous == null)
            {
                throw Error.ArgumentNull("previous");
            }
            if (metadata == null)
            {
                throw Error.ArgumentNull("metadata");
            }
            Previous = previous;
            EdmType = null;
            EntitySet = null;
            EdmElement = metadata;
        }

        /// <summary>
        /// Create an ODataPathSegment that navigates from the previous segment via the built-in $batch command.
        /// </summary>
        public ODataPathSegment(ODataPathSegment previous, Batch batch)
        {
            if (previous == null)
            {
                throw Error.ArgumentNull("previous");
            }
            if (batch == null)
            {
                throw Error.ArgumentNull("batch");
            }
            Previous = previous;
            EdmType = null;
            EntitySet = null;
            EdmElement = batch;
        }

        /// <summary>
        /// Create an ODataPathSegment that navigates from the previous segment via the built-in $value command.
        /// </summary>
        public ODataPathSegment(ODataPathSegment previous, ODataValue value)
        {
            if (previous == null)
            {
                throw Error.ArgumentNull("previous");
            }
            if (value == null)
            {
                throw Error.ArgumentNull("value");
            }
            Previous = previous;
            EdmType = previous.EdmType;
            EntitySet = previous.EntitySet;
            EdmElement = value;
        }

        /// <summary>
        /// Create an ODataPathSegment that navigates from the previous segment via the built-in $links command.
        /// </summary>
        public ODataPathSegment(ODataPathSegment previous, Links links)
        {
            if (previous == null)
            {
                throw Error.ArgumentNull("previous");
            }
            if (links == null)
            {
                throw Error.ArgumentNull("links");
            }
            Contract.Assert(previous.EdmType as IEdmEntityType != null);
            Previous = previous;
            EdmType = previous.EdmType;
            EntitySet = previous.EntitySet;
            EdmElement = links;
        }

        /// <summary>
        /// The IEdmEntitySet associated with this Uri segment.
        /// <remarks>The EntitySet must not be null if SegmentType is an IEdmEntityType, otherwise it must be null.</remarks>
        /// </summary>
        public IEdmEntitySet EntitySet { get; private set; }

        /// <summary>
        /// The Type of this Uri segment.
        /// </summary>
        public IEdmType EdmType { get; private set; }

        /// <summary>
        /// The model associated with this segment
        /// </summary>
        public IEdmModel Model { get; private set; }

        /// <summary>
        /// The EntityContainer that functions/actions/etc come from
        /// </summary>
        public IEdmEntityContainer Container { get; private set; }

        /// <summary>
        /// The previous Segment in the path.
        /// </summary>
        public ODataPathSegment Previous
        {
            get
            {
                return _previous;
            }
            set
            {
                _previous = value;
                Model = value == null ? null : value.Model;
                Container = value == null ? null : value.Container;
            }
        }

        /// <summary>
        /// Extra information about the segment.
        /// For example this might be any of the following IEdmEntitySet, IEdmFunctionImport, ODataKeyValue, ODataServiceBase, IEdmProperty, IEdmNavigationProperty, ODataUnresolvedElement 
        /// </summary>
        public IEdmNamedElement EdmElement { get; private set; }

        /// <summary>
        /// The text for the Segment
        /// </summary>
        public string Text
        {
            get
            {
                if (_text == null)
                {
                    _text = EdmElement.Name;
                }
                return _text;
            }
            private set
            {
                Contract.Assert(value != null);
                _text = value;
            }
        }

        public IEnumerable<ODataPathSegment> PreviousSegments
        {
            get
            {
                ODataPathSegment current = this;
                while (current != null)
                {
                    yield return current;
                    current = current.Previous;
                }
            }
        }

        public IEnumerable<ODataPathSegment> Segments
        {
            get
            {
                return PreviousSegments.Reverse();
            }
        }

        public string KindPath()
        {
            return string.Join("/", Segments.Select(s => s.Kind()).ToArray());
        }

        public string Kind()
        {
            Type type = this.EdmElement.GetType();

            if (type == typeof(ServiceBase))
                return "~";
            else if (type == typeof(KeyValue))
                return "key";
            else if (type == typeof(Links))
                return "$links";
            else if (type == typeof(ODataValue))
                return "$value";
            else if (type == typeof(ODataMetadata))
                return "$metadata";
            else if (type == typeof(Batch))
                return "$batch";
            else if (type == typeof(UnresolvedIdentifier))
                return "unresolved";
            else if (typeof(IEdmEntitySet).IsAssignableFrom(type))
                return "entityset";
            else if (typeof(IEdmNavigationProperty).IsAssignableFrom(type))
                return "navigation";
            else if (typeof(IEdmProperty).IsAssignableFrom(type))
                return "property";
            else if (typeof(IEdmEntityType).IsAssignableFrom(type))
                return "cast";
            else if (typeof(IEdmFunctionImport).IsAssignableFrom(type))
                return "action";
            else
                throw Error.NotSupported(SRResources.UnsupportedSegmentKind, type.FullName);
        }
    }
}
