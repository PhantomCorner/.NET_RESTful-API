namespace MyNewApp.Services;
using MyNewApp.Models;

public interface ITaskService
{
    TodoItem? GetTodoById(int id);
    List<TodoItem> GetTodos();
    TodoItem AddTodo(TodoItem item);
    void DeleteToDo(int id);
}



