# Current State — comportamiento y arquitectura observados

## 1. Agregados implementados

| Artefacto de código | Tipo | Spec asociada | Estado |
|---|---|---|---|
| `Users/User.cs` | AR | `01-models.md` §1, `02-aggregates-and-entities.md` §1 | parcial |
| `Users/ValueObjects/FullName.cs` | VO | no listado explícitamente en la spec | extra |
| `Users/ValueObjects/Email.cs` | VO | `02-value-objects.md` (`EmailAddress`) | nombre desviado |
| `Users/Enums/UserRole.cs` | enum | `02-value-objects.md` (`Admin`) | nombre desviado |
| `Users/Enums/UserStatus.cs` | enum | `ZENTRIC.md` Dominio 1 (Activo, Bloqueado, etc.) | extra (`Deleted`) |
| `Users/Ports/IUserRepository.cs` | puerto | `05-ports.md` | parcial |
| `Buyers/Buyer.cs` | AR | `ZENTRIC.md` Dominio 2 (**no** en `02-aggregates`) | extra en la spec de dominio |
| `Products/Product.cs` | AR | `01-models.md` §2 | parcial (+ gestión de variantes) |
| `Products/ProductVariant.cs` | entidad hija | `02-aggregates:18-19` | **implementado** (ADR-0002) |
| `Products/ValueObjects/VariantAttribute.cs` | VO | `02-aggregates:18` | **implementado** `[PROPUESTO]` (Q-11) |
| `Products/ValueObjects/Money.cs` | VO | `02-value-objects.md` | conforme |
| `Products/Enums/ProductType.cs` | enum | `02-value-objects.md` | conforme |
| `Warehouses/Warehouse.cs` | AR | `01-models.md` §3 | parcial |
| `Warehouses/Enum/WarehouseType.cs` | enum | `02-value-objects.md` | nombre desviado (`Seller` vs `Vendor`) |
| `Inventories/Inventory.cs` | AR | `01-models.md` §3 (`InventoryItem`) | parcial + nombre desviado; clave `VariantId` (ADR-0002) |
| `Orders/Order.cs` | stub | `01-models.md` §4 (`CustomerOrder`) | **vacío** |

## 2. Reglas de negocio efectivamente protegidas hoy

`[CONFIRMADO]` por lectura del código:

1. **No-negatividad de inventario (INV-01 / invariante 1).** Protegida en el
   constructor y en cada mutación de `Inventory`
   (`Inventory.cs:33`, `:60`, `:99`, `:116`, `:132`).
2. **Bodega Marketplace sin vendedor y bodega de vendedor con dueño.**
   `Warehouse.cs:44-52`.
3. **No renombrar/reubicar/activar/eliminar bodega inactiva o borrada.**
   `Warehouse.cs:66-208`.
4. **No eliminar inventario con existencias físicas.** `Inventory.cs:174-177`
   (regla extra, no está en la spec → `[PENDIENTE]` documentarla o retirarla).
5. **Usuario bloqueado no puede cambiar nombre, correo, contraseña ni rol.**
   `User.cs:52`, `:73`, `:96`, `:158`.
6. **Producto creado nace activo (CAT-01).** `Product.cs:59`.
7. **Variante obligatoria en productos físicos (CAT-03).** Constructor,
   `UpdateType` y `RemoveVariant` en `Product.cs` (ADR-0003).
8. **Moneda homogénea en operaciones con `Money`.** `Money.cs:78-84`.
9. **Correo con formato mínimo validado.** `Email.cs:8-39`.

## 3. Arquitectura observada

`[CONFIRMADO]`

- Patrón: hexagonal + DDD respetado **en el papel**: `Zentric.Domain` no
  referencia nada (correcto y verificado en el `csproj`).
- Convención de carpetas: **módulo por bounded context** (`Users/`, `Products/`,
  `Warehouses/`, `Inventories/`, `Orders/`, `Buyers/`), con `Enums/`,
  `ValueObjects/` y `Ports/` anidados por módulo.
- "Ports" aparece dentro del módulo (`Users/Ports/`), coherente con `AGENTS.md`
  §2.2 pero distinto del catálogo genérico de la skill.

`[CONFIRMADO]` Ausencias estructurales respecto a `AGENTS.md`:

- No existen `Application`, `Infrastructure`, `Api` ni pruebas.
- No hay `Result<T>`, ni `IResult`, ni tipo de error de dominio.
- No hay eventos de dominio ni dispatcher.
- No hay tipos fuertemente tipados de ID (todos `Guid`).
- No hay `Address` VO (las direcciones son `string`).
- No hay abstracción de tiempo (`DateTime.UtcNow` directo en el dominio).

## 4. Contrato de dominio observable (puntos de dolor)

`[CONFIRMADO]` `Inventory.DispatchStock(int)` (`Inventory.cs:109-123`) valida
`AvalibleQuantity < quantity` cuando la cantidad que decrementa es
`ReservedQuantity`. Con `Available = 0`, `Reserved = 10`, `DispatchStock(5)`
decrementa `Reserved` a 5 sin error; con `quantity > ReservedQuantity` puede
dejar `ReservedQuantity` **negativo**, violando la invariante 1 referida al stock
reservado y rompiendo el balance
`Avalible + Reservado + Dañado`.

`[CONFIRMADO]` `Inventory.UpdateQuantities(...)` permite sobrescribir los tres
contadores de golpe, saltándose `Reserve/Release/Dispatch`, lo que anula la
protección de las invariantes por la puerta de atrás (setter anémico
disfrazado de método).

`[CONFIRMADO]` `MarkAsDeleted()` en `Inventory.cs:167` y en `Warehouse.cs:206`
es un alias de `Delete()`, duplicando API sin valor semántico.

## 5. Cambios aplicados desde la línea base

`[CONFIRMADO]` T-003a (2026-09-17): `Inventory.ReturnToAvalible` ahora valida
`quantity <= 0` y lanza `ArgumentOutOfRangeException`. Antes, una cantidad
negativa dejaba `AvalibleQuantity` por debajo de cero, violando INV-01. Se añadió
la prueba de regresión correspondiente.

`[CONFIRMADO]` Se creó el proyecto de pruebas `Zentric.Tests` (xUnit) con **164
pruebas** en verde (`verification-baseline.md` §9), que fijan el comportamiento
actual de `Inventory`, `Warehouse`, `User`, `Product` (incl. CAT-03),
`ProductVariant`, `VariantAttribute` y `Money`. Evidencia inicial en
`verification-baseline.md` §7 y §8.

`[CONFIRMADO]` T-010/T-004a (2026-09-17, ADR-0002): se implementó `ProductVariant`
(SKU = `VariantId`) y `VariantAttribute`, `Product` gestiona sus variantes con
SKU único por producto, y `Inventory` cambió su clave de `ProductId` a `VariantId`.
El inventario dejó de referenciar el producto directamente: la unidad de stock es
la variante.

`[CONFIRMADO]` T-010c (2026-09-17, ADR-0003, Q-10 = C3): `Product` hace cumplir
CAT-03 — un `Physical` exige ≥1 variante (constructor, `UpdateType`,
`RemoveVariant`) y `CanBeSold` exige variante vendible en físicos. Evidencia en
`verification-baseline.md` §9 (164/164 PASS).

`[RIESGO]` Sigue **abierto** H-01: `DispatchStock` valida `AvalibleQuantity` y
decrementa `ReservedQuantity`, por lo que puede dejar el reservado negativo. Su
corrección (T-003b) requiere aprobación porque cambia comportamiento observable y
la especificación no fija explícitamente la no-negatividad del contador reservado.