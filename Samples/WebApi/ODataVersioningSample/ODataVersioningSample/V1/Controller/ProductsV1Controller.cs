using AutoMapper;
using AutoMapper.QueryableExtensions;
using ODataVersioningSample.Models;
using ODataVersioningSample.V1.ViewModels;
using V2VM = ODataVersioningSample.V2.ViewModels;
using ODataVersioningSample.V2.Controller;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;

namespace ODataVersioningSample.V1.Controller
{
    public class ProductsV1Controller : EntitySetController<Product, int>
    {
        private ProductsV2Controller _controller;
        private DbProductsContext _db = new DbProductsContext();

        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);

            _controller = new ProductsV2Controller();
            _controller.Request = Request;
            _controller.Configuration = Configuration;
            _controller.ControllerContext = ControllerContext;
            _controller.Url = Url;
        }

        public override IQueryable<Product> Get()
        {
            return _controller.Get().Project().To<Product>();
        }

        public override HttpResponseMessage Get([FromODataUri] int key)
        {
            var v2Product = _controller.GetProductImpl((long)key);

            return Request.CreateResponse(
                HttpStatusCode.Created,
                Mapper.Map<Product>(v2Product));
        }

        protected override Product CreateEntity(Product entity)
        {
            var v2Product = _controller.CreateEntityImpl(Mapper.Map<V2VM.Product>(entity));
            return Mapper.Map<Product>(v2Product);
        }

        protected override Product PatchEntity(int key, Delta<Product> patch)
        {
            Delta<V2VM.Product> v2Patch = new Delta<V2VM.Product>();
            foreach (string name in patch.GetChangedPropertyNames())
            {
                object value;
                if (patch.TryGetPropertyValue(name, out value))
                {
                    v2Patch.TrySetPropertyValue(name, value);
                }
            }
            var v2Product = _controller.PatchEntityImpl((long)key, v2Patch);
            return Mapper.Map<Product>(v2Product);
        }

        protected override Product UpdateEntity(int key, Product update)
        {
            var v2Product = _controller.UpdateEntityImpl((long)key, Mapper.Map<V2VM.Product>(update));
            return Mapper.Map<Product>(v2Product);
        }

        public override void Delete([FromODataUri] int key)
        {
            _controller.Delete((long)key);
        }

        protected override int GetKey(Product entity)
        {
            return entity.ID;
        }
    }
}