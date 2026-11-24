
using ShopBridge.Domain.Entities.Commons;
using ShopBridge.Domain.Exceptions;

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
        ValidateInformation(
            name,
            price,
            stock);

        Id = Guid.NewGuid();
        Name = name;
        Price = price;
        Stock = stock;
    }

    public void UpdateInformation(
        string name,
        decimal price,
        int stock)
    {
        ValidateInformation(
            name,
            price, 
            stock);

        Name = name;
        Price = price;
        Stock = stock;
    }

    private void ValidateInformation(
        string name,
        decimal price,
        int stock)
    {
        if(string.IsNullOrWhiteSpace(name)
            || name.Length < 2
            || name.Length > 200)
        {
            BusinessException.Throw(
                611241205,
                "Product name must be between 2 and 200 characters.",
                BusinessErrorType.Validation);
        }

        if(price <= 0)
        {
            BusinessException.Throw(
                611241208,
                "Product price must be greater than zero.",
                BusinessErrorType.Validation);
        }

        if(stock < 0)
        {
            BusinessException.Throw(
                611241210,
                "Product stock cannot be negative.",
                BusinessErrorType.Validation);
        }
    }
}
