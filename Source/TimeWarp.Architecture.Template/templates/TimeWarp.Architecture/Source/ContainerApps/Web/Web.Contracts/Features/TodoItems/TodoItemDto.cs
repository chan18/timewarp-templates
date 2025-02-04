namespace TimeWarp.Architecture.Features.TodoItems;

[CreateCommand, UpdateCommand, DeleteCommand, GetQuery, GetListQuery]
public class TodoItemDto
{
  public Guid TodoItemId { get; set; }

  public Guid TodoListId { get; set; }

  public string Title { get; set; } = string.Empty;

  public bool Done { get; set; }

  public int Priority { get; set; }

  public string Note { get; set; } = string.Empty;
}
