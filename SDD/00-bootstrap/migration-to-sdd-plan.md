# Migration to SDD Plan — roadmap de adopción

```yaml
id: 000
type: discovery
status: in-progress
risk: medium
updated: 2026-09-17
```

## 1. Priorización (regla B5)

Orden aplicado: (1) módulo que se modificará próximamente → inventario y ordering
son el núcleo; (2) flujos con dinero/permisos/auditoría → reserva, pago, ajuste
de stock, devolución; (3) contratos públicos → aún no existen; (4) bugs → H-01,
H-02; (5) ausencia de pruebas → 0 % de cobertura; (6) deuda bloqueante → C-01…C-07.

## 2. Fases y tareas

### Fase 0 — Gobernanza y línea base · `[x]` HECHA

| ID | Tarea | Evidencia |
|---|---|---|
| T-000a | Skill SDD v3.0.0 (router + referencias + overlay) en `.agents/skills/` | `SKILL.md` + 11 referencias |
| T-000b | Bootstrap brownfield en `SDD/00-bootstrap/` | 7 documentos |
| T-000c | Crear `SDD/01-system-overview.md` y `SDD/02-software-architecture.md` exigidos por `AGENTS.md` §0.1 | ambos creados |
| T-000d | Línea base de verificación ejecutada | `verification-baseline.md` |
| T-000e | Matriz de conformidad spec ↔ código | `spec-conformance-matrix.md` |

### Fase 1 — Aseguramiento sin cambios de comportamiento (no bloqueada)

| ID | Tarea | Cubre | Verificación | Estado |
|---|---|---|---|---|
| T-001 | Crear proyecto `Zentric.Tests` (xUnit) y añadirlo a `Zentric.slnx` | R-05 | `dotnet test` en verde | `[x]` **hecha** |
| T-002 | Tests de caracterización de invariantes confirmadas: INV-01 (`Inventory`), reglas de `Warehouse`, usuario bloqueado (`User`), CAT-01 (`Product`), `Money` | INV-01, CAT-01, RG-02, RG-03 | 105/105 PASS | `[x]` **hecha** |
| T-002b | Tests de los VOs `Email` y `FullName` y del agregado `Buyer` | `ZENTRIC.md` Dom.1 y Dom.2 | pendiente | `[ ]` |
| T-002c | Migrar `InventoryTests` de `ProductId` a `VariantId` + pruebas de `ProductVariant` y `VariantAttribute` | ADR-0002 | 150/150 PASS | `[x]` **hecha** |
| T-010c | Hacer cumplir CAT-03 en `Product` (constructor, `UpdateType`, `RemoveVariant`, `CanBeSold`) | ADR-0003 (Q-10 = C3) | 164/164 PASS (`verification-baseline.md` §9) | `[x]` **hecha** (sub-decisiones Q-12 `[PROPUESTO]`) |
| T-007 | Higiene: `.editorconfig`, `TreatWarningsAsErrors`, CI mínimo (`dotnet build` + `dotnet test`) | R-08 | pipeline verde | `[ ]` |
| T-008 | Renombrado de lenguaje ubicuo (Q-09) | R-09 | build + suite en verde | `[ ]` bloqueada por Q-05/Q-09 |

### Fase 2 — Correcciones y modelo de inventario (bloqueada por decisiones)

| ID | Tarea | Cubre | Bloqueada por | Riesgo |
|---|---|---|---|---|
| T-003a | **Corregir H-08**: `ReturnToAvalible` no validaba cantidad no positiva → violación real de INV-01 | INV-01 | 105/105 PASS con prueba de regresión | `[x]` **hecha** |
| T-003b | Corregir H-01 (`DispatchStock` valida el contador equivocado y puede dejar el reservado negativo) + prueba de regresión | INV-01, balance de stock | aprobación (cambia comportamiento observable) | `[ ]` pendiente de aprobación |
| T-004 | Rediseñar la API de `Inventory` (eliminar `UpdateQuantities`, alinear `Reserve`/`Release`/`Deduct`/`Adjust`) + abstracción de tiempo (`IClock`) + IDs fuertemente tipados (`record struct VariantId`, G-07) | H-02, H-06, G-07, R-06 | — (Q-01 y Q-02 resueltas) | alto |
| T-004a | Cambiar la clave del inventario: `Inventory.ProductId` → `Inventory.VariantId` | INV-03, ADR-0002 | — | `[x]` **hecha** (riesgo medio) |
| T-005 | `ManualAdjust(qty, userId, role)` con regla dura de privilegios | INV-04 (invariante 4) | Q-02 | medio |
| T-006 | `IdentityDocument` como VO obligatorio y único en `User` | `ZENTRIC.md` Dom.1, RG-01 | Q-07 | medio |
| T-009 | Resolver `Buyer.PaymentTokens` vs invariante 9 | invariante 9 | Q-08 | medio |

### Fase 3 — Núcleo de negocio faltante (tras Fase 2)

| ID | Tarea | Cubre | Bloqueada por |
|---|---|---|---|
| T-010 | `ProductVariant` (SKU) y clave de inventario definitiva | CAT-01, CAT-02, INV-03 | `[x]` **hecha** (ADR-0002) |
| T-010b | Refinamiento del modelo de atributos de variante | `02-aggregates` §2 | Q-11 |
| T-010c | Hacer cumplir CAT-03 (variante obligatoria en físicos) en `Product` | ADR-0003 (Q-10 = C3) | `[x]` **hecha** (sub-decisiones Q-12 `[PROPUESTO]`) |
| T-011 | `CustomerOrder` + `OrderLine` + `FlatShippingFee` + estados | PED-02, PED-03, invariante 7 | Q-04 |
| T-012 | Eventos de dominio (7) + `IDomainEventDispatcher` | `04-domain-events.md` | ninguna |
| T-013 | `InventoryReservationService` con desglose de reserva (`ReservationDetails`) | INV-02 revisada (ADR-0001), PED-01 | Q-03 (Q-10 resuelta: todo `Physical` trae variante) |
| T-014 | `OrderSplitterService` (`FulfillmentOrder` + `Shipment`, N guías si hubo fraccionamiento) | ADR-0001, `services/order-splitter-service.md` | **Q-03** |
| T-015 | Timeout de 15 min (`CheckoutTimeoutService`) | PED-01 | T-004, T-011 |
| T-016 | Devoluciones con doble aprobación (`ReturnsApprovalService`) | DEV-01 | T-012 |
| T-017 | Suspensión en cascada al bloquear vendedor | invariante 6 | T-012 |
| T-018 | Puertos de repositorio restantes (5) | `05-ports.md` §1 | — (clave definida por ADR-0002) |

### Fase 4 — Capas exteriores

| ID | Tarea | Cubre |
|---|---|---|
| T-019 | `Zentric.Application`: puertos de entrada, casos de uso CQRS, FluentValidation, `Result<T>` | `AGENTS.md` §2 |
| T-020 | `Zentric.Infrastructure`: EF Core + PostgreSQL, mapeos, migraciones | `AGENTS.md` §2.1 |
| T-021 | `Zentric.Api`: controladores delgados + Problem Details (RFC 7807) | `AGENTS.md` §3.4 |
| T-022 | Tests de integración y contrato (OpenAPI) | `AGENTS.md` §4.4 |

## 3. Regla de avance

Ninguna tarea de Fase 2 en adelante se implementa antes de que el owner responda
las preguntas bloqueantes de `questions-for-owner.md`. Las tareas de Fase 1 sí
pueden ejecutarse: no cambian comportamiento ni contratos.

## 4. Trazabilidad

Cada tarea ejecutada añade su fila a `SDD/00-bootstrap/verification-baseline.md`
o al `verification.md` de su spec, con el comando exacto y el resultado.