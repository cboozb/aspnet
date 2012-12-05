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
            : base(new Uri(string.Format("http://{0}:50231", Environment.MachineName)), DataServiceProtocolVersion.V3)
        {
            this.IgnoreResourceNotFoundException = true;
            this.ResolveName = new global::System.Func<global::System.Type, string>(this.ResolveNameFromType);
            this.ResolveType = new global::System.Func<string, global::System.Type>(this.ResolveTypeFromName);
            this.OnContextCreated();
        }
    }
}
