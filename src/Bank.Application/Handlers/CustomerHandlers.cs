using Bank.Application.DTOs;
using Bank.Application.Interfaces;
using Bank.Domain.Common;
using Bank.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bank.Application.Handlers;

/// <summary>
///     Request for creating a new customer
/// </summary>
public class CreateCustomerRequest : IRequest<CustomerOutput>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string NationalCode { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
}

/// <summary>
///     Handler for creating a new customer
/// </summary>
public class CreateCustomerHandler : IRequestHandler<CreateCustomerRequest, CustomerOutput>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<CreateCustomerHandler> _logger;

    public CreateCustomerHandler(
        ICustomerRepository customerRepository,
        ILogger<CreateCustomerHandler> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<CustomerOutput> Handle(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new customer with NationalCode: {NationalCode}", request.NationalCode);

        if (await _customerRepository.NationalCodeExistsAsync(request.NationalCode, cancellationToken))
        {
            throw new DomainException("Customer with this national code already exists");
        }

        var customer = Customer.Create(
            request.FirstName,
            request.LastName,
            request.NationalCode,
            request.Email,
            request.PhoneNumber,
            request.DateOfBirth);

        await _customerRepository.AddAsync(customer, cancellationToken);

        _logger.LogInformation("Customer created successfully with ID: {CustomerId}", customer.Id);

        return MapToOutput(customer);
    }

    private static CustomerOutput MapToOutput(Customer customer)
    {
        return new CustomerOutput
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            FullName = customer.FullName,
            NationalCode = customer.NationalCode.ToString(),
            Email = customer.Email.ToString(),
            PhoneNumber = customer.PhoneNumber.ToString(),
            DateOfBirth = customer.DateOfBirth,
            Status = customer.Status.ToString(),
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }
}

/// <summary>
///     Request for getting customer by ID
/// </summary>
public class GetCustomerByIdRequest : IRequest<CustomerOutput?>
{
    public Guid CustomerId { get; set; }
}

/// <summary>
///     Handler for getting customer by ID
/// </summary>
public class GetCustomerByIdHandler : IRequestHandler<GetCustomerByIdRequest, CustomerOutput?>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<GetCustomerByIdHandler> _logger;

    public GetCustomerByIdHandler(
        ICustomerRepository customerRepository,
        ILogger<GetCustomerByIdHandler> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<CustomerOutput?> Handle(GetCustomerByIdRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting customer by ID: {CustomerId}", request.CustomerId);

        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);

        return customer is null ? null : MapToOutput(customer);
    }

    private static CustomerOutput MapToOutput(Customer customer)
    {
        return new CustomerOutput
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            FullName = customer.FullName,
            NationalCode = customer.NationalCode.ToString(),
            Email = customer.Email.ToString(),
            PhoneNumber = customer.PhoneNumber.ToString(),
            DateOfBirth = customer.DateOfBirth,
            Status = customer.Status.ToString(),
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }
}

/// <summary>
///     Request for getting all customers
/// </summary>
public class GetAllCustomersRequest : IRequest<IEnumerable<CustomerOutput>>
{
    public string? SearchTerm { get; set; }
    public string? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
///     Handler for getting all customers
/// </summary>
public class GetAllCustomersHandler : IRequestHandler<GetAllCustomersRequest, IEnumerable<CustomerOutput>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<GetAllCustomersHandler> _logger;

    public GetAllCustomersHandler(
        ICustomerRepository customerRepository,
        ILogger<GetAllCustomersHandler> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<CustomerOutput>> Handle(GetAllCustomersRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all customers");

        var customers = await _customerRepository.GetAllAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLowerInvariant();
            customers = customers.Where(c =>
                c.FullName.ToLowerInvariant().Contains(searchTerm) ||
                c.NationalCode.ToString().Contains(searchTerm) ||
                c.Email.ToString().Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (Enum.TryParse<CustomerStatus>(request.Status, true, out var status))
            {
                customers = customers.Where(c => c.Status == status);
            }
        }

        var skip = (request.Page - 1) * request.PageSize;
        customers = customers.Skip(skip).Take(request.PageSize);

        return customers.Select(MapToOutput);
    }

    private static CustomerOutput MapToOutput(Customer customer)
    {
        return new CustomerOutput
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            FullName = customer.FullName,
            NationalCode = customer.NationalCode.ToString(),
            Email = customer.Email.ToString(),
            PhoneNumber = customer.PhoneNumber.ToString(),
            DateOfBirth = customer.DateOfBirth,
            Status = customer.Status.ToString(),
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }
}
