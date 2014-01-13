using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Client
{
    public interface IAccessTokenProvider
    {
        Task<string> GetTokenAsync();
    }
}
