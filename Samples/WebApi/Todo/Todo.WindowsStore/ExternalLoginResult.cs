using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;

namespace Todo.WindowsStore
{
    public class ExternalLoginResult
    {
        public WebAuthenticationResult WebAuthenticationResult { get; set; }
        public string AccessToken { get; set; }
    }
}
