# Capa de Aplicación - Casos de Uso y Puertos

Este documento detalla los comandos (CQRS) y puertos que definen los casos de uso para ZENTRIC.md.

## 1. Patrón Arquitectónico
Se utiliza **CQRS** (Command Query Responsibility Segregation) con la librería `MediatR`. Todo caso de uso se define como un `IRequest<Result<T>>` o `IRequest<Result>`.
Las validaciones de entrada se realizan con `FluentValidation` en el Pipeline de MediatR (`ValidationBehavior<TRequest, TResponse>`), antes de que el Command toque el Dominio.

## 2. Puertos de Salida (Output Ports)
Siguiendo la Arquitectura Hexagonal estricta (AGENTS.md §2.1), las interfaces de persistencia de agregados residen en sus respectivos módulos de Dominio (`Zentric.Domain/*/Ports/`) y son consumidas por los Handlers de Aplicación:
- `IUserRepository`, `IBuyerRepository`, `IProductRepository`, `IWarehouseRepository`, `IInventoryRepository`, `ICustomerOrderRepository`, `IFulfillmentOrderRepository`, `IReturnRequestRepository`, `IInvoiceRepository`.
- Puerto de transacción y persistencia: `IUnitOfWork` (`Zentric.Application.Common.Ports.IUnitOfWork`).

---

## 3. Catálogo Completo de Casos de Uso (Commands)

### 3.1. Módulo de Usuarios (`Zentric.Application.Users`)
- **`CreateUserCommand`**: Registra usuarios con roles (Buyer, Seller, LogisticsOperator, Admin, Supervisor). Valida unicidad de correo y documento de identidad.
  - *Validador:* `CreateUserCommandValidator`.

### 3.2. Módulo de Bodegas (`Zentric.Application.Warehouses`)
- **`CreateWarehouseCommand`**: Registra bodegas Marketplace (sin vendor) o Vendor (con vendor obligatorio).
  - *Validador:* `CreateWarehouseCommandValidator`.

### 3.3. Módulo de Inventario (`Zentric.Application.Inventories`)
- **`AddStockCommand`**: Registra o incrementa existencias disponibles de una variante (SKU) en una bodega específica.
  - *Validador:* `AddStockCommandValidator`.

### 3.4. Módulo de Catálogo (`Zentric.Application.Catalog`)
- **`CreateProductCommand`**: Crea productos físicos (con variantes obligatorias) o digitales.
  - *Validador:* `CreateProductCommandValidator`.
- **`PublishProductCommand`**: Publica un producto para comercialización pública.
  - *Validador:* `PublishProductCommandValidator`.

### 3.5. Módulo de Pedidos (`Zentric.Application.Orders`)
- **`CreateCartCommand`**: Inicializa un `CustomerOrder` en estado `Cart`.
  - *Validador:* `CreateCartCommandValidator`.
- **`AddOrderItemCommand`**: Añade ítems al carrito validando cantidad y precio.
  - *Validador:* `AddOrderItemCommandValidator`.
- **`CheckoutOrderCommand`**: Pasa el pedido de `Cart` a `PendingPayment`, reservando stock a través de `InventoryReservationService` y generando los `FulfillmentOrder` por vendedor.
  - *Validador:* `CheckoutOrderCommandValidator`.
- **`PayOrderCommand`**: Valida y confirma la transacción de pago, pasando el pedido de `PendingPayment` a `Paid` y disparando `OrderPaidDomainEvent`.
  - *Validador:* `PayOrderCommandValidator`.

### 3.6. Módulo de Logística (`Zentric.Application.Logistics`)
- **`CreateFulfillmentOrderCommand`**: Genera un despacho individual por vendedor.
  - *Validador:* `CreateFulfillmentOrderCommandValidator`.
- **`DispatchFulfillmentCommand`**: Marca como despachado (`Shipped`) y despacha físicamente el inventario.
  - *Validador:* `DispatchFulfillmentCommandValidator`.
- **`CancelFulfillmentOrderDueToNoStockCommand`**: Cancela unilateralmente por quiebre de stock, reconcilia inventario a 0 (`ReconcileGhostStock`) y detona solicitud obligatoria de devolución/reembolso.
  - *Validador:* `CancelFulfillmentOrderDueToNoStockCommandValidator`.

### 3.7. Módulo de Devoluciones (`Zentric.Application.Returns`)
- **`RequestReturnCommand`**: Crea la solicitud inicial (`Requested`), prohibiendo devoluciones en productos digitales.
  - *Validador:* `RequestReturnCommandValidator`.
- **`InspectReturnCommand`**: Registra inspección física de condición (`IsGoodCondition`) por el operador logístico.
  - *Validador:* `InspectReturnCommandValidator`.
- **`ApproveReturnCommand`**: Aprobación comercial final del vendedor, reintegrando las unidades al inventario como Usadas (`ReturnToUsedStock`).
  - *Validador:* `ApproveReturnCommandValidator`.

### 3.8. Módulo de Facturación (`Zentric.Application.Billing`)
- **`GenerateInvoicesCommand`**: Dado un `CustomerOrderId` pagado, genera:
  1. `InvoiceType.Master` (Factura Maestra consolidada para el comprador).
  2. `InvoiceType.ZentricDetail` (Tarifa de servicio de la plataforma Zentric).
  3. `InvoiceType.VendorDetail` (Factura de desglose / split para cada vendedor).

---

## 4. Pipeline de Validación y Resiliencia
- `ValidationBehavior<TRequest, TResponse>` intercepta todas las llamadas a MediatR antes de invocar el handler. Si hay errores de validación, retorna `Result.Failure` con el desglose RFC 7807 sin arrojar excepciones no controladas.
