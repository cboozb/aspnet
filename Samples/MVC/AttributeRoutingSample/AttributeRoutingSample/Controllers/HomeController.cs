using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttributeRoutingSample.Controllers
{
    /// <summary>
    /// This controller demonstrates the following:
    /// - Controller-level Route attribute
    /// - Overriding RoutePrefix using '~/'
    /// - Multiple route attributes on action
    /// 
    /// NOTE:
    /// - Route attribute creates routes in the route table, but RoutePrefix does not.
    /// - Route attribute decorated on an action overrides the controller-level route.
    /// - RoutePrefix attribute applies to Route attributes decorated on controller-level and action-level
    /// </summary>
    [RoutePrefix("Home")]
    [Route("{action}")]
    public class HomeController : Controller
    {
        // GET /
        // GET /Home
        // GET /Home/Index
        [Route("~/")]
        [Route]
        [Route("Index")]
        public ActionResult Index()
        {
            return View();
        }

        // GET /Home/About
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        // GET /Home/Contact
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}