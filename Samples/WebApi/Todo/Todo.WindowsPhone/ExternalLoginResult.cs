using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;

namespace Todo.WindowsPhone
{
    public class ExternalLoginResult
    {
        public WebAuthenticationResult WebAuthenticationResult { get; set; }
        public string AccessToken { get; set; }
    }
}
