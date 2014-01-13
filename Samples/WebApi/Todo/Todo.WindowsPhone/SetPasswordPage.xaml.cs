using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Account.Client;
using WebApi.Client;

namespace Todo.WindowsPhone
{
    public partial class SetPasswordPage : PhoneApplicationPage
    {
        public SetPasswordPage()
        {
            InitializeComponent();
        }

        private async void SetPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            ClearErrors();
            SetPassword setPassword = new SetPassword()
            {
                NewPassword = NewPasswordBox.Password,
                ConfirmPassword = ConfirmPasswordBox.Password
            };

            HttpResult result;
            using (AccountClient accountClient = ClientFactory.CreateAccountClient())
            {
                result = await accountClient.SetPasswordAsync(setPassword);
            }
            
            if (result.Succeeded)
            {
                this.NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative)); 
            }
            else
            {
                DisplayErrors(result.Errors);
            }
            ClearPasswords();
        }

        private void ClearPasswords()
        {
            NewPasswordBox.Password = "";
            ConfirmPasswordBox.Password = "";
        }

        private void DisplayErrors(IEnumerable<string> errors)
        {
            foreach (string error in errors)
            {
                ErrorList.Items.Add(error);
            }
        }

        private void ClearErrors()
        {
            ErrorList.Items.Clear();
        }
    }
}