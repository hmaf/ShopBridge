
using ShopBridge.Domain.Entities.Commons;

namespace ShopBridge.Domain.Entities;

public class Product : BaseEntity<Guid>
{
    public string Name { get; private set; } = null!;

    public decimal Price { get; private set; }

    public int Stock { get; private set; }

    private Product()
    {
    }

    public Product(
        string name,
        decimal price,
        int stock)
    {
        Id = Guid.NewGuid();
        Name = name;
        Price = price;
        Stock = stock;
    }
}
