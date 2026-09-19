using Microsoft.EntityFrameworkCore;
using Zentric.Infrastructure.Persistence;
using Zentric.Application.Orders.Commands;
using Zentric.Infrastructure.Repositories;
using Zentric.Application.Orders.Ports;
using Zentric.Application.Logistics.Ports;
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

// Register Repositories
builder.Services.AddScoped<ICustomerOrderRepository, CustomerOrderRepository>();
builder.Services.AddScoped<IFulfillmentOrderRepository, FulfillmentOrderRepository>();

// Register MediatR + validación de entrada (FluentValidation): AGENTS.md §4.3 y SDD/Application §1 y §7.
builder.Services.AddValidatorsFromAssemblyContaining<CreateCartCommand>();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateCartCommand).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

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
