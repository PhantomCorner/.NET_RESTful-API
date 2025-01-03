using Microsoft.AspNetCore.Mvc;
using MyNewApp.Models;
using MyNewApp.Services;
using System.Collections.Generic;

namespace MyNewApp.Controllers
{
    [ApiController]
    [Route("todos")]
    public class TodoController : ControllerBase
    {
        private readonly ITaskService _todoService;

        public TodoController(ITaskService todoService)
        {
            _todoService = todoService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<TodoItem>> GetAll()
        {
            var items = _todoService.GetTodos();
            return items;
        }

        [HttpGet("{id}")]
        public IActionResult GetTodoItem(int id)
        {
            var todo = _todoService.GetTodoById(id);
            if (todo == null)
            {
                return NotFound();
            }
            return Ok(todo);
        }

        [HttpPost]
        public IActionResult CreateTodoItem([FromBody] TodoItem todo)
        {
            if (todo.DueDate < DateTime.Now)
            {
                var validationProblemDetails = new ValidationProblemDetails(new Dictionary<string, string[]>
                {
                    { nameof(TodoItem.DueDate), new[] { "Due date must be in the future" } }
                });
                return ValidationProblem(validationProblemDetails);
            }
            if (todo.IsComplete)
            {
                var validationProblemDetails = new ValidationProblemDetails(new Dictionary<string, string[]>
                {
                    { nameof(TodoItem.IsComplete), new[] { "New tasks cannot be completed" } }
                });
                return ValidationProblem(validationProblemDetails);
            }
            var createdTodo = _todoService.AddTodo(todo);
            return Created($"/todos/{createdTodo.Id}", createdTodo);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTodoItem(int id)
        {
            _todoService.DeleteToDo(id);
            return NoContent();
        }

    }
}