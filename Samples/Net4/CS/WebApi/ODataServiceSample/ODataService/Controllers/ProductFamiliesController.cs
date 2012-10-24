using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using ODataService.Models;

namespace ODataService.Controllers
{
    /// <summary>
    /// This controller is responsible for the ProductFamilies entity set.
    /// 
    /// <remarks>
    /// In this example we leverage the EntitySetController<TEntity,TKey> class that
    /// provides basic plumbing for implementing an OData EntitySet.
    /// 
    /// This class overrides all the method needed to provide full support for 
    /// the OData operations currently supported by Web API.
    /// </remarks>
    /// </summary>
    public class ProductFamiliesController : EntitySetController<ProductFamily, int>
    {
        ProductsContext _db = new ProductsContext();

        /// <summary>
        /// Support for querying ProductFamilies
        /// </summary>
        /// <remarks>
        /// The [Queryable] attribute is commented out, because it is inherited from the base.Get() method.
        /// if however you have no base class simply add it.
        /// </remarks>
        // [Queryable]
        public override IQueryable<ProductFamily> Get()
        {
            // if you need to secure this data, one approach would be
            // to apply a where clause before returning. This way any $filter etc, 
            // will be applied only after $filter
            return _db.ProductFamilies;
        }

        /// <summary>
        /// Support for getting a ProductFamily by key
        /// </summary>
        protected override ProductFamily GetEntityById(int id)
        {
            return _db.ProductFamilies.SingleOrDefault(f => f.ID == id);
        }

        /// <summary>
        /// Support for creating a ProductFamily
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected override ProductFamily CreateEntity(ProductFamily entity)
        {
            _db.ProductFamilies.Add(entity);
            _db.SaveChanges();
            return entity;
        }

        /// <summary>
        /// Support for deleting a ProductFamily
        /// </summary>
        /// <param name="id"></param>
        protected override void DeleteEntity(int id)
        {
            ProductFamily toDelete = _db.ProductFamilies.FirstOrDefault(f => f.ID == id);
            if (toDelete == null)
            {
                throw ODataErrors.EntityNotFound(Request);
            }
            _db.ProductFamilies.Remove(toDelete);
            _db.SaveChanges();
        }

        /// <summary>
        /// Support for patching a ProductFamily
        /// </summary>
        /// <remarks>
        /// TODO: Should verify that the patch.ID == id for security reasons
        /// </remarks>
        protected override ProductFamily PatchEntity(int id, Delta<ProductFamily> patch)
        {
            ProductFamily toUpdate = _db.ProductFamilies.FirstOrDefault(f => f.ID == id);
            if (toUpdate == null)
            {
                throw ODataErrors.EntityNotFound(Request);
            }
            patch.Patch(toUpdate);
            _db.SaveChanges();
            return toUpdate;
        }

        /// <summary>
        /// Support for replacing a ProductFamily
        /// </summary>
        /// <remarks>
        /// TODO: Should verify that the update.ID == id for security reasons
        /// </remarks>
        protected override ProductFamily UpdateEntity(int id, ProductFamily update)
        {
            if (!_db.ProductFamilies.Any(p => p.ID == id))
            {
                throw ODataErrors.EntityNotFound(Request);
            }
            update.ID = id; // ignore the ID in the entity use the ID in the URL.

            _db.ProductFamilies.Attach(update);
            _db.Entry(update).State = System.Data.EntityState.Modified;
            _db.SaveChanges();
            return update;
        }

        /// <summary>
        /// Support for /ProductFamily(1)/Products
        /// </summary>
        [Queryable]
        public IQueryable<Product> GetProducts(int parentId)
        {
            return _db.ProductFamilies.Where(pf => pf.ID == parentId).SelectMany(pf => pf.Products);
        }

        /// <summary>
        /// Support for /ProductFamily(1)/Supplier
        /// </summary>
        public Supplier GetSupplier(int parentId)
        {
            return _db.ProductFamilies.Where(pf => pf.ID == parentId).Select(pf => pf.Supplier).SingleOrDefault();
        }

        /// <summary>
        /// Support ProductFamily.Products.Add(Product) and ProductFamily.Supplier = Supplier
        /// <remarks>
        /// TODO: When override EntitySetController.CreateLink(), the [FromBody] attribute is missing.
        /// it is required otherwise link will always be null.
        /// </remarks>
        /// </summary>
        public override HttpResponseMessage CreateLink(int id, string navigationProperty, [FromBody] Uri link)
        {
            ProductFamily family = _db.ProductFamilies.SingleOrDefault(p => p.ID == id);
            if (family == null)
            {
                throw ODataErrors.EntityNotFound(Request);
            }
            switch (navigationProperty)
            {
                case "Products":
                    int productId = Configuration.GetKeyValue<int>(link);
                    Product product = _db.Products.SingleOrDefault(p => p.ID == productId);
                    if (product == null)
                    {
                        throw ODataErrors.EntityNotFound(Request);
                    }
                    product.Family = family;
                    break;

                case "Supplier":
                    int supplierId = Configuration.GetKeyValue<int>(link);
                    Supplier supplier = _db.Suppliers.SingleOrDefault(s => s.ID == supplierId);
                    if (supplier == null)
                    {
                        throw ODataErrors.EntityNotFound(Request);
                    }
                    family.Supplier = supplier;
                    break;

                default:
                    return base.CreateLink(id, navigationProperty, link);
            }
            _db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Support for ProductFamily.Supplier = null
        /// which uses this Url shape:
        ///     DELETE ~/ProductFamilies(id)/$links/Supplier
        ///     headers
        ///     
        ///     [link]
        /// </summary>
        /// <remarks>
        /// TODO: When override EntitySetController.DeleteLink(), the [FromBody] attribute is missing.
        /// it is required otherwise link will always be null.
        /// </remarks>
        public override HttpResponseMessage DeleteLink(int id, string navigationProperty, [FromBody] Uri link)
        {
            ProductFamily family = _db.ProductFamilies.SingleOrDefault(p => p.ID == id);
            if (family == null)
            {
                throw ODataErrors.EntityNotFound(Request);
            }

            switch (navigationProperty)
            {
                case "Supplier":
                    family.Supplier = null;
                    break;

                default:
                    return base.DeleteLink(id, navigationProperty, link);

            }
            _db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Support for ProductFamily.Products.Delete(Product)
        /// 
        /// which uses this URL shape:
        ///     DELETE ~/ProductFamilies(id)/$links/Products(relatedId)
        /// </summary>
        public override HttpResponseMessage DeleteLink(int id, int relatedId, string navigationProperty)
        {
            ProductFamily family = _db.ProductFamilies.SingleOrDefault(p => p.ID == id);
            if (family == null)
            {
                throw ODataErrors.EntityNotFound(Request);
            }

            switch (navigationProperty)
            {
                case "Products":
                    Product product = _db.Products.SingleOrDefault(p => p.ID == relatedId);
                    if (product == null)
                    {
                        throw ODataErrors.EntityNotFound(Request);
                    }
                    product.Family = null;
                    break;


                default:
                    return base.DeleteLink(id, relatedId, navigationProperty);

            }
            _db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public int CreateProduct(int boundId, ODataActionParameters parameters)
        {
            int createdId = -1;
            try
            {
                ProductFamily productFamily = _db.ProductFamilies.SingleOrDefault(p => p.ID == boundId);
                string productName = parameters["Name"].ToString();
                
                Product product = new Product
                {
                    Name = productName,
                    Family = productFamily,
                    ReleaseDate = DateTime.Now,
                    SupportedUntil = DateTime.Now.AddYears(10)
                };
                _db.Products.Add(product);
                _db.SaveChanges();
                createdId = product.ID;
            }
            catch 
            {
            }
            return createdId;
        }

        /// <summary>
        /// Required override to help the base class build self/edit/id links.
        /// </summary>
        protected override int GetId(ProductFamily entity)
        {
            return entity.ID;
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
