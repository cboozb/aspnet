using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Security.Authentication.Web;
using System.Threading.Tasks;

namespace WebAuthenticationBrokerPhone
{
    public partial class LoginPage : PhoneApplicationPage
    {
        public static TaskCompletionSource<WebAuthenticationResult> WebAuthenticationResultSource;

        Uri endUri;

        public LoginPage()
        {
            InitializeComponent();
            BackKeyPress += LoginPage_BackKeyPress;
            browserControl.Navigating += browserControl_Navigating;
            browserControl.NavigationFailed += browserControl_NavigationFailed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Uri startUri = new Uri(this.NavigationContext.QueryString["startUri"]);
            this.endUri = new Uri(this.NavigationContext.QueryString["endUri"]);
            this.browserControl.Source = startUri;
        }

        void browserControl_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            WebBrowserNavigationException ex = e.Exception as WebBrowserNavigationException;
            uint error = ex != null ? (uint)ex.StatusCode : 0u;
            WebAuthenticationResult result = new WebAuthenticationResult("", WebAuthenticationStatus.ErrorHttp, error);
            WebAuthenticationResultSource.TrySetResult(result);
            e.Handled = true;
        }

        void browserControl_Navigating(object sender, NavigatingEventArgs e)
        {
            if (IsEndUri(e.Uri))
            {
                WebAuthenticationResult result = new WebAuthenticationResult(e.Uri.AbsoluteUri, WebAuthenticationStatus.Success, 0u);
                WebAuthenticationResultSource.TrySetResult(result); 
            }
        }

        bool IsEndUri(Uri uri)
        {
            UriComponents uriComponents = UriComponents.SchemeAndServer | UriComponents.Path;
            return Uri.Compare(uri, endUri, uriComponents, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) == 0;
        }

        void LoginPage_BackKeyPress(object sender, global::System.ComponentModel.CancelEventArgs e)
        {
            WebAuthenticationResult result = new WebAuthenticationResult("", WebAuthenticationStatus.UserCancel, 0u);
            WebAuthenticationResultSource.TrySetResult(result);
        }
    }
}