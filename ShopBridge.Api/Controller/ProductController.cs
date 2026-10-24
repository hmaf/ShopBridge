using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopBridge.Infra.Persistence;

namespace ShopBridge.Api.Controller;

[ApiController]
[Route("api/Product")]
public class ProductController(AppDbContext db) : ControllerBase
{

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var products = await db.Products.AsNoTracking().ToListAsync();

        return Ok(products);
    }
}
