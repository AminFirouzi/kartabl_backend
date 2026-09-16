using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kartabl_Backend.Presentation.Controllers;

[ApiController]
[Route("api/v1/auth")] // Or [Route("api/v1/[controller]")]
public abstract class ApiController : ControllerBase
{
    private ISender? _mediator;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}