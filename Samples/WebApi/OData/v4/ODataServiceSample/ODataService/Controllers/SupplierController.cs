using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using ODataService.Models;

namespace ODataService.Controllers
{
    /// <summary>
    /// This controller implements support for Suppliers EntitySet.
    /// It does not implement everything, it only supports Query, Get by Key and Create, 
    /// by handling these requests:
    /// 
    /// GET /Suppliers
    /// GET /Suppliers(key)
    /// GET /Suppliers?$filter=..&$orderby=..&$top=..&$skip=..
    /// POST /Suppliers
    /// </summary>
    public class SuppliersController : ODataController
    {
        ProductsContext db = new ProductsContext();

        public IQueryable<Supplier> Get()
        {
            return db.Suppliers;
        }

        [EnableQuery]
        public SingleResult<Supplier> Get([FromODataUri] int key)
        {
            return SingleResult.Create(db.Suppliers.Where(s => s.Id == key));
        }

        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> Post(Supplier supplier)
        {
            supplier.ProductFamilies = null;

            Supplier addedSupplier = db.Suppliers.Add(supplier);
            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
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
