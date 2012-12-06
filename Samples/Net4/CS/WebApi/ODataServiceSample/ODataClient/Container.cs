using System;
using System.Data.Services.Common;

namespace ODataClient.MSProducts.ODataService.Models
{
    /// <summary>
    /// Create an override for the generated Container (i.e. ctx) for the ODataService.Sample
    /// that:
    /// 1) Uses V3 of the protocol
    /// 2) Converts 404's into empty enumerations (i.e. ctx.Products.Where(p => p.ID == missingID) doesn't throw exception).
    /// </summary>
    public partial class Container
    {
        public Container()
            : base(new Uri("http://localhost:50231"), DataServiceProtocolVersion.V3)
        {
            this.SendingRequest += Container_SendingRequest;
            this.IgnoreResourceNotFoundException = true;
            this.ResolveName = new global::System.Func<global::System.Type, string>(this.ResolveNameFromType);
            this.ResolveType = new global::System.Func<string, global::System.Type>(this.ResolveTypeFromName);
            this.OnContextCreated();
        }

        void Container_SendingRequest(object sender, System.Data.Services.Client.SendingRequestEventArgs e)
        {
            Console.WriteLine("\t{0} {1}", e.Request.Method, e.Request.RequestUri.ToString());
        }
    }
}
