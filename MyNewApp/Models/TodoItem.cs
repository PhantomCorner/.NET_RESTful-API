namespace MyNewApp.Models;
public record TodoItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsComplete { get; set; }
}