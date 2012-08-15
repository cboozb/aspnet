using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
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
    public class SuppliersController : ApiController
    {
        ProductsContext _db = new ProductsContext();

        [Queryable]
        public IQueryable<Supplier> GetSuppliers()
        {
            return _db.Suppliers;
        }

        public HttpResponseMessage GetById(int id)
        {
            Supplier supplier = _db.Suppliers.SingleOrDefault(s => s.ID == id);
            if (supplier == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, supplier);
            }
        }

        public HttpResponseMessage PostSupplier(Supplier supplier)
        {
            supplier.ProductFamilies = null;

            Supplier addedSupplier = _db.Suppliers.Add(supplier);
            _db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.Created, addedSupplier);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
