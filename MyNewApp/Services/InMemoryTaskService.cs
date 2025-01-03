using MyNewApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace MyNewApp.Services
{
    public class InMemoryTaskService : ITaskService
    {
        private readonly List<TodoItem> _todos = [];

        public TodoItem AddTodo(TodoItem item)
        {
            _todos.Add(item);
            return item;
        }

        public void DeleteToDo(int id)
        {
            _todos.RemoveAll(task => id == task.Id);
        }

        public TodoItem? GetTodoById(int id)
        {
            return _todos.SingleOrDefault(task => id == task.Id);
        }

        public List<TodoItem> GetTodos()
        {
            return _todos;
        }
    }
}