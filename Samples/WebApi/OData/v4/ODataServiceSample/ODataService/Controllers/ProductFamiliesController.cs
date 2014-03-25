using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using ODataService.Models;

namespace ODataService.Controllers
{
    /// <summary>
    /// This controller is responsible for the ProductFamilies entity set.
    /// </summary>
    public class ProductFamiliesController : ODataController
    {
        private ProductsContext db = new ProductsContext();

        /// <summary>
        /// Support for querying ProductFamilies
        /// </summary>
        public IQueryable<ProductFamily> Get()
        {
            // if you need to secure this data, one approach would be
            // to apply a where clause before returning. This way any $filter etc, 
            // will be applied only after $filter
            return db.ProductFamilies;
        }

        /// <summary>
        /// Support for getting a ProductFamily by key
        /// </summary>
        /// <param name="key"></param>
        [EnableQuery]
        public SingleResult<ProductFamily> Get(int key)
        {
            return SingleResult.Create(db.ProductFamilies.Where(f => f.Id == key));
        }

        /// <summary>
        /// Support for creating a ProductFamily
        /// </summary>
        /// <param name="productFamily"></param>
        [AcceptVerbs("POST")]
        [ODataRoute("ProductFamilies")]
        public async Task<IHttpActionResult> CreateProductFamily(ProductFamily productFamily)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ProductFamilies.Add(productFamily);
            await db.SaveChangesAsync();

            return Created(productFamily);
        }

        /// <summary>
        /// Support for deleting a ProductFamily
        /// </summary>
        /// <param name="key"></param>
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            var productFamily = await db.ProductFamilies.FindAsync(key);
            if (productFamily == null)
            {
                return Content(HttpStatusCode.NoContent, ODataErrors.EntityNotFound());
            }

            //// Remove the following line after fixing https://aspnetwebstack.codeplex.com/workitem/1768
            productFamily = db.ProductFamilies.Where(p => p.Id == key).Include(p => p.Products).FirstOrDefault();

            foreach (var product in productFamily.Products)
            {
                product.Family = null;
            }

            db.ProductFamilies.Remove(productFamily);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Support for patching a ProductFamily
        /// </summary>
        /// <param name="key"></param>
        /// <param name="productFamily"></param>
        [AcceptVerbs("PATCH")]
        public async Task<IHttpActionResult> Patch(int key, Delta<ProductFamily> productFamily)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var update = await db.ProductFamilies.FindAsync(key);
            if (update == null)
            {
                return Content(HttpStatusCode.NotFound, ODataErrors.EntityNotFound());
            }

            productFamily.Patch(update);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!db.ProductFamilies.Any(p => p.Id == key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Updated(update);
        }

        /// <summary>
        /// Support for replacing a ProductFamily
        /// </summary>
        public async Task<IHttpActionResult> Put([FromODataUri] int key, ProductFamily family)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != family.Id)
            {
                return BadRequest();
            }

            db.Entry(family).State = EntityState.Modified;
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!db.ProductFamilies.Any(p => p.Id == key))
                {
                    return Content(HttpStatusCode.NotFound, ODataErrors.EntityNotFound());
                }
                throw;
            }

            return Updated(family);
        }

        /// <summary>
        /// Support for /ProductFamilies(1)/Products
        /// </summary>
        [EnableQuery]
        public IQueryable<Product> GetProducts([FromODataUri] int key)
        {
            return db.ProductFamilies.Where(pf => pf.Id == key).SelectMany(pf => pf.Products);
        }

        /// <summary>
        /// Support for /ProductFamilies(1)/Supplier
        /// </summary>
        [EnableQuery]
        public SingleResult<Supplier> GetSupplier([FromODataUri] int key)
        {
            return SingleResult.Create(db.ProductFamilies.Where(pf => pf.Id == key).Select(pf => pf.Supplier));
        }

        /// <summary>
        /// Support for POST /ProductFamiles(1)/Products
        /// </summary>
        public async Task<IHttpActionResult> PostToProducts([FromODataUri] int key, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var family = await db.ProductFamilies.FindAsync(key);
            if (family == null)
            {
                return NotFound();
            }

            family.Products.Add(product);

            await db.SaveChangesAsync();

            return Created(product);
        }

        /// <summary>
        /// Support ProductFamily.Products.Add(Product)
        /// </summary>
        [AcceptVerbs("POST", "PUT")]
        [ODataRoute("ProductFamilies({key})/Products/$ref")]
        public async Task<IHttpActionResult> CreateProductsLink([FromODataUri] int key, [FromBody] Uri uri)
        {
            var family = await db.ProductFamilies.SingleOrDefaultAsync(i => i.Id == key);
            if (family == null)
            {
                return Content(HttpStatusCode.NotFound, ODataErrors.EntityNotFound());
            }

            var productId = Request.GetKeyValue<int>(uri);
            var product = await db.Products.SingleOrDefaultAsync(p => p.Id == productId);
            if (product == null)
            {
                return Content(HttpStatusCode.NotFound, ODataErrors.EntityNotFound());
            }
            product.Family = family;

            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Support ProductFamily.Supplier = Supplier
        /// </summary>
        [AcceptVerbs("POST", "PUT")]
        [ODataRoute("ProductFamilies({key})/Supplier/$ref")]
        public async Task<IHttpActionResult> CreateSupplierLink([FromODataUri] int key, [FromBody] Uri uri)
        {
            var family = await db.ProductFamilies.SingleOrDefaultAsync(i => i.Id == key);
            if (family == null)
            {
                return Content(HttpStatusCode.NotFound, ODataErrors.EntityNotFound());
            }

            var supplierId = Request.GetKeyValue<int>(uri);
            var supplier = await db.Suppliers.SingleOrDefaultAsync(p => p.Id == supplierId);
            if (supplier == null)
            {
                return Content(HttpStatusCode.NotFound, ODataErrors.EntityNotFound());
            }
            family.Supplier = supplier;

            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }


        /// <summary>
        /// Support for ProductFamily.Products.Delete(Product)
        /// 
        /// which uses this URL shape:
        ///     DELETE ~/ProductFamilies(id)/Products(relatedId)/$ref
        /// </summary>
        public async Task<IHttpActionResult> DeleteRef([FromODataUri] int key, [FromODataUri] string relatedKey, string navigationProperty)
        {
            var family = await db.ProductFamilies.SingleOrDefaultAsync(p => p.Id == key);
            if (family == null)
            {
                return Content(HttpStatusCode.NotFound, ODataErrors.EntityNotFound());
            }

            switch (navigationProperty)
            {
                case "Products":
                    var productId = Convert.ToInt32(relatedKey);
                    var product = await db.Products.SingleOrDefaultAsync(p => p.Id == productId);

                    if (product == null)
                    {
                        return Content(HttpStatusCode.NotFound, ODataErrors.EntityNotFound());
                    }
                    product.Family = null;
                    break;
                default:
                    return Content(HttpStatusCode.NotImplemented, ODataErrors.DeletingLinkNotSupported(navigationProperty));

            }
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Support for /ProductFamilies(1)/CreateProduct
        /// </summary>
        /// <param name="key">Bound key</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> CreateProduct([FromODataUri] int key, ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productFamily = db.ProductFamilies.SingleOrDefault(p => p.Id == key);
            var productName = parameters["Name"].ToString();

            var product = new Product
            {
                Name = productName,
                Family = productFamily,
                ReleaseDate = DateTime.Now,
                SupportedUntil = DateTime.Now.AddYears(10)
            };
            db.Products.Add(product);

            await db.SaveChangesAsync();

            return Ok(product.Id);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
