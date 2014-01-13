using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Todo.Client;
using WebApi.Client;
using Todo.Client.Models;

namespace Todo.WindowsPhone
{
    public partial class AddTodoItemPage : PhoneApplicationPage
    {
        public const string TodoListId = "todoListId";

        int todoListId;

        public AddTodoItemPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.todoListId = int.Parse(this.NavigationContext.QueryString[TodoListId]);
        }

        private async void OkButton_Click(object sender, EventArgs e)
        {
            TodoItem todoItem = new TodoItem()
            {
                Title = TitleTextBox.Text,
                TodoListId = todoListId
            };

            HttpResult<TodoItem> result;
            using (TodoClient todoClient = ClientFactory.CreateTodoClient())
            {
                result = await todoClient.AddTodoItemAsync(todoItem);
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

        public static Uri GetNavigationUri(int todoListId)
        {
            string uri = String.Format("/AddTodoItemPage.xaml?{0}={1}", TodoListId, todoListId);
            return new Uri(uri, UriKind.Relative);
        }
    }
}