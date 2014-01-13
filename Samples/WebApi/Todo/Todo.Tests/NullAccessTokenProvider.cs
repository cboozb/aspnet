using System.Threading.Tasks;
using Account.Client;

namespace Todo.Tests
{
    class NullAccessTokenProvider : IAccessTokenProvider
    {
        public Task<string> GetTokenAsync()
        {
            return Task.FromResult<string>(null);
        }
    }
}
