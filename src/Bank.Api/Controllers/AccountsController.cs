using Bank.Application.DTOs;
using Bank.Application.Handlers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers;

/// <summary>
///     API Controller for Account operations
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(IMediator mediator, ILogger<AccountsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    ///     Create a new account
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AccountOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateAccountRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("API: Creating new account for customer: {CustomerId}", request.CustomerId);

        var result = await _mediator.Send(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    ///     Get account by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AccountOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("API: Getting account by ID: {AccountId}", id);

        var result = await _mediator.Send(new GetBalanceRequest { AccountId = id }, cancellationToken);

        if (result is null)
        {
            return NotFound(new { Message = "Account not found" });
        }

        return Ok(result);
    }

    /// <summary>
    ///     Get account balance
    /// </summary>
    [HttpGet("{id:guid}/balance")]
    [ProducesResponseType(typeof(BalanceOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBalance(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("API: Getting balance for account: {AccountId}", id);

        var result = await _mediator.Send(new GetBalanceRequest { AccountId = id }, cancellationToken);

        if (result is null)
        {
            return NotFound(new { Message = "Account not found" });
        }

        return Ok(result);
    }

    /// <summary>
    ///     Deposit money into account
    /// </summary>
    [HttpPost("{id:guid}/deposit")]
    [ProducesResponseType(typeof(TransactionOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Deposit(Guid id, [FromBody] DepositRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("API: Deposit to account: {AccountId}", id);

        request.AccountId = id;
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    ///     Withdraw money from account
    /// </summary>
    [HttpPost("{id:guid}/withdraw")]
    [ProducesResponseType(typeof(TransactionOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Withdraw(Guid id, [FromBody] WithdrawRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("API: Withdraw from account: {AccountId}", id);

        request.AccountId = id;
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }
}
