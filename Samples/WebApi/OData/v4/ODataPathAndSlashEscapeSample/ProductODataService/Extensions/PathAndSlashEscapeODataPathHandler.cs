using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using System.Web.OData.Routing;
using Microsoft.OData.Edm;

namespace ProductODataService.Extensions
{
    public class PathAndSlashEscapeODataPathHandler : DefaultODataPathHandler
    {
        private const string Quotation = "%27";

        public override ODataPath Parse(IEdmModel model, string serviceRoot, string odataPath)
        {
            if (!odataPath.Contains(Quotation))
            {
                return base.Parse(model, serviceRoot, odataPath);
            }

            var pathBuilder = new StringBuilder();
            var queryStringIndex = odataPath.IndexOf('?');
            if (queryStringIndex == -1)
            {
                EscapeSlashString(odataPath, pathBuilder);
            }
            else
            {
                EscapeSlashString(odataPath.Substring(0, queryStringIndex), pathBuilder);
                pathBuilder.Append(odataPath.Substring(queryStringIndex));
            }
            return base.Parse(model, serviceRoot, pathBuilder.ToString());
        }

        private StringBuilder EscapeSlashString(string uri, StringBuilder pathBuilder)
        {
            const string slash = "%2F";
            const string backSlash = "%5C";

            var startIndex = uri.IndexOf(Quotation, StringComparison.OrdinalIgnoreCase);
            var endIndex = uri.IndexOf(Quotation, startIndex + Quotation.Length, StringComparison.OrdinalIgnoreCase);
            if (startIndex == -1 || endIndex == -1)
            {
                return pathBuilder.Append(uri);
            }

            endIndex = endIndex + Quotation.Length;
            pathBuilder.Append(uri.Substring(0, startIndex));
            for (var i = startIndex; i < endIndex; ++i)
            {
                switch (uri[i])
                {
                    case '/':
                        pathBuilder.Append(slash);
                        break;
                    case '\\':
                        pathBuilder.Append(backSlash);
                        break;
                    default:
                        pathBuilder.Append(uri[i]);
                        break;
                }
            }
            EscapeSlashString(uri.Substring(endIndex), pathBuilder);
            return pathBuilder;
        }
    }
}