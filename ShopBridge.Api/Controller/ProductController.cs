using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShopBridge.Application.Features.Products.Commands.CreateProduct;
using ShopBridge.Application.Features.Products.Queries.GetProducts;

namespace ShopBridge.Api.Controller;

[ApiController]
[Route("api/Product")]
public class ProductController : BaseController
{
    public ProductController(ISender sender) : base(sender)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetProductsQuery(), cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductCommand command, 
        CancellationToken cancellationToken)
    {
        var productId = await _sender.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetAll), new { id = productId });
    }
        
}
