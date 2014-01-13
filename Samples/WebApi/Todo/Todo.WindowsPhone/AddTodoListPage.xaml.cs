using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WebApi.Client;
using Todo.Client.Models;
using Todo.Client;

namespace Todo.WindowsPhone
{
    public partial class AddTodoListPage : PhoneApplicationPage
    {
        public AddTodoListPage()
        {
            InitializeComponent();
        }

        private async void OkButton_Click(object sender, EventArgs e)
        {
            TodoList todoList = new TodoList() { Title = TitleTextBox.Text, UserId = "unknown"};

            HttpResult<TodoList> result;
            using (TodoClient todoClient = ClientFactory.CreateTodoClient())
            {
                result = await todoClient.AddTodoListAsync(todoList);
            }

            if (result.Succeeded)
            {
                this.NavigationService.GoBack();
            }
            else
            {
                ErrorDialog.ShowErrors(result.Errors);
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.NavigationService.GoBack();
        }

        public static Uri GetNavigationUri()
        {
            return new Uri("/AddTodoListPage.xaml", UriKind.Relative);
        }
    }
}