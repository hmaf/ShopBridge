using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShopBridge.Application.Features.Products.Queries.GetProducts;

namespace ShopBridge.Api.Controller;

[ApiController]
[Route("api/Product")]
public class ProductController : BaseController
{
    public ProductController(ISender sender) : base(sender)
    {
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetProductsQuery(), cancellationToken));
}
