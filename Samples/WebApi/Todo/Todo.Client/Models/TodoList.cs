using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Client.Models
{
    public class TodoList
    {
        public int TodoListId { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public Collection<TodoItem> Todos { get; set; }
    }
}
