# Capa de Infraestructura - Persistencia y Adaptadores

Este documento define la estrategia de acceso a datos para Zentric-Backend.

## 1. ORM y Base de Datos
- **Proveedor:** PostgreSQL.
- **ORM:** Entity Framework Core 10 (`Npgsql.EntityFrameworkCore.PostgreSQL`).
- **Contexto Principal:** `ZentricDbContext` ubicado en `Zentric.Infrastructure.Persistence`.

## 2. Mapeo de Entidades (Entity Mapping Audit)
Para evitar amnesia de entidades (Regla 2.4 de AGENTS.md), el DbContext incluye obligatoriamente:
1. `Users` (Usuario general, ValueObjects: FullName, Email)
2. `Buyers` (Compradores)
3. `Products` (Agregado Raíz)
   - *OwnsMany:* `Variants` (SKUs)
     - *OwnsMany:* `Attributes` (Talla, Color)
4. `Inventories` (Manejo de stock: físico, reservado, dañado, usado)
5. `Warehouses` (Bodegas físicas)
6. `CustomerOrders` (Pedidos de compra)
   - *OwnsMany:* `Items`
7. `FulfillmentOrders` (Despachos logísticos)
   - *OwnsMany:* `Shipments`
8. `ReturnRequests` (Devoluciones, prohibido para digitales)
9. `Invoices` (Facturas maestras y de vendedor)

## 3. Repositorios Concretos (Adaptadores de Salida)
Implementan las interfaces definidas en la capa de Aplicación:
- `CustomerOrderRepository`: Administra la persistencia de los pedidos y sus items asociados.
- `FulfillmentOrderRepository`: Administra la persistencia de las órdenes logísticas.
- *(Pendientes de implementar)*: `ReturnRequestRepository`, `InvoiceRepository`, `ProductRepository`, etc.

> `[CONFIRMADO]` **(2026-09-18)** Implementados: **solo** `CustomerOrderRepository` y
> `FulfillmentOrderRepository`. Pendientes y **sin puerto definido todavía**: `ReturnRequest`,
> `Invoice`, `Product`, `Warehouse`, `Inventory` y `User` (el puerto `IUserRepository` existe en
> `Zentric.Domain/Users/Ports/` pero **no tiene implementación** → [H-07](../00-bootstrap/spec-conformance-matrix.md)).

## 4. Estrategia de Migraciones
Las migraciones se generan usando `Zentric.Api` como Composition Root (Startup Project). 
- **Migración actual:** `CompleteSchema` (Generada con éxito, contiene todas las tablas históricas y nuevas).

> `[RIESGO]` **(2026-09-18)** Las migraciones `InitialCreate` (`20260918192602`) y `CompleteSchema`
> (`20260918193539`) están **generadas pero nunca aplicadas** contra un PostgreSQL, y no existen
> pruebas de integración. Su corrección en runtime se desconoce `[PENDIENTE]`.

## 5. Desviaciones registradas (2026-09-18)

| ID | Desviación | Evidencia | Estado |
|---|---|---|---|
| [H-13](../00-bootstrap/spec-conformance-matrix.md) | El `DbContext` mapea **directamente** los agregados de dominio, sin entidades EF separadas ni mappers | `ZentricDbContext.OnModelCreating` | `[RIESGO]` contradice [AGENTS.md :4.2](../../AGENTS.md#42-infrastructure-adapter-agent); requiere ADR o corrección |
| — | Los `OwnsMany`/`OwnsOne` de `Money`, `VariantAttribute` y `Email` no están validados en ejecución | `ZentricDbContext.cs:39-88` | `[PENDIENTE]` |
| — | `Inventory` tiene clave `Id`; la unicidad de `(VariantId, WarehouseId)` **no** está declarada como índice | `ZentricDbContext.cs:71` | `[PENDIENTE]` |
| — | No hay `IUnitOfWork` ni transacciones explícitas: cada repositorio hace `SaveChangesAsync` | `CustomerOrderRepository.cs:20` | `[RIESGO]` patrón declarado en [AGENTS.md :1](../../AGENTS.md#1-vision-general-del-proyecto) |


