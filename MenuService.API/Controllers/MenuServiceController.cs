using Microsoft.AspNetCore.Mvc;
using MenuService.Application.Commands;
using MenuService.Application.Handlers;
using MenuService.Domain.Entities;

namespace MenuService.API.Controllers;

[ApiController]
[Route("/[controller]/[action]")]
public class MenuServiceController(MenuServiceHandler handler) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMenuItemCommand command)
    {
        await handler.Handle(command);
        return Ok(new { message = "Item criado com sucesso", item = command });
    }

    [HttpGet]
    public async Task<IEnumerable<MenuItem>> GetAll() => await handler.GetAllAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<MenuItem>> GetById(string id)
    {
        var item = await handler.GetByIdAsync(id);
        if (item is null) return NotFound();
        return item;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateMenuItemCommand command)
    {
        await handler.UpdateAsync(id, command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await handler.DeleteAsync(id);
        return NoContent();
    }
}