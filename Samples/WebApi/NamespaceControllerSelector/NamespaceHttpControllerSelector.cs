using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;

namespace NamespaceControllerSelectorSample
{
    public class NamespaceHttpControllerSelector : IHttpControllerSelector
    {
        private const string NamespaceKey = "namespace";

        private readonly DefaultHttpControllerSelector _defaultSelector;
        private readonly Dictionary<string, HttpControllerDescriptor> _controllers; 

        public NamespaceHttpControllerSelector(HttpConfiguration config)
        {
            _defaultSelector = new DefaultHttpControllerSelector(config);
            _controllers = new Dictionary<string, HttpControllerDescriptor>(StringComparer.OrdinalIgnoreCase);

            InitializeControllerDictionary(config);
        }

        private void InitializeControllerDictionary(HttpConfiguration config)
        {
            // Create a lookup table where key is "namespace.type"
            IAssembliesResolver assembliesResolver = config.Services.GetAssembliesResolver();
            IHttpControllerTypeResolver controllersResolver = config.Services.GetHttpControllerTypeResolver();

            ICollection<Type> controllerTypes = controllersResolver.GetControllerTypes(assembliesResolver);
            HashSet<string> duplicates = new HashSet<string>();

            foreach (Type t in controllerTypes)
            {
                var segments = t.Namespace.Split(Type.Delimiter);

                // For the dictionary key, strip "Controller" from the end of the type name.
                // This matches the behavior of DefaultHttpControllerSelector.
                var controllerName = t.Name.Remove(t.Name.Length - DefaultHttpControllerSelector.ControllerSuffix.Length);

                var key = string.Format("{0}.{1}", segments[segments.Length - 1], controllerName);

                if (_controllers.Keys.Contains(key))
                {
                    duplicates.Add(key);
                }
                else
                {
                    _controllers[key] = new HttpControllerDescriptor(config, t.Name, t);  
                }
            }

            foreach (string s in duplicates)
            {
                _controllers.Remove(s);
            }
        }

        // Get the namespace variable from the route data, if present.
        static private string GetNamespace(IHttpRouteData routeData)
        {
            object namespaceName = null;
            if (routeData.Values.TryGetValue(NamespaceKey, out namespaceName))
            {
                return namespaceName as string;
            }
            return null;
        }

        public HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            IHttpRouteData routeData = request.GetRouteData();
            if (routeData == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            string controllerName = _defaultSelector.GetControllerName(request);
            if (controllerName == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            string namespaceName = GetNamespace(routeData);
            if (namespaceName == null)
            {
                // When route data does not contain a value for "{namespace}",
                // fall back to default selection logic.
                return _defaultSelector.SelectController(request);
            }
            
            // Find a matching controller.
            string key = string.Format("{0}.{1}", namespaceName, controllerName);

            HttpControllerDescriptor controllerDescriptor;
            if (_controllers.TryGetValue(key, out controllerDescriptor))
            {
                return controllerDescriptor;
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        public IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
        {
            return _controllers;
        }
    }
}