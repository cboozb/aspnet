using System;
using System.Web.Http;
using System.Web.Mvc;
using AttributeRoutingSample.Areas.HelpPage.Models;

namespace AttributeRoutingSample.Areas.HelpPage.Controllers
{
    /// <summary>
    /// This controller demonstrates the following:
    /// - Using RouteArea attribute
    /// - RouteArea's area prefix which is like an alias that end users can use to browse
    /// - Supplying default values for route variables. Ex: Here the default action is 'Index'
    /// 
    /// NOTE:
    /// - There is no area registration file (like in Administration\AdministrationAreaRegistration.cs) for HelpPage area. In general, if any 
    ///   area uses only attribute routing on its controllers, then you do not need this file as RouteArea provides the necessary hooks.
    /// </summary>
    [RouteArea("HelpPage", AreaPrefix = "Help")]
    [System.Web.Mvc.Route("{action=Index}/{apiId?}")]
    public class HelpController : Controller
    {
        public HelpController()
            : this(GlobalConfiguration.Configuration)
        {
        }

        public HelpController(HttpConfiguration config)
        {
            Configuration = config;
        }

        public HttpConfiguration Configuration { get; private set; }

        // GET /Help
        // GET /Help/Index
        public ActionResult Index()
        {
            ViewBag.DocumentationProvider = Configuration.Services.GetDocumentationProvider();
            return View(Configuration.Services.GetApiExplorer().ApiDescriptions);
        }

        // GET /Help/Api/<apiId>
        public ActionResult Api(string apiId)
        {
            if (!String.IsNullOrEmpty(apiId))
            {
                HelpPageApiModel apiModel = Configuration.GetHelpPageApiModel(apiId);
                if (apiModel != null)
                {
                    return View(apiModel);
                }
            }

            return View("Error");
        }
    }
}