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

> `[CONTRADICCIÓN]` **([H-10](../SDD.md), 2026-09-18)** Las tres afirmaciones anteriores **no se corresponden
> con el código**:
> - `CreateCartCommand` y `AddOrderItemCommand` **sí existen** (`Zentric.Application/Orders/Commands/`).
> - `AddOrderItemCommand` **no valida stock**: no existe `IInventoryRepository` en todo el repositorio.
> - `CheckoutOrderCommand` **no existe** (el agregado sí expone `Checkout()`, pero no hay caso de uso ni endpoint).
>
> El estado real deja además `[PED-01](../Domain/06-business-rules.md)` (reserva preventiva + liberación a los 15 minutos) **sin implementar**.
> Se corrige el texto cuando el Owner resuelva [Q-04](../SDD.md#q-04-c-04-cart-es-un-estado-de-customerorder) (¿el carrito reserva stock?) — **no se inventa aquí**.

## 4. Casos de Uso (Commands) - Logística (Fulfillment)
- **`CreateFulfillmentOrderCommand`**: Genera un `FulfillmentOrder` derivado de un pedido pagado.
- **`PackFulfillmentOrderCommand`**: Pasa a PendingPack a Packed.
- **`DispatchFulfillmentCommand`**: Marca como despachado (Shipped).
- **`CancelFulfillmentOrderDueToNoStockCommand`**: [SPEC-008] Caso crítico. Cancela el despacho, llama a Inventario para ejecutar `ReconcileGhostStock` (asegurando AvailableQuantity en 0) y abre automáticamente el `ReturnRequest` para iniciar el reembolso.

## 5. Casos de Uso (Commands) - Devoluciones (Returns)
- **`RequestReturnCommand`**: Crea el request inicial (`ReturnStatus.Requested`). Bloquea si es Producto Digital.
- **`InspectReturnCommand`**: Inspección física inicial por logística (`IsGoodCondition`).
- **`ApproveReturnByVendorCommand`**: [SPEC-008] Flujo de aprobación. Si el operador logístico y el vendor son el mismo (`isSameWarehouseAndVendor`), aprueba directo saltando la inspección. Luego interactúa con `IInventoryRepository` para invocar `ReturnToUsedStock()`.

## 6. Casos de Uso (Commands) - Facturación (Billing)
- **`GenerateInvoicesCommand`**: [SPEC-008] Dado un `CustomerOrderId`, genera:
  1. `InvoiceType.Master` (Para el comprador por el total).
  2. `InvoiceType.ZentricDetail` (Fee de plataforma).
  3. `InvoiceType.VendorDetail` (Split para los vendedores).
  Guarda en `IInvoiceRepository`.

## 7. Casos de Uso (Commands) - Catálogo (Products)
- **`CreateProductCommand`**: Crea producto y sus variantes forzosas.
- **`PublishProductCommand`**: Publica el producto.

## 8. Validaciones
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
> ([verification-baseline.md :11](../SDD.md#11-sexta-iteracion-spec-007-validacion-de-entrada-rfc-7807-e-higiene-2026-09-18), 206/206).
>
> `[PENDIENTE]` Los comandos **especificados pero no implementados** ([:3](../SDD.md#3-formato-lint-y-analisis-estatico)–[:6](../SDD.md#6-como-repetir-esta-linea-base)) no tienen validador
> porque no existen: `CheckoutOrderCommand`, `DispatchFulfillmentCommand`, `RequestReturnCommand`,
> `ApproveReturnCommand`, `CreateProductCommand`, `PublishProductCommand`.

