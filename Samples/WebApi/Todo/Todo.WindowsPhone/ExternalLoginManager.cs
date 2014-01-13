using System;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;

namespace Todo.WindowsPhone
{
    public static class ExternalLoginManager
    {
        public static async Task<ExternalLoginResult> GetExternalAccessTokenAsync(string externalLoginUri)
        {
            Uri authorizationRequestUri = new Uri(new Uri(ClientFactory.BaseAddress), externalLoginUri);
           
            Uri endUri = new Uri(authorizationRequestUri, "/");
            WebAuthenticationResult authenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                WebAuthenticationOptions.None, 
                authorizationRequestUri, 
                endUri);
            ExternalLoginResult loginExternalResult = new ExternalLoginResult() { WebAuthenticationResult = authenticationResult };
            if (authenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
            {
                Uri responseDataUri = new Uri(authenticationResult.ResponseData);
                string fragment = responseDataUri.Fragment;
                if (fragment != null && fragment.Length > 0)
                {
                    loginExternalResult.AccessToken = GetAccessTokenFromFragment(fragment);
                }
            }
            return loginExternalResult;
        }

        static string GetAccessTokenFromFragment(string fragment)
        {
            if (fragment != null)
            {
                fragment = fragment.TrimStart('#');
                string[] values = fragment.Split('&');
                foreach (string value in values)
                {
                    string[] nameValuePair = value.Split('=');
                    if (nameValuePair.Length == 2 && nameValuePair[0].Equals("access_token", StringComparison.OrdinalIgnoreCase))
                    {
                        return Uri.UnescapeDataString(nameValuePair[1]);
                    }
                }
            }
            return null;
        }
    }
}
