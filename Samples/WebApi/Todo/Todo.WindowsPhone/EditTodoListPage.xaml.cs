using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Todo.Client.Models;
using WebApi.Client;
using Todo.Client;
using Todo.WindowsPhone.Models;

namespace Todo.WindowsPhone
{
    public partial class EditTodoListPage : PhoneApplicationPage
    {
        public const string TodoListId = "id";
        public const string TodoListTitle = "title";

        string todoListTitle;
        int todoListId;

        public EditTodoListPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.todoListTitle = this.NavigationContext.QueryString[TodoListTitle];
            this.TitleTextBox.Text = this.todoListTitle;
            this.todoListId = int.Parse(this.NavigationContext.QueryString[TodoListId]);
        }

        private async void OkButton_Click(object sender, EventArgs e)
        {
            TodoList todoList = new TodoList() { TodoListId = todoListId, Title = TitleTextBox.Text };

            HttpResult result;
            using (TodoClient todoClient = ClientFactory.CreateTodoClient())
            {
                result = await todoClient.UpdateTodoListAsync(todoList);
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

        public static Uri GetNavigationUri(TodoListModel todoList)
        {
            string title = Uri.EscapeDataString(todoList.Title);
            int todoListId = todoList.TodoListId;
            string uri = String.Format("/EditTodoListPage.xaml?{0}={1}&{2}={3}", TodoListTitle, title, TodoListId, todoListId);
            return new Uri(uri, UriKind.Relative);
        }
    }
}