using System.Web.Mvc;

namespace ODataEFBatchSample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return Redirect("/Tests/SpecRunner.html");
        }
    }
}
