using System;
using System.ServiceModel;
using System.Web.Http;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Routing;
using System.Web.Http.Tracing;
using Microsoft.Data.Edm;
using Microsoft.Data.OData;
using Microsoft.Data.OData.Query;
using ODataService.Models;

namespace ODataService
{
    /// <summary>
    /// Helper class to build the EdmModels by either explicit or implicit method.
    /// </summary>
    public static class ModelBuilder
    {
        /// <summary>
        /// Get the EdmModel.
        /// </summary>
        /// <returns></returns>
        public static IEdmModel GetEdmModel()
        {
            // build the model by convention
            return GetImplicitEdmModel();
            // or build the model by hand
            //return GetExplicitEdmModel();
        }

        /// <summary>
        /// Generates a model explicitly.
        /// </summary>
        /// <returns></returns>
        static IEdmModel GetExplicitEdmModel()
        {
            ODataModelBuilder modelBuilder = new ODataModelBuilder();

            var products = modelBuilder.EntitySet<Product>("Products");
            products.HasEditLink(
                entityContext => entityContext.Url.ODataLink(
                    new EntitySetPathSegment(entityContext.EntitySet.Name),
                    new KeyValuePathSegment(ODataUriUtils.ConvertToUriLiteral(entityContext.EntityInstance.ID, ODataVersion.V3))),
                followsConventions: true);

            var suppliers = modelBuilder.EntitySet<Supplier>("Suppliers");
            suppliers.HasEditLink(
                entityContext => entityContext.Url.ODataLink(
                    new EntitySetPathSegment(entityContext.EntitySet.Name),
                    new KeyValuePathSegment(ODataUriUtils.ConvertToUriLiteral(entityContext.EntityInstance.ID, ODataVersion.V3))),
                followsConventions: true);

            var families = modelBuilder.EntitySet<ProductFamily>("ProductFamilies");
            families.HasEditLink(
                entityContext => entityContext.Url.ODataLink(
                    new EntitySetPathSegment(entityContext.EntitySet.Name),
                    new KeyValuePathSegment(ODataUriUtils.ConvertToUriLiteral(entityContext.EntityInstance.ID, ODataVersion.V3))),
                followsConventions: true);

            var product = products.EntityType;

            product.HasKey(p => p.ID);
            product.Property(p => p.Name);
            product.Property(p => p.ReleaseDate);
            product.Property(p => p.SupportedUntil);

            modelBuilder.Entity<RatedProduct>().DerivesFrom<Product>().Property(rp => rp.Rating);

            var address = modelBuilder.ComplexType<Address>();
            address.Property(a => a.City);
            address.Property(a => a.Country);
            address.Property(a => a.State);
            address.Property(a => a.Street);
            address.Property(a => a.ZipCode);

            var supplier = suppliers.EntityType;
            supplier.HasKey(s => s.ID);
            supplier.Property(s => s.Name);
            supplier.ComplexProperty(s => s.Address);

            var productFamily = families.EntityType;
            productFamily.HasKey(pf => pf.ID);
            productFamily.Property(pf => pf.Name);
            productFamily.Property(pf => pf.Description);

            // Create relationships and bindings in one go
            products.HasRequiredBinding(p => p.Family, families);
            families.HasManyBinding(pf => pf.Products, products);
            families.HasOptionalBinding(pf => pf.Supplier, suppliers);
            suppliers.HasManyBinding(s => s.ProductFamilies, families);

            // Create navigation Link builders
            products.HasNavigationPropertiesLink(
                product.NavigationProperties,
                (entityContext, navigationProperty) => 
                    new Uri(entityContext.Url.ODataLink(
                        new EntitySetPathSegment(entityContext.EntitySet.Name),
                        new KeyValuePathSegment(ODataUriUtils.ConvertToUriLiteral(entityContext.EntityInstance.ID, ODataVersion.V3)),
                        new NavigationPathSegment(navigationProperty.Name))),
                followsConventions: true);

            families.HasNavigationPropertiesLink(
                productFamily.NavigationProperties,
                (entityContext, navigationProperty) => 
                    new Uri(entityContext.Url.ODataLink(
                        new EntitySetPathSegment(entityContext.EntitySet.Name),
                        new KeyValuePathSegment(ODataUriUtils.ConvertToUriLiteral(entityContext.EntityInstance.ID, ODataVersion.V3)),
                        new NavigationPathSegment(navigationProperty.Name))),
                followsConventions: true);

            suppliers.HasNavigationPropertiesLink(
                supplier.NavigationProperties,
                (entityContext, navigationProperty) => 
                    new Uri(entityContext.Url.ODataLink(
                        new EntitySetPathSegment(entityContext.EntitySet.Name),
                        new KeyValuePathSegment(ODataUriUtils.ConvertToUriLiteral(entityContext.EntityInstance.ID, ODataVersion.V3)),
                        new NavigationPathSegment(navigationProperty.Name))),
                followsConventions: true);

            ActionConfiguration createProduct = product.Action("CreateProduct");
            createProduct.Parameter<string>("Name");
            createProduct.Returns<int>();

            return modelBuilder.GetEdmModel();
        }

        /// <summary>
        /// Generates a model from a few seeds (i.e. the names and types of the entity sets)
        /// by applying conventions.
        /// </summary>
        /// <returns>An implicitly configured model</returns>    
        static IEdmModel GetImplicitEdmModel()
        {
            ODataModelBuilder modelBuilder = new ODataConventionModelBuilder();
            modelBuilder.EntitySet<Product>("Products");
            modelBuilder.Entity<RatedProduct>().DerivesFrom<Product>();
            modelBuilder.EntitySet<ProductFamily>("ProductFamilies");
            modelBuilder.EntitySet<Supplier>("Suppliers");

            ActionConfiguration createProduct = modelBuilder.Entity<ProductFamily>().Action("CreateProduct");
            createProduct.Parameter<string>("Name");
            createProduct.Returns<int>();

            return modelBuilder.GetEdmModel();
        }
    }
}
