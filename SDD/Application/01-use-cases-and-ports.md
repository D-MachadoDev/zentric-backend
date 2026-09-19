# Capa de Aplicación - Casos de Uso y Puertos

Este documento detalla los comandos (CQRS), queries y puertos de salida que definen los casos de uso para ZENTRIC.md.

## 1. Patrón Arquitectónico
Se utiliza **CQRS** (Command Query Responsibility Segregation) con la librería `MediatR`. Todo caso de uso se define como un `IRequest<Result>` o `IRequest<Result<T>>`.
Las validaciones de entrada se realizan con `FluentValidation` en el Pipeline de MediatR, antes de que el Command toque el Dominio.

## 2. Puertos de Salida (Output Ports)
Se definen interfaces (Repositorios) en la capa de aplicación, que serán implementadas por la Infraestructura.
- `IUserRepository`, `IProductRepository`, `IInventoryRepository`, `IWarehouseRepository`, `ICustomerOrderRepository`, `IFulfillmentOrderRepository`, `IReturnRequestRepository`, `IInvoiceRepository`.

## 3. Casos de Uso (Commands) - Dominio de Pedidos (Customer Orders)
- **`CreateCartCommand`**: Inicializa un `CustomerOrder` (Cart).
- **`AddOrderItemCommand`**: Añade un item al carrito. Valida stock físico disponible usando `IInventoryRepository`.
- **`CheckoutOrderCommand`**: Pasa de Cart a PendingPayment.

> `[CONTRADICCIÓN]` **([H-10](../00-bootstrap/spec-conformance-matrix.md), 2026-09-18)** Las tres afirmaciones anteriores **no se corresponden
> con el código**:
> - `CreateCartCommand` y `AddOrderItemCommand` **sí existen** (`Zentric.Application/Orders/Commands/`).
> - `AddOrderItemCommand` **no valida stock**: no existe `IInventoryRepository` en todo el repositorio.
> - `CheckoutOrderCommand` **no existe** (el agregado sí expone `Checkout()`, pero no hay caso de uso ni endpoint).
>
> El estado real deja además `[PED-01](../Domain/06-business-rules.md)` (reserva preventiva + liberación a los 15 minutos) **sin implementar**.
> Se corrige el texto cuando el Owner resuelva [Q-04](../00-bootstrap/questions-for-owner.md#q-04-c-04-cart-es-un-estado-de-customerorder) (¿el carrito reserva stock?) — **no se inventa aquí**.

## 4. Casos de Uso (Commands) - Logística (Fulfillment)
- **`CreateFulfillmentOrderCommand`**: Genera un `FulfillmentOrder` derivado de un pedido pagado.
- **`DispatchFulfillmentCommand`**: Marca como despachado.

> `[CONTRADICCIÓN]` **(2026-09-18)** `CreateFulfillmentOrderCommand` existe
> (`Zentric.Application/Logistics/Commands/`), pero **no valida que el pedido esté pagado** y
> `DispatchFulfillmentCommand` **no existe**.

## 5. Casos de Uso (Commands) - Devoluciones (Returns)
- **`RequestReturnCommand`**: Crea el request inicial.
- **`ApproveReturnCommand`**: Flujo de aprobación dual (logística + vendor) que interactúa con el inventario para invocar `ReturnToUsedStock()`.

## 6. Casos de Uso (Commands) - Catálogo (Products)
- **`CreateProductCommand`**: Crea producto y sus variantes forzosas.
- **`PublishProductCommand`**: Publica el producto.

## 7. Validaciones
Todas las entradas de los usuarios, como emails, GUIDs vacíos, cantidades negativas o valores monetarios, deben validarse con FluentValidation antes del handler.

> `[CONFIRMADO]` **(SPEC-007, 2026-09-18)** Implementado: `ValidationBehavior<TRequest, TResponse>`
> (`Zentric.Application/Common/Behaviors/ValidationBehavior.cs`) se ejecuta antes del handler y
> devuelve `Result.Failure` (sin excepción) cuando la entrada es inválida.
>
> | Comando | Validador | Reglas (espejo de las guardas del dominio) |
> |---|---|---|
> | `CreateCartCommand` | `CreateCartCommandValidator` | `BuyerId` obligatorio |
> | `AddOrderItemCommand` | `AddOrderItemCommandValidator` | `OrderId`/`VariantId` obligatorios, `Quantity > 0`, `UnitPrice >= 0`, `Currency` ISO de 3 caracteres |
> | `CreateFulfillmentOrderCommand` | `CreateFulfillmentOrderCommandValidator` | `CustomerOrderId` y `VendorId` obligatorios |
>
> Registro: `AddValidatorsFromAssemblyContaining<CreateCartCommand>()` + `cfg.AddOpenBehavior(typeof(ValidationBehavior<,>))`
> en `Zentric.Api/Program.cs`. Evidencia: 21 pruebas unitarias + 7 de integración DI
> ([verification-baseline.md :11](../00-bootstrap/verification-baseline.md#11-sexta-iteracion-spec-007-validacion-de-entrada-rfc-7807-e-higiene-2026-09-18), 206/206).
>
> `[PENDIENTE]` Los comandos **especificados pero no implementados** ([:3](../00-bootstrap/verification-baseline.md#3-formato-lint-y-analisis-estatico)–[:6](../00-bootstrap/verification-baseline.md#6-como-repetir-esta-linea-base)) no tienen validador
> porque no existen: `CheckoutOrderCommand`, `DispatchFulfillmentCommand`, `RequestReturnCommand`,
> `ApproveReturnCommand`, `CreateProductCommand`, `PublishProductCommand`.

