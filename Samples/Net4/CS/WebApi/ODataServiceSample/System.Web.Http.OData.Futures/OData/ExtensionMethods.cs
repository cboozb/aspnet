// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Formatter;
using System.Web.Http.OData.Properties;
using System.Web.Http.OData.Query;
using System.Web.Http.SelfHost;
using Microsoft.Data.Edm;
using Microsoft.Data.Edm.Library;

namespace System.Web.Http.OData
{
    /// <summary>
    /// A series of extension methods that should be moved into the core OData assembly.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Adds OData support to the configuration, based on the provided IEdmModel.
        /// </summary>
        /// <remarks>
        /// This method does the following:
        /// 1) Stores the IEdmModel so it can be accessed when needed
        /// 2) Registers the OData specific IHttpControllerSelector
        /// 3) Registers the OData specific IHttpActionSelector
        /// 4) Registers the ODataMediaTypeFormatter
        /// 5) Sets up the OData wildcard route for handling all incoming OData requests
        /// 6) Sets up useful OData routes for generating out bound urls that comply with OData conventions
        /// </remarks>
        public static void EnableOData(this HttpSelfHostConfiguration configuration, IEdmModel model)
        {
            // register the model
            configuration.SetEdmModel(model);
            // Register a Controller selector
            configuration.Services.Replace(typeof(IHttpControllerSelector), new ODataControllerSelector(configuration));
            // Register an Action selector that can include template parameters in the name
            configuration.Services.Replace(typeof(IHttpActionSelector), new ODataActionSelector());

            // Create the OData formatter and give it the model
            ODataMediaTypeFormatter odataFormatter = new ODataMediaTypeFormatter(model);

            // Register the OData formatter
            configuration.Formatters.Insert(0, odataFormatter);

            // wild card route for handing everything that hangs off a single item.
            // TODO: add PathInfo to ODataRouteNames
            configuration.Routes.MapHttpRoute(ODataRouteNames2.PathInfo, DefaultODataRoutes.PathInfo);

            // Route for manipulating links. 
            configuration.Routes.MapHttpRoute(ODataRouteNames.Link, DefaultODataRoutes.Links);

            // Route for handling GET/PUT/PATCH/DELETE by id.
            configuration.Routes.MapHttpRoute(ODataRouteNames.GetById, DefaultODataRoutes.GetById);
            configuration.Routes.MapHttpRoute(ODataRouteNames.GetByIdWithCast, DefaultODataRoutes.GetByIdWithCast);

            // Relationship routes (notice the parameters is {type}Id not id, this avoids colliding with GetById(id)).
            configuration.Routes.MapHttpRoute(ODataRouteNames.PropertyNavigation, DefaultODataRoutes.PropertyNavigation);
            configuration.Routes.MapHttpRoute(ODataRouteNames.PropertyNavigationWithCast, DefaultODataRoutes.PropertyNavigationWithCast);
            configuration.Routes.MapHttpRoute(ODataRouteNames.InvokeBoundAction, DefaultODataRoutes.InvokeBoundAction);
            configuration.Routes.MapHttpRoute(ODataRouteNames.InvokeBoundActionWithCast, DefaultODataRoutes.InvokeBoundActionWithCast);

            // TODO: remove the Action link convention dependency on this route.
            configuration.Routes.MapHttpRoute(ODataRouteNames.Metadata, "$metadata", new { Controller = "ODataMetadata", Action = "GetMetadata" });
            // TODO: remove the dependency on this route in the SelfLink conventions
            configuration.Routes.MapHttpRoute(ODataRouteNames.Default, "{controller}");

            // TODO: These routes are not required anymore... we should completely remove.             
            //configuration.Routes.MapHttpRoute(ODataRouteNames.DefaultWithParentheses, "{controller}()");
            //configuration.Routes.MapHttpRoute(ODataRouteNames.ServiceDocument, "", new { Controller = "ODataMetadata", Action = "GetServiceDocument" });
        }

        /// <summary>
        /// TODO: this extension methods would not be required if ActionConfiguration.ReturnsFromEntitySet<TReturnType>(..) was tolerant.
        /// The ideas in here should be added to ODataModelBuilder.
        /// </summary>
        public static ActionConfiguration ActionReturnsFromEntitySet<TEntityType>(this ODataModelBuilder builder, ActionConfiguration action, string entitySetName) where TEntityType : class
        {
            action.EntitySet = builder.CreateOrReuseEntitySet<TEntityType>(entitySetName);
            action.ReturnType = builder.GetTypeConfigurationOrNull(typeof(TEntityType));
            return action;
        }

        /// <summary>
        /// TODO: this extension methods would not be required if ActionConfiguration.ReturnsCollectionFromEntitySet<TReturnType>(..) was tolerant.
        /// The ideas in here should be added to ODataModelBuilder.
        /// </summary>
        public static ActionConfiguration ActionReturnsCollectionFromEntitySet<TElementEntityType>(this ODataModelBuilder builder, ActionConfiguration action, string entitySetName) where TElementEntityType : class
        {
            Type clrCollectionType = typeof(IEnumerable<TElementEntityType>);
            action.EntitySet = builder.CreateOrReuseEntitySet<TElementEntityType>(entitySetName);
            IEdmTypeConfiguration elementType = builder.GetTypeConfigurationOrNull(typeof(TElementEntityType));
            action.ReturnType = new CollectionTypeConfiguration(elementType, clrCollectionType);
            return action;
        }

        /// <summary>
        /// TODO: this extension methods would not be required if ActionConfiguration.ReturnsCollectionFromEntitySet<TReturnType>(..) was tolerant.
        /// The ideas in here should be added to ODataModelBuilder.
        /// </summary>
        public static IEntitySetConfiguration CreateOrReuseEntitySet<TElementEntityType>(this ODataModelBuilder builder, string entitySetName) where TElementEntityType : class
        {
            IEntitySetConfiguration entitySet = builder.EntitySets.SingleOrDefault(s => s.Name == entitySetName);

            if (entitySet == null)
            {
                builder.EntitySet<TElementEntityType>(entitySetName);
                entitySet = builder.EntitySets.Single(s => s.Name == entitySetName);
            }
            else
            {
                if (!entitySet.EntityType.ClrType.IsAssignableFrom(typeof(TElementEntityType)))
                {
                    throw Error.InvalidOperation(SRResources.IncompatibleEntitySetFound, entitySetName, typeof(TElementEntityType).FullName, entitySet.EntityType.ClrType.FullName);
                }
                builder.Entity<TElementEntityType>();
            }
            return entitySet;
        }

        public static TKey GetKeyValue<TKey>(this HttpConfiguration configuration, Uri uri)
        {
            ODataPathParser parser = new ODataPathParser();
            Uri baseUri = new Uri(uri, configuration.VirtualPathRoot);
            ODataPathSegment segment = parser.Parse(uri, baseUri, configuration.GetEdmModel());
            KeyValue key = segment.EdmElement as KeyValue;
            if (key == null)
            {
                Contract.Assert(segment.Previous != null);
                key = segment.Previous.EdmElement as KeyValue;
                Contract.Assert(key != null);
            }
            string oId = key.Value;

            // TODO: this needs to use ODataLib to convert from OData literal form into the appropriate primitive type instance.
            return (TKey)Convert.ChangeType(oId, typeof(TKey));
        }

        /// <summary>
        /// Checks the Prefer header to see if the client prefers the content to be returned.
        /// </summary>
        public static bool PreferReturnContent(this HttpRequestMessage request)
        {
            IEnumerable<string> preferences = null;
            if (request.Headers.TryGetValues("Prefer", out preferences))
            {
                return preferences.Contains("return-content");
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks the Prefer header to see if the client prefers the content to not be retunned.
        /// </summary>
        public static bool PreferReturnNoContent(this HttpRequestMessage request)
        {
            IEnumerable<string> preferences = null;
            if (request.Headers.TryGetValues("Prefer", out preferences))
            {
                return preferences.Contains("return-no-content");
            }
            else
            {
                return false;
            }
        }

        internal static bool CanBindTo(this IEdmFunctionImport function, IEdmType type)
        {
            if (function == null)
            {
                throw Error.ArgumentNull("function");
            }
            if (type == null)
            {
                throw Error.ArgumentNull("type");
            }
            if (type.TypeKind == EdmTypeKind.Entity)
            {
                return function.CanBindTo(type as IEdmEntityType);
            }
            else if (type.TypeKind == EdmTypeKind.Collection)
            {
                return function.CanBindTo(type as IEdmCollectionType);
            }
            else
            {
                return false;
            }
        }

        internal static bool CanBindTo(this IEdmFunctionImport function, IEdmEntityType entity)
        {
            if (function == null)
            {
                throw Error.ArgumentNull("function");
            }
            if (entity == null)
            {
                throw Error.ArgumentNull("entity");
            }
            if (!function.IsBindable)
            {
                return false;
            }
            IEdmFunctionParameter bindingParameter = function.Parameters.FirstOrDefault();
            if (bindingParameter == null)
            {
                return false;
            }
            IEdmEntityType bindingParameterType = bindingParameter.Type.Definition as IEdmEntityType;
            if (bindingParameterType == null)
            {
                return false;
            }
            return entity.IsOrInheritsFrom(bindingParameterType);
        }

        internal static bool CanBindTo(this IEdmFunctionImport function, IEdmCollectionType collection)
        {
            if (function == null)
            {
                throw Error.ArgumentNull("function");
            }
            if (collection == null)
            {
                throw Error.ArgumentNull("collection");
            }
            if (!function.IsBindable)
            {
                return false;
            }
            IEdmFunctionParameter bindingParameter = function.Parameters.FirstOrDefault();
            if (bindingParameter == null)
            {
                return false;
            }
            IEdmCollectionType bindingParameterType = bindingParameter.Type.Definition as IEdmCollectionType;
            if (bindingParameterType == null)
            {
                return false;
            }
            IEdmEntityType bindingParameterElementType = bindingParameterType.ElementType.Definition as IEdmEntityType;
            IEdmEntityType entity = collection.ElementType.Definition as IEdmEntityType;
            if (bindingParameterElementType == null || entity == null)
            {
                return false;
            }
            return entity.IsOrInheritsFrom(bindingParameterElementType);
        }

        /// <summary>
        /// The Extension method Multiplicity in ODataLib, is backwards, it gives the Source Multiplicity, but 99.9% of the time you only
        /// care about TargetMultiplicity. Also when there are true one way relationships in OData, a proposal to OASIS, SourceMultiplicity is officially meaningless.
        /// </summary>
        /// <returns>The EdmMultiplicity of the target of this navigation property.</returns>
        internal static EdmMultiplicity TargetMultiplicity(this IEdmNavigationProperty property)
        {
            if (property == null)
            {
                throw Error.ArgumentNull("property");
            }
            return property.Partner.Multiplicity();
        }

        internal static IEdmFunctionImport GetMatch(this IEnumerable<IEdmFunctionImport> sequence, string name, string uri)
        {
            if (sequence == null)
            {
                throw Error.ArgumentNull("sequence");
            }
            if (name == null)
            {
                throw Error.ArgumentNull("name");
            }
            if (uri == null)
            {
                throw Error.ArgumentNull("uri");
            }

            IEdmFunctionImport[] matches = sequence.ToArray();
            if (matches.Length == 0)
            {
                throw Error.InvalidOperation(SRResources.ActionNotFound, name, uri);
            }
            else if (matches.Length > 1)
            {
                throw Error.InvalidOperation(SRResources.ActionResolutionFailed, name, uri);
            }
            return matches[0];
        }

        internal static IEdmCollectionType GetCollection(this IEdmEntityType entityType)
        {
            if (entityType == null)
            {
                throw Error.ArgumentNull("entityType");
            }
            return new EdmCollectionType(new EdmEntityTypeReference(entityType, false));
        }

        internal static IEdmType SafelyGetDefinition(this IEdmTypeReference typeReference)
        {
            IEdmType type = null;

            if (typeReference != null)
            {
                type = typeReference.Definition;
            }

            return type;
        }

        internal static void AssertAssignableTo(this IEdmEntityType from, IEdmEntityType to)
        {
            if (from == null)
            {
                throw Error.ArgumentNull("from");
            }
            if (to == null)
            {
                throw Error.ArgumentNull("to");
            }

            if (!from.InheritsFrom(to))
            {
                throw Error.InvalidOperation("Invalid type cast encountered, {0} does not derive from {1}.", from.FullName(), to.FullName());
            }
        }

        internal static IEdmType CastToTypeOrCollectionOfType(this IEdmType from, IEdmEntityType to)
        {
            if (from == null)
            {
                throw Error.ArgumentNull("from");
            }
            if (to == null)
            {
                throw Error.ArgumentNull("to");
            }

            IEdmType type = from;
            if (type.TypeKind == EdmTypeKind.Collection)
            {
                IEdmEntityType elementType = (type as IEdmCollectionType).ElementType.Definition as IEdmEntityType;
                if (to.IsOrInheritsFrom(elementType))
                {
                    return to.GetCollection();
                }
                else
                {
                    throw Error.InvalidOperation("Invalid type cast encountered.");
                }
            }
            else if (to.IsOrInheritsFrom(from))
            {
                return to;
            }
            else
            {
                throw Error.InvalidOperation("Invalid type cast encountered.");
            }
        }

        internal static IEdmType GetTargetType(this IEdmNavigationProperty navigationProperty)
        {
            if (navigationProperty == null)
            {
                throw Error.ArgumentNull("navigationProperty");
            }

            if (navigationProperty.TargetMultiplicity() == EdmMultiplicity.Many)
            {
                return navigationProperty.ToEntityType().GetCollection();
            }
            else
            {
                return navigationProperty.ToEntityType();
            }
        }

        internal static IEdmFunctionImport FindBindableProcedure(this IEnumerable<IEdmFunctionImport> functions, IEdmEntityType entityType, string fullname)
        {
            if (functions == null)
            {
                throw Error.ArgumentNull("functions");
            }
            if (entityType == null)
            {
                throw Error.ArgumentNull("entityType");
            }
            if (fullname == null)
            {
                throw Error.ArgumentNull("fullname");
            }

            ProcedureIdentifierFilter filter = new ProcedureIdentifierFilter(fullname);
            IEdmFunctionImport[] matches = filter.Apply(functions).Where(fi => fi.CanBindTo(entityType)).ToArray();

            if (matches.Length > 1)
            {
                throw Error.InvalidOperation(SRResources.ActionResolutionFailed, fullname, entityType.FullName());
            }
            else if (matches.Length == 1)
            {
                return matches[0];
            }
            else
            {
                return null;
            }
        }

        internal static IEdmFunctionImport FindBindableProcedure(this IEnumerable<IEdmFunctionImport> functions, IEdmCollectionType collectionType, string fullname)
        {
            if (functions == null)
            {
                throw Error.ArgumentNull("functions");
            }
            if (collectionType == null)
            {
                throw Error.ArgumentNull("collectionType");
            }
            if (fullname == null)
            {
                throw Error.ArgumentNull("fullname");
            }

            ProcedureIdentifierFilter filter = new ProcedureIdentifierFilter(fullname);
            IEdmFunctionImport[] matches = filter.Apply(functions).Where(fi => fi.CanBindTo(collectionType)).ToArray();

            if (matches.Length > 1)
            {
                IEdmEntityType elementType = collectionType.ElementType as IEdmEntityType;
                Contract.Assert(elementType != null);
                throw Error.InvalidOperation(SRResources.ActionResolutionFailed, fullname, String.Format(CultureInfo.InvariantCulture, "Collection({0})", elementType.FullName()));
            }
            else if (matches.Length == 1)
            {
                return matches[0];
            }
            else
            {
                return null;
            }
        }
    }
}
