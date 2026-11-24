using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShopBridge.Application.Abstractions.Logging;
using ShopBridge.Application.Common.Mapping;
using ShopBridge.Application.Features.Products.Commands.CreateProduct;
using ShopBridge.Application.Features.Products.Commands.UpdateProduct;
using ShopBridge.Application.Features.Products.DTOs;
using ShopBridge.Application.Features.Products.Queries.GetProductById;
using ShopBridge.Application.Features.Products.Queries.GetProducts;
using System.Diagnostics;

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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetProductByIdQuery(id),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductCommand command, 
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetById), 
            new { id = result.Data },
            result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
    Guid id,
    [FromBody] UpdateProductCommand command,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            command with { id = id },
            cancellationToken);

        return Ok(result);
    }
}
