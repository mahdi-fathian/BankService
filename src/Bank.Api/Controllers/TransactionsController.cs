using Bank.Application.DTOs;
using Bank.Application.Handlers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers;

/// <summary>
///     API Controller for Transaction operations
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class TransactionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TransactionsController> _logger;

    public TransactionsController(IMediator mediator, ILogger<TransactionsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    ///     Transfer money between accounts
    /// </summary>
    [HttpPost("transfer")]
    [ProducesResponseType(typeof(TransferResultOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "API: Transfer from {SourceAccountId} to {TargetAccountId}",
            request.SourceAccountId,
            request.TargetAccountId);

        var result = await _mediator.Send(request, cancellationToken);

        if (!result.Success)
        {
            return BadRequest(new { Message = result.ErrorMessage });
        }

        return Ok(result);
    }

    /// <summary>
    ///     Get transaction by reference number
    /// </summary>
    [HttpGet("reference/{referenceNumber}")]
    [ProducesResponseType(typeof(TransactionOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByReference(string referenceNumber, CancellationToken cancellationToken)
    {
        _logger.LogInformation("API: Getting transaction by reference: {ReferenceNumber}", referenceNumber);

        // Note: In a real implementation, you would add a GetByReferenceNumberHandler
        return Ok(new { Message = "Transaction lookup by reference number" });
    }

    /// <summary>
    ///     Get transactions for an account
    /// </summary>
    [HttpGet("account/{accountId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<TransactionOutput>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByAccount(Guid accountId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("API: Getting transactions for account: {AccountId}", accountId);

        // Note: In a real implementation, you would add a GetTransactionsByAccountHandler
        return Ok(new List<TransactionOutput>());
    }
}
