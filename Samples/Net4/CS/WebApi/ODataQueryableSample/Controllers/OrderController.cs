using ODataQueryableSample.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData.Query;

namespace ODataQueryableSample.Controllers
{
    /// <summary>
    /// This sample order controller demonstrates how to create an action which supports
    /// OData style queries using by accessing the query directly before applying it.
    /// This allows for inspection and manipulation of the query before it is being
    /// applied.
    /// </summary>
    public class OrderController : ApiController
    {
        private static List<Order> OrderList = new List<Order>
        {  
            new Order{ Id = 11, Name = "Order1", Quantity = 1 }, 
            new Order{ Id = 33, Name = "Order3", Quantity = 3 }, 
            new Order { Id = 22, Name = "Order2", Quantity = 2 }, 
            new Order { Id = 3, Name = "Order0", Quantity = 0 },
        };

        public IQueryable<Order> Get(ODataQueryOptions queryOptions)
        {
            // Validate the top parameter
            if (!ValidateTopQueryOption(queryOptions.Top))
            {
                HttpResponseMessage response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid value for $top query parameter");
                throw new HttpResponseException(response);
            }

            return queryOptions.ApplyTo(OrderList.AsQueryable()) as IQueryable<Order>;
        }

        private static bool ValidateTopQueryOption(TopQueryOption top)
        {
            if (top != null && top.RawValue != null)
            {
                int topValue = Int32.Parse(top.RawValue, NumberStyles.None);
                return topValue < 10;
            }
            return true;
        }
    }
}
