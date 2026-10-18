using MediatR;
using Microsoft.AspNetCore.Mvc;
using PricingService.Api.Commands;

namespace PricingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PricingController : ControllerBase
{
    private readonly IMediator _mediator;

    public PricingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CalculatePriceCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return new JsonResult(result);
        }
        catch (FluentValidation.ValidationException ex)
        {
            return BadRequest(ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }
    }
}
