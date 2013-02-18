using AutoMapper;
using AutoMapper.QueryableExtensions;
using ODataVersioningSample.Models;
using ODataVersioningSample.V2.ViewModels;
using ODataVersioningSample.Extensions;
using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;

namespace ODataVersioningSample.V2.Controller
{
    public class ProductsV2Controller : EntitySetController<Product, long>
    {
        private DbProductsContext _db = new DbProductsContext();

        public override IQueryable<Product> Get()
        {
            return _db.Products.Project().To<Product>();
        }

        [NonAction]
        public Product GetProductImpl(long key)
        {
            var dbProduct = _db.Products.Find(key);
            if (dbProduct == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return Mapper.Map<Product>(dbProduct);
        }

        public override HttpResponseMessage Get([FromODataUri] long key)
        {
            return Request.CreateResponse(
                HttpStatusCode.OK,
                GetProductImpl(key));
        }

        [NonAction]
        public Product CreateEntityImpl(Product entity)
        {
            var dbProduct = Mapper.Map<DbProduct>(entity);
            _db.Products.Add(dbProduct);
            _db.SaveChanges();

            return Mapper.Map<Product>(dbProduct);
        }

        protected override Product CreateEntity(Product entity)
        {
            return CreateEntityImpl(entity);
        }

        [NonAction]
        public Product PatchEntityImpl(long key, Delta<Product> patch)
        {
            var dbProduct = _db.Products.Find(key);
            if (dbProduct == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            var product = Mapper.Map<Product>(dbProduct);
            patch.Patch(product);
            if (product.ID != key)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    "Changing key property is not allowed for PATCH method."));
            }

            dbProduct = Mapper.Map(product, dbProduct);
            _db.Entry(dbProduct).State = EntityState.Modified;
            _db.SaveChanges();

            return product;
        }

        protected override Product PatchEntity(long key, Delta<Product> patch)
        {
            return PatchEntityImpl(key, patch);
        }

        [NonAction]
        public Product UpdateEntityImpl(long key, Product update)
        {
            if (key != update.ID)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    "Changing key property is not allowed for PUT method."));
            }

            var dbProduct = _db.Products.Find(key);
            if (dbProduct == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            dbProduct = Mapper.Map<DbProduct>(update);
            _db.Products.Attach(dbProduct);
            _db.Entry(dbProduct).State = EntityState.Modified;
            _db.SaveChanges();

            return update;
        }

        protected override Product UpdateEntity(long key, Product update)
        {
            return UpdateEntityImpl(key, update);
        }

        public override void Delete([FromODataUri] long key)
        {
            var dbProduct = _db.Products.Find(key);
            if (dbProduct == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }
            _db.Products.Remove(dbProduct);
            _db.SaveChanges();
        }

        public ProductFamily GetFamily([FromODataUri] long key)
        {
            var dbProduct = _db.Products.Find(key);
            if (dbProduct == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            _db.Entry(dbProduct).Reference(p => p.Family).Load();

            return Mapper.Map<ProductFamily>(dbProduct.Family);
        }

        [AcceptVerbs("POST", "PUT")]
        public override void CreateLink([FromODataUri] long key, string navigationProperty, [FromBody] Uri link)
        {
            DbProduct dbProduct = _db.Products.Find(key);

            switch (navigationProperty)
            {
                case "Family":
                    // The utility method uses routing (ODataRoutes.GetById should match) to get the value of {id} parameter 
                    // which is the id of the ProductFamily.
                    int relatedKey = Request.GetKeyValue<int>(link);
                    DbProductFamily dbFamily = _db.ProductFamilies.Find(relatedKey);
                    dbProduct.Family = dbFamily;
                    break;

                default:
                    throw new NotSupportedException("The property is not supported by creating link.");
            }
            _db.SaveChanges();
        }

        protected override long GetKey(Product entity)
        {
            return entity.ID;
        }
    }
}