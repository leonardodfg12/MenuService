using System.Diagnostics.CodeAnalysis;

namespace MenuService.Application.Commands;

[ExcludeFromCodeCoverage]
public record CreateMenuItemCommand(string Name, string Description, decimal Price, bool IsAvailable);