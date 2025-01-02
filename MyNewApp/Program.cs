using System.Xml.Linq;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Rewrite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITaskService>(new InMemoryTaskService());
var app = builder.Build();

// redirect tasks/123 to /todos/123
// tasks/1 -> /todos/1
app.UseRewriter(new RewriteOptions().AddRedirect("tasks/(.*)", "todos/$1"));
// when receive a request, log the request and response status code
app.Use(async (context, next) =>
{
    Console.WriteLine($"[{context.Request.Method}] {context.Request.Path} ");
    await next();
    Console.WriteLine($"[{context.Response.StatusCode}] {context.Request.Path}");
});
var todos = new List<TodoItem>();

app.MapGet("/", () => "Hello World!");

// return all todos
app.MapGet("/todos", (ITaskService service) => service.GetTodos());
// create a new todo with content validation
app.MapPost("/todos", (TodoItem todo, ITaskService service) =>
{
    service.AddTodo(todo);
    return TypedResults.Created("/todos/{id}", todo);
}).AddEndpointFilter(async (context, next) =>
{
    var taskArgument = context.GetArgument<TodoItem>(0);
    var errors = new Dictionary<string, string[]>();
    if (taskArgument.DueDate < DateTime.Now)
    {
        errors.Add(nameof(TodoItem.DueDate), new[] { "Due date must be in the future" });
    }
    if (taskArgument.IsComplete)
    {
        errors.Add(nameof(TodoItem.IsComplete), new[] { "New tasks cannot be completed" });
    }
    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }
    return await next(context);
});


app.MapGet("/todos/{id}", Results<Ok<TodoItem>, NotFound> (int id, ITaskService service) =>
{
    var todo = service.GetTodoById(id);
    if (todo == null)
    {
        return TypedResults.NotFound();
    }
    return TypedResults.Ok(todo);
});

app.MapDelete("/todos/{id}", Results<NoContent, NotFound> (int id, ITaskService service) =>
{
    service.DeleteToDo(id);
    return TypedResults.NoContent();
});
app.Run();



public record TodoItem(int Id, string Name, DateTime DueDate, bool IsComplete);

interface ITaskService
{
    TodoItem? GetTodoById(int id);
    List<TodoItem> GetTodos();
    TodoItem AddTodo(TodoItem item);
    void DeleteToDo(int id);
}
class InMemoryTaskService : ITaskService
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