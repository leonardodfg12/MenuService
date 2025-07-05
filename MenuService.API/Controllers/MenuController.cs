using Microsoft.AspNetCore.Mvc;
using MenuService.Application.Commands;
using MenuService.Application.Handlers;
using MenuService.Domain.Entities;

namespace MenuService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly MenuServiceHandler _handler;

    public MenuController(MenuServiceHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMenuItemCommand command)
    {
        await _handler.Handle(command);
        return Ok();
    }

    [HttpGet]
    public async Task<IEnumerable<MenuItem>> GetAll() => await _handler.GetAllAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<MenuItem>> GetById(string id)
    {
        var item = await _handler.GetByIdAsync(id);
        if (item is null) return NotFound();
        return item;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateMenuItemCommand command)
    {
        await _handler.UpdateAsync(id, command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _handler.DeleteAsync(id);
        return NoContent();
    }
}