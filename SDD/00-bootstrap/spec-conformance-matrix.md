# Matriz de conformidad Spec ↔ Código

Leyenda: **OK** conforme · **PARCIAL** existe pero incompleto · **DESVIADO**
existe con nombre/formato distinto · **FALTA** no implementado · **CONTRA**
contradice la especificación.

## 1. Identity (usuarios)

| Requisito de spec | Fuente | Código | Estado |
|---|---|---|---|
| AR `User` con `Id`, `FullName`, `Email`, `Role`, `Status` | [01-models.md :1](../Domain/01-models.md#1-bounded-context-identity-access-usuarios) | `User.cs:7-43` | OK |
| `IdentityDocument` obligatorio y único | [01-models.md :1](../Domain/01-models.md#1-bounded-context-identity-access-usuarios), `02-aggregates:9`, [`ZENTRIC.md`](../Domain/ZENTRIC.md) Dom.1 | — | **FALTA** |
| Correo único en todo el sistema | [01-models.md :1](../Domain/01-models.md#1-bounded-context-identity-access-usuarios), [`ZENTRIC.md`](../Domain/ZENTRIC.md) 11 | `IUserRepository.GetByEmailAsync` es solo lectura | **PARCIAL** |
| Un solo rol por usuario | [`ZENTRIC.md`](../Domain/ZENTRIC.md) RG-02 | `User.Role` único | OK |
| `Lock()` / `Unlock()` / `ChangeRole()` | [01-models.md :1](../Domain/01-models.md#1-bounded-context-identity-access-usuarios) | `Block()` / `Activate()` / `UpdateRole()` | **DESVIADO** |
| `Block()` dispara evento de suspensión en cascada | `02-aggregates:10`, invariante 6 | sin eventos de dominio | **FALTA** |
| Suspensión en cascada de productos del vendedor | invariante 6 (`04-invariants`) | — | **FALTA** |
| `UserStatus`: Activo, Bloqueado | [`ZENTRIC.md`](../Domain/ZENTRIC.md) Dom.1 | `Active, Blocked, Deleted` | OK + extra |

## 2. Buyer

| Requisito | Fuente | Código | Estado |
|---|---|---|---|
| Dirección principal obligatoria | [`ZENTRIC.md`](../Domain/ZENTRIC.md) Dom.2 | `Buyer.cs:26-29` | OK |
| Direcciones adicionales (opcional, añadir/quitar) | [`ZENTRIC.md`](../Domain/ZENTRIC.md) Dom.2 | `Buyer.cs:45-70` | OK |
| Estado comercial | [`ZENTRIC.md`](../Domain/ZENTRIC.md) Dom.2 | `IsActiveForCommerce` | OK |
| Agregado `Buyer` declarado en la spec de dominio | [`01-models.md`](../Domain/01-models.md), `02-aggregates` | existe en código | **FALTA en spec** |
| Sin almacenamiento de medios de pago | invariante 9 (`04-invariants`) | `PaymentTokens` (`Buyer.cs:13,112-139`) | **CONTRA** |
| Dirección como VO `Address` | [`02-value-objects.md`](../Domain/02-value-objects.md) | `string` (`Buyer.cs:10-11`) | **DESVIADO** |

## 3. Catalog (productos)

| Requisito | Fuente | Código | Estado |
|---|---|---|---|
| AR `Product` con `SellerId`, `Price`, `Type` | [01-models.md :2](../Domain/01-models.md#2-bounded-context-catalog-catalogo) | `Product.cs` | OK |
| Propiedad `Title` | [01-models.md :2](../Domain/01-models.md#2-bounded-context-catalog-catalogo) | `Name` | **DESVIADO** |
| `ProductStatus` = Published / Suspended / Discontinued | [`02-value-objects.md`](../Domain/02-value-objects.md), [`ZENTRIC.md`](../Domain/ZENTRIC.md) Dom.5 | `bool IsActive` + `DeletedAt` | **DESVIADO** |
| Nace publicado sin aprobación (CAT-01) | invariante 5, `06-business-rules` CAT-01 | `Product.cs:59` | OK |
| `Suspend()` reactivo | `02-aggregates:17` | existe | OK |
| `Discontinue()` | `01-models:22` | — | **FALTA** |
| `ProductVariant` (SKU) con `VariantId` | `02-aggregates:18-19`, `03-value-objects:15` | `ProductVariant.cs` + `Product.AddVariant()` | **OK** (ADR-0002) |
| Variante obligatoria solo en físicos (CAT-03) | [`06-business-rules.md`](../Domain/06-business-rules.md) CAT-03, ADR-0003 ([Q-10](questions-for-owner.md#9-cuarta-iteracion-adr-0003-variante-obligatoria-en-fisicos-q-10-c3) = C3) | constructor + `UpdateType` + `RemoveVariant` + `CanBeSold` en `Product.cs` | **OK** (sub-decisiones [Q-12](questions-for-owner.md#sub-decisiones-propuesto-ver-q-12) `[PROPUESTO]`) |
| Atributos de variante (Talla/Color) | `02-aggregates:18` | `VariantAttribute` (nombre + valor) | **OK** `[PROPUESTO]` ([Q-11](questions-for-owner.md#q-11-detalle-del-modelo-de-atributos-de-variante-abierta)) |
| SKU único dentro del producto | consecuencia de ADR-0002 / INV-03 | `Product.AddVariant()` rechaza SKU duplicado | **OK** (decisión, unicidad global pendiente en persistencia) |
| `UpdatePrice(Money)` | `01-models:22` | `Product.cs:92` | OK |
| Soft delete / restaurar producto | no especificado | `Delete()` / `Restore()` | **PENDIENTE** documentar |

## 4. Inventory y Warehouse

| Requisito | Fuente | Código | Estado |
|---|---|---|---|
| AR `Warehouse` con `Type` y dueño opcional | `01-models` [`:3`](../../Zentric.Domain/Warehouses/Warehouse.cs#L3) | `Warehouse.cs` | OK |
| `WarehouseType` = Marketplace / **Vendor** | `02-value-objects` | `Marketplace` / `Seller` | **DESVIADO** |
| `Location` como VO `Address` | `01-models:28` | `string` | **DESVIADO** |
| `OwnerId` / `OwnerSellerId` | `01-models:28`, `02-aggregates:25` | `SellerId` | **DESVIADO** |
| AR de stock con `WarehouseId` y cantidades | `01-models` [`:3`](../../Zentric.Domain/Inventories/Inventory.cs#L3) | `Inventory.cs` | OK |
| Nombre `InventoryItem` | `01-models:31` | `Inventory` | **DESVIADO** |
| `AvailableQuantity` (ortografía) | toda la spec | `AvalibleQuantity` | **DESVIADO (typo)** |
| Clave de stock = `VariantId` (SKU) | `02-aggregates:29`, `03-value-objects:15`, INV-03 | `Inventory.VariantId` | **OK** (ADR-0002) |
| Clave de stock = `(VariantId, WarehouseId)` | INV-03 (ADR-0002) | `VariantId` + `WarehouseId` | **OK** |
| `Reserve(qty)` | `01-models:35` | `ReserveStock(qty)` | DESVIADO (naming) |
| `Release(qty)` | `01-models:35` | `ReturnToAvalible(qty)` | DESVIADO (naming) |
| `Deduct(qty)` | `01-models:35` | `DispatchStock(qty)` (con defecto H-01) | PARCIAL |
| `Adjust(qty)` para devoluciones | `01-models:35`, servicio de devoluciones | `UpdateQuantities` / `AddStock` / `ReciveReturnedStock` | DESVIADO |
| `ManualAdjust(qty, UserId, Role)` con regla dura | `02-aggregates:34`, invariante 4 | ningún método recibe `Role` | **FALTA** |
| `AvailableQuantity` nunca negativo | INV-01, invariante 1 | protegido (`Inventory.cs:33,60,99,116,132`) | OK |
| Cantidad dañada no reservable | [`ZENTRIC.md`](../Domain/ZENTRIC.md) 11 | `DamagedQuantity` existe; no bloquea la reserva | **PARCIAL** |
| Movimientos: Ingreso, Reserva, Salida, Ajuste, Devolución | [`ZENTRIC.md`](../Domain/ZENTRIC.md) Dom.6 | 5 operaciones presentes | OK |

## 5. Ordering

| Requisito | Fuente | Código | Estado |
|---|---|---|---|
| AR `CustomerOrder` con `BuyerId`, `Items`, `TotalAmount`, `Status` | `01-models` [`:4`](../../Zentric.Domain/Buyers/Buyer.cs#L4) | `Orders/CustomerOrder.cs` | **PARCIAL** — `Items` (no `Lines`); `TotalAmount` con moneda "USD" **inventada** (`[`:24`](../../Zentric.Domain/Buyers/Buyer.cs#L24)`) |
| `OrderLine` (variante, cantidad, precio unitario, total, tipo) | `01-models:43`, `02-aggregates:41` | `Orders/Entities/OrderItem.cs` | **DESVIADO** (nombre: `OrderItem`) |
| `FlatShippingFee` | `02-aggregates:40`, invariante 7 | — | **FALTA** |
| Estados `Cart`, `PendingPayment`, `Paid`, `PartiallyDelivered`, `Completed`, `Cancelled` | `02-value-objects:45-51` | `Orders/Enums/OrderStatus.cs` | **PARCIAL** — la Ley (Dom.7) fija Cart, PendingPayment, Paid, Dispatched, Delivered; faltan `PartiallyDelivered`, `Completed`, `Cancelled` |
| `ConfirmPayment()`, `CancelEarly()`, `ApplyPartialRefund()` | `02-aggregates:42-45` | `MarkAsPaid()`, `Dispatch()`, `Deliver()` | **DESVIADO** — sin cancelación ni reembolso parcial |
| `PaymentReceipt` (simulación de pasarela) | invariante 9 | — | **FALTA** |
| Timeout de reserva 15 min (PED-01) | `06-business-rules` PED-01, `07-lifecycle` | — | **FALTA** — y la reserva preventiva tampoco existe (**H-10**) |
| Pedido entregado **no modificable** | [ZENTRIC.md :11](../Domain/ZENTRIC.md#11-validaciones-criticas) (Validaciones Críticas) | `CustomerOrder.EnsureNotDelivered()` (`[`:152-159`](../../Zentric.Domain/Orders/CustomerOrder.cs#L152-L159)`) | **OK** (con prueba) |

## 6. Fulfillment, devoluciones, eventos y puertos

| Requisito | Fuente | Código | Estado |
|---|---|---|---|
| AR `FulfillmentOrder` + `Shipment` | `01-models` [`:5`](../../Zentric.Domain/Logistics/FulfillmentOrder.cs#L5) | `Logistics/FulfillmentOrder.cs`, `Entities/Shipment.cs` | **PARCIAL** — agregados sin servicio ([T-014](migration-to-sdd-plan.md) parcial) |
| Estados de despacho | `02-value-objects:53-58`, `03-value-objects:36-41`, [Ley [ADD-001](../SDD.md)](../SDD.md#addendum---dictado-por-owner-add-001-dominio-8-logistica-y-despachos-fulfillment) | `FulfillmentStatus` = Packed, Dispatched, Delivered, CancelledNoStock | **CONTRADICCIÓN** — 4 valores vs 5; **falta `PendingPack`** y el orden de la Ley (**[C-08](risks-and-gaps.md)**) |
| `CancelDueToGhostStock`, `RequestReturn` (no digital) | `02-aggregates:53-54`, [Ley [ADD-001](../SDD.md)](../SDD.md#addendum---dictado-por-owner-add-001-dominio-8-logistica-y-despachos-fulfillment)/002 | `CancelDueToNoStock()`, `ReturnRequest` (rechaza `Digital`) | **PARCIAL** — nombres desviados; sin la "devolución obligatoria" que exige [ADD-001](../SDD.md) |
| `ReturnStatus` (Requested → Refunded / Rejected) | `03-value-objects:43-48`, Ley Dom.10 | `Returns/Enums/ReturnStatus.cs` | **OK** |
| Retorno a stock con etiqueta "Usado" tras devolución aprobada | [Ley [ADD-002](../SDD.md)](../SDD.md#addendum---dictado-por-owner-add-002-dominio-9-devoluciones-y-reembolsos) | `Inventory.ReturnToUsedStock()` existe pero **nadie lo invoca** (`// TODO` en `ReturnRequest.cs:63`) | **CONTRADICCIÓN** (**H-14**) |
| Facturación: Maestra · Detalle Zentric · Vendedor | [Ley [ADD-003](../SDD.md)](../SDD.md#addendum---dictado-por-owner-add-003-dominio-10-facturacion-y-pagos) | `Billing/Invoice.cs` (`CreateMaster`, `CreateVendorDetail`) | **PARCIAL** — **falta "Detalle Zentric"** (**[C-08](risks-and-gaps.md)**) |
| 7 eventos de dominio | [`04-domain-events.md`](../Domain/04-domain-events.md) | — | **FALTA** |
| Puerto `IDomainEventDispatcher` | [05-ports.md :2](../Domain/05-ports.md#2-puertos-de-mensajeria-event-bus) | — | **FALTA** |
| Puertos `IProductRepository`, `IWarehouseRepository`, `IInventoryRepository`, `IReturnRequestRepository`, `IInvoiceRepository`, `ICustomerOrderRepository`, `IFulfillmentOrderRepository` | [05-ports.md :1](../Domain/05-ports.md#1-puertos-de-repositorios-persistencia), `SDD/Application` [`:2`](../../Zentric.Application/Logistics/Ports/IFulfillmentOrderRepository.cs#L2) | 2 de 7 (`ICustomerOrderRepository`, `IFulfillmentOrderRepository`); `IUserRepository` sigue en Domain | **PARCIAL** |
| 4 servicios de dominio | [`03-domain-services.md`](../Domain/03-domain-services.md), `services/*` | — | **FALTA** |
| IDs fuertemente tipados (`UserId`, `VariantId`, …) | `02-value-objects:23-25`, `03-value-objects:13-15` | `Guid` plano | **FALTA** |
| VO `Address` compartido (bodega y despacho) | `02-value-objects:13-16` | — | **FALTA** |
| Contrato `Result<T>` / `Result<ReservationDetails>` en servicios | [services/inventory-reservation-service.md :3](../Domain/services/inventory-reservation-service.md#3-entradas-y-salidas) | `Zentric.Application/Common/Models/Result.cs` | **PARCIAL** — existe el tipo, no se usa en servicios de dominio (no hay servicios) |

## 7. Defectos detectados en el código implementado

| ID | Evidencia | Riesgo |
|---|---|---|
| H-01 | `Inventory.cs:109-123`: `DispatchStock` valida `AvalibleQuantity` y decrementa `ReservedQuantity`; puede dejar el reservado negativo | Rompe el balance de stock — severidad **alta** |
| H-02 | `Inventory.cs:58-79`: `UpdateQuantities` sobrescribe los tres contadores saltándose las operaciones de negocio | Setter anémico: anula invariantes — severidad **alta** |
| H-03 | `Buyer.cs:13,112-139`: `PaymentTokens` contradice la invariante 9 | Regla de negocio violada por diseño — **media** |
| H-04 | `Warehouse.cs:206-209` y `Inventory.cs:167`: `MarkAsDeleted()` duplica `Delete()` | API redundante y lenguaje ambiguo — **baja** |
| H-05 | `User.cs:29-43`: `IdentityDocument` ausente pese a ser obligatorio y único | No se puede registrar un usuario conforme a spec — **alta** |
| H-06 | `DateTime.UtcNow` usado directamente en entidades de dominio: **46 usos en 5 archivos** (`Buyer` 8, `Inventory` 9, `Product` 10, `User` 10, `Warehouse` 9) | Reglas temporales (timeout 15 min) y pruebas no deterministas — **media** |
| H-07 | `IUserRepository.cs:10-11`: unicidad de correo marcada como TODO | Invariante no aplicada en ningún punto — **alta** |
| ~~H-08~~ | ~~`Inventory.cs:142-152`: `ReturnToAvalible` no valida `quantity > 0`~~ | **CORREGIDO en [T-003](migration-to-sdd-plan.md)** (ver [verification-baseline.md :7](verification-baseline.md#7-segunda-iteracion-suite-de-pruebas-y-correccion-t-003)). Cerró una violación real de INV-01: `ReturnToAvalible(-100)` dejaba el disponible en −100 |
| ~~H-09~~ | ~~`AddOrderItemCommand.cs:35-38`: `catch (Exception ex) { return Result.Failure(ex.Message); }`~~ | **CORREGIDO en SPEC-007 (2026-09-18):** precondición explícita (`Status != Cart`) → `Result.Failure`; sin `try-catch`. Regresión en `MediatRValidationPipelineTests` |
| H-10 | [SDD/Application/01-use-cases-and-ports.md :15](../Application/01-use-cases-and-ports.md)` afirma que `AddOrderItemCommand` valida stock con `IInventoryRepository`: el puerto **no existe** y no se valida stock; `PED-01` sin implementar | Especificación que miente sobre el código + regla de la Ley incumplida — **alta** |
| ~~H-11~~ | ~~`Program.cs:35` `UseExceptionHandler("/error")` sin endpoint ni `AddProblemDetails()`; FluentValidation referenciado con **0 validadores** y **0 pipeline**~~ | **CORREGIDO en SPEC-007 (2026-09-18):** `AddProblemDetails()` + `UseExceptionHandler()`; 3 validadores + `ValidationBehavior<,>` con `AddOpenBehavior`, verificados en contenedor DI real (`[PENDIENTE]` HTTP real) |
| ~~H-12~~ | ~~`Zentric.Application/Class1.cs` y `Zentric.Infrastructure/Class1.cs` vacíos~~ | **CORREGIDO en SPEC-007 (2026-09-18):** eliminados (no estaban referenciados) |
| H-13 | `ZentricDbContext.cs` mapea directamente los agregados de dominio | Contradice [AGENTS.md :4.2](../../AGENTS.md#42-infrastructure-adapter-agent) ("aislamiento estricto de las entidades EF Core"); desviación sin ADR — **media** |
| H-14 | `FulfillmentOrder.cs:75` y `ReturnRequest.cs:63` con `// TODO` de eventos: `ReturnToUsedStock` **nunca se invoca** | La [Ley [ADD-002](../SDD.md)](../SDD.md#addendum---dictado-por-owner-add-002-dominio-9-devoluciones-y-reembolsos) (stock "Usado") no se cumple en runtime — **alta** |

## 8. Totales

> **Reconteo 2026-09-18** tras auditar la tanda no registrada (secciones :4, :5 y :6). La aritmética
> cierra: 50 filas antes + 2 filas nuevas = **52** filas (15+12+13+10+2).

| Métrica | :8 anterior (2026-09-17) | Delta del 2026-09-18 | Ahora |
|---|---|---|---|
| Requisitos conformes | 13 | +2 (pedido entregado inmutable; `ReturnStatus`) | **15** |
| Parciales | 6 | +6 (`CustomerOrder`, estados del pedido, `FulfillmentOrder`+`Shipment`, `CancelDueToGhostStock`, puertos, `Result<T>`) | **12** |
| Desviados | 10 | +3 (`OrderItem`, estados de despacho, métodos de pago/cancelación) | **13** |
| Faltantes | 20 | −10 (dejaron de faltar 10 requisitos de :5 y :6) | **10** |
| Contradicciones directas | 1 (medios de pago, H-03 → [Q-08](questions-for-owner.md#q-08-r-07-buyerpaymenttokens-contra-la-invariante-9)) | +1 (retorno a stock "Usado" no se ejecuta, H-14) | **2** |
| Hallazgos de defecto **abiertos** | 7 (H-01…H-07) | +6 (**H-09…H-14**) −3 (H-09, H-11 y H-12 corregidos en SPEC-007) | **10** |
| Hallazgos **corregidos** | 1 (H-08) | — | 1 |
| Pruebas automatizadas en verde | 164 | +14 (Orders 6, Logistics 3, Billing 2, Returns 3) | **178** |

- **Deuda declarada:** `VariantId` es `Guid` plano, no un `record struct` fuertemente tipado (G-07 / [T-004](migration-to-sdd-plan.md)).
- `[RIESGO]` Los 13 hallazgos abiertos **no** están clasificados como tareas con verificación en el roadmap; H-09, H-10, H-11 y H-14 son incumplimientos directos de [`AGENTS.md`](../../AGENTS.md) o de la Ley.

