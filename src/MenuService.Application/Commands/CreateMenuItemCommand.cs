namespace MenuService.Application.Commands;

public record CreateMenuItemCommand(string Name, string Description, decimal Price, bool IsAvailable);