using Account.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Todo.Client;
using Windows.UI.Xaml.Controls;

namespace Todo.WindowsStore
{
    public static class ClientFactory
    {
        // HTTPS address is required by the WebAuthenticationBroker
        public const string BaseAddress = "https://securetodo.azurewebsites.net/";

        public static HttpClient CreateHttpClient()
        {
            AppSettings settings = new AppSettings();
            AccessTokenProvider loginProvider = new AccessTokenProvider();
            OAuth2BearerTokenHandler oauth2Handler = new OAuth2BearerTokenHandler(settings, loginProvider);
            HttpClient httpClient = HttpClientFactory.Create(oauth2Handler);
            httpClient.BaseAddress = new Uri(BaseAddress);
            httpClient.Timeout = TimeSpan.FromDays(1);
            return httpClient;
        }
        
        public static AccountClient CreateAccountClient()
        {
            return new AccountClient(CreateHttpClient());
        }

        public static TodoClient CreateTodoClient()
        {
            return new TodoClient(CreateHttpClient());
        }
    }
}
