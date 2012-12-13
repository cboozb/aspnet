using System.Web;
using System.Web.Http;
using System.Web.Http.OData.Builder;
using Microsoft.Data.Edm;
using ODataPagingSample.Models;

namespace ODataPagingSample
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            // Creates the model for our Books entity set
            ODataConventionModelBuilder modelBuilder = new ODataConventionModelBuilder();
            EntityTypeConfiguration<Book> bookConfiguration = modelBuilder.Entity<Book>();
            bookConfiguration.HasKey(book => book.ISBN);
            modelBuilder.EntitySet<Book>("Books");
            IEdmModel model = modelBuilder.GetEdmModel();

            // Enables OData with the 'api' prefix. Requests can then be made to 'virtual root'/api/Books for example
            // This call does several things: it creates a route for OData requests and enables OData querying, routing, and formatting
            GlobalConfiguration.Configuration.EnableOData(model, "api");
        }
    }
}