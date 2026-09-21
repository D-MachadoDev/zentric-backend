using Microsoft.EntityFrameworkCore;
using Zentric.Infrastructure.Persistence;
using Zentric.Application.Orders.Commands;
using Zentric.Infrastructure.Persistence.Repositories;
using Zentric.Domain.Orders.Ports;
using Zentric.Domain.Logistics.Ports;
using FluentValidation;
using Zentric.Application.Common.Behaviors;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Mapeo de errores a RFC 7807 (Problem Details), exigido por AGENTS.md §3.4.
builder.Services.AddProblemDetails();

// Register DbContext
builder.Services.AddDbContext<ZentricDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Zentric.Infrastructure")));

// Register Repositories and UnitOfWork
builder.Services.AddScoped<Zentric.Application.Common.Ports.IUnitOfWork, Zentric.Infrastructure.Persistence.UnitOfWork>();
builder.Services.AddScoped<Zentric.Infrastructure.Persistence.IDomainEventDispatcher, Zentric.Infrastructure.Persistence.DomainEventDispatcher>();
builder.Services.AddScoped<Zentric.Domain.Users.Ports.IUserRepository, UserRepository>();
builder.Services.AddScoped<Zentric.Domain.Products.Ports.IProductRepository, ProductRepository>();
builder.Services.AddScoped<Zentric.Domain.Inventories.Ports.IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<Zentric.Domain.Warehouses.Ports.IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<Zentric.Domain.Orders.Ports.ICustomerOrderRepository, CustomerOrderRepository>();
builder.Services.AddScoped<Zentric.Domain.Logistics.Ports.IFulfillmentOrderRepository, FulfillmentOrderRepository>();
builder.Services.AddScoped<Zentric.Domain.Returns.Ports.IReturnRequestRepository, ReturnRequestRepository>();
builder.Services.AddScoped<Zentric.Domain.Billing.Ports.IInvoiceRepository, InvoiceRepository>();

// Register Background Services
builder.Services.AddHostedService<Zentric.Infrastructure.BackgroundServices.CheckoutTimeoutService>();

// Register MediatR + validación de entrada (FluentValidation): AGENTS.md §4.3 y SDD/Application §1 y §7.
builder.Services.AddValidatorsFromAssemblyContaining<CreateCartCommand>();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateCartCommand).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});
builder.Services.AddScoped<Zentric.Domain.Returns.Services.ReturnsApprovalService>();
builder.Services.AddScoped<Zentric.Domain.Inventories.Services.InventoryReservationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Middleware global de excepciones: las fallas técnicas no controladas se convierten en Problem Details (AGENTS.md §3.4).
app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
