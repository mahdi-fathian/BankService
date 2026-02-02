using Bank.Application.DTOs;
using Bank.Application.Handlers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers;

/// <summary>
///     API Controller for Customer operations
///     RESTful, versioned endpoints
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(IMediator mediator, ILogger<CustomersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    ///     Create a new customer
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CustomerOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("API: Creating new customer");

        var result = await _mediator.Send(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    ///     Get customer by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("API: Getting customer by ID: {CustomerId}", id);

        var result = await _mediator.Send(new GetCustomerByIdRequest { CustomerId = id }, cancellationToken);

        if (result is null)
        {
            return NotFound(new { Message = "Customer not found" });
        }

        return Ok(result);
    }

    /// <summary>
    ///     Get all customers with optional filtering
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CustomerOutput>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? searchTerm,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("API: Getting all customers");

        var result = await _mediator.Send(new GetAllCustomersRequest
        {
            SearchTerm = searchTerm,
            Status = status,
            Page = page,
            PageSize = pageSize
        }, cancellationToken);

        return Ok(result);
    }
}
