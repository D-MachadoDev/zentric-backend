# Matriz de conformidad Spec ↔ Código

Leyenda: **OK** conforme · **PARCIAL** existe pero incompleto · **DESVIADO**
existe con nombre/formato distinto · **FALTA** no implementado · **CONTRA**
contradice la especificación.

## 1. Identity (usuarios)

| Requisito de spec | Fuente | Código | Estado |
|---|---|---|---|
| AR `User` con `Id`, `FullName`, `Email`, `Role`, `Status` | `01-models.md` §1 | `User.cs:7-43` | OK |
| `IdentityDocument` obligatorio y único | `01-models.md` §1, `02-aggregates:9`, `ZENTRIC.md` Dom.1 | — | **FALTA** |
| Correo único en todo el sistema | `01-models.md` §1, `ZENTRIC.md` 11 | `IUserRepository.GetByEmailAsync` es solo lectura | **PARCIAL** |
| Un solo rol por usuario | `ZENTRIC.md` RG-02 | `User.Role` único | OK |
| `Lock()` / `Unlock()` / `ChangeRole()` | `01-models.md` §1 | `Block()` / `Activate()` / `UpdateRole()` | **DESVIADO** |
| `Block()` dispara evento de suspensión en cascada | `02-aggregates:10`, invariante 6 | sin eventos de dominio | **FALTA** |
| Suspensión en cascada de productos del vendedor | invariante 6 (`04-invariants`) | — | **FALTA** |
| `UserStatus`: Activo, Bloqueado | `ZENTRIC.md` Dom.1 | `Active, Blocked, Deleted` | OK + extra |

## 2. Buyer

| Requisito | Fuente | Código | Estado |
|---|---|---|---|
| Dirección principal obligatoria | `ZENTRIC.md` Dom.2 | `Buyer.cs:26-29` | OK |
| Direcciones adicionales (opcional, añadir/quitar) | `ZENTRIC.md` Dom.2 | `Buyer.cs:45-70` | OK |
| Estado comercial | `ZENTRIC.md` Dom.2 | `IsActiveForCommerce` | OK |
| Agregado `Buyer` declarado en la spec de dominio | `01-models.md`, `02-aggregates` | existe en código | **FALTA en spec** |
| Sin almacenamiento de medios de pago | invariante 9 (`04-invariants`) | `PaymentTokens` (`Buyer.cs:13,112-139`) | **CONTRA** |
| Dirección como VO `Address` | `02-value-objects.md` | `string` (`Buyer.cs:10-11`) | **DESVIADO** |

## 3. Catalog (productos)

| Requisito | Fuente | Código | Estado |
|---|---|---|---|
| AR `Product` con `SellerId`, `Price`, `Type` | `01-models.md` §2 | `Product.cs` | OK |
| Propiedad `Title` | `01-models.md` §2 | `Name` | **DESVIADO** |
| `ProductStatus` = Published / Suspended / Discontinued | `02-value-objects.md`, `ZENTRIC.md` Dom.5 | `bool IsActive` + `DeletedAt` | **DESVIADO** |
| Nace publicado sin aprobación (CAT-01) | invariante 5, `06-business-rules` CAT-01 | `Product.cs:59` | OK |
| `Suspend()` reactivo | `02-aggregates:17` | existe | OK |
| `Discontinue()` | `01-models:22` | — | **FALTA** |
| `ProductVariant` (SKU) con `VariantId` | `02-aggregates:18-19`, `03-value-objects:15` | `ProductVariant.cs` + `Product.AddVariant()` | **OK** (ADR-0002) |
| Variante obligatoria solo en físicos (CAT-03) | `06-business-rules.md` CAT-03, ADR-0003 (Q-10 = C3) | constructor + `UpdateType` + `RemoveVariant` + `CanBeSold` en `Product.cs` | **OK** (sub-decisiones Q-12 `[PROPUESTO]`) |
| Atributos de variante (Talla/Color) | `02-aggregates:18` | `VariantAttribute` (nombre + valor) | **OK** `[PROPUESTO]` (Q-11) |
| SKU único dentro del producto | consecuencia de ADR-0002 / INV-03 | `Product.AddVariant()` rechaza SKU duplicado | **OK** (decisión, unicidad global pendiente en persistencia) |
| `UpdatePrice(Money)` | `01-models:22` | `Product.cs:92` | OK |
| Soft delete / restaurar producto | no especificado | `Delete()` / `Restore()` | **PENDIENTE** documentar |

## 4. Inventory y Warehouse

| Requisito | Fuente | Código | Estado |
|---|---|---|---|
| AR `Warehouse` con `Type` y dueño opcional | `01-models` §3 | `Warehouse.cs` | OK |
| `WarehouseType` = Marketplace / **Vendor** | `02-value-objects` | `Marketplace` / `Seller` | **DESVIADO** |
| `Location` como VO `Address` | `01-models:28` | `string` | **DESVIADO** |
| `OwnerId` / `OwnerSellerId` | `01-models:28`, `02-aggregates:25` | `SellerId` | **DESVIADO** |
| AR de stock con `WarehouseId` y cantidades | `01-models` §3 | `Inventory.cs` | OK |
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
| Cantidad dañada no reservable | `ZENTRIC.md` 11 | `DamagedQuantity` existe; no bloquea la reserva | **PARCIAL** |
| Movimientos: Ingreso, Reserva, Salida, Ajuste, Devolución | `ZENTRIC.md` Dom.6 | 5 operaciones presentes | OK |

## 5. Ordering

| Requisito | Fuente | Código | Estado |
|---|---|---|---|
| AR `CustomerOrder` con `BuyerId`, `Lines`, `TotalAmount`, `Status` | `01-models` §4 | `Order.cs` (solo `Id`) | **FALTA** |
| `OrderLine` (variante, cantidad, precio unitario, total, tipo) | `01-models:43`, `02-aggregates:41` | — | **FALTA** |
| `FlatShippingFee` | `02-aggregates:40`, invariante 7 | — | **FALTA** |
| Estados `Cart`, `PendingPayment`, `Paid`, `PartiallyDelivered`, `Completed`, `Cancelled` | `02-value-objects:45-51` | — | **FALTA** |
| `ConfirmPayment()`, `CancelEarly()`, `ApplyPartialRefund()` | `02-aggregates:42-45` | — | **FALTA** |
| `PaymentReceipt` (simulación de pasarela) | invariante 9 | — | **FALTA** |
| Timeout de reserva 15 min (PED-01) | `06-business-rules` PED-01, `07-lifecycle` | — | **FALTA** |

## 6. Fulfillment, devoluciones, eventos y puertos

| Requisito | Fuente | Código | Estado |
|---|---|---|---|
| AR `FulfillmentOrder` + `Shipment` | `01-models` §5 | — | **FALTA** |
| Estados `Pending`, `Packed`, `Shipped`, `Delivered`, `CancelledByStockBreak` | `02-value-objects:53-58`, `03-value-objects:36-41` | — | **FALTA** |
| `CancelDueToGhostStock`, `RequestReturn` (no digital) | `02-aggregates:53-54` | — | **FALTA** |
| `ReturnStatus` (Requested → Refunded / Rejected) | `03-value-objects:43-48` | — | **FALTA** |
| 7 eventos de dominio | `04-domain-events.md` | — | **FALTA** |
| Puerto `IDomainEventDispatcher` | `05-ports.md` §2 | — | **FALTA** |
| Puertos `IProductRepository`, `IWarehouseRepository`, `IInventoryRepository`, `ICustomerOrderRepository`, `IFulfillmentOrderRepository` | `05-ports.md` §1 | solo `IUserRepository` | **FALTA** |
| 4 servicios de dominio | `03-domain-services.md`, `services/*` | — | **FALTA** |
| IDs fuertemente tipados (`UserId`, `VariantId`, …) | `02-value-objects:23-25`, `03-value-objects:13-15` | `Guid` plano | **FALTA** |
| VO `Address` compartido (bodega y despacho) | `02-value-objects:13-16` | — | **FALTA** |
| Contrato `Result<T>` / `Result<ReservationDetails>` en servicios | `services/inventory-reservation-service.md` §3 | — | **FALTA** |

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
| ~~H-08~~ | ~~`Inventory.cs:142-152`: `ReturnToAvalible` no valida `quantity > 0`~~ | **CORREGIDO en T-003** (ver `verification-baseline.md` §7). Cerró una violación real de INV-01: `ReturnToAvalible(-100)` dejaba el disponible en −100 |

## 8. Totales

- **13** requisitos conformes (incluye `ProductVariant`, la clave de stock y CAT-03).
- **6** parciales.
- **10** desviados (naming/forma): bajan desde 12 porque la clave de stock y
  `ProductVariant` dejaron de ser desviaciones.
- **20** faltantes: baja desde 22 por G-08 (`ProductVariant`) y la clave del inventario.
- **1** contradicción directa pendiente con la especificación (medios de pago,
  H-03 → Q-08). La contradicción de clave de stock quedó resuelta por ADR-0002.
- **7** hallazgos de defecto **abiertos** en código existente (H-01 … H-07).
- **1** hallazgo de defecto **corregido** (H-08 — violación de INV-01, corregido en T-003a).
- **164** pruebas automatizadas en verde (`Zentric.Tests`), que cubren las
  invariantes `[CONFIRMADO]` de `Inventory`, `Warehouse`, `User`, `Product`,
  `ProductVariant`, `VariantAttribute` y `Money`.
- **Deuda declarada:** `VariantId` es `Guid` plano, no un `record struct`
  fuertemente tipado (G-07 / T-004).
