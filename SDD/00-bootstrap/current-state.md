# Current State — comportamiento y arquitectura observados

## 1. Agregados implementados

| Artefacto de código | Tipo | Spec asociada | Estado |
|---|---|---|---|
| `Users/User.cs` | AR | [01-models.md :1](../Domain/01-models.md#1-bounded-context-identity-access-usuarios), [02-aggregates-and-entities.md :1](../Domain/02-aggregates-and-entities.md#1-identity-module) | parcial |
| `Users/ValueObjects/FullName.cs` | VO | no listado explícitamente en la spec | extra |
| `Users/ValueObjects/Email.cs` | VO | [02-value-objects.md](../Domain/02-value-objects.md) (`EmailAddress`) | nombre desviado |
| `Users/Enums/UserRole.cs` | enum | [02-value-objects.md](../Domain/02-value-objects.md) (`Admin`) | nombre desviado |
| `Users/Enums/UserStatus.cs` | enum | [ZENTRIC.md](../Domain/ZENTRIC.md) Dominio 1 (Activo, Bloqueado, etc.) | extra (`Deleted`) |
| `Users/Ports/IUserRepository.cs` | puerto | [05-ports.md](../Domain/05-ports.md) | parcial |
| `Buyers/Buyer.cs` | AR | [ZENTRIC.md](../Domain/ZENTRIC.md) Dominio 2 (**no** en `02-aggregates`) | extra en la spec de dominio |
| `Products/Product.cs` | AR | [01-models.md :2](../Domain/01-models.md#2-bounded-context-catalog-catalogo) | parcial (+ gestión de variantes) |
| `Products/ProductVariant.cs` | entidad hija | `02-aggregates:18-19` | **implementado** ([ADR-0002](../Adr/0002-clave-inventario-variantid.md)) |
| `Products/ValueObjects/VariantAttribute.cs` | VO | `02-aggregates:18` | **implementado** `[PROPUESTO]` ([Q-11](questions-for-owner.md#q-11-detalle-del-modelo-de-atributos-de-variante-abierta)) |
| `Products/ValueObjects/Money.cs` | VO | [02-value-objects.md](../Domain/02-value-objects.md) | conforme |
| `Products/Enums/ProductType.cs` | enum | [02-value-objects.md](../Domain/02-value-objects.md) | conforme |
| `Warehouses/Warehouse.cs` | AR | [01-models.md :3](../Domain/01-models.md#3-bounded-context-inventory-inventario-y-bodegas) | parcial |
| `Warehouses/Enum/WarehouseType.cs` | enum | [02-value-objects.md](../Domain/02-value-objects.md) | nombre desviado (`Seller` vs `Vendor`) |
| `Inventories/Inventory.cs` | AR | [01-models.md :3](../Domain/01-models.md#3-bounded-context-inventory-inventario-y-bodegas) (`InventoryItem`) | parcial + nombre desviado; clave `VariantId` ([ADR-0002](../Adr/0002-clave-inventario-variantid.md)) |
| `Orders/Order.cs` | stub | [01-models.md :4](../Domain/01-models.md#4-bounded-context-ordering-pedidos---interfaz-del-comprador) (`CustomerOrder`) | **vacío** |

## 2. Reglas de negocio efectivamente protegidas hoy

`[CONFIRMADO]` por lectura del código:

1. **No-negatividad de inventario ([INV-01](../Domain/06-business-rules.md) / [invariante 1](../Domain/04-invariants-and-rules.md)).** Protegida en el
   constructor y en cada mutación de `Inventory`
   (`Inventory.cs:33`, [Inventory.cs:60](../Zentric.Domain/Inventories/Inventory.cs#L60), [Inventory.cs:99](../Zentric.Domain/Inventories/Inventory.cs#L99), [Inventory.cs:116](../Zentric.Domain/Inventories/Inventory.cs#L116), [Inventory.cs:132](../Zentric.Domain/Inventories/Inventory.cs#L132)).
2. **Bodega Marketplace sin vendedor y bodega de vendedor con dueño.**
   `Warehouse.cs:44-52`.
3. **No renombrar/reubicar/activar/eliminar bodega inactiva o borrada.**
   `Warehouse.cs:66-208`.
4. **No eliminar inventario con existencias físicas.** `Inventory.cs:174-177`
   (regla extra, no está en la spec → `[PENDIENTE]` documentarla o retirarla).
5. **Usuario bloqueado no puede cambiar nombre, correo, contraseña ni rol.**
   `User.cs:52`, [User.cs:73](../Zentric.Domain/Users/User.cs#L73), [User.cs:96](../Zentric.Domain/Users/User.cs#L96), [User.cs:158](../Zentric.Domain/Users/User.cs#L158).
6. **Producto creado nace activo ([CAT-01](../Domain/06-business-rules.md)).** `Product.cs:59`.
7. **Variante obligatoria en productos físicos ([CAT-03](../Domain/06-business-rules.md)).** Constructor,
   `UpdateType` y `RemoveVariant` en `Product.cs` ([ADR-0003](../Adr/0003-variante-obligatoria-productos-fisicos.md)).
8. **Moneda homogénea en operaciones con `Money`.** `Money.cs:78-84`.
9. **Correo con formato mínimo validado.** `Email.cs:8-39`.

## 3. Arquitectura observada

`[CONFIRMADO]`

- Patrón: hexagonal + DDD respetado **en el papel**: `Zentric.Domain` no
  referencia nada (correcto y verificado en el `csproj`).
- Convención de carpetas: **módulo por bounded context** (`Users/`, `Products/`,
  `Warehouses/`, `Inventories/`, `Orders/`, `Buyers/`), con `Enums/`,
  `ValueObjects/` y `Ports/` anidados por módulo.
- "Ports" aparece dentro del módulo (`Users/Ports/`), coherente con [AGENTS.md :2.2](../../AGENTS.md#22-puertos-y-adaptadores) pero distinto del catálogo genérico de la skill.

`[CONFIRMADO]` Ausencias estructurales respecto a [AGENTS.md](../../AGENTS.md):
`[OBSOLETO — corregido el 2026-09-18]` El párrafo siguiente describía el estado del 2026-09-17. **Ya no es cierto:** existen los proyectos `Zentric.Application`, `Zentric.Infrastructure`, `Zentric.Api` y `Zentric.Tests` (ver [:6](../../AGENTS.md#6-inyeccion-de-dependencias-di)). Se conserva tachado para trazabilidad.

- ~~No existen `Application`, `Infrastructure`, `Api` ni pruebas.~~ → existen (5 proyectos en `Zentric.slnx`)
- No hay eventos de dominio ni dispatcher. ← **sigue siendo cierto** (G-04)
- No hay tipos fuertemente tipados de ID (todos `Guid`). ← sigue siendo cierto (G-07)
- No hay `Address` VO (las direcciones son `string`). ← sigue siendo cierto
- No hay abstracción de tiempo (`DateTime.UtcNow` directo en el dominio). ← sigue siendo cierto ([H-06](spec-conformance-matrix.md))
- No hay `.editorconfig`, ni analizadores, ni CI. ← sigue siendo cierto ([R-08](risks-and-gaps.md))

## 4. Contrato de dominio observable (puntos de dolor)

`[CONFIRMADO]` `Inventory.DispatchStock(int)` (`Inventory.cs:109-123`) valida
`AvalibleQuantity < quantity` cuando la cantidad que decrementa es
`ReservedQuantity`. Con `Available = 0`, `Reserved = 10`, `DispatchStock(5)`
decrementa `Reserved` a 5 sin error; con `quantity > ReservedQuantity` puede
dejar `ReservedQuantity` **negativo**, violando la [invariante 1](../Domain/04-invariants-and-rules.md) referida al stock
reservado y rompiendo el balance
`Avalible + Reservado + Dañado`.

`[CONFIRMADO]` `Inventory.UpdateQuantities(...)` permite sobrescribir los tres
contadores de golpe, saltándose `Reserve/Release/Dispatch`, lo que anula la
protección de las invariantes por la puerta de atrás (setter anémico
disfrazado de método).

`[CONFIRMADO]` `MarkAsDeleted()` en `Inventory.cs:167` y en `Warehouse.cs:206`
es un alias de `Delete()`, duplicando API sin valor semántico.

## 5. Cambios aplicados desde la línea base

`[CONFIRMADO]` [T-003a](migration-to-sdd-plan.md) (2026-09-17): `Inventory.ReturnToAvalible` ahora valida
`quantity <= 0` y lanza `ArgumentOutOfRangeException`. Antes, una cantidad
negativa dejaba `AvalibleQuantity` por debajo de cero, violando [INV-01](../Domain/06-business-rules.md). Se añadió
la prueba de regresión correspondiente.

`[CONFIRMADO]` Se creó el proyecto de pruebas `Zentric.Tests` (xUnit) con **164
pruebas** en verde ([verification-baseline.md :9](verification-baseline.md#9-cuarta-iteracion-adr-0003-variante-obligatoria-en-fisicos-q-10-c3)), que fijan el comportamiento
actual de `Inventory`, `Warehouse`, `User`, `Product` (incl. [CAT-03](../Domain/06-business-rules.md)),
`ProductVariant`, `VariantAttribute` y `Money`. Evidencia inicial en
[verification-baseline.md :7](verification-baseline.md#7-segunda-iteracion-suite-de-pruebas-y-correccion-t-003) y [:8](verification-baseline.md#8-tercera-iteracion-adr-0002-clave-del-inventario-variantid).

`[CONFIRMADO]` [T-010](migration-to-sdd-plan.md)/[T-004a](migration-to-sdd-plan.md) (2026-09-17, [ADR-0002](../Adr/0002-clave-inventario-variantid.md)): se implementó `ProductVariant`
(SKU = `VariantId`) y `VariantAttribute`, `Product` gestiona sus variantes con
SKU único por producto, y `Inventory` cambió su clave de `ProductId` a `VariantId`.
El inventario dejó de referenciar el producto directamente: la unidad de stock es
la variante.

`[CONFIRMADO]` [T-010c](migration-to-sdd-plan.md) (2026-09-17, [ADR-0003](../Adr/0003-variante-obligatoria-productos-fisicos.md), [Q-10](questions-for-owner.md#9-cuarta-iteracion-adr-0003-variante-obligatoria-en-fisicos-q-10-c3) = [C3](../Adr/0003-variante-obligatoria-productos-fisicos.md)): `Product` hace cumplir
[CAT-03](../Domain/06-business-rules.md) — un `Physical` exige ≥1 variante (constructor, `UpdateType`,
`RemoveVariant`) y `CanBeSold` exige variante vendible en físicos. Evidencia en
[verification-baseline.md :9](verification-baseline.md#9-cuarta-iteracion-adr-0003-variante-obligatoria-en-fisicos-q-10-c3) (164/164 PASS).

`[RIESGO]` Sigue **abierto** [H-01](spec-conformance-matrix.md): `DispatchStock` valida `AvalibleQuantity` y
decrementa `ReservedQuantity`, por lo que puede dejar el reservado negativo. Su
corrección ([T-003b](migration-to-sdd-plan.md)) requiere aprobación porque cambia comportamiento observable y
la especificación no fija explícitamente la no-negatividad del contador reservado.
---

## 6. Estado observado el 2026-09-18 — tanda no registrada

`[CONFIRMADO]` Estos artefactos existen en el working tree y **no estaban documentados** en [:1](migration-to-sdd-plan.md#fase-1-aseguramiento-sin-cambios-de-comportamiento-no-bloqueada)–[:5](migration-to-sdd-plan.md#fase-5-deuda-de-gobernanza-cerrada-en-esta-auditoria)
(auditoría de solo lectura; cero código modificado). Ver [SDD/SDD.md :7](../SDD.md#7-estado-y-proximos-pasos) para el estado tarea por
tarea y [verification-baseline.md :10](verification-baseline.md#10-quinta-iteracion-auditoria-de-la-tanda-no-registrada-2026-09-18) para la evidencia.

### 6.1 Agregados y entidades nuevos

| Artefacto de código | Tipo | Spec / Ley asociada | Estado |
|---|---|---|---|
| `Orders/CustomerOrder.cs` | AR | [ZENTRIC.md](../Domain/ZENTRIC.md) Dominio 7 (5 estados) | **implementado sin autorización** ([Q-04](questions-for-owner.md#q-04-c-04-cart-es-un-estado-de-customerorder) abierta); `TotalAmount` con moneda "USD" inventada |
| `Orders/Entities/OrderItem.cs` | entidad hija | [01-models.md :4](../Domain/01-models.md#4-bounded-context-ordering-pedidos---interfaz-del-comprador) | implementado |
| `Orders/Enums/OrderStatus.cs` | enum | [ZENTRIC.md](../Domain/ZENTRIC.md) Dominio 7 | conforme a la Ley (Cart, PendingPayment, Paid, Dispatched, Delivered) |
| `Logistics/FulfillmentOrder.cs` | AR | [ZENTRIC.md](../Domain/ZENTRIC.md) Dominio 8 (duplicado) | **incompleto**: falta `PendingPack`; nace en `Packed` |
| `Logistics/Entities/Shipment.cs` | entidad hija | `01-models` [Shipment.cs[:5](../Domain/ZENTRIC.md#dominio-5-gestion-del-catalogo)](../Zentric.Domain/Logistics/Entities/Shipment.cs#L5) | implementado |
| `Logistics/Enums/FulfillmentStatus.cs` | enum | [ZENTRIC.md](../Domain/ZENTRIC.md) Dominio 8 (duplicado) | **desviado**: 4 valores vs 5 de la Ley |
| `Billing/Invoice.cs` + `Enums/InvoiceType.cs` | AR + enum | [Ley [ADD-003](../SDD.md)](../SDD.md#addendum---dictado-por-owner-add-003-dominio-10-facturacion-y-pagos) | **incompleto**: falta "Detalle Zentric" |
| `Returns/ReturnRequest.cs` + `Enums/ReturnStatus.cs` | AR + enum | [Ley [ADD-002](../SDD.md)](../SDD.md#addendum---dictado-por-owner-add-002-dominio-9-devoluciones-y-reembolsos) / Dominio 10 | implementado; **no retorna stock** (pendiente de eventos) |
| `Products/Enums/ProductStatus.cs` | enum | [ZENTRIC.md](../Domain/ZENTRIC.md) Dominio 5 | **creado sin integrar** en `Product` |
| `Inventory.UsedQuantity`, `ReturnToUsedStock()`, `ReciveReturnedStock()` | mutaciones | [Ley [ADD-002](../SDD.md)](../SDD.md#addendum---dictado-por-owner-add-002-dominio-9-devoluciones-y-reembolsos) ("etiqueta Usado") | añadidas; el typo `Recive` sigue pendiente |

### 6.2 Capas exteriores nuevas

- `Zentric.Application`: `Common/Models/Result.cs`, 3 commands con handler, 2 puertos, **0 validadores** (FluentValidation referenciado sin uso).
- `Zentric.Infrastructure`: `ZentricDbContext` (**9 `DbSet`**), 2 repositorios, 2 migraciones EF.
- `Zentric.Api`: `Program.cs` (Composition Root), `ApiControllerBase`, 2 controladores, 3 endpoints.
- `Zentric.Tests`: 4 suites nuevas → **178 pruebas** en verde (antes 164).

### 6.3 Reglas nuevas efectivamente protegidas (con prueba)

1. Pedido entregado **no modificable** (`CustomerOrder.EnsureNotDelivered`, [:152-159](../../Zentric.Domain/Orders/CustomerOrder.cs#L152-L159)).
2. Transiciones del pedido solo en orden Cart → PendingPayment → Paid → Dispatched → Delivered.
3. No se puede hacer checkout de un carrito vacío.
4. Un despacho cancelado por quiebre no se despacha ni recibe envíos.
5. **Devolución de producto digital prohibida** ([Ley [ADD-002](../SDD.md)](../SDD.md#addendum---dictado-por-owner-add-002-dominio-9-devoluciones-y-reembolsos)).
6. La devolución no puede aprobarse sin inspección favorable previa (doble aprobación).
7. La factura de vendedor exige `VendorId` no vacío.

### 6.4 Reglas de la Ley **aún no** protegidas por el código

- `[PED-01](../Domain/06-business-rules.md)`: reserva preventiva al añadir al carrito + liberación a los 15 minutos → **no implementado**.
- [ADD-002](../SDD.md): la devolución aprobada devuelve stock con etiqueta "Usado" → **no implementado** (sin eventos).
- [ADD-003](../SDD.md): "Detalle Zentric" → **no existe** en `InvoiceType`.
- Dominio 8: estado inicial "Pendiente de Empaque" → **no existe**.
- :11: `IdentityDocument` único → **no existe** ([H-05](spec-conformance-matrix.md)).
- Invariante 6: suspensión en cascada al bloquear un vendedor → **no implementado**.
