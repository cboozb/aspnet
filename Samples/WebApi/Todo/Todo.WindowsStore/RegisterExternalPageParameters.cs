using Account.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo.WindowsStore
{
    public class RegisterExternalPageParameters
    {
        public TaskCompletionSource<string> AccessTokenSource { get; set; }
        public string ExternalLoginUri { get; set; }
        public UserInfo UserInfo { get; set; }
    }
}
