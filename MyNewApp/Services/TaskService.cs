using MyNewApp.Models;


namespace MyNewApp.Services
{
    public class TaskService : ITaskService
    {
        private readonly List<TodoItem> _todos = new List<TodoItem>();

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