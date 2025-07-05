namespace MenuService.Domain.Entities;

public class MenuItem
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public bool IsAvailable { get; private set; }

    public MenuItem(string name, string description, decimal price, bool isAvailable)
    {
        Name = name;
        Description = description;
        Price = price;
        IsAvailable = isAvailable;
    }

    public void Update(string name, string description, decimal price, bool isAvailable)
    {
        Name = name;
        Description = description;
        Price = price;
        IsAvailable = isAvailable;
    }
}