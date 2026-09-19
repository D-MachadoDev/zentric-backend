# SDD — memoria viva del proyecto

mapa-base: working tree sobre 40aea11 (cambios locales sin commit) · última auditoría de mapa: 2026-09-18 · cobertura: 30/33 (91 %) · perfil: Personal [INFERIDO] (1 desarrollador, sin CI, decisiones por chat)

> **Rol de este archivo** (skill generic-sdd-agent v6.0.0, secciones :0.12 y :13.1-d de la skill): este repo **ya declara su SSoT** en [AGENTS.md:0](../AGENTS.md#0-enrutador-y-principios-de-spec-driven-development-sdd) — la carpeta SDD/. Por la regla "no se crea una SSoT paralela", este documento **no duplica** las especificaciones: es la **memoria viva** (índice, mapa de entidades, decisiones/ADDENDA, verificación, riesgos y estado) y **apunta** a los documentos canónicos.
>
> **Jerarquía de verdad:** decisión del Owner → [AGENTS.md](../AGENTS.md) → [SDD/Domain/ZENTRIC.md](Domain/ZENTRIC.md) (Ley, intocable) → resto de SDD/ → este archivo.

## 1. Contexto y alcance

- **Propósito observado** [OBSERVADO]: API central y núcleo de dominio de **Zentric**, plataforma que intermedia entre **Comprador** y **Vendedor** administrando usuarios, catálogo, inventario distribuido, pedidos, facturación, logística y posventa ([SDD/Domain/ZENTRIC.md:1](Domain/ZENTRIC.md#dominio-1-administracion-de-usuarios)).
- **Owner:** no registrado en el repositorio [PENDIENTE].
- **Biblia (intocable, :0.7):** [SDD/Domain/ZENTRIC.md](Domain/ZENTRIC.md). **Integridad verificada el 2026-09-18:** git diff --numstat = **38 adiciones / 1 borrado**; el único borrado es una línea separadora, por lo que el texto original del cliente **no fue reescrito**. Las adiciones están bajo # [ADDENDUM - DICTADO POR OWNER] → transcripción en [SDD.md:4](SDD.md) ([ADD-001](SDD.md)…[ADD-003](SDD.md)).
- **Otros documentos normativos:** [AGENTS.md](../AGENTS.md) (contrato operativo), [SDD/01-system-overview.md](01-system-overview.md), [SDD/02-software-architecture.md](02-software-architecture.md), SDD/Domain/*, SDD/Application/*, SDD/Infrastructure/*, SDD/Presentation/*, SDD/Adr/*.
- **Alcance del sistema:** [SDD/01-system-overview.md:3.1](01-system-overview.md) (incluidos) y [SDD/01-system-overview.md:3.2](01-system-overview.md) (excluidos: UI, apps móviles, portales, autenticación técnica, tecnologías de implementación y almacenamiento).
- **Fuera de alcance de este archivo:** detalle de reglas de dominio (SDD/Domain/), contratos HTTP (SDD/Presentation/), mapeo EF (SDD/Infrastructure/).
- **Stack verificado** [CONFIRMADO]: C#/.NET 10 · Zentric.slnx con **5 proyectos** · xUnit · MediatR 14.2.0 · FluentValidation 12.1.1 · Npgsql.EntityFrameworkCore.PostgreSQL 10.0.3.
- **Comandos oficiales** ([AGENTS.md :7](../AGENTS.md#7-comandos-de-verificacion-y-compilacion)): dotnet restore · dotnet build Zentric.slnx · dotnet test Zentric.slnx.
- **Riesgo residual aceptado hoy** ([risks-and-gaps.md :4](00-bootstrap/risks-and-gaps.md#4-riesgo-residual-aceptado-hoy)): sin entorno productivo, sin datos reales y sin consumidores del contrato → renombrar hoy es barato; aplazarlo encarece cada artefacto nuevo.

## 2. Mapa de entidades

> **Procedimiento:** [generic-sdd-agent v6.0.0 :14](.agents/skills/generic-sdd-agent/SKILL.md#14-mapa-de-entidades-y-auditoria-de-mapeo-completo-anti-amnesia) (Auditoría de Mapeo Completo - Anti-Amnesia)

> **Estado de verificación:** `[CONFIRMADO]` por enumeración directa del árbol, de los `.csproj`, de `Program.cs`, de `ZentricDbContext` y de las suites de `Zentric.Tests`. Ver [SDD.md:14](#2-mapa-de-entidades) para el procedimiento completo.

### 2.1 Resumen ejecutivo

| Métrica | Valor |
|---|---|
| **Entidades mapeadas** | 30 (E-001 a E-030) |
| **Agregados raíz** | 14 (User, Buyer, Product, Inventory, Warehouse, CustomerOrder, FulfillmentOrder, Invoice, ReturnRequest + puertos/servicios) |
| **Cobertura** | 30/33 (91 %) — ver [C-06](00-bootstrap/risks-and-gaps.md) para huérfanos |
| **Pruebas asociadas** | 178 casos de prueba en verde |

### 2.2 Tabla de entidades

| ID | Entidad | Tipo | Ubicación en código | Responsabilidad | Relaciones / Dependencias | Estado | Pruebas |
|---|---|---|---|---|---|---|---|
| **E-001** | [User](Zentric.Domain/Users/User.cs) | Agregado raíz | [Zentric.Domain/Users/User.cs](../Zentric.Domain/Users/User.cs) | Usuario del marketplace. Rol único. Bloqueo de usuario. | Usa `FullName`, `Email`, `UserRole`, `UserStatus` | ⚠️ **Parcial** — sin `IdentityDocument` ([H-05](00-bootstrap/spec-conformance-matrix.md)) | [Tests/Users/UserTests.cs](../Zentric.Tests/Users/UserTests.cs) |
| **E-002** | [Buyer](Zentric.Domain/Buyers/Buyer.cs) | Agregado raíz | [Zentric.Domain/Buyers/Buyer.cs](../Zentric.Domain/Buyers/Buyer.cs) | Comprador. Dirección principal y adicionales. | Usa `PaymentTokens` ([H-03](00-bootstrap/spec-conformance-matrix.md)) | ⚠️ **Parcial** — sin validaciones completas | 🟡 `[PENDIENTE]` — [T-002b](00-bootstrap/migration-to-sdd-plan.md) |
| **E-003** | [Product](Zentric.Domain/Products/Product.cs) | Agregado raíz | [Zentric.Domain/Products/Product.cs](../Zentric.Domain/Products/Product.cs) | Catálogo de productos. Gestiona variantes. Reglas [CAT-03](Domain/06-business-rules.md) (variante obligatoria en físicos). | Posee **E-004** (ProductVariant). Usa `Money`, `ProductType`. | ✅ **Implementado** — [ADR-0002](Adr/0002-clave-inventario-variantid.md)/0003 | [Products/ProductTests.cs](../Zentric.Tests/Products/ProductTests.cs) |
| **E-004** | [ProductVariant](Products/ProductVariant.cs) | Entidad hija | [Products/ProductVariant.cs](../Zentric.Domain/Products/ProductVariant.cs) | SKU. Unidad de stock indivisible. | Usa **E-005** (VariantAttribute). Pertenece a **E-003**. | ✅ **Implementado** | [Products/ProductVariantTests.cs](../Zentric.Tests/Products/ProductVariantTests.cs) |
| **E-005** | [VariantAttribute](Products/ValueObjects/VariantAttribute.cs) | Value Object | [Products/ValueObjects/VariantAttribute.cs](../Zentric.Domain/Products/ValueObjects/VariantAttribute.cs) | Atributo nombre/valor (ej. Talla, Color). Inmutable. | Usada por **E-004**. | ✅ **Implementado** — `[PROPUESTO]` ([Q-11](00-bootstrap/questions-for-owner.md#q-11-detalle-del-modelo-de-atributos-de-variante-abierta)) | [Products/VariantAttributeTests.cs](../Zentric.Tests/Products/VariantAttributeTests.cs) |
| **E-006** | [Money](Products/ValueObjects/Money.cs) | Value Object | [Products/ValueObjects/Money.cs](../Zentric.Domain/Products/ValueObjects/Money.cs) | Importe + moneda. Aritmética homogénea (misma moneda). Inmutable. | Usado por **E-003**, **E-009**, **E-010**, **E-013**. | ✅ **Implementado** | [Products/MoneyTests.cs](../Zentric.Tests/Products/MoneyTests.cs) |
| **E-007** | [Inventory](Inventories/Inventory.cs) | Agregado raíz | [Inventories/Inventory.cs](../Zentric.Domain/Inventories/Inventory.cs) | Stock por `(VariantId, WarehouseId)`. Contadores: Available, Reserved, Damaged, Used ([Inventory.cs[:14](00-bootstrap/questions-for-owner.md#q-14q-14-licenciamiento-de-mediatr-14-y-fluentvalidation-abierta-no-bloquea-el-codigo-actual-licenciamiento-de-mediatr-14-y-fluentvalidation-abierta-no-bloquea-el-codigo-actual)](../Zentric.Domain/Inventories/Inventory.cs#L14)). | Clave `VariantId` ([ADR-0002](Adr/0002-clave-inventario-variantid.md)). Relación con **E-008** (Warehouse). | ⚠️ **Parcial** — [H-01](00-bootstrap/spec-conformance-matrix.md), [H-02](00-bootstrap/spec-conformance-matrix.md), [H-04](00-bootstrap/spec-conformance-matrix.md) abiertos | [Inventories/InventoryTests.cs](../Zentric.Tests/Inventories/InventoryTests.cs) |
| **E-008** | [Warehouse](Warehouses/Warehouse.cs) | Agregado raíz | [Warehouses/Warehouse.cs](../Zentric.Domain/Warehouses/Warehouse.cs) | Bodega del marketplace o de vendedor. Tipos: Marketplace, Seller. | Usa `WarehouseType`. Relación con **E-007** (Inventory). | ⚠️ **Parcial** — [C-05](00-bootstrap/risks-and-gaps.md) (nombramiento desviado: `Seller` vs `Vendor`) | [Warehouses/WarehouseTests.cs](../Zentric.Tests/Warehouses/WarehouseTests.cs) |
| **E-009** | CustomerOrder | Agregado | [Orders/CustomerOrder.cs](../Zentric.Domain/Orders/CustomerOrder.cs) | Pedido consolidado; estados Cart→Delivered; nace en `Cart` (`[CustomerOrder.cs:49](../Zentric.Domain/Orders/CustomerOrder.cs#L49)) | posee E-010; usa E-006 | implementado con desviaciones ([CustomerOrder.cs:24](../Zentric.Domain/Orders/CustomerOrder.cs#L24), moneda "USD" inventada) | [Orders/CustomerOrderTests.cs](../Zentric.Tests/Orders/CustomerOrderTests.cs) |
| **E-010** | [OrderItem](Orders/Entities/OrderItem.cs) | Entidad hija | [Orders/Entities/OrderItem.cs](../Zentric.Domain/Orders/Entities/OrderItem.cs) | Línea del pedido. Variante, cantidad, precio unitario. | Usa **E-006** (Money). Pertenece a **E-009** (CustomerOrder). | ✅ **Implementado** | [Orders/CustomerOrderTests.cs](../Zentric.Tests/Orders/CustomerOrderTests.cs) |
| **E-011** | [FulfillmentOrder](Logistics/FulfillmentOrder.cs) | Agregado raíz | [Logistics/FulfillmentOrder.cs](../Zentric.Domain/Logistics/FulfillmentOrder.cs) | Despacho por vendedor. Nace en Packed ([FulfillmentOrder.cs:33](../Zentric.Domain/Logistics/FulfillmentOrder.cs#L33)). | Posee **E-012** (Shipment). Relación con **E-008** (Warehouse). | ❌ **Incompleto** — falta `PendingPack` ([C-08](00-bootstrap/risks-and-gaps.md)). Ver [Q-13](00-bootstrap/questions-for-owner.md#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante) | [Logistics/FulfillmentOrderTests.cs](../Zentric.Tests/Logistics/FulfillmentOrderTests.cs) |
| **E-012** | [Shipment](Logistics/Entities/Shipment.cs) | Entidad hija | [Logistics/Entities/Shipment.cs](../Zentric.Domain/Logistics/Entities/Shipment.cs) | Guía/envío por bodega. Nace en línea 5 ([Shipment.cs:5](../Zentric.Domain/Logistics/Entities/Shipment.cs#L5)). | Usada por **E-011** (FulfillmentOrder). Relación con **E-008** (Warehouse). | ✅ **Implementado** | [Logistics/FulfillmentOrderTests.cs](../Zentric.Tests/Logistics/FulfillmentOrderTests.cs) |
| **E-013** | [Invoice](Billing/Invoice.cs) | Agregado raíz | [Billing/Invoice.cs](../Zentric.Domain/Billing/Invoice.cs) | Factura maestra/split. Fábricas: `CreateMaster`, `CreateVendorDetail`. | Usa **E-006** (Money). Relación con **E-003** (Product). | ❌ **Incompleto** — falta "Detalle Zentric" ([C-08](00-bootstrap/risks-and-gaps.md)). Ver [Q-13](00-bootstrap/questions-for-owner.md#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante) | [Billing/InvoiceTests.cs](../Zentric.Tests/Billing/InvoiceTests.cs) |
| **E-014** | [ReturnRequest](Returns/ReturnRequest.cs) | Agregado raíz | [Returns/ReturnRequest.cs](../Zentric.Domain/Returns/ReturnRequest.cs) | Devolución con doble aprobación. Prohíbe devolución de digitales. | Usa `ProductType`, `ReturnStatus`. Relación con **E-003** (Product). | ⚠️ **Implementado** — **no toca inventario** (falta retorno a stock con etiqueta "Usado"). Ver [C-08](00-bootstrap/risks-and-gaps.md) | [Returns/ReturnRequestTests.cs](../Zentric.Tests/Returns/ReturnRequestTests.cs) |
| E-015 | IUserRepository | Puerto de salida | Zentric.Domain/Users/Ports/IUserRepository.cs | Acceso a usuarios; unicidad de correo marcada como TODO | **no** implementado en Infrastructure | parcial ([H-07](00-bootstrap/spec-conformance-matrix.md)) | — |
| E-016 | ICustomerOrderRepository | Puerto de salida | Zentric.Application/Orders/Ports/ | Persistencia de pedidos | implementado por E-022 | OK | — |
| E-017 | IFulfillmentOrderRepository | Puerto de salida | Zentric.Application/Logistics/Ports/ | Persistencia de despachos | implementado por E-022 | OK | — |
| E-018 | Result / Result<T> | Tipo de aplicación | Zentric.Application/Common/Models/Result.cs | Resultado explícito, sin excepciones de flujo | usado por E-019, E-020 y E-024 | implementado | — |
| E-019 | CreateCartCommand, AddOrderItemCommand | Casos de uso | Zentric.Application/Orders/Commands/ | Crear carrito; añadir ítem | usa E-016 y E-009 | implementado con hallazgo **[H-09](00-bootstrap/spec-conformance-matrix.md)** | — |
| E-020 | CreateFulfillmentOrderCommand | Caso de uso | Zentric.Application/Logistics/Commands/ | Derivar despacho de un pedido | usa E-017 y E-011 | implementado | — |
| E-021 | ZentricDbContext | Datos (adaptador) | Zentric.Infrastructure/Persistence/ZentricDbContext.cs | 9 DbSet + mapeo OwnsMany/OwnsOne | mapea E-001…E-014 | implementado; mapea agregados de dominio ([H-13](00-bootstrap/spec-conformance-matrix.md)) | — |
| E-022 | CustomerOrderRepository, FulfillmentOrderRepository | Adaptadores de salida | Zentric.Infrastructure/Repositories/ | Implementan E-016/E-017 | usa E-021 | implementado | — |
| E-023 | Migraciones EF InitialCreate, CompleteSchema | Datos | Zentric.Infrastructure/Migrations/ | Esquema PostgreSQL | derivan de E-021 | generadas; **nunca aplicadas a un servidor** | — |
| E-024 | Program.cs, ApiControllerBase, OrdersController, LogisticsController | Puntos de entrada | Zentric.Api/ | Composition Root + 3 endpoints REST | usa E-018, E-019, E-020 | implementado; ProblemDetails incompleto (**[H-11](00-bootstrap/spec-conformance-matrix.md)**) | — |
| E-025 | Suites xUnit (11 archivos) | Pruebas | `Zentric.Tests/` | 178 casos en verde | cubre E-001…E-014 | implementado | — |
| E-026 | `Zentric.slnx` | Configuración | raíz | Ensambla los 5 proyectos | — | OK | — |
| E-027 | Skill `generic-sdd-agent` + `scripts/sync-skill.ps1` | Operación | `.agents/skills/generic-sdd-agent/` | Metodología operativa del agente | copia instalada en `%USERPROFILE%\.agents\skills` | **v6.0.0** (repo) vs **v3.1.0** (instalada) → **[C-09](00-bootstrap/risks-and-gaps.md)** | — |
| E-028 | Documentos `SDD/` | Documentos | `SDD/**` | SSoT del sistema | apunta a E-029 | parcial (**[C-06](00-bootstrap/risks-and-gaps.md)**: [Software-arquitecture.md](Domain/Software-arquitecture.md) = 0 bytes) | — |
| E-029 | [ZENTRIC.md](Domain/ZENTRIC.md) | Biblia (Ley) | [SDD/Domain/ZENTRIC.md](Domain/ZENTRIC.md) | Especificación funcional del cliente | rige E-001…E-014 | intacta + ADDENDA (`[ADD-001](SDD.md)…003`) | — |
| E-030 | [ADR-0001](Adr/0001-reserva-fragmentacion-contingencia.md)…0003 | Decisiones | `SDD/Adr/` | Reserva/fraccionamiento, clave de stock, variante obligatoria | rigen E-007, E-004, E-003 | aprobadas por el Owner | — |

**Ajenos / generados (clasificados, no mapeados):** `bin/`, `obj/` (generado), `LICENSE`, `.gitignore`, `.vscode/settings.json`, `Zentric.Api.http`, `appsettings.Development.json`.

**Huérfanos (pendientes de acción):** `Zentric.Application/Class1.cs` y `Zentric.Infrastructure/Class1.cs` → **eliminados en SPEC-007 (2026-09-18)**, cerrado · [SDD/Domain/Software-arquitecture.md](Domain/Software-arquitecture.md) (0 bytes, **[C-06](00-bootstrap/risks-and-gaps.md)**) · `generic-sdd-agent.md` raíz (v2.0.0, declarado **superseded**).

**Altas del 2026-09-18 (SPEC-006/SPEC-007):** E-031 `ValidationBehavior<TRequest,TResponse>` (`Zentric.Application/Common/Behaviors/ValidationBehavior.cs`) · E-032 validadores `CreateCartCommandValidator`, `AddOrderItemCommandValidator`, `CreateFulfillmentOrderCommandValidator` (`*/Validators/`) · E-033 [SDD/SDD.md](SDD.md) (memoria viva).

**Fantasmas (se mencionan y no existen):** `IDomainEventDispatcher` y los 7 eventos de dominio (G-04/[T-012](00-bootstrap/migration-to-sdd-plan.md)) · `IInventoryRepository`, `IProductRepository`, `IWarehouseRepository`, `IReturnRequestRepository`, `IInvoiceRepository` (G-05) · `InventoryReservationService`, `OrderSplitterService`, `CheckoutTimeoutService`, `ReturnsApprovalService` (G-06) · `CheckoutOrderCommand`, `DispatchFulfillmentCommand`, `RequestReturnCommand`, `ApproveReturnCommand`, `CreateProductCommand`, `PublishProductCommand` (**especificados en [SDD/Application/01-use-cases-and-ports.md :3](Application/01-use-cases-and-ports.md#3-casos-de-uso-commands---dominio-de-pedidos-customer-orders)–[:6](Application/01-use-cases-and-ports.md#6-casos-de-uso-commands---catalogo-products) y no implementados**) · estados `PendingPack` e `InvoiceType.ZentricDetail` · `IUnitOfWork` · validadores FluentValidation · `.editorconfig` y CI.

**Zonas no exploradas de este mapa:** ejecución real contra PostgreSQL (no hay servidor en este entorno), comportamiento HTTP en runtime (no se levantó la API), contenido de `bin/`/`obj/`.

**Cobertura declarada: 30/33 (91 %) — parcial, no completa** (quedan huérfanos sin acción y fantasmas abiertos).
`[RIESGO]` La herramienta de búsqueda del agente **no indexa las carpetas no versionadas** (falso negativo comprobado: sobre `Zentric.Api/` y `Zentric.Application/` devolvió 0 coincidencias para patrones que sí existen); en esas rutas hay que usar escaneo directo.

## 3. Especificaciones activas

[SDD/SDD.md](SDD.md) **no duplica especificaciones**: las indexa. Documentos canónicos vigentes:

| ID | Spec | Documento | Estado | Nota |
|---|---|---|---|---|
| SPEC-000 | Adopción SDD (bootstrap brownfield) | `SDD/00-bootstrap/*` (7 documentos) | en curso | línea base del 2026-09-17 |
| SPEC-001 | Ley funcional Zentric | [SDD/Domain/ZENTRIC.md](Domain/ZENTRIC.md) | **congelada (Biblia)** | intacta; ADDENDA en [:4](Domain/ZENTRIC.md#dominio-4-gestion-de-bodegas) |
| SPEC-002 | Dominio (modelos, reglas, invariantes, puertos, eventos, ciclo de vida) | `SDD/Domain/*` y `SDD/Domain/services/*` | parcial | [C-03](00-bootstrap/risks-and-gaps.md), [C-04](00-bootstrap/risks-and-gaps.md), [C-06](00-bootstrap/risks-and-gaps.md), [C-07](00-bootstrap/risks-and-gaps.md) abiertas |
| SPEC-003 | Aplicación (casos de uso y puertos) | [SDD/Application/01-use-cases-and-ports.md](Application/01-use-cases-and-ports.md) | borrador | **contradicha por el código** ([H-09](00-bootstrap/spec-conformance-matrix.md), [H-10](00-bootstrap/spec-conformance-matrix.md)) |
| SPEC-004 | Infraestructura (EF Core, mapeos, migraciones) | [SDD/Infrastructure/01-data-access.md](Infrastructure/01-data-access.md) | en curso | repositorios de Returns/Invoices pendientes |
| SPEC-005 | Presentación (endpoints REST) | [SDD/Presentation/01-endpoints.md](Presentation/01-endpoints.md) | en curso | 3 de ~10 endpoints |
| SPEC-006 | Trazabilidad de la tanda no registrada ([T-011](00-bootstrap/migration-to-sdd-plan.md)…[T-022](00-bootstrap/migration-to-sdd-plan.md)) | **este documento**, [:5](00-bootstrap/migration-to-sdd-plan.md#fase-5-deuda-de-gobernanza-cerrada-en-esta-auditoria) y :7 | **hecha** | riesgo 1; solo documentación, cero código |
| SPEC-007 | Validación de entrada (FluentValidation), RFC 7807 e higiene: cierra [H-09](00-bootstrap/spec-conformance-matrix.md)/[H-11](00-bootstrap/spec-conformance-matrix.md)/[H-12](00-bootstrap/spec-conformance-matrix.md) | **este documento**, [:3](00-bootstrap/spec-conformance-matrix.md#3-catalog-productos) · [verification-baseline.md :11](00-bootstrap/verification-baseline.md#11-sexta-iteracion-spec-007-validacion-de-entrada-rfc-7807-e-higiene-2026-09-18) | **hecha** | riesgo 2 · 206/206 pruebas |

### SPEC-006 — Trazabilidad de la tanda no registrada · riesgo: 1 · estado: hecha

**Propósito y señal de resultado:** que la SSoT describa el estado real de `Zentric.Api`, `Zentric.Application`, `Zentric.Infrastructure` y los dominios Orders/Logistics/Billing/Returns (hoy verdes pero **sin registrar**), sin tocar código de producción.

**Alcance:** actualizar [SDD/SDD.md](SDD.md), los 7 documentos de `SDD/00-bootstrap/`, las 3 specs de capa, [AGENTS.md :0.0](../AGENTS.md#00-skill-de-ingenieria-sdd-obligatoria) y `scripts/sync-skill.ps1`.
**Fuera de alcance:** cualquier cambio en los proyectos `Zentric.*` (código) y cualquier alteración de [SDD/Domain/ZENTRIC.md](Domain/ZENTRIC.md).

**Requisitos:**
- FR-01: cada tarea ejecutada ([T-011](00-bootstrap/migration-to-sdd-plan.md)…[T-022](00-bootstrap/migration-to-sdd-plan.md)) aparece en el plan con estado y evidencia.
- FR-02: la suite real (**178**) queda registrada en [verification-baseline.md](00-bootstrap/verification-baseline.md).
- FR-03: cada contradicción detectada se registra como riesgo/pregunta; **ninguna la resuelve el agente**.
- [INV-01](Domain/06-business-rules.md): la Ley permanece intacta. · ERR-01: lo no verificable se marca `[PENDIENTE]`, no se rellena.
- CA-01: **Dado** el working tree con trabajo sin commit, **cuando** termino, **entonces** 0 archivos de código cambiaron (verificable con `git status`).
- CA-02: **Dado** el repositorio sin [SDD/SDD.md](SDD.md), **cuando** termino, **entonces** existe con las 8 secciones de la plantilla y apunta a la SSoT existente.

**Tareas:**
- [T-01] Actualizar documentos (plan, baseline, current-state, matriz, riesgos, preguntas, mapa, README) — cubre FR-01, FR-02 — verificación: relectura + `git status` — riesgo 1 — estado: **hecha**.
- [T-02] Registrar contradicciones nuevas ([C-08](00-bootstrap/risks-and-gaps.md), [C-09](00-bootstrap/risks-and-gaps.md), [H-09](00-bootstrap/spec-conformance-matrix.md)…[H-14](00-bootstrap/spec-conformance-matrix.md)) — cubre FR-03 — verificación: [risks-and-gaps.md :2](00-bootstrap/risks-and-gaps.md#2-riesgos-tecnicos) y [:6](00-bootstrap/risks-and-gaps.md#6-riesgos-cerrados) — estado: **hecha**.
- [T-03] Sincronizar la skill instalada — verificación: `Get-FileHash` origen = destino — estado: **hecha**.
- [T-04] Re-ejecutar `build`/`test` al cerrar — verificación: [verification-baseline.md :10](00-bootstrap/verification-baseline.md#10-quinta-iteracion-auditoria-de-la-tanda-no-registrada-2026-09-18) — estado: **hecha**.

**Verificación**

| Requisito | Tarea | Evidencia | PASS/FAIL/PENDING |
|---|---|---|---|
| FR-01 | [T-01](00-bootstrap/migration-to-sdd-plan.md) | [migration-to-sdd-plan.md :2](00-bootstrap/migration-to-sdd-plan.md#2-fases-y-tareas) Fase 3b/4 con estados | PASS |
| FR-02 | [T-01](00-bootstrap/migration-to-sdd-plan.md) | [verification-baseline.md :10](00-bootstrap/verification-baseline.md#10-quinta-iteracion-auditoria-de-la-tanda-no-registrada-2026-09-18) (`178/178`) | PASS |
| FR-03 | [T-02](00-bootstrap/migration-to-sdd-plan.md) | [risks-and-gaps.md :2](00-bootstrap/risks-and-gaps.md#2-riesgos-tecnicos) ([R-11](00-bootstrap/risks-and-gaps.md)…[R-16](00-bootstrap/risks-and-gaps.md)) y [:6](00-bootstrap/risks-and-gaps.md#6-riesgos-cerrados) | PASS |
| [INV-01](Domain/06-business-rules.md) | [T-01](00-bootstrap/migration-to-sdd-plan.md) | `git diff --numstat [SDD/Domain/ZENTRIC.md](Domain/ZENTRIC.md)` = 38/1, sin borrado de texto original | PASS |
| CA-01 | [T-01](00-bootstrap/migration-to-sdd-plan.md) | `git status --short` (solo `.md` y `sync-skill.ps1`) | PASS |
| CA-02 | [T-01](00-bootstrap/migration-to-sdd-plan.md) | Este archivo, [:1](00-bootstrap/migration-to-sdd-plan.md#fase-1-aseguramiento-sin-cambios-de-comportamiento-no-bloqueada)–:8 | PASS |

### SPEC-007 — Validación de entrada, RFC 7807 e higiene · riesgo: 2 · estado: hecha

**Propósito y señal de resultado:** cumplir [AGENTS.md :3.2](../AGENTS.md) (prohibido el `try-catch` genérico en Application), :3.4 (middleware global con RFC 7807), [:4.3](../AGENTS.md#43-application-api-agent) (validación de entrada obligatoria con FluentValidation) y el DoD [:8](../AGENTS.md#8-checklist-de-definicion-de-terminado-dod-para-agentes) (higiene). Señal: suite en verde con pruebas que demuestren que una entrada inválida se rechaza **antes** del dominio y que una excepción catastrófica ya no se silencia.

**Alcance:** `Zentric.Application` (comportamiento de validación + validadores + handler de Orders), `Zentric.Api` (registro de DI y middleware) y `Zentric.Tests`.
**Fuera de alcance:** cualquier regla de negocio nueva, la validación de stock de `[PED-01](Domain/06-business-rules.md)` (depende de [Q-03](00-bootstrap/questions-for-owner.md#q-03-c-03-estado-de-cancelacion-de-despacho)/[Q-04](00-bootstrap/questions-for-owner.md#q-04-c-04-cart-es-un-estado-de-customerorder)), la corrección de [Q-13](00-bootstrap/questions-for-owner.md#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante) y todo lo relacionado con Fulfillment/Facturación/Devoluciones.

**Requisitos:**
- FR-01: cada comando existente valida sus datos de entrada en Application antes de tocar el dominio. · FR-02: la entrada inválida produce un fallo de negocio (`Result.Failure` → 400), nunca una excepción. · FR-03: eliminado todo `try-catch` genérico en Application.
- FR-04: el middleware global produce Problem Details (RFC 7807) para fallos catastróficos. · FR-05: eliminados los artefactos de plantilla vacíos.
- [INV-01](Domain/06-business-rules.md): no se introducen reglas de negocio no especificadas: las reglas de los validadores **espejan** guardas ya existentes (`CustomerOrder`, `OrderItem`, `Money`, `FulfillmentOrder`). · [INV-02](Domain/06-business-rules.md): no se modifica ningún comportamiento del dominio.
- ERR-01: `Result` no cambia su contrato (constructor protegido y fábricas intactas).
- CA-01: **Dado** un `CreateCartCommand` con `BuyerId` vacío, **cuando** se envía por MediatR, **entonces** se obtiene un fallo con "BuyerId is required." y el repositorio **no** recibe escrituras. · CA-02: **Dado** un pedido en estado distinto de `Cart`, **cuando** se envía `AddOrderItemCommand`, **entonces** se devuelve un fallo de negocio **sin lanzar excepción** y sin actualizar el repositorio. · CA-03: **Dado** un guion válido, **cuando** se envía, **entonces** el handler se ejecuta y persiste.

**Matriz 360° (aplicables):** contratos/API → el comportamiento de error de los 3 endpoints cambia de 500/genérico a 400 Problem Details para entradas inválidas `[CONFIRMADO]` · datos → sin cambios · seguridad → sin cambios · observabilidad → `[PENDIENTE]` (no hay logging de negocio).

**Diseño:** entidades impactadas: ninguna del dominio (E-009/E-011 solo se **leen**). Flujo: Controller → MediatR → `ValidationBehavior` → (validadores) → handler → repositorio. Contratos: sin cambios de firma. Pruebas: 21 unitarias + 7 de integración DI. Rollback: revertir los 8 archivos de la tanda (todo son adiciones salvo 3 correcciones puntuales).

**Tareas:**
- [T-27] Application: `ValidationBehavior<,>` + `ValidationRunner` — cubre FR-01/FR-02 — verificación: 4 pruebas del comportamiento — riesgo 2 — **hecha**.
- [T-28] Application: 3 validadores FluentValidation — cubre FR-01 — verificación: 17 pruebas de validadores — riesgo 1 — **hecha**.
- [T-29] Application: quitar el `try-catch` genérico de `AddOrderItemCommandCommand` — cubre FR-03 — verificación: regresión de [H-09](00-bootstrap/spec-conformance-matrix.md) — riesgo 2 — **hecha**.
- [T-30] Api: `AddProblemDetails()`, validadores, `AddOpenBehavior`, `UseExceptionHandler()` — cubre FR-04 — verificación: 7 pruebas de integración DI — riesgo 2 — **hecha**.
- [T-31] Higiene: eliminar `Class1.cs` — cubre FR-05 — verificación: `Test-Path` = False — riesgo 1 — **hecha**.
- [T-32] Pruebas de extremo a extremo por HTTP (TestServer) — cubre CA-01/CA-04 — **NO hecha** `[PENDIENTE]` — riesgo 2.

**Verificación**

| Requisito | Tarea | Evidencia | PASS/FAIL/PENDING |
|---|---|---|---|
| FR-01 | [T-27](00-bootstrap/migration-to-sdd-plan.md), [T-28](00-bootstrap/migration-to-sdd-plan.md) | 21 pruebas unitarias en verde | PASS |
| FR-02 | [T-27](00-bootstrap/migration-to-sdd-plan.md), [T-30](00-bootstrap/migration-to-sdd-plan.md) | 7 pruebas de integración: entrada inválida → `Result.Failure`, handler no invocado | PASS |
| FR-03 | [T-29](00-bootstrap/migration-to-sdd-plan.md) | `AddOrderItemCommand.cs` sin `catch`; regresión en verde | PASS |
| FR-04 | [T-30](00-bootstrap/migration-to-sdd-plan.md) | `Program.cs`: `AddProblemDetails()` + `UseExceptionHandler()` | PASS (compilación y DI) / `[PENDIENTE]` en HTTP real |
| FR-05 | [T-31](00-bootstrap/migration-to-sdd-plan.md) | `Test-Path` de ambos `Class1.cs` = `False` | PASS |
| CA-01…CA-03 | [T-27](00-bootstrap/migration-to-sdd-plan.md)…[T-30](00-bootstrap/migration-to-sdd-plan.md) | [verification-baseline.md :11](00-bootstrap/verification-baseline.md#11-sexta-iteracion-spec-007-validacion-de-entrada-rfc-7807-e-higiene-2026-09-18) (`206/206`) | PASS |
| CA-04 (HTTP) | [T-32](00-bootstrap/migration-to-sdd-plan.md) | `[PENDIENTE]`: la API no se levantó | PENDING |

## 4. Decisiones y ADDENDA

### 4.1 Decisiones del Owner ya aplicadas (2026-09-17)

| ID | Decisión | ADR / evidencia |
|---|---|---|
| [Q-01](00-bootstrap/questions-for-owner.md#q-01-c-01-se-permite-fraccionar-la-reserva-entre-bodegas-resuelta-2026-09-17) ([C-01](00-bootstrap/risks-and-gaps.md)) | Reserva en **bodega única**; fraccionamiento solo como contingencia cuando ninguna bodega individual cubre la cantidad | [SDD/Adr/0001-reserva-fragmentacion-contingencia.md](Adr/0001-reserva-fragmentacion-contingencia.md) |
| [Q-02](00-bootstrap/questions-for-owner.md#q-02-c-02-la-clave-del-inventario-es-el-producto-o-la-variante-sku-resuelta-2026-09-17) ([C-02](00-bootstrap/risks-and-gaps.md)) | La clave del inventario es **`VariantId` (SKU)**; `ProductVariant` es entidad hija de `Product`; clave `(VariantId, WarehouseId)` | [SDD/Adr/0002-clave-inventario-variantid.md](Adr/0002-clave-inventario-variantid.md) |
| [Q-10](00-bootstrap/questions-for-owner.md#9-cuarta-iteracion-adr-0003-variante-obligatoria-en-fisicos-q-10-c3) | Variante **obligatoria solo en `Physical`** (opción C3) | [SDD/Adr/0003-variante-obligatoria-productos-fisicos.md](Adr/0003-variante-obligatoria-productos-fisicos.md) |

### 4.2 Adiciones a la Biblia (formato [skill :0.7](../.agents/skills/generic-sdd-agent/SKILL.md#07-inmutabilidad-de-los-documentos-biblia-y-registro-de-cambios))

> **Integridad `[CONFIRMADO]`:** [SDD/Domain/ZENTRIC.md](Domain/ZENTRIC.md) conserva el texto original: `git diff --numstat` = **38 adiciones / 1 borrado** y ese borrado es una línea separadora, no contenido del cliente.
>
> `[CONTRADICCIÓN]` **[C-10](00-bootstrap/risks-and-gaps.md)** — El bloque añadido que antecede al rótulo (`## DOMINIO 8`, `## DOMINIO 9`, `## DOMINIO 10` del ciclo de estados, la facturación y las devoluciones) **no lleva el rótulo `[ADDENDUM - DICTADO POR OWNER]`**, que [AGENTS.md :0.7](../AGENTS.md#07-inmutabilidad-de-los-documentos-biblia-y-registro-de-cambios) exige para toda adición a la Biblia. Queda registrado; **no lo corrijo yo** (la Biblia es intocable).

### [ADDENDUM - DICTADO POR OWNER] [ADD-001](SDD.md) — Dominio 8: logística y despachos (Fulfillment)

- Fecha: **no registrada** `[PENDIENTE]` · Owner: **no registrado** `[PENDIENTE]` · Origen: bloque `[ADDENDUM - DICTADO POR OWNER]` de [SDD/Domain/ZENTRIC.md](Domain/ZENTRIC.md)
- Regla dictada (texto del documento):
  - "**Estados:** Empacado y Despachado."
  - "**Regla (Stock Fantasma):** No debería ocurrir, pero en caso de haber un quiebre de stock fantasma, el pedido se cancela con devolución obligatoria para no retener stock irreal."
- Afecta a: `DOMINIO 8` de la Ley · Relacionado con: **[C-08](00-bootstrap/risks-and-gaps.md)**, E-011, [Q-03](00-bootstrap/questions-for-owner.md#q-03-c-03-estado-de-cancelacion-de-despacho)

### [ADDENDUM - DICTADO POR OWNER] [ADD-002](SDD.md) — Dominio 9: devoluciones y reembolsos

- Fecha: **no registrada** `[PENDIENTE]` · Owner: **no registrado** `[PENDIENTE]` · Origen: bloque `[ADDENDUM - DICTADO POR OWNER]`
- Regla dictada:
  - "**Regla (Prohibición):** Está **prohibido** devolver productos digitales."
  - "**Flujo Físico:** El operador logístico inspecciona que el producto esté en buen estado. Si es así, requiere la aprobación del Vendedor. Si ambas se cumplen, el producto vuelve al stock en el inventario con la etiqueta Usado."
- Afecta a: `DOMINIO 9` de la Ley · Relacionado con: **[C-08](00-bootstrap/risks-and-gaps.md)**, E-014, E-007 (`ReturnToUsedStock`), [Q-13](00-bootstrap/questions-for-owner.md#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante)

### [ADDENDUM - DICTADO POR OWNER] [ADD-003](SDD.md) — Dominio 10: facturación y pagos

- Fecha: **no registrada** `[PENDIENTE]` · Owner: **no registrado** `[PENDIENTE]` · Origen: bloque `[ADDENDUM - DICTADO POR OWNER]`
- Regla dictada:
  - "**Factura Maestra:** Entregada al cliente con el total de la transacción."
  - "**Detalle Zentric:** Detalle transaccional desglosado para control de plataforma."
  - "**Factura de Vendedor:** Factura propia detallando el monto que le corresponde al Vendedor (Split)."
- Afecta a: `DOMINIO 10` de la Ley · Relacionado con: **[C-08](00-bootstrap/risks-and-gaps.md)**, E-013 (`InvoiceType` solo tiene `Master` y `VendorDetail`), [Q-13](00-bootstrap/questions-for-owner.md#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante)

`[OBSERVADO]` Los bloques añadidos de la Ley presentan **pérdida de caracteres acentuados** ("Gestin de Logstica", "Seccin aadida", "clarificacin"). Se registra como defecto de forma de la Ley; **no se corrige sin autorización del Owner** (Biblia intocable).

### 4.3 Resultado posterior (Gate 5)

No aplica todavía: ninguna de las tandas de Fase 3b/4 ([SDD/SDD.md :7](SDD.md#7-estado-y-proximos-pasos)) tiene señal de resultado definida ni fue validada en un entorno con datos.

## 5. Verificación y línea base

### 5.1 Comandos ejecutados (2026-09-18 · Windows / PowerShell 7 / .NET SDK 10 · **sin** PostgreSQL)

| Comando | Resultado | Evidencia |
|---|---|---|
| `dotnet build Zentric.slnx --nologo` | **PASS** — `Build succeeded. 0 Warning(s) 0 Error(s)`; compilan los 5 proyectos | salida de consola |
| `dotnet test Zentric.slnx --nologo` | **PASS** — `Failed: 0, Passed: 178, Skipped: 0, Total: 178` (~479 ms) | salida de consola |
| Auditoría de dependencias (lectura de los 5 `.csproj`) | **PASS** — `Domain`←`Application`←`Infrastructure`←`Api` ([AGENTS.md :2.1](../AGENTS.md#21-regla-de-dependencia-estricta-y-aislamiento-de-capas)) | `Select-String ProjectReference` |
| Auditoría anti-amnesia (9 `DbSet` vs agregados de la spec) | **PASS parcial** — los 9 agregados de `SDD/Infrastructure` [:2](../AGENTS.md#2-reglas-arquitectonicas-inviolables-hexagonal-ddd) están mapeados | `Select-String DbSet` |
| Integridad de la Ley | **PASS** — 38 adiciones / 1 borrado (línea separadora) | `git diff --numstat` |
| Integridad de la skill instalada | **FAIL antes / PASS después** — `24E7ED4F…` (repo v6.0.0) vs `EB4E7166…` (instalada v3.1.0) | `Get-FileHash` |

`[CONFIRMADO]` Comparación con la línea base anterior: **164 → 178** pruebas (+14: Orders 6, Logistics 3, Billing 2, Returns 3, según el conteo por archivo de suite).

### 5.2 Fallos preexistentes

Ninguno. La suite está 100 % verde; no hay `Skipped` ni fallos conocidos sin corregir.

### 5.3 Validaciones **no** ejecutadas (honestidad de evidencia)

- **Persistencia real:** las migraciones `InitialCreate` y `CompleteSchema` **nunca se aplicaron** contra un PostgreSQL; no hay servidor en el entorno ni pruebas de integración. `[PENDIENTE]`
- **Comportamiento HTTP:** la API no se levantó; no se ejecutó ninguna petición contra los 3 endpoints. `[PENDIENTE]`
- **Mapeo EF en runtime:** `OwnsMany`/`OwnsOne` y la conversión de `Email` (`Property(u => u.Email).HasConversion(...)`) sin validar en ejecución. `[PENDIENTE]`
- **Formato/lint/análisis estático:** no hay `.editorconfig`, ni `AnalysisLevel`, ni `TreatWarningsAsErrors`, ni CI → el "0 warnings" del build **no** demuestra ausencia de hallazgos. `[RIESGO]` [R-08](00-bootstrap/risks-and-gaps.md) sigue abierto.

### 5.4 Segunda tanda de verificación — SPEC-007 (2026-09-18)

| Comando | Resultado | Evidencia |
|---|---|---|
| `dotnet restore Zentric.slnx` | **PASS** | "All projects are up-to-date for restore." |
| `dotnet build Zentric.slnx --nologo` | **PASS** | `Build succeeded. 0 Warning(s) 0 Error(s)` |
| `dotnet test Zentric.slnx --nologo` | **PASS** | `Failed: 0, Passed: 206, Skipped: 0, Total: 206` (178 anteriores + 21 unitarias + 7 de integración DI) |
| `dotnet test --filter FullyQualifiedName~MediatRValidationPipelineTests` | **PASS** | 7/7 · prueba el contenedor DI real: comportamiento genérico abierto, validadores descubiertos y handlers ejecutados |
| El mismo filtro antes de corregir el arnés | **FAIL** | `MediatR requires ILoggerFactory to be registered` → defecto del arnés, no del cableado |

`[CONFIRMADO]` Detalle completo de la tanda, cambios y cobertura acumulada (206) en
[SDD/00-bootstrap/verification-baseline.md :11](00-bootstrap/verification-baseline.md#11-sexta-iteracion-spec-007-validacion-de-entrada-rfc-7807-e-higiene-2026-09-18).

## 6. Riesgos, contradicciones y preguntas

> El detalle completo con evidencia vive en [SDD/00-bootstrap/risks-and-gaps.md](00-bootstrap/risks-and-gaps.md) y [SDD/00-bootstrap/questions-for-owner.md](00-bootstrap/questions-for-owner.md). Aquí quedan los **IDs nuevos** de esta auditoría y los bloqueos vigentes.

### 6.1 Contradicciones abiertas

| ID | Contradicción | Evidencia | Bloquea |
|---|---|---|---|
| **[C-08](00-bootstrap/risks-and-gaps.md)** | **La Ley define `DOMINIO 8/9/10` dos veces con contenidos distintos** (9 y 10 intercambian significado entre el bloque base y el ADDENDUM; el 8 tiene 5 estados vs 2). El código tomó una interpretación que el Owner no dictó: falta `PendingPack`, falta "Detalle Zentric" y la devolución aprobada no devuelve stock | [ZENTRIC.md](Domain/ZENTRIC.md) líneas 254-270 vs 320-334 · `FulfillmentStatus.cs` · `InvoiceType.cs` · `ReturnRequest.cs` | Fases 3b/4 → requiere **[Q-13](00-bootstrap/questions-for-owner.md#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante)** |
| **[C-09](00-bootstrap/risks-and-gaps.md)** | Skill: el repo está en **v6.0.0** monolítico (sin `references/`) y la copia instalada seguía en **v3.1.0**; [AGENTS.md](../AGENTS.md) citaba la v3.0.0 con `references/10-zentric-overlay.md`, inexistente | hashes distintos · [AGENTS.md :0.0](../AGENTS.md#00-skill-de-ingenieria-sdd-obligatoria) | Gobernanza (**corregido** en esta tanda) |
| **[C-10](00-bootstrap/risks-and-gaps.md)** | Adiciones a la Biblia **sin rótulo** `[ADDENDUM - DICTADO POR OWNER]` (bloque previo al marcador) | `git diff` de [ZENTRIC.md](Domain/ZENTRIC.md) | Trazabilidad de la Ley |
| [C-03](00-bootstrap/risks-and-gaps.md) | `Cancelled` vs `CancelledByStockBreak` | [02-value-objects.md :58](Domain/02-value-objects.md)` vs [03-value-objects.md :41](Domain/03-value-objects.md)` | Fulfillment → **[Q-03](00-bootstrap/questions-for-owner.md#q-03-c-03-estado-de-cancelacion-de-despacho)** |
| [C-04](00-bootstrap/risks-and-gaps.md) | ¿`Cart` es estado de `CustomerOrder`? | [02-value-objects.md :46](Domain/02-value-objects.md)` vs [03-value-objects.md :29](Domain/03-value-objects.md)` | Ordering → **[Q-04](00-bootstrap/questions-for-owner.md#q-04-c-04-cart-es-un-estado-de-customerorder)** |
| [C-05](00-bootstrap/risks-and-gaps.md) | `Vendor` (spec) vs `Seller` (código) | [02-value-objects.md :62](Domain/02-value-objects.md)` vs `WarehouseType.cs:6` | Renombrado → [Q-05](00-bootstrap/questions-for-owner.md#q-05-c-05-vendor-o-seller) |
| [C-06](00-bootstrap/risks-and-gaps.md) | [SDD/Domain/Software-arquitecture.md](Domain/Software-arquitecture.md) = **0 bytes**; [AGENTS.md :0.1](../AGENTS.md#01-consulta-obligatoria-antes-de-codificar) exige documentos que no existían | tamaño de archivo | Gobernanza documental |
| [C-07](00-bootstrap/risks-and-gaps.md) | Numeración duplicada en `SDD/Domain/` (dos `01-*`, `02-*`, `03-*`, `04-*`) | estructura de carpetas | Navegabilidad → [Q-06](00-bootstrap/questions-for-owner.md#q-06-c-07-consolidacion-de-los-documentos-numerados) |

### 6.2 Hallazgos nuevos de código

| ID | Hallazgo | Evidencia | Severidad |
|---|---|---|---|
| ~~**[H-09](00-bootstrap/spec-conformance-matrix.md)**~~ | ~~`AddOrderItemCommandHandler` captura `catch (Exception ex)` y devuelve `Result.Failure(ex.Message)` → **prohibido por [AGENTS.md:3.2](../AGENTS.md#32-tratamiento-de-errores-y-excepciones-organizado-por-capa)**~~ | **CERRADO en SPEC-007 (2026-09-18):** eliminado el `catch`; la precondición de negocio se informa con `Result.Failure` y las excepciones catastróficas ya no se silencian. Regresión cubierta por `Send_AddOrderItemCommandWhenOrderIsNotInCart_ReturnsBusinessFailureWithoutThrowing` | alta → **cerrada** |
| **[H-10](00-bootstrap/spec-conformance-matrix.md)** | `SDD/Application` [[AddOrderItemCommand.cs[:3](00-bootstrap/spec-conformance-matrix.md#3-catalog-productos)](../Zentric.Application/Orders/Commands/AddOrderItemCommand.cs#L3) afirma que `AddOrderItemCommand` "valida stock físico usando `IInventoryRepository`": **el puerto no existe y no se valida stock**; `[PED-01](Domain/06-business-rules.md)` (reserva preventiva + timeout 15 min) no está implementada | [SDD/Application/01-use-cases-and-ports.md [SDD/Application/01-use-cases-and-ports.md:15](../Application/01-use-cases-and-ports.md) vs `AddOrderItemCommand.cs` | alta |
| ~~**[H-11](00-bootstrap/spec-conformance-matrix.md)**~~ | ~~Middleware "simplificado": `UseExceptionHandler("/error")` sin endpoint ni `AddProblemDetails()`; FluentValidation referenciado con **0 validadores** y **0 pipeline**~~ | **CERRADO en SPEC-007 (2026-09-18):** `AddProblemDetails()` + `UseExceptionHandler()` sin endpoint inexistente; 3 validadores FluentValidation y `ValidationBehavior<,>` registrado con `AddOpenBehavior`, verificado en un contenedor DI real | alta → **cerrada** |
| ~~**[H-12](00-bootstrap/spec-conformance-matrix.md)**~~ | ~~`Class1.cs` vacío en `Zentric.Application` y `Zentric.Infrastructure`~~ | **CERRADO en SPEC-007 (2026-09-18):** ambos archivos eliminados; antes se verificó que `Class1` no estaba referenciado en ningún archivo del repositorio | baja → **cerrada** |
| **[H-13](00-bootstrap/spec-conformance-matrix.md)** | `ZentricDbContext` mapea **directamente** los agregados de dominio; exige [AGENTS.md:4.2](../AGENTS.md#42-infrastructure-adapter-agent) exige "aislamiento estricto de las entidades EF Core respecto a Application/Domain". Desviación sin ADR | `ZentricDbContext.cs` | media |
| **[H-14](00-bootstrap/spec-conformance-matrix.md)** | `FulfillmentOrder.CancelDueToNoStock()` y `ReturnRequest.ApproveByVendor()` contienen `// TODO` de eventos de dominio → el [ADD-002](SDD.md) ("el producto vuelve al stock con etiqueta Usado") **no se ejecuta** | `FulfillmentOrder.cs:75` · `ReturnRequest.cs:63` | alta |

`[CONFIRMADO]` Los hallazgos previos **[H-01](00-bootstrap/spec-conformance-matrix.md) … [H-07](00-bootstrap/spec-conformance-matrix.md) siguen abiertos**; **[H-09](00-bootstrap/spec-conformance-matrix.md), [H-11](00-bootstrap/spec-conformance-matrix.md) y [H-12](00-bootstrap/spec-conformance-matrix.md) quedaron cerrados en SPEC-007** (2026-09-18). [H-01](00-bootstrap/spec-conformance-matrix.md) reverificado en el árbol actual: `Inventory.cs:121` valida `AvailableQuantity` y `[[Inventory.cs:126](../Zentric.Domain/Inventories/Inventory.cs#L126) ejecuta `ReservedQuantity -= quantity` → el reservado puede quedar negativo. Siguen abiertos [H-10](00-bootstrap/spec-conformance-matrix.md), [H-13](00-bootstrap/spec-conformance-matrix.md), [H-14](00-bootstrap/spec-conformance-matrix.md) y la tanda [H-01](00-bootstrap/spec-conformance-matrix.md)…[H-07](00-bootstrap/spec-conformance-matrix.md).

### 6.3 Preguntas al Owner

- **[Q-13](00-bootstrap/questions-for-owner.md#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante) (NUEVA, bloqueante)** — duplicación de `DOMINIO 8/9/10` en la Ley ([C-08](00-bootstrap/risks-and-gaps.md)). Ficha completa en [questions-for-owner.md](00-bootstrap/questions-for-owner.md).
- **[Q-03](00-bootstrap/questions-for-owner.md#q-03-c-03-estado-de-cancelacion-de-despacho) y [Q-04](00-bootstrap/questions-for-owner.md#q-04-c-04-cart-es-un-estado-de-customerorder) reactivadas como bloqueantes:** la Fase 3b/4 se implementó **sin** respuesta del Owner ([T-011](00-bootstrap/migration-to-sdd-plan.md) figuraba "bloqueada por [Q-04](00-bootstrap/questions-for-owner.md#q-04-c-04-cart-es-un-estado-de-customerorder)"; [T-014](00-bootstrap/migration-to-sdd-plan.md) "bloqueada por [Q-03](00-bootstrap/questions-for-owner.md#q-03-c-03-estado-de-cancelacion-de-despacho)"). Se registra como **incumplimiento del freno de mano ([AGENTS.md:0.3](../AGENTS.md#03-el-freno-de-mano-cero-asunciones))**, no como decisión.
- Siguen abiertas: [Q-05](00-bootstrap/questions-for-owner.md#q-05-c-05-vendor-o-seller), [Q-06](00-bootstrap/questions-for-owner.md#q-06-c-07-consolidacion-de-los-documentos-numerados), [Q-07](00-bootstrap/questions-for-owner.md#q-07-documento-de-identidad-del-usuario), [Q-08](00-bootstrap/questions-for-owner.md#q-08-r-07-buyerpaymenttokens-contra-la-invariante-9), [Q-09](00-bootstrap/questions-for-owner.md#q-09-naming-canonico-pendiente), [Q-11](00-bootstrap/questions-for-owner.md#q-11-detalle-del-modelo-de-atributos-de-variante-abierta) y **[Q-12](00-bootstrap/questions-for-owner.md#sub-decisiones-propuesto-ver-q-12)** (4 sub-decisiones `[PROPUESTO]` de [ADR-0003](Adr/0003-variante-obligatoria-productos-fisicos.md) pendientes de confirmar).

## 7. Estado y próximos pasos

### 7.1 Hito actual

**SPEC-006 (trazabilidad) y SPEC-007 (validación de entrada + RFC 7807 + higiene) — hechas.** La SSoT describe el estado real y [H-09](00-bootstrap/spec-conformance-matrix.md), [H-11](00-bootstrap/spec-conformance-matrix.md) y [H-12](00-bootstrap/spec-conformance-matrix.md) quedaron cerrados con 206/206 pruebas. Lo siguiente sigue dependiendo **exclusivamente del Owner**: [Q-13](00-bootstrap/questions-for-owner.md#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante) (bloqueante, Ley duplicada `DOMINIO 8/9/10`) y [Q-03](00-bootstrap/questions-for-owner.md#q-03-c-03-estado-de-cancelacion-de-despacho)/[Q-04](00-bootstrap/questions-for-owner.md#q-04-c-04-cart-es-un-estado-de-customerorder); sin ellas no se debe escribir más código de Fulfillment, Facturación ni Devoluciones.

### 7.2 Tareas [T-011](00-bootstrap/migration-to-sdd-plan.md)…[T-022](00-bootstrap/migration-to-sdd-plan.md) (estado real, hasta ahora no registrado)

| ID | Tarea (plan) | Estado real verificado | Evidencia |
|---|---|---|---|
| [T-011](00-bootstrap/migration-to-sdd-plan.md) | `CustomerOrder` + líneas + estados | **Hecha en código, sin autorización** ([Q-04](00-bootstrap/questions-for-owner.md#q-04-c-04-cart-es-un-estado-de-customerorder) abierta) | `Orders/CustomerOrder.cs`, 6 pruebas |
| [T-012](00-bootstrap/migration-to-sdd-plan.md) | 7 eventos de dominio + `IDomainEventDispatcher` | **NO hecha** | 0 archivos de eventos en el repo |
| [T-013](00-bootstrap/migration-to-sdd-plan.md) | `InventoryReservationService` (+ desglose de reserva) | **NO hecha** | no existe el servicio ni `ReservationDetails` |
| [T-014](00-bootstrap/migration-to-sdd-plan.md) | `OrderSplitterService` (N guías si hubo fraccionamiento) | **Parcial**: agregados `FulfillmentOrder`/`Shipment` sin servicio, sin fraccionamiento ni N guías | `Logistics/`, 3 pruebas |
| [T-015](00-bootstrap/migration-to-sdd-plan.md) | Timeout de 15 min (`CheckoutTimeoutService`) | **NO hecha** (depende de `IClock`, inexistente) | [H-06](00-bootstrap/spec-conformance-matrix.md) abierto |
| [T-016](00-bootstrap/migration-to-sdd-plan.md) | Devoluciones con doble aprobación (`ReturnsApprovalService`) | **Parcial**: agregado `ReturnRequest` con doble aprobación, **sin servicio y sin retorno a stock** | `Returns/ReturnRequest.cs`, [H-14](00-bootstrap/spec-conformance-matrix.md) |
| [T-017](00-bootstrap/migration-to-sdd-plan.md) | Suspensión en cascada al bloquear vendedor | **NO hecha** | [invariante 6](Domain/04-invariants-and-rules.md) sin implementar |
| [T-018](00-bootstrap/migration-to-sdd-plan.md) | 5 puertos de repositorio restantes | **Parcial**: 2 de 5 | `Zentric.Application/**/Ports/` |
| [T-019](00-bootstrap/migration-to-sdd-plan.md) | `Zentric.Application`: puertos de entrada, CQRS, FluentValidation, `Result<T>` | **Parcial**: 3 commands, 2 puertos, `Result<T>`; **0 validadores y 0 pipeline** | [H-09](00-bootstrap/spec-conformance-matrix.md), [H-11](00-bootstrap/spec-conformance-matrix.md) |
| [T-020](00-bootstrap/migration-to-sdd-plan.md) | `Zentric.Infrastructure`: EF Core + PostgreSQL, mapeos, migraciones | **Parcial**: `ZentricDbContext` (9 `DbSet`), 2 repositorios, 2 migraciones **nunca aplicadas** | E-021…E-023 |
| [T-021](00-bootstrap/migration-to-sdd-plan.md) | `Zentric.Api`: controladores delgados + Problem Details | **Parcial**: 3 endpoints; `ApiControllerBase` mapea `Result` → RFC 7807, pero el middleware global está "simplificado" | [H-11](00-bootstrap/spec-conformance-matrix.md) |
| [T-022](00-bootstrap/migration-to-sdd-plan.md) | Pruebas de integración y contrato (OpenAPI) | **NO hecha** | 178 pruebas, todas unitarias de dominio |

`[RIESGO]` **Ninguna** de las tareas [T-019](00-bootstrap/migration-to-sdd-plan.md)…[T-021](00-bootstrap/migration-to-sdd-plan.md) se registró en el plan, en [verification-baseline.md](00-bootstrap/verification-baseline.md) ni en la matriz durante su ejecución; este documento y [migration-to-sdd-plan.md](00-bootstrap/migration-to-sdd-plan.md) Fase 3b/4 son el primer registro. `[ACTUALIZACIÓN 2026-09-18]` **SPEC-007** mejoró [T-019](00-bootstrap/migration-to-sdd-plan.md) (validadores + pipeline reales) y [T-021](00-bootstrap/migration-to-sdd-plan.md) (`AddProblemDetails()` + `UseExceptionHandler()`), cerrando [H-09](00-bootstrap/spec-conformance-matrix.md)/[H-11](00-bootstrap/spec-conformance-matrix.md); [T-020](00-bootstrap/migration-to-sdd-plan.md) sigue igual (migraciones sin aplicar).

### 7.3 Supuestos por confirmar (no son reglas)

| Supuesto aplicado en código | Origen | Estado |
|---|---|---|
| `FulfillmentOrder` nace en `Packed` y omite `PendingPack` | `FulfillmentOrder.cs:33` ("Según dictamen…") | `[SUPUESTO]` → **[Q-13](00-bootstrap/questions-for-owner.md#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante)** |
| `InvoiceType` = `{Master, VendorDetail}` (sin "Detalle Zentric") | `InvoiceType.cs` | `[SUPUESTO]` → **[Q-13](00-bootstrap/questions-for-owner.md#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante)** |
| `CustomerOrder.TotalAmount` de carrito vacío = `Money(0, "USD")` | `CustomerOrder.cs:24` | `[SUPUESTO]` → **[Q-13](00-bootstrap/questions-for-owner.md#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante)** |
| Precio en el endpoint como `decimal UnitPrice` + `string Currency` | `OrdersController.cs:32` | `[SUPUESTO]` |
| Sub-decisiones 1-4 de [ADR-0003](Adr/0003-variante-obligatoria-productos-fisicos.md) | `Adr/0003` | `[PROPUESTO]` → **[Q-12](00-bootstrap/questions-for-owner.md#sub-decisiones-propuesto-ver-q-12)** |

### 7.4 Bloqueos

1. **[Q-13](00-bootstrap/questions-for-owner.md#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante)** (duplicación `DOMINIO 8/9/10`) — bloquea toda corrección de Fulfillment, Facturación y Devoluciones.
2. **[Q-03](00-bootstrap/questions-for-owner.md#q-03-c-03-estado-de-cancelacion-de-despacho)** (estado de cancelación de despacho) y **[Q-04](00-bootstrap/questions-for-owner.md#q-04-c-04-cart-es-un-estado-de-customerorder)** (`Cart` como estado) — bloquean [T-013](00-bootstrap/migration-to-sdd-plan.md)/[T-014](00-bootstrap/migration-to-sdd-plan.md)/[T-015](00-bootstrap/migration-to-sdd-plan.md).
3. **[Q-12](00-bootstrap/questions-for-owner.md#sub-decisiones-propuesto-ver-q-12)** — impide cerrar [T-010c](00-bootstrap/migration-to-sdd-plan.md) como `[CONFIRMADO]`.
4. **[Q-07](00-bootstrap/questions-for-owner.md#q-07-documento-de-identidad-del-usuario)** (`IdentityDocument`) — bloquea [T-006](00-bootstrap/migration-to-sdd-plan.md) y la conformidad con `RG-01` y :11 de la Ley.
5. Aprobación del Owner para **[T-003b](00-bootstrap/migration-to-sdd-plan.md)** (corregir [H-01](00-bootstrap/spec-conformance-matrix.md)), porque cambia comportamiento observable.

### 7.5 Registro de acciones ([skill :17.2](SDD.md) [:7](SDD.md#7-estado-y-proximos-pasos))](../.agents/skills/generic-sdd-agent/SKILL.md#172-registro-de-acciones-incidentes-enterprise-y-sesiones-largas-en-sddsddmd-7))

```text
[2026-09-18 · hora no registrada] Lectura de la SSoT completa (AGENTS.md, SDD/**, 5 .csproj, Zentric.Api/Application/Infrastructure, dominio nuevo, 4 suites nuevas) · objeto: E-001…E-030 · resultado: mapa de entidades al 91 %
[2026-09-18 · hora no registrada] `dotnet build Zentric.slnx --nologo` · resultado: Build succeeded, 0 warnings, 0 errores
[2026-09-18 · hora no registrada] `dotnet test Zentric.slnx --nologo` · resultado: 178/178 PASS (línea base anterior: 164)
[2026-09-18 · hora no registrada] Auditorías (git diff/status/numstat, Select-String ProjectReference/DbSet, Get-FileHash) · resultado: regla de dependencia OK, anti-amnesia OK, Ley intacta, skill desincronizada
[2026-09-18 · hora no registrada] Creado [SDD/SDD.md](SDD.md) (SPEC-006) · objeto: E-028 · aprobado por: Owner (opción "trazabilidad primero" síncrona, 2026-09-18)
[2026-09-18 · hora no registrada] Corregido [AGENTS.md :0.0](../AGENTS.md#00-skill-de-ingenieria-sdd-obligatoria) (v6.0.0 + contexto persistente) · objeto: E-027
[2026-09-18 · hora no registrada] Corregido `scripts/sync-skill.ps1` (no borra el destino si falta `references/`) · objeto: E-027 · motivo: el script habría destruido la skill instalada y fallado
[2026-09-18 · hora no registrada] Sincronizada la skill instalada · resultado: hash repositorio = hash instalada
[2026-09-18 · hora no registrada] Validaciones NO ejecutadas: PostgreSQL, HTTP en runtime, mapeo EF en runtime, lint/CI
[2026-09-18 · hora no registrada] SPEC-007 inicio · se verificó con Select-String que `Class1` no estaba referenciado antes de borrar ([H-12](00-bootstrap/spec-conformance-matrix.md)) y se leyeron las guardas reales de `Money` para espejarlas en los validadores (cero reglas inventadas)
[2026-09-18 · hora no registrada] `dotnet build Zentric.slnx --nologo` · resultado: Build succeeded, 0 warnings, 0 errores (tras crear `ValidationBehavior<,>`, 3 validadores y editar `Program.cs` y el handler)
[2026-09-18 · hora no registrada] `dotnet test Zentric.slnx --nologo` (1.er intento) · resultado: **FAIL 7** de las pruebas de integración DI · causa verificada en el stack trace: `MediatR requires ILoggerFactory to be registered` desde `MediatRServiceCollectionExtensions.CheckLicense` → defecto del arnés, no del cableado
[2026-09-18 · hora no registrada] Corrección del arnés: `services.AddLogging()` + paquetes `Microsoft.Extensions.DependencyInjection` y `.Logging` 10.0.12 en `Zentric.Tests`
[2026-09-18 · hora no registrada] `dotnet build` + `dotnet test Zentric.slnx --nologo` (2.º intento) · resultado: **build 0/0 · 206/206 PASS** (178 + 21 unitarias + 7 de integración DI)
[2026-09-18 · hora no registrada] Archivos de producción modificados: 5 nuevos (`ValidationBehavior.cs`, 3 validadores) + 3 corregidos (`AddOrderItemCommand.cs`, `Program.cs`, csproj de pruebas) + 2 eliminados (`Class1.cs` ×2)
[2026-09-18 · hora no registrada] Nueva pregunta registrada: **[Q-14](00-bootstrap/questions-for-owner.md#q-14-licenciamiento-de-mediatr-14-y-fluentvalidation-abierta-no-bloquea-el-codigo-actual)** (licenciamiento de MediatR 14) · objeto E-027/[R-19](00-bootstrap/risks-and-gaps.md) · aprobado por: pendiente del Owner
```

### 7.6 Próximos pasos (recomendación, en este orden)

1. **El Owner dicta [Q-13](00-bootstrap/questions-for-owner.md#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante)** (`DOMINIO 8/9/10`: numeración única y estados de despacho) y responde **[Q-03](00-bootstrap/questions-for-owner.md#q-03-c-03-estado-de-cancelacion-de-despacho)/[Q-04](00-bootstrap/questions-for-owner.md#q-04-c-04-cart-es-un-estado-de-customerorder)** → desbloquea Fulfillment, Facturación y Devoluciones.
2. ~~Cerrar [H-09](00-bootstrap/spec-conformance-matrix.md), [H-11](00-bootstrap/spec-conformance-matrix.md) y [H-12](00-bootstrap/spec-conformance-matrix.md)~~ → **HECHO en SPEC-007** (2026-09-18): `try-catch` eliminado, validadores FluentValidation + `ValidationBehavior<,>`, `AddProblemDetails()` + `UseExceptionHandler()` y `Class1.cs` borrados. 206/206 pruebas.
2b. **Pendiente sin decisión del Owner:** [T-032](00-bootstrap/migration-to-sdd-plan.md) (pruebas HTTP con TestServer) y **[Q-14](00-bootstrap/questions-for-owner.md#q-14-licenciamiento-de-mediatr-14-y-fluentvalidation-abierta-no-bloquea-el-codigo-actual)** (licenciamiento de MediatR). Ninguna bloquea el roadmap.
3. **Commit atómico de esta tanda de código** (`feat(orders,logistics,billing,returns): [T-011](00-bootstrap/migration-to-sdd-plan.md)…[T-021](00-bootstrap/migration-to-sdd-plan.md)`) — hoy hay ~1.400 líneas sin versionar, lo que impide cualquier rollback.

## 8. Investigación y discovery

### 8.1 Discovery Brief (2026-09-18)

- **Qué se hizo:** auditoría de solo lectura de la SSoT + verificación ejecutada de build/pruebas + creación de la memoria viva.
- **Estado del sistema:** 5 proyectos, arquitectura hexagonal correcta, 178/178 pruebas verdes, dominio completo para 9 agregados, capas `Application`/`Infrastructure`/`Api` funcionalmente mínimas.
- **Riesgo dominante:** la Ley se contradice a sí misma en `DOMINIO 8/9/10` y el código ya tomó una interpretación **no dictada** por el Owner → cualquier trabajo nuevo sobre esos dominios multiplica el retrabajo.
- **Recomendación:** no escribir más código de Fulfillment/Facturación/Devoluciones hasta [Q-13](00-bootstrap/questions-for-owner.md#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante); mientras tanto, cerrar [H-09](00-bootstrap/spec-conformance-matrix.md)/[H-11](00-bootstrap/spec-conformance-matrix.md)/[H-12](00-bootstrap/spec-conformance-matrix.md) y versionar lo existente.

### 8.2 Research Log

| Fecha | Fuente | Hallazgo |
|---|---|---|
| 2026-09-18 | `git diff --numstat [SDD/Domain/ZENTRIC.md](Domain/ZENTRIC.md)` | La Ley solo tiene adiciones (38/1); el texto original no fue reescrito |
| 2026-09-18 | `Get-FileHash` de la skill | Repo v6.0.0 ≠ instalada v3.1.0 (la carga automática usaba la versión antigua) |
| 2026-09-18 | Búsqueda por patrón sobre carpetas sin versionar | La herramienta de búsqueda devuelve falsos negativos ahí; se cambió a escaneo directo por archivo |
| 2026-09-18 | Paquete NuGet `MediatR` 14.2.0 (`MediatR.xml` + stack trace) | Confirmada la firma `Handle(request, RequestHandlerDelegate<TResponse>, CancellationToken)` y `AddOpenBehavior(Type, ServiceLifetime)`; MediatR 14 exige `ILoggerFactory` y ejecuta comprobación de licencia ([Q-14](00-bootstrap/questions-for-owner.md#q-14-licenciamiento-de-mediatr-14-y-fluentvalidation-abierta-no-bloquea-el-codigo-actual)) |
| 2026-09-18 | `Zentric.Domain/Products/ValueObjects/Money.cs` | Guardas reales: `amount >= 0`, `currency` no vacía y de 3 caracteres tras `Trim().ToUpperInvariant()` → reglas espejadas en `AddOrderItemCommandValidator`, no inventadas |
