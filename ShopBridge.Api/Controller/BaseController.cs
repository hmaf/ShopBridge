using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ShopBridge.Api.Controller;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected readonly ISender _sender;

    protected BaseController(ISender sender)
    {
        _sender = sender;
    }
}
