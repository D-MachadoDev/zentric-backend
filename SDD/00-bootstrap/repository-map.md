# Repository Map — zentric-backend

`[CONFIRMADO]` por inspección directa del árbol de archivos y de `Zentric.slnx`.

## 1. Raíz del repositorio

| Elemento | Tipo | Nota |
|---|---|---|
| `Zentric.slnx` | solución | referencia **5** proyectos: `Zentric.Domain`, `Zentric.Application`, `Zentric.Infrastructure`, `Zentric.Api`, `Zentric.Tests` |
| [AGENTS.md](../../AGENTS.md) | gobernanza | directrices de agentes, arquitectura y DoD (actualizado 2026-09-18: skill v6.0.0) |
| ~~`generic-sdd-agent.md`~~ | gobernanza | ~~prompt SDD v2.0.0 monolítico — superseded~~ → **ELIMINADO el 2026-09-18**: el Owner autorizó su eliminación (el propio archivo la pedía); recuperable con `git checkout -- generic-sdd-agent.md` |
| `SDD/` | especificaciones | SSoT declarada por [AGENTS.md :0](../../AGENTS.md#0-enrutador-y-principios-de-spec-driven-development-sdd) |
| [SDD/SDD.md](../SDD.md) | gobernanza | **memoria viva** creada el 2026-09-18 (mapa de entidades, ADDENDA, verificación, riesgos, estado) |
| `Zentric.Domain/` | código | núcleo de dominio, sin dependencias externas |
| `Zentric.Application/` | código | casos de uso, puertos, `Result<T>` (MediatR + FluentValidation) |
| `Zentric.Infrastructure/` | código | EF Core + Npgsql, `ZentricDbContext`, repositorios, migraciones |
| `Zentric.Api/` | código | Composition Root + controladores REST |
| `Zentric.Tests/` | pruebas | xUnit, **178** casos en verde |
| [README.md](README.md) | doc | 2 líneas, sin instrucciones de ejecución |
| `LICENSE` | legal | — |
| `.gitignore` | configuración | 7.906 bytes |
| `.vscode/settings.json` | configuración | preferencias del editor |
| `.agents/skills/generic-sdd-agent/` | tooling | skill SDD **v6.0.0** (documento único + `scripts/sync-skill.ps1`) |

## 2. Proyectos de la solución

| Proyecto | Framework | Referencias | Estado |
|---|---|---|---|
| `Zentric.Domain` | `net10.0` | ninguna (correcto: centro puro) | implementación parcial (9 agregados) |
| `Zentric.Application` | `net10.0` | `Zentric.Domain` + MediatR 14.2.0 + FluentValidation 12.1.1 | **parcial** (3 commands, 2 puertos, sin validadores) |
| `Zentric.Infrastructure` | `net10.0` | `Zentric.Application` + Npgsql EF Core 10.0.3 | **parcial** (DbContext, 2 repos, 2 migraciones sin aplicar) |
| `Zentric.Api` | `net10.0` | `Application` + `Infrastructure` + ASP.NET OpenAPI | **parcial** (3 endpoints, middleware simplificado) |
| `Zentric.Tests` | `net10.0` | `Zentric.Domain` + xUnit 2.9.3 | **178** casos en verde |

`[CONFIRMADO]` Regla de dependencia verificada el 2026-09-18 leyendo los 5 `.csproj`:
`Domain` ← `Application` ← `Infrastructure` ← `Api` (nadie referencia a `Api`, y `Domain` no
referencia nada). `Zentric.Domain.csproj` sigue sin `PackageReference`.

## 3. Estructura de `Zentric.Domain`

```text
Zentric.Domain/
├── Billing/
│   ├── Invoice.cs                          # AR (factura maestra / detalle de vendedor)
│   └── Enums/InvoiceType.cs
├── Buyers/Buyer.cs
├── Inventories/Inventory.cs                # stock por (VariantId, WarehouseId); incl. UsedQuantity
├── Logistics/
│   ├── FulfillmentOrder.cs                 # AR de despacho
│   ├── Entities/Shipment.cs
│   └── Enums/FulfillmentStatus.cs
├── Orders/
│   ├── CustomerOrder.cs                    # AR del pedido (Cart → Delivered)
│   ├── Entities/OrderItem.cs
│   └── Enums/OrderStatus.cs
├── Products/
│   ├── Product.cs, ProductVariant.cs
│   ├── Enums/ProductType.cs, ProductStatus.cs
│   ── ValueObjects/Money.cs, VariantAttribute.cs
├── Returns/
│   ├── ReturnRequest.cs
│   └── Enums/ReturnStatus.cs
├── Users/
│   ├── User.cs
│   ├── Enums/UserRole.cs, UserStatus.cs
│   ├── Ports/IUserRepository.cs
│   └── ValueObjects/Email.cs, FullName.cs
└── Warehouses/
    ├── Warehouse.cs
    └── Enum/WarehouseType.cs
```

Observaciones `[CONFIRMADO]`:

- El código se organiza por **módulo de dominio** (carpeta por contexto), con `Enums/`,
  `ValueObjects/`, `Entities/` y `Ports/` anidados. Convención del repo: **respetarla**.
- `Ports/` existe dentro de un módulo (`Users/Ports/IUserRepository.cs`), mientras que los puertos
  nuevos (`Orders`, `Logistics`) viven en `Zentric.Application/**/Ports/`: **ubicación inconsistente**
  (ver [spec-conformance-matrix.md :6](spec-conformance-matrix.md#6-fulfillment-devoluciones-eventos-y-puertos) y [SDD/SDD.md :2](../.agents/skills/generic-sdd-agent/SKILL.md#14-mapa-de-entidades-y-auditoria-de-mapeo-completo-anti-amnesia))](../SDD.md#2-mapa-de-entidades-procedimiento-skill-14)).
- `Orders/Order.cs` (stub vacío) fue reemplazado por `Orders/CustomerOrder.cs`; el stub ya no existe.
- No hay carpeta de eventos de dominio, ni tipos fuertemente tipados de ID, ni VO `Address`.
- Los archivos `bin/` y `obj/` están presentes en el árbol de trabajo (generados).

## 4. Estructura de `SDD/`

```text
SDD/
├── SDD.md                                         # memoria viva (2026-09-18)
├── 01-system-overview.md
├── 02-software-architecture.md
├── 00-bootstrap/                                  # SPEC-000 (8 documentos)
├── Adr/                                           # [ADR-0001](../Adr/0001-reserva-fragmentacion-contingencia.md), [ADR-0002](../Adr/0002-clave-inventario-variantid.md), [ADR-0003](../Adr/0003-variante-obligatoria-productos-fisicos.md)
├── Application/01-use-cases-and-ports.md           # SPEC-003
├── Infrastructure/01-data-access.md                # SPEC-004
├── Presentation/01-endpoints.md                    # SPEC-005
└── Domain/
    ├── ZENTRIC.md                                 # Biblia del cliente + ADDENDA (16 KB)
    ├── Software-arquitecture.md                   # 0 bytes  [CONTRADICCIÓN: vacío]
    ├── 01-domain-overview.md
    ├── 01-models.md
    ├── 02-aggregates-and-entities.md
    ├── 02-value-objects.md
    ├── 03-domain-services.md
    ├── 03-value-objects.md
    ├── 04-domain-events.md
    ├── 04-invariants-and-rules.md
    ├── 05-ports.md
    ├── 06-business-rules.md
    ├── 07-lifecycle.md
    └── services/
        ├── checkout-timeout-service.md
        ├── inventory-reservation-service.md
        ├── order-splitter-service.md
        └── returns-approval-service.md
```

`[CONTRADICCIÓN]` [AGENTS.md :0.1](../../AGENTS.md#01-consulta-obligatoria-antes-de-codificar) exige como lectura obligatoria [SDD/01-system-overview.md](../01-system-overview.md),
[SDD/02-software-architecture.md](../02-software-architecture.md), `SDD/Application/`, `SDD/Infrastructure/` y `SDD/Presentation/`:
los dos primeros se crearon el 2026-09-17 y las tres carpetas de capa existen desde el 2026-09-18.
Queda **[C-06](risks-and-gaps.md)**: [SDD/Domain/Software-arquitecture.md](../Domain/Software-arquitecture.md) sigue con **0 bytes**.

`[CONTRADICCIÓN]` Numeración duplicada dentro de `SDD/Domain/`: dos `01-*`, dos `02-*`, dos `03-*`,
dos `04-*`, con solapes y contradicciones de contenido (**[C-07](risks-and-gaps.md)** → [Q-06](questions-for-owner.md#q-06-c-07-consolidacion-de-los-documentos-numerados)).

## 5. Puntos de entrada ejecutables

`[CONFIRMADO]` (2026-09-18) Existe **un** punto de entrada HTTP:

| Ruta | Método | Controlador | Caso de uso |
|---|---|---|---|
| `/api/orders/cart` | POST | `OrdersController.CreateCart` | `CreateCartCommand` |
| `/api/orders/{id}/items` | POST | `OrdersController.AddItem` | `AddOrderItemCommand` |
| `/api/logistics/fulfillment` | POST | `LogisticsController.CreateFulfillment` | `CreateFulfillmentOrderCommand` |

- Composition Root: `Zentric.Api/Program.cs` (DbContext Npgsql, 2 repositorios en DI, MediatR).
- Configuración: `appsettings.json` con `ConnectionStrings:DefaultConnection` (**credenciales en claro → [R-16](risks-and-gaps.md)**).
- `[PENDIENTE]` No hay worker, CLI, ni script de migración documentado; las migraciones EF existen pero **nunca se aplicaron**.
- `[PENDIENTE]` La API **no se ha levantado** en este entorno: la evidencia es estática (código y build).

