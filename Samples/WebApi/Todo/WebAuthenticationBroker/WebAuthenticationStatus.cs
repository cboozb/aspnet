using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows.Security.Authentication.Web
{
    /// <summary>
    /// This class mimics the functionality provided by WebAuthenticationStatus available in Win8.
    /// </summary>
    public enum WebAuthenticationStatus
    {
        Success,
        UserCancel,
        ErrorHttp
    }
}
