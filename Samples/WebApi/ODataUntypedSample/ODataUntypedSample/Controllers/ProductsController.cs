using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.OData;
using System.Web.OData.Extensions;
using System.Web.OData.Query;
using System.Web.OData.Routing;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;

namespace ODataUntypedSample.Controllers
{
    public class ProductsController : ODataController
    {
        private static IQueryable Products = Enumerable.Range(0, 10).Select(i =>
            new
            {
                Id = i,
                Name = "Product " + i,
                Price = i + 0.01,
                Category =
                    new
                    {
                        Id = i % 5,
                        Name = "Category " + (i % 5)
                    }
            }).AsQueryable();

        public EdmEntityObjectCollection Get()
        {
            // Get Edm type from request.
            ODataPath path = Request.ODataProperties().Path;
            IEdmType edmType = path.EdmType;
            Contract.Assert(edmType.TypeKind == EdmTypeKind.Collection);

            IEdmCollectionType collectionType = edmType as IEdmCollectionType;
            IEdmEntityType entityType = collectionType.ElementType.Definition as IEdmEntityType;
            IEdmModel model = Request.ODataProperties().Model;

            ODataQueryContext queryContext = new ODataQueryContext(model, entityType, path);
            ODataQueryOptions queryOptions = new ODataQueryOptions(queryContext, Request);

            // Apply the query option on the IQueryable here.

            return new EdmEntityObjectCollection(
                new EdmCollectionTypeReference(collectionType),
                Products.Cast<object>().Select(product => ConvertProductToEdmEntityObject(product, entityType)).ToList());
        }

        public IEdmEntityObject GetProduct(int key)
        {
            // Get Edm type from request.
            ODataPath path = Request.ODataProperties().Path;
            IEdmType edmType = path.EdmType;
            Contract.Assert(edmType.TypeKind == EdmTypeKind.Entity);

            Func<dynamic, bool> predict = p => p.Id == key;
            object product = Products.Cast<object>().Single(p => predict(p));

            return ConvertProductToEdmEntityObject(product, (IEdmEntityType)edmType);
        }

        public IEdmEntityObject GetCategoryFromProduct(int key)
        {
            // Get Edm type from request.
            ODataPath path = Request.ODataProperties().Path;
            IEdmType edmType = path.EdmType;
            Contract.Assert(edmType.TypeKind == EdmTypeKind.Entity);

            Func<dynamic, bool> predict = p => p.Id == key;
            object category = ((dynamic)Products.Cast<object>().Single(p => predict(p))).Category;

            return ConvertCategoryToEdmEntityObject(category, (IEdmEntityType)edmType);
        }

        public HttpResponseMessage Post(IEdmEntityObject entity)
        {
            // Get Edm type from request.
            ODataPath path = Request.ODataProperties().Path;
            IEdmType edmType = path.EdmType;
            Contract.Assert(edmType.TypeKind == EdmTypeKind.Collection);

            IEdmEntityTypeReference entityType = (edmType as IEdmCollectionType).ElementType.AsEntity();

            // Do something with the entity object here.

            return Request.CreateResponse(HttpStatusCode.Created, entity);
        }

        private IEdmEntityObject ConvertProductToEdmEntityObject(dynamic product, IEdmEntityType entityType)
        {
            EdmEntityObject entityObject = new EdmEntityObject(entityType);
            entityObject.TrySetPropertyValue("Id", product.Id);
            entityObject.TrySetPropertyValue("Name", product.Name);
            entityObject.TrySetPropertyValue("Price", product.Price);

            EdmEntityObject category = new EdmEntityObject((IEdmEntityType)entityType.FindProperty("Category").DeclaringType);
            category.TrySetPropertyValue("Id", product.Category.Id);
            category.TrySetPropertyValue("Name", product.Category.Name);

            entityObject.TrySetPropertyValue("Category", category);
            return entityObject;
        }

        private IEdmEntityObject ConvertCategoryToEdmEntityObject(dynamic category, IEdmEntityType entityType)
        {
            EdmEntityObject entityObject = new EdmEntityObject(entityType);
            entityObject.TrySetPropertyValue("Id", category.Id);
            entityObject.TrySetPropertyValue("Name", category.Name);
            return entityObject;
        }
    }
}
