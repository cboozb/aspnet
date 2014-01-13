using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Todo.WindowsPhone
{
    public partial class AccountDetailsPage : PhoneApplicationPage
    {
        public AccountDetailsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.AccountNameTextBlock.Text = this.NavigationContext.QueryString["accountName"];
            this.UsernameTextBlock.Text = this.NavigationContext.QueryString["username"];
        }
    }
}