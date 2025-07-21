using Microsoft.AspNetCore.Mvc;
using MenuService.Application.Commands;
using MenuService.Application.Handlers;
using MenuService.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace MenuService.API.Controllers;

[ApiController]
[Route("/[controller]/[action]")]
public class MenuServiceController(MenuServiceHandler handler) : ControllerBase
{
    [Authorize(Roles = "Admin, Cozinha")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMenuItemCommand command)
    {
        await handler.Handle(command);
        return Ok(new { message = "Item criado com sucesso", item = command });
    }

    [Authorize(Roles = "Admin, Cozinha")]
    [HttpGet]
    public async Task<IEnumerable<MenuItem>> GetAll() => await handler.GetAllAsync();

    [Authorize(Roles = "Admin, Cozinha")]
    [HttpGet("{id}")]
    public async Task<ActionResult<MenuItem>> GetById(string id)
    {
        var item = await handler.GetByIdAsync(id);
        if (item is null) return NotFound();
        return item;
    }

    [Authorize(Roles = "Admin, Cozinha")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateMenuItemCommand command)
    {
        await handler.UpdateAsync(id, command);
        return NoContent();
    }

    [Authorize(Roles = "Admin, Cozinha")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await handler.DeleteAsync(id);
        return NoContent();
    }
}