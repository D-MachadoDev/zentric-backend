# Migration to SDD Plan — roadmap de adopción

```yaml
id: 000
type: discovery
status: in-progress
risk: medium
updated: 2026-09-18
```

## 1. Priorización (regla B5)

Orden aplicado: (1) módulo que se modificará próximamente → inventario y ordering
son el núcleo; (2) flujos con dinero/permisos/auditoría → reserva, pago, ajuste
de stock, devolución; (3) contratos públicos → aún no existen; (4) bugs → [H-01](spec-conformance-matrix.md),
[H-02](spec-conformance-matrix.md); (5) ausencia de pruebas → 0 % de cobertura; (6) deuda bloqueante → [C-01](risks-and-gaps.md)…[C-07](risks-and-gaps.md).

## 2. Fases y tareas

### Fase 0 — Gobernanza y línea base · `[x]` HECHA

| ID | Tarea | Evidencia |
|---|---|---|
| T-000a | Skill SDD v3.0.0 (router + referencias + overlay) en `.agents/skills/` | `SKILL.md` + 11 referencias |
| T-000b | Bootstrap brownfield en `SDD/00-bootstrap/` | 7 documentos |
| T-000c | Crear [`SDD/01-system-overview.md`](../01-system-overview.md) y [`SDD/02-software-architecture.md`](../02-software-architecture.md) exigidos por [AGENTS.md :0.1](../../AGENTS.md#01-consulta-obligatoria-antes-de-codificar) | ambos creados |
| T-000d | Línea base de verificación ejecutada | [`verification-baseline.md`](verification-baseline.md) |
| T-000e | Matriz de conformidad spec ↔ código | [`spec-conformance-matrix.md`](spec-conformance-matrix.md) |

### Fase 1 — Aseguramiento sin cambios de comportamiento (no bloqueada)

| ID | Tarea | Cubre | Verificación | Estado |
|---|---|---|---|---|
| T-001 | Crear proyecto `Zentric.Tests` (xUnit) y añadirlo a `Zentric.slnx` | [R-05](risks-and-gaps.md) | `dotnet test` en verde | `[x]` **hecha** |
| T-002 | Tests de caracterización de invariantes confirmadas: INV-01 (`Inventory`), reglas de `Warehouse`, usuario bloqueado (`User`), CAT-01 (`Product`), `Money` | INV-01, CAT-01, RG-02, RG-03 | 105/105 PASS | `[x]` **hecha** |
| T-002b | Tests de los VOs `Email` y `FullName` y del agregado `Buyer` | [`ZENTRIC.md`](../Domain/ZENTRIC.md) Dom.1 y Dom.2 | pendiente | `[ ]` |
| T-002c | Migrar `InventoryTests` de `ProductId` a `VariantId` + pruebas de `ProductVariant` y `VariantAttribute` | ADR-0002 | 150/150 PASS | `[x]` **hecha** |
| T-010c | Hacer cumplir CAT-03 en `Product` (constructor, `UpdateType`, `RemoveVariant`, `CanBeSold`) | ADR-0003 ([Q-10](questions-for-owner.md#9-cuarta-iteracion-adr-0003-variante-obligatoria-en-fisicos-q-10-c3) = C3) | 164/164 PASS ([verification-baseline.md :9](verification-baseline.md#9-cuarta-iteracion-adr-0003-variante-obligatoria-en-fisicos-q-10-c3)) | `[x]` **hecha** (sub-decisiones [Q-12](questions-for-owner.md#sub-decisiones-propuesto-ver-q-12) `[PROPUESTO]`) |
| T-007 | Higiene: `.editorconfig`, `TreatWarningsAsErrors`, CI mínimo (`dotnet build` + `dotnet test`) | [R-08](risks-and-gaps.md) | pipeline verde | `[ ]` |
| T-008 | Renombrado de lenguaje ubicuo ([Q-09](questions-for-owner.md#q-09-naming-canonico-pendiente)) | [R-09](risks-and-gaps.md) | build + suite en verde | `[ ]` bloqueada por [Q-05](questions-for-owner.md#q-05-c-05-vendor-o-seller)/[Q-09](questions-for-owner.md#q-09-naming-canonico-pendiente) |

### Fase 2 — Correcciones y modelo de inventario (bloqueada por decisiones)

| ID | Tarea | Cubre | Bloqueada por | Riesgo |
|---|---|---|---|---|
| T-003a | **Corregir [H-08](spec-conformance-matrix.md)**: `ReturnToAvalible` no validaba cantidad no positiva → violación real de INV-01 | INV-01 | 105/105 PASS con prueba de regresión | `[x]` **hecha** |
| T-003b | Corregir [H-01](spec-conformance-matrix.md) (`DispatchStock` valida el contador equivocado y puede dejar el reservado negativo) + prueba de regresión | INV-01, balance de stock | aprobación (cambia comportamiento observable) | `[ ]` pendiente de aprobación |
| T-004 | Rediseñar la API de `Inventory` (eliminar `UpdateQuantities`, alinear `Reserve`/`Release`/`Deduct`/`Adjust`) + abstracción de tiempo (`IClock`) + IDs fuertemente tipados (`record struct VariantId`, G-07) | [H-02](spec-conformance-matrix.md), [H-06](spec-conformance-matrix.md), G-07, [R-06](risks-and-gaps.md) | — ([Q-01](questions-for-owner.md#q-01-c-01-se-permite-fraccionar-la-reserva-entre-bodegas-resuelta-2026-09-17) y [Q-02](questions-for-owner.md#q-02-c-02-la-clave-del-inventario-es-el-producto-o-la-variante-sku-resuelta-2026-09-17) resueltas) | alto |
| T-004a | Cambiar la clave del inventario: `Inventory.ProductId` → `Inventory.VariantId` | INV-03, ADR-0002 | — | `[x]` **hecha** (riesgo medio) |
| T-005 | `ManualAdjust(qty, userId, role)` con regla dura de privilegios | INV-04 (invariante 4) | [Q-02](questions-for-owner.md#q-02-c-02-la-clave-del-inventario-es-el-producto-o-la-variante-sku-resuelta-2026-09-17) | medio |
| T-006 | `IdentityDocument` como VO obligatorio y único en `User` | [`ZENTRIC.md`](../Domain/ZENTRIC.md) Dom.1, RG-01 | [Q-07](questions-for-owner.md#q-07-documento-de-identidad-del-usuario) | medio |
| T-009 | Resolver `Buyer.PaymentTokens` vs invariante 9 | invariante 9 | [Q-08](questions-for-owner.md#q-08-r-07-buyerpaymenttokens-contra-la-invariante-9) | medio |

### Fase 3 — Núcleo de negocio faltante (tras Fase 2)

| ID | Tarea | Cubre | Bloqueada por |
|---|---|---|---|
| T-010 | `ProductVariant` (SKU) y clave de inventario definitiva | CAT-01, CAT-02, INV-03 | `[x]` **hecha** (ADR-0002) |
| T-010b | Refinamiento del modelo de atributos de variante | `02-aggregates` :2 | [Q-11](questions-for-owner.md#q-11-detalle-del-modelo-de-atributos-de-variante-abierta) |
| T-010c | Hacer cumplir CAT-03 (variante obligatoria en físicos) en `Product` | ADR-0003 ([Q-10](questions-for-owner.md#9-cuarta-iteracion-adr-0003-variante-obligatoria-en-fisicos-q-10-c3) = C3) | `[x]` **hecha** (sub-decisiones [Q-12](questions-for-owner.md#sub-decisiones-propuesto-ver-q-12) `[PROPUESTO]`) |
| T-011 | `CustomerOrder` + `OrderLine` + `FlatShippingFee` + estados | PED-02, PED-03, invariante 7 | [Q-04](questions-for-owner.md#q-04-c-04-cart-es-un-estado-de-customerorder) |
| T-012 | Eventos de dominio (7) + `IDomainEventDispatcher` | [`04-domain-events.md`](../Domain/04-domain-events.md) | ninguna |
| T-013 | `InventoryReservationService` con desglose de reserva (`ReservationDetails`) | INV-02 revisada (ADR-0001), PED-01 | [Q-03](questions-for-owner.md#q-03-c-03-estado-de-cancelacion-de-despacho) ([Q-10](questions-for-owner.md#9-cuarta-iteracion-adr-0003-variante-obligatoria-en-fisicos-q-10-c3) resuelta: todo `Physical` trae variante) |
| T-014 | `OrderSplitterService` (`FulfillmentOrder` + `Shipment`, N guías si hubo fraccionamiento) | ADR-0001, [`services/order-splitter-service.md`](../Domain/services/order-splitter-service.md) | **[Q-03](questions-for-owner.md#q-03-c-03-estado-de-cancelacion-de-despacho)** |
| T-015 | Timeout de 15 min (`CheckoutTimeoutService`) | PED-01 | T-004, T-011 |
| T-016 | Devoluciones con doble aprobación (`ReturnsApprovalService`) | DEV-01 | T-012 |
| T-017 | Suspensión en cascada al bloquear vendedor | invariante 6 | T-012 |
| T-018 | Puertos de repositorio restantes (5) | [05-ports.md :1](../Domain/05-ports.md#1-puertos-de-repositorios-persistencia) | — (clave definida por ADR-0002) |

### Fase 4 — Capas exteriores

| ID | Tarea | Cubre |
|---|---|---|
| T-019 | `Zentric.Application`: puertos de entrada, casos de uso CQRS, FluentValidation, `Result<T>` | [AGENTS.md :2](../../AGENTS.md#2-reglas-arquitectonicas-inviolables-hexagonal-ddd) |
| T-020 | `Zentric.Infrastructure`: EF Core + PostgreSQL, mapeos, migraciones | [AGENTS.md :2.1](../../AGENTS.md#21-regla-de-dependencia-estricta-y-aislamiento-de-capas) |
| T-021 | `Zentric.Api`: controladores delgados + Problem Details (RFC 7807) | [AGENTS.md :3.4](../../AGENTS.md) |
| T-022 | Tests de integración y contrato (OpenAPI) | [AGENTS.md :4.4](../../AGENTS.md#44-qa-testing-agent) |

### Fase 3b — Núcleo de negocio implementado **sin registrar** (auditado el 2026-09-18)

`[RIESGO]` Estas tareas **no se registraron al ejecutarse**; este es su primer registro. Varias se
ejecutaron con preguntas bloqueantes aún abiertas ([AGENTS.md :0.3](../../AGENTS.md#03-el-freno-de-mano-cero-asunciones) → freno de mano).

| ID | Tarea | Cubre | Estado real verificado | Evidencia |
|---|---|---|---|---|
| T-011 | `CustomerOrder` + `OrderItem` + estados | PED-02, PED-03, invariante 7 | **hecha en código, sin autorización** ([Q-04](questions-for-owner.md#q-04-c-04-cart-es-un-estado-de-customerorder) seguía abierta) | `Zentric.Domain/Orders/`, 6 pruebas |
| T-014 (parte) | `FulfillmentOrder` + `Shipment` | ADR-0001, [`order-splitter-service.md`](../Domain/services/order-splitter-service.md) | **parcial**: agregados sin servicio, sin fraccionamiento ni N guías, sin `PendingPack` | `Zentric.Domain/Logistics/`, 3 pruebas |
| T-016 (parte) | Doble aprobación de devoluciones | DEV-01 | **parcial**: agregado `ReturnRequest`, **sin** servicio y **sin** retorno a stock | `Zentric.Domain/Returns/`, 3 pruebas |
| T-018 (parte) | Puertos de repositorio | [05-ports.md :1](../Domain/05-ports.md#1-puertos-de-repositorios-persistencia) | **parcial**: 2 de 5 (`ICustomerOrderRepository`, `IFulfillmentOrderRepository`) | `Zentric.Application/**/Ports/` |
| — | `Invoice` (maestra / detalle de vendedor) | [ADD-003](../SDD.md) de la Ley | **parcial**: falta "Detalle Zentric" | `Zentric.Domain/Billing/`, 2 pruebas |
| — | `ProductStatus` (Published/Suspended/Discontinued) | [`ZENTRIC.md`](../Domain/ZENTRIC.md) Dom.5 | **creado sin integrar** en `Product` (sigue con `bool IsActive`) | `Products/Enums/ProductStatus.cs` |

### Fase 4 — Capas exteriores (auditada el 2026-09-18)

| ID | Tarea | Estado real verificado | Evidencia / hallazgo |
|---|---|---|---|
| T-019 | `Zentric.Application`: puertos de entrada, CQRS, FluentValidation, `Result<T>` | **parcial** | 3 commands + 2 puertos + `Result<T>`; **0 validadores y 0 pipeline** ([H-11](spec-conformance-matrix.md)) y `try-catch` genérico ([H-09](spec-conformance-matrix.md)) |
| T-020 | `Zentric.Infrastructure`: EF Core + PostgreSQL, mapeos, migraciones | **parcial** | `ZentricDbContext` con **9 `DbSet`** (anti-amnesia OK), 2 repositorios, 2 migraciones **nunca aplicadas**; mapea agregados de dominio ([H-13](spec-conformance-matrix.md)) |
| T-021 | `Zentric.Api`: controladores delgados + Problem Details | **parcial** | 3 endpoints (`/api/orders/cart`, `/api/orders/{id}/items`, `/api/logistics/fulfillment`); middleware "simplificado" sin `AddProblemDetails()` ([H-11](spec-conformance-matrix.md)) |
| T-022 | Tests de integración y contrato | **no hecha** | 178 pruebas, todas unitarias de dominio |

### Fase 5 — Deuda de gobernanza cerrada en esta auditoría

| ID | Tarea | Estado |
|---|---|---|
| T-023 | Crear [`SDD/SDD.md`](../SDD.md) (memoria viva exigida por la skill v6.0.0) | `[x]` **hecha** |
| T-024 | Sincronizar la skill instalada y corregir `sync-skill.ps1` | `[x]` **hecha** (el script habría borrado la skill instalada y fallado) |
| T-025 | Alinear [AGENTS.md :0.0](../../AGENTS.md#00-skill-de-ingenieria-sdd-obligatoria) con la skill v6.0.0 (ya no usa `references/`) | `[x]` **hecha** |
| T-026 | Registrar [Q-13](questions-for-owner.md#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante) (duplicación de `DOMINIO 8/9/10` en la Ley) | `[x]` **registrada**, pendiente de dictamen |

### Fase 6 — Corrección de incumplimientos de [`AGENTS.md`](../../AGENTS.md) (SPEC-007, 2026-09-18)

`[x]` HECHA. Alcance acotado a lo que **no** depende de [Q-13](questions-for-owner.md#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante)/[Q-03](questions-for-owner.md#q-03-c-03-estado-de-cancelacion-de-despacho)/[Q-04](questions-for-owner.md#q-04-c-04-cart-es-un-estado-de-customerorder).

| ID | Tarea | Cubre | Verificación | Estado |
|---|---|---|---|---|
| T-027 | `ValidationBehavior<,>` (pipeline de MediatR) que devuelve `Result.Failure` en vez de lanzar | [`:3`](../../Zentric.Application/Common/Behaviors/ValidationBehavior.cs#L3).2, [`:4`](../../Zentric.Application/Common/Behaviors/ValidationBehavior.cs#L4).3 · FR-01/FR-02 | 4 pruebas del comportamiento | `[x]` **hecha** |
| T-028 | 3 validadores FluentValidation (Orders y Logistics), espejo de las guardas del dominio | :4.3 · FR-01 | 17 pruebas de validadores | `[x]` **hecha** |
| T-029 | Eliminar el `catch (Exception)` genérico de `AddOrderItemCommandHandler` ([H-09](spec-conformance-matrix.md)) | [`:3`](../../Zentric.Application/Orders/Commands/AddOrderItemCommand.cs#L3).2 · FR-03 | regresión de [H-09](spec-conformance-matrix.md) en verde | `[x]` **hecha** |
| T-030 | `AddProblemDetails()` + `UseExceptionHandler()` + registro de validadores y del comportamiento ([H-11](spec-conformance-matrix.md)) | :3.4 · FR-04 | 7 pruebas de integración DI | `[x]` **hecha** |
| T-031 | Eliminar `Class1.cs` de Application e Infrastructure ([H-12](spec-conformance-matrix.md)) | DoD :8 · FR-05 | `Test-Path` = `False` | `[x]` **hecha** |
| T-032 | Pruebas HTTP de extremo a extremo (TestServer) | CA-04 | `[PENDIENTE]`: la API no se levantó | `[ ]` pendiente |
| [Q-14](questions-for-owner.md#q-14-licenciamiento-de-mediatr-14-y-fluentvalidation-abierta-no-bloquea-el-codigo-actual) | Confirmar condiciones de licencia de MediatR 14 / FluentValidation (hallazgo de SPEC-007) | [R-19](risks-and-gaps.md) | pendiente de dictamen del Owner | `[ ]` bloqueante de dependencias |

Evidencia: [verification-baseline.md :11](verification-baseline.md#11-sexta-iteracion-spec-007-validacion-de-entrada-rfc-7807-e-higiene-2026-09-18) (`build 0/0` · `206/206 PASS`).

## 3. Regla de avance

Ninguna tarea de Fase 2 en adelante se implementa antes de que el owner responda
las preguntas bloqueantes de [`questions-for-owner.md`](questions-for-owner.md). Las tareas de Fase 1 sí
pueden ejecutarse: no cambian comportamiento ni contratos.

## 4. Trazabilidad

Cada tarea ejecutada añade su fila a [`SDD/00-bootstrap/verification-baseline.md`](verification-baseline.md)
o al `verification.md` de su spec, con el comando exacto y el resultado.
