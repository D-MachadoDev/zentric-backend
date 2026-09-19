# SDD — memoria viva del proyecto

mapa-base: working tree sobre 40aea11 (cambios locales sin commit) · última auditoría de mapa: 2026-09-18 · cobertura: 30/33 (91 %) · perfil: Personal [INFERIDO] (1 desarrollador, sin CI, decisiones por chat)

> **Rol de este archivo** (skill generic-sdd-agent v6.0.0, secciones :0.12 y :13.1-d de la skill): este repo **ya declara su SSoT** en [AGENTS.md:0](#agentsmd:0) — la carpeta SDD/. Por la regla "no se crea una SSoT paralela", este documento **no duplica** las especificaciones: es la **memoria viva** (índice, mapa de entidades, decisiones/ADDENDA, verificación, riesgos y estado) y **apunta** a los documentos canónicos.
>
> **Jerarquía de verdad:** decisión del Owner → [AGENTS.md](#agentsmd) → [ZENTRIC.md](/ZENTRIC.md) (Ley, intocable) → resto de SDD/ → este archivo.

## 1. Contexto y alcance
- **Propósito observado** [OBSERVADO]: API central y núcleo de dominio de **Zentric**, plataforma que intermedia entre **Comprador** y **Vendedor** administrando usuarios, catálogo, inventario distribuido, pedidos, facturación, logística y posventa ([ZENTRIC.md](/ZENTRIC.md)).
- **Owner:** no registrado en el repositorio [PENDIENTE].
- **Biblia (intocable, :0.7):** [ZENTRIC.md](/ZENTRIC.md). **Integridad verificada el 2026-09-18:** git diff --numstat = **38 adiciones / 1 borrado**; el único borrado es una línea separadora, por lo que el texto original del cliente **no fue reescrito**. Las adiciones están bajo # [ADDENDUM - DICTADO POR OWNER] → transcripción en [SDD.md:4](#addendum---dictado-por-owner]-→-transcripción-en-[sddmd:4) ([ADD-001](#add-001)…[ADD-003](#add-003)).
- **Otros documentos normativos:** [AGENTS.md](#agentsmd) (contrato operativo), [SDD/01-system-overview.md](#sdd/01-system-overviewmd), [SDD/02-software-architecture.md](#sdd/02-software-architecturemd), SDD/Domain/*, SDD/Application/*, SDD/Infrastructure/*, SDD/Presentation/*, SDD/Adr/*.
- **Alcance del sistema:** [SDD/01-system-overview.md:3.1](#sdd/01-system-overviewmd:31) (incluidos) y [SDD/01-system-overview.md:3.2](#sdd/01-system-overviewmd:32) (excluidos: UI, apps móviles, portales, autenticación técnica, tecnologías de implementación y almacenamiento).
- **Fuera de alcance de este archivo:** detalle de reglas de dominio (SDD/Domain/), contratos HTTP (SDD/Presentation/), mapeo EF (SDD/Infrastructure/).
- **Stack verificado** [CONFIRMADO]: C#/.NET 10 · Zentric.slnx con **5 proyectos** · xUnit · MediatR 14.2.0 · FluentValidation 12.1.1 · Npgsql.EntityFrameworkCore.PostgreSQL 10.0.3.
- **Comandos oficiales** ([AGENTS.md :7](#agentsmd-:7)): dotnet restore · dotnet build Zentric.slnx · dotnet test Zentric.slnx.
- **Riesgo residual aceptado hoy** ([risks-and-gaps.md :4](#risks-and-gapsmd-:4)): sin entorno productivo, sin datos reales y sin consumidores del contrato → renombrar hoy es barato; aplazarlo encarece cada artefacto nuevo.

### Metodología Bootstrap
- Modo de operación: **F (Brownfield sin contexto confiable)** + **A (arranque)**.
- Existe una spec funcional de negocio sólida ([ZENTRIC.md](/ZENTRIC.md)) y una spec
  de dominio detallada, pero con **solapes de numeración y contradicciones**.
- `[OBSOLETO — 2026-09-18]` ~~Solo existe el proyecto `Zentric.Domain`, con 6 artefactos implementados, 1 stub vacío y **0 pruebas**~~ → hoy la solución tiene **5 proyectos** (Domain, Application, Infrastructure, Api, Tests) con **178 pruebas** en verde, pero las capas exteriores y los dominios Orders/Logistics/Billing/Returns se implementaron **sin registrar** y con decisiones no dictadas por el Owner.
- `[OBSOLETO — 2026-09-18]` ~~[AGENTS.md](#obsoleto-—-2026-09-18]-~~[agentsmd) referencia dos documentos de `/SDD` que no existen~~ → existen [SDD/01-system-overview.md](#sdd/01-system-overviewmd), [SDD/02-software-architecture.md](#sdd/02-software-architecturemd) y las carpetas `SDD/Application/`, `SDD/Infrastructure/` y `SDD/Presentation/`.
- `[RIESGO]` **Nuevo bloqueante:** la Ley define `DOMINIO 8/9/10` dos veces con contenidos cruzados → **[Q-13](#q-13)** ([questions-for-owner.md](#questions-for-ownermd)).

### Mapa del Repositorio y Dependencias
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

## 2. Mapa de entidades
> **Procedimiento:** [generic-sdd-agent v6.0.0 :14](#generic-sdd-agent-v600-:14) (Auditoría de Mapeo Completo - Anti-Amnesia)

> **Estado de verificación:** `[CONFIRMADO]` por enumeración directa del árbol, de los `.csproj`, de `Program.cs`, de `ZentricDbContext` y de las suites de `Zentric.Tests`. Ver [SDD.md:14](#2-mapa-de-entidades) para el procedimiento completo.

### 2.1 Resumen ejecutivo

| Métrica | Valor |
|---|---|
| **Entidades mapeadas** | 30 (E-001 a E-030) |
| **Agregados raíz** | 14 (User, Buyer, Product, Inventory, Warehouse, CustomerOrder, FulfillmentOrder, Invoice, ReturnRequest + puertos/servicios) |
| **Cobertura** | 30/33 (91 %) — ver [C-06](#c-06) para huérfanos |
| **Pruebas asociadas** | 178 casos de prueba en verde |

### 2.2 Tabla de entidades

| ID | Entidad | Tipo | Ubicación en código | Responsabilidad | Relaciones / Dependencias | Estado | Pruebas |
|---|---|---|---|---|---|---|---|
| **E-001** | [User](Zentric.Domain/Users/User.cs) | Agregado raíz | [Zentric.Domain/Users/User.cs](../Zentric.Domain/Users/User.cs) | Usuario del marketplace. Rol único. Bloqueo de usuario. | Usa `FullName`, `Email`, `UserRole`, `UserStatus` | ⚠️ **Parcial** — sin `IdentityDocument` ([H-05](#h-05)) | [Tests/Users/UserTests.cs](../Zentric.Tests/Users/UserTests.cs) |
| **E-002** | [Buyer](Zentric.Domain/Buyers/Buyer.cs) | Agregado raíz | [Zentric.Domain/Buyers/Buyer.cs](../Zentric.Domain/Buyers/Buyer.cs) | Comprador. Dirección principal y adicionales. | Usa `PaymentTokens` ([H-03](#h-03)) | ⚠️ **Parcial** — sin validaciones completas | 🟡 `[PENDIENTE]` — [T-002b](#t-002b) |
| **E-003** | [Product](Zentric.Domain/Products/Product.cs) | Agregado raíz | [Zentric.Domain/Products/Product.cs](../Zentric.Domain/Products/Product.cs) | Catálogo de productos. Gestiona variantes. Reglas [CAT-03](Domain/06-business-rules.md) (variante obligatoria en físicos). | Posee **E-004** (ProductVariant). Usa `Money`, `ProductType`. | ✅ **Implementado** — [ADR-0002](Adr/0002-clave-inventario-variantid.md)/0003 | [Products/ProductTests.cs](../Zentric.Tests/Products/ProductTests.cs) |
| **E-004** | [ProductVariant](Products/ProductVariant.cs) | Entidad hija | [Products/ProductVariant.cs](../Zentric.Domain/Products/ProductVariant.cs) | SKU. Unidad de stock indivisible. | Usa **E-005** (VariantAttribute). Pertenece a **E-003**. | ✅ **Implementado** | [Products/ProductVariantTests.cs](../Zentric.Tests/Products/ProductVariantTests.cs) |
| **E-005** | [VariantAttribute](Products/ValueObjects/VariantAttribute.cs) | Value Object | [Products/ValueObjects/VariantAttribute.cs](../Zentric.Domain/Products/ValueObjects/VariantAttribute.cs) | Atributo nombre/valor (ej. Talla, Color). Inmutable. | Usada por **E-004**. | ✅ **Implementado** — `[PROPUESTO]` ([Q-11](#q-11)) | [Products/VariantAttributeTests.cs](../Zentric.Tests/Products/VariantAttributeTests.cs) |
| **E-006** | [Money](Products/ValueObjects/Money.cs) | Value Object | [Products/ValueObjects/Money.cs](../Zentric.Domain/Products/ValueObjects/Money.cs) | Importe + moneda. Aritmética homogénea (misma moneda). Inmutable. | Usado por **E-003**, **E-009**, **E-010**, **E-013**. | ✅ **Implementado** | [Products/MoneyTests.cs](../Zentric.Tests/Products/MoneyTests.cs) |
| **E-007** | [Inventory](Inventories/Inventory.cs) | Agregado raíz | [Inventories/Inventory.cs](../Zentric.Domain/Inventories/Inventory.cs) | Stock por `(VariantId, WarehouseId)`. Contadores: Available, Reserved, Damaged, Used ([Inventory.cs[:14](#inventorycs[:14)](../Zentric.Domain/Inventories/Inventory.cs#L14)). | Clave `VariantId` ([ADR-0002](Adr/0002-clave-inventario-variantid.md)). Relación con **E-008** (Warehouse). | ⚠️ **Parcial** — [H-01](#h-01), [H-02](#h-02), [H-04](#h-04) abiertos | [Inventories/InventoryTests.cs](../Zentric.Tests/Inventories/InventoryTests.cs) |
| **E-008** | [Warehouse](Warehouses/Warehouse.cs) | Agregado raíz | [Warehouses/Warehouse.cs](../Zentric.Domain/Warehouses/Warehouse.cs) | Bodega del marketplace o de vendedor. Tipos: Marketplace, Seller. | Usa `WarehouseType`. Relación con **E-007** (Inventory). | ⚠️ **Parcial** — [C-05](#c-05) (nombramiento desviado: `Seller` vs `Vendor`) | [Warehouses/WarehouseTests.cs](../Zentric.Tests/Warehouses/WarehouseTests.cs) |
| **E-009** | CustomerOrder | Agregado | [Orders/CustomerOrder.cs](../Zentric.Domain/Orders/CustomerOrder.cs) | Pedido consolidado; estados Cart→Delivered; nace en `Cart` (`[CustomerOrder.cs:49](../Zentric.Domain/Orders/CustomerOrder.cs#L49)) | posee E-010; usa E-006 | implementado con desviaciones ([CustomerOrder.cs:24](../Zentric.Domain/Orders/CustomerOrder.cs#L24), moneda "USD" inventada) | [Orders/CustomerOrderTests.cs](../Zentric.Tests/Orders/CustomerOrderTests.cs) |
| **E-010** | [OrderItem](Orders/Entities/OrderItem.cs) | Entidad hija | [Orders/Entities/OrderItem.cs](../Zentric.Domain/Orders/Entities/OrderItem.cs) | Línea del pedido. Variante, cantidad, precio unitario. | Usa **E-006** (Money). Pertenece a **E-009** (CustomerOrder). | ✅ **Implementado** | [Orders/CustomerOrderTests.cs](../Zentric.Tests/Orders/CustomerOrderTests.cs) |
| **E-011** | [FulfillmentOrder](Logistics/FulfillmentOrder.cs) | Agregado raíz | [Logistics/FulfillmentOrder.cs](../Zentric.Domain/Logistics/FulfillmentOrder.cs) | Despacho por vendedor. Nace en Packed ([FulfillmentOrder.cs:33](../Zentric.Domain/Logistics/FulfillmentOrder.cs#L33)). | Posee **E-012** (Shipment). Relación con **E-008** (Warehouse). | ❌ **Incompleto** — falta `PendingPack` ([C-08](#c-08)). Ver [Q-13](#q-13) | [Logistics/FulfillmentOrderTests.cs](../Zentric.Tests/Logistics/FulfillmentOrderTests.cs) |
| **E-012** | [Shipment](Logistics/Entities/Shipment.cs) | Entidad hija | [Logistics/Entities/Shipment.cs](../Zentric.Domain/Logistics/Entities/Shipment.cs) | Guía/envío por bodega. Nace en línea 5 ([Shipment.cs:5](../Zentric.Domain/Logistics/Entities/Shipment.cs#L5)). | Usada por **E-011** (FulfillmentOrder). Relación con **E-008** (Warehouse). | ✅ **Implementado** | [Logistics/FulfillmentOrderTests.cs](../Zentric.Tests/Logistics/FulfillmentOrderTests.cs) |
| **E-013** | [Invoice](Billing/Invoice.cs) | Agregado raíz | [Billing/Invoice.cs](../Zentric.Domain/Billing/Invoice.cs) | Factura maestra/split. Fábricas: `CreateMaster`, `CreateVendorDetail`. | Usa **E-006** (Money). Relación con **E-003** (Product). | ❌ **Incompleto** — falta "Detalle Zentric" ([C-08](#c-08)). Ver [Q-13](#q-13) | [Billing/InvoiceTests.cs](../Zentric.Tests/Billing/InvoiceTests.cs) |
| **E-014** | [ReturnRequest](Returns/ReturnRequest.cs) | Agregado raíz | [Returns/ReturnRequest.cs](../Zentric.Domain/Returns/ReturnRequest.cs) | Devolución con doble aprobación. Prohíbe devolución de digitales. | Usa `ProductType`, `ReturnStatus`. Relación con **E-003** (Product). | ⚠️ **Implementado** — **no toca inventario** (falta retorno a stock con etiqueta "Usado"). Ver [C-08](#c-08) | [Returns/ReturnRequestTests.cs](../Zentric.Tests/Returns/ReturnRequestTests.cs) |
| E-015 | IUserRepository | Puerto de salida | Zentric.Domain/Users/Ports/IUserRepository.cs | Acceso a usuarios; unicidad de correo marcada como TODO | **no** implementado en Infrastructure | parcial ([H-07](#h-07)) | — |
| E-016 | ICustomerOrderRepository | Puerto de salida | Zentric.Application/Orders/Ports/ | Persistencia de pedidos | implementado por E-022 | OK | — |
| E-017 | IFulfillmentOrderRepository | Puerto de salida | Zentric.Application/Logistics/Ports/ | Persistencia de despachos | implementado por E-022 | OK | — |
| E-018 | Result / Result<T> | Tipo de aplicación | Zentric.Application/Common/Models/Result.cs | Resultado explícito, sin excepciones de flujo | usado por E-019, E-020 y E-024 | implementado | — |
| E-019 | CreateCartCommand, AddOrderItemCommand | Casos de uso | Zentric.Application/Orders/Commands/ | Crear carrito; añadir ítem | usa E-016 y E-009 | implementado con hallazgo **[H-09](#h-09)** | — |
| E-020 | CreateFulfillmentOrderCommand | Caso de uso | Zentric.Application/Logistics/Commands/ | Derivar despacho de un pedido | usa E-017 y E-011 | implementado | — |
| E-021 | ZentricDbContext | Datos (adaptador) | Zentric.Infrastructure/Persistence/ZentricDbContext.cs | 9 DbSet + mapeo OwnsMany/OwnsOne | mapea E-001…E-014 | implementado; mapea agregados de dominio ([H-13](#h-13)) | — |
| E-022 | CustomerOrderRepository, FulfillmentOrderRepository | Adaptadores de salida | Zentric.Infrastructure/Repositories/ | Implementan E-016/E-017 | usa E-021 | implementado | — |
| E-023 | Migraciones EF InitialCreate, CompleteSchema | Datos | Zentric.Infrastructure/Migrations/ | Esquema PostgreSQL | derivan de E-021 | generadas; **nunca aplicadas a un servidor** | — |
| E-024 | Program.cs, ApiControllerBase, OrdersController, LogisticsController | Puntos de entrada | Zentric.Api/ | Composition Root + 3 endpoints REST | usa E-018, E-019, E-020 | implementado; ProblemDetails incompleto (**[H-11](#h-11)**) | — |
| E-025 | Suites xUnit (11 archivos) | Pruebas | `Zentric.Tests/` | 178 casos en verde | cubre E-001…E-014 | implementado | — |
| E-026 | `Zentric.slnx` | Configuración | raíz | Ensambla los 5 proyectos | — | OK | — |
| E-027 | Skill `generic-sdd-agent` + `scripts/sync-skill.ps1` | Operación | `.agents/skills/generic-sdd-agent/` | Metodología operativa del agente | copia instalada en `%USERPROFILE%\.agents\skills` | **v6.0.0** (repo) vs **v3.1.0** (instalada) → **[C-09](#c-09)** | — |
| E-028 | Documentos `SDD/` | Documentos | `SDD/**` | SSoT del sistema | apunta a E-029 | parcial (**[C-06](#c-06)**: [Software-arquitecture.md](Domain/Software-arquitecture.md) = 0 bytes) | — |
| E-029 | [ZENTRIC.md](/ZENTRIC.md) | Biblia (Ley) | [ZENTRIC.md](/ZENTRIC.md) | Especificación funcional del cliente | rige E-001…E-014 | intacta + ADDENDA (`[ADD-001](#add-001)…003`) | — |
| E-030 | [ADR-0001](Adr/0001-reserva-fragmentacion-contingencia.md)…0003 | Decisiones | `SDD/Adr/` | Reserva/fraccionamiento, clave de stock, variante obligatoria | rigen E-007, E-004, E-003 | aprobadas por el Owner | — |

**Ajenos / generados (clasificados, no mapeados):** `bin/`, `obj/` (generado), `LICENSE`, `.gitignore`, `.vscode/settings.json`, `Zentric.Api.http`, `appsettings.Development.json`.

**Huérfanos (pendientes de acción):** `Zentric.Application/Class1.cs` y `Zentric.Infrastructure/Class1.cs` → **eliminados en SPEC-007 (2026-09-18)**, cerrado · [SDD/Domain/Software-arquitecture.md](Domain/Software-arquitecture.md) (0 bytes, **[C-06](#c-06)**) · `generic-sdd-agent.md` raíz (v2.0.0, declarado **superseded**).

**Altas del 2026-09-18 (SPEC-006/SPEC-007):** E-031 `ValidationBehavior<TRequest,TResponse>` (`Zentric.Application/Common/Behaviors/ValidationBehavior.cs`) · E-032 validadores `CreateCartCommandValidator`, `AddOrderItemCommandValidator`, `CreateFulfillmentOrderCommandValidator` (`*/Validators/`) · E-033 [SDD/SDD.md](#sdd/sddmd) (memoria viva).

**Fantasmas (se mencionan y no existen):** `IDomainEventDispatcher` y los 7 eventos de dominio (G-04/[T-012](#t-012)) · `IInventoryRepository`, `IProductRepository`, `IWarehouseRepository`, `IReturnRequestRepository`, `IInvoiceRepository` (G-05) · `InventoryReservationService`, `OrderSplitterService`, `CheckoutTimeoutService`, `ReturnsApprovalService` (G-06) · `CheckoutOrderCommand`, `DispatchFulfillmentCommand`, `RequestReturnCommand`, `ApproveReturnCommand`, `CreateProductCommand`, `PublishProductCommand` (**especificados en [SDD/Application/01-use-cases-and-ports.md :3](Application/01-use-cases-and-ports.md#3-casos-de-uso-commands---dominio-de-pedidos-customer-orders)–[:6](Application/01-use-cases-and-ports.md#6-casos-de-uso-commands---catalogo-products) y no implementados**) · estados `PendingPack` e `InvoiceType.ZentricDetail` · `IUnitOfWork` · validadores FluentValidation · `.editorconfig` y CI.

**Zonas no exploradas de este mapa:** ejecución real contra PostgreSQL (no hay servidor en este entorno), comportamiento HTTP en runtime (no se levantó la API), contenido de `bin/`/`obj/`.

**Cobertura declarada: 30/33 (91 %) — parcial, no completa** (quedan huérfanos sin acción y fantasmas abiertos).
`[RIESGO]` La herramienta de búsqueda del agente **no indexa las carpetas no versionadas** (falso negativo comprobado: sobre `Zentric.Api/` y `Zentric.Application/` devolvió 0 coincidencias para patrones que sí existen); en esas rutas hay que usar escaneo directo.

## 3. Especificaciones activas
[SDD/SDD.md](#sdd/sddmd) **no duplica especificaciones**: las indexa. Documentos canónicos vigentes:

| ID | Spec | Documento | Estado | Nota |
|---|---|---|---|---|
| SPEC-000 | Adopción SDD (bootstrap brownfield) | [SDD.md](SDD.md) (consolidado) | en curso | línea base del 2026-09-17 |
| SPEC-001 | Ley funcional Zentric | [ZENTRIC.md](/ZENTRIC.md) | **congelada (Biblia)** | intacta; ADDENDA en [:4](/ZENTRIC.md#dominio-4-gestion-de-bodegas) |
| SPEC-002 | Dominio (modelos, reglas, invariantes, puertos, eventos, ciclo de vida) | `SDD/Domain/*` y `SDD/Domain/services/*` | parcial | [C-03](#c-03), [C-04](#c-04), [C-06](#c-06), [C-07](#c-07) abiertas |
| SPEC-003 | Aplicación (casos de uso y puertos) | [SDD/Application/01-use-cases-and-ports.md](Application/01-use-cases-and-ports.md) | borrador | **contradicha por el código** ([H-09](#h-09), [H-10](#h-10)) |
| SPEC-004 | Infraestructura (EF Core, mapeos, migraciones) | [SDD/Infrastructure/01-data-access.md](Infrastructure/01-data-access.md) | en curso | repositorios de Returns/Invoices pendientes |
| SPEC-005 | Presentación (endpoints REST) | [SDD/Presentation/01-endpoints.md](Presentation/01-endpoints.md) | en curso | 3 de ~10 endpoints |
| SPEC-006 | Trazabilidad de la tanda no registrada ([T-011](#t-011)…[T-022](#t-022)) | **este documento**, [:5](#:5) y :7 | **hecha** | riesgo 1; solo documentación, cero código |
| SPEC-007 | Validación de entrada (FluentValidation), RFC 7807 e higiene: cierra [H-09](#h-09)/[H-11](#h-11)/[H-12](#h-12) | **este documento**, [:3](#:3) · [verification-baseline.md :11](#verification-baselinemd-:11) | **hecha** | riesgo 2 · 206/206 pruebas |

### SPEC-006 — Trazabilidad de la tanda no registrada · riesgo: 1 · estado: hecha

**Propósito y señal de resultado:** que la SSoT describa el estado real de `Zentric.Api`, `Zentric.Application`, `Zentric.Infrastructure` y los dominios Orders/Logistics/Billing/Returns (hoy verdes pero **sin registrar**), sin tocar código de producción.

**Alcance:** actualizar [SDD/SDD.md](#sdd/sddmd), los 7 documentos de [SDD.md](SDD.md), las 3 specs de capa, [AGENTS.md :0.0](#agentsmd-:00) y `scripts/sync-skill.ps1`.
**Fuera de alcance:** cualquier cambio en los proyectos `Zentric.*` (código) y cualquier alteración de [ZENTRIC.md](/ZENTRIC.md).

**Requisitos:**
- FR-01: cada tarea ejecutada ([T-011](#t-011)…[T-022](#t-022)) aparece en el plan con estado y evidencia.
- FR-02: la suite real (**178**) queda registrada en [verification-baseline.md](#verification-baselinemd).
- FR-03: cada contradicción detectada se registra como riesgo/pregunta; **ninguna la resuelve el agente**.
- [INV-01](Domain/06-business-rules.md): la Ley permanece intacta. · ERR-01: lo no verificable se marca `[PENDIENTE]`, no se rellena.
- CA-01: **Dado** el working tree con trabajo sin commit, **cuando** termino, **entonces** 0 archivos de código cambiaron (verificable con `git status`).
- CA-02: **Dado** el repositorio sin [SDD/SDD.md](#sdd/sddmd), **cuando** termino, **entonces** existe con las 8 secciones de la plantilla y apunta a la SSoT existente.

**Tareas:**
- [T-01] Actualizar documentos (plan, baseline, current-state, matriz, riesgos, preguntas, mapa, README) — cubre FR-01, FR-02 — verificación: relectura + `git status` — riesgo 1 — estado: **hecha**.
- [T-02] Registrar contradicciones nuevas ([C-08](#t-02), [C-09](#c-09), [H-09](#h-09)…[H-14](#h-14)) — cubre FR-03 — verificación: [risks-and-gaps.md :2](#risks-and-gapsmd-:2) y [:6](#:6) — estado: **hecha**.
- [T-03] Sincronizar la skill instalada — verificación: `Get-FileHash` origen = destino — estado: **hecha**.
- [T-04] Re-ejecutar `build`/`test` al cerrar — verificación: [verification-baseline.md :10](#t-04) — estado: **hecha**.

**Verificación**

| Requisito | Tarea | Evidencia | PASS/FAIL/PENDING |
|---|---|---|---|
| FR-01 | [T-01](#t-01) | [migration-to-sdd-plan.md :2](#migration-to-sdd-planmd-:2) Fase 3b/4 con estados | PASS |
| FR-02 | [T-01](#t-01) | [verification-baseline.md :10](#verification-baselinemd-:10) (`178/178`) | PASS |
| FR-03 | [T-02](#t-02) | [risks-and-gaps.md :2](#risks-and-gapsmd-:2) ([R-11](#r-11)…[R-16](#r-16)) y [:6](#:6) | PASS |
| [INV-01](Domain/06-business-rules.md) | [T-01](#t-01) | `git diff --numstat [ZENTRIC.md](/ZENTRIC.md)` = 38/1, sin borrado de texto original | PASS |
| CA-01 | [T-01](#t-01) | `git status --short` (solo `.md` y `sync-skill.ps1`) | PASS |
| CA-02 | [T-01](#t-01) | Este archivo, [:1](#:1)–:8 | PASS |

### SPEC-007 — Validación de entrada, RFC 7807 e higiene · riesgo: 2 · estado: hecha

**Propósito y señal de resultado:** cumplir [AGENTS.md :3.2](#agentsmd-:32) (prohibido el `try-catch` genérico en Application), :3.4 (middleware global con RFC 7807), [:4.3](#:43) (validación de entrada obligatoria con FluentValidation) y el DoD [:8](#:8) (higiene). Señal: suite en verde con pruebas que demuestren que una entrada inválida se rechaza **antes** del dominio y que una excepción catastrófica ya no se silencia.

**Alcance:** `Zentric.Application` (comportamiento de validación + validadores + handler de Orders), `Zentric.Api` (registro de DI y middleware) y `Zentric.Tests`.
**Fuera de alcance:** cualquier regla de negocio nueva, la validación de stock de [PED-01](Domain/06-business-rules.md) (depende de [Q-03](#q-03)/[Q-04](#q-04)), la corrección de [Q-13](#q-13) y todo lo relacionado con Fulfillment/Facturación/Devoluciones.

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
- [T-29] Application: quitar el `try-catch` genérico de `AddOrderItemCommandCommand` — cubre FR-03 — verificación: regresión de [H-09](#t-29) — riesgo 2 — **hecha**.
- [T-30] Api: `AddProblemDetails()`, validadores, `AddOpenBehavior`, `UseExceptionHandler()` — cubre FR-04 — verificación: 7 pruebas de integración DI — riesgo 2 — **hecha**.
- [T-31] Higiene: eliminar `Class1.cs` — cubre FR-05 — verificación: `Test-Path` = False — riesgo 1 — **hecha**.
- [T-32] Pruebas de extremo a extremo por HTTP (TestServer) — cubre CA-01/CA-04 — **NO hecha** `[PENDIENTE]` — riesgo 2.

**Verificación**

| Requisito | Tarea | Evidencia | PASS/FAIL/PENDING |
|---|---|---|---|
| FR-01 | [T-27](#t-27), [T-28](#t-28) | 21 pruebas unitarias en verde | PASS |
| FR-02 | [T-27](#t-27), [T-30](#t-30) | 7 pruebas de integración: entrada inválida → `Result.Failure`, handler no invocado | PASS |
| FR-03 | [T-29](#t-29) | `AddOrderItemCommand.cs` sin `catch`; regresión en verde | PASS |
| FR-04 | [T-30](#t-30) | `Program.cs`: `AddProblemDetails()` + `UseExceptionHandler()` | PASS (compilación y DI) / `[PENDIENTE]` en HTTP real |
| FR-05 | [T-31](#t-31) | `Test-Path` de ambos `Class1.cs` = `False` | PASS |
| CA-01…CA-03 | [T-27](#t-27)…[T-30](#t-30) | [verification-baseline.md :11](#verification-baselinemd-:11) (`206/206`) | PASS |
| CA-04 (HTTP) | [T-32](#t-32) | `[PENDIENTE]`: la API no se levantó | PENDING |

## 4. Decisiones y ADDENDA
### 4.1 Decisiones del Owner ya aplicadas (2026-09-17)

| ID | Decisión | ADR / evidencia |
|---|---|---|
| [Q-01](#q-01) ([C-01](#c-01)) | Reserva en **bodega única**; fraccionamiento solo como contingencia cuando ninguna bodega individual cubre la cantidad | [SDD/Adr/0001-reserva-fragmentacion-contingencia.md](Adr/0001-reserva-fragmentacion-contingencia.md) |
| [Q-02](#q-02) ([C-02](#c-02)) | La clave del inventario es **`VariantId` (SKU)**; `ProductVariant` es entidad hija de `Product`; clave `(VariantId, WarehouseId)` | [SDD/Adr/0002-clave-inventario-variantid.md](Adr/0002-clave-inventario-variantid.md) |
| [Q-10](#q-10) | Variante **obligatoria solo en `Physical`** (opción C3) | [SDD/Adr/0003-variante-obligatoria-productos-fisicos.md](Adr/0003-variante-obligatoria-productos-fisicos.md) |

### 4.2 Adiciones a la Biblia (formato [skill :0.7](#skill-:07))

> **Integridad `[CONFIRMADO]`:** [ZENTRIC.md](/ZENTRIC.md) conserva el texto original: `git diff --numstat` = **38 adiciones / 1 borrado** y ese borrado es una línea separadora, no contenido del cliente.
>
> `[CONTRADICCIÓN]` **[C-10](#c-10)** — El bloque añadido que antecede al rótulo (`## DOMINIO 8`, `## DOMINIO 9`, `## DOMINIO 10` del ciclo de estados, la facturación y las devoluciones) **no lleva el rótulo `[ADDENDUM - DICTADO POR OWNER]`**, que [AGENTS.md :0.7](#addendum---dictado-por-owner]**,-que-[agentsmd-:07) exige para toda adición a la Biblia. Queda registrado; **no lo corrijo yo** (la Biblia es intocable).

### [ADDENDUM - DICTADO POR OWNER] [ADD-001](#addendum---dictado-por-owner]-[add-001) — Dominio 8: logística y despachos (Fulfillment)

- Fecha: **no registrada** `[PENDIENTE]` · Owner: **no registrado** `[PENDIENTE]` · Origen: bloque `[ADDENDUM - DICTADO POR OWNER]` de [ZENTRIC.md](/ZENTRIC.md)
- Regla dictada (texto del documento):
  - "**Estados:** Empacado y Despachado."
  - "**Regla (Stock Fantasma):** No debería ocurrir, pero en caso de haber un quiebre de stock fantasma, el pedido se cancela con devolución obligatoria para no retener stock irreal."
- Afecta a: `DOMINIO 8` de la Ley · Relacionado con: **[C-08](#c-08)**, E-011, [Q-03](#q-03)

### [ADDENDUM - DICTADO POR OWNER] [ADD-002](#addendum---dictado-por-owner]-[add-002) — Dominio 9: devoluciones y reembolsos

- Fecha: **no registrada** `[PENDIENTE]` · Owner: **no registrado** `[PENDIENTE]` · Origen: bloque `[ADDENDUM - DICTADO POR OWNER]`
- Regla dictada:
  - "**Regla (Prohibición):** Está **prohibido** devolver productos digitales."
  - "**Flujo Físico:** El operador logístico inspecciona que el producto esté en buen estado. Si es así, requiere la aprobación del Vendedor. Si ambas se cumplen, el producto vuelve al stock en el inventario con la etiqueta Usado."
- Afecta a: `DOMINIO 9` de la Ley · Relacionado con: **[C-08](#c-08)**, E-014, E-007 (`ReturnToUsedStock`), [Q-13](#q-13)

### [ADDENDUM - DICTADO POR OWNER] [ADD-003](#addendum---dictado-por-owner]-[add-003) — Dominio 10: facturación y pagos

- Fecha: **no registrada** `[PENDIENTE]` · Owner: **no registrado** `[PENDIENTE]` · Origen: bloque `[ADDENDUM - DICTADO POR OWNER]`
- Regla dictada:
  - "**Factura Maestra:** Entregada al cliente con el total de la transacción."
  - "**Detalle Zentric:** Detalle transaccional desglosado para control de plataforma."
  - "**Factura de Vendedor:** Factura propia detallando el monto que le corresponde al Vendedor (Split)."
- Afecta a: `DOMINIO 10` de la Ley · Relacionado con: **[C-08](#c-08)**, E-013 (`InvoiceType` solo tiene `Master` y `VendorDetail`), [Q-13](#q-13)

`[OBSERVADO]` Los bloques añadidos de la Ley presentan **pérdida de caracteres acentuados** ("Gestin de Logstica", "Seccin aadida", "clarificacin"). Se registra como defecto de forma de la Ley; **no se corrige sin autorización del Owner** (Biblia intocable).

### 4.3 Resultado posterior (Gate 5)

No aplica todavía: ninguna de las tandas de Fase 3b/4 ([SDD/SDD.md :7](#sdd/sddmd-:7)) tiene señal de resultado definida ni fue validada en un entorno con datos.

## 5. Verificación y línea base
### 5.1 Comandos ejecutados (2026-09-18 · Windows / PowerShell 7 / .NET SDK 10 · **sin** PostgreSQL)

| Comando | Resultado | Evidencia |
|---|---|---|
| `dotnet build Zentric.slnx --nologo` | **PASS** — `Build succeeded. 0 Warning(s) 0 Error(s)`; compilan los 5 proyectos | salida de consola |
| `dotnet test Zentric.slnx --nologo` | **PASS** — `Failed: 0, Passed: 178, Skipped: 0, Total: 178` (~479 ms) | salida de consola |
| Auditoría de dependencias (lectura de los 5 `.csproj`) | **PASS** — `Domain`←`Application`←`Infrastructure`←`Api` ([AGENTS.md :2.1](#agentsmd-:21)) | `Select-String ProjectReference` |
| Auditoría anti-amnesia (9 `DbSet` vs agregados de la spec) | **PASS parcial** — los 9 agregados de `SDD/Infrastructure` [:2](#:2) están mapeados | `Select-String DbSet` |
| Integridad de la Ley | **PASS** — 38 adiciones / 1 borrado (línea separadora) | `git diff --numstat` |
| Integridad de la skill instalada | **FAIL antes / PASS después** — `24E7ED4F…` (repo v6.0.0) vs `EB4E7166…` (instalada v3.1.0) | `Get-FileHash` |

`[CONFIRMADO]` Comparación con la línea base anterior: **164 → 178** pruebas (+14: Orders 6, Logistics 3, Billing 2, Returns 3, según el conteo por archivo de suite).

### 5.2 Fallos preexistentes

Ninguno. La suite está 100 % verde; no hay `Skipped` ni fallos conocidos sin corregir.

### 5.3 Validaciones **no** ejecutadas (honestidad de evidencia)

- **Persistencia real:** las migraciones `InitialCreate` y `CompleteSchema` **nunca se aplicaron** contra un PostgreSQL; no hay servidor en el entorno ni pruebas de integración. `[PENDIENTE]`
- **Comportamiento HTTP:** la API no se levantó; no se ejecutó ninguna petición contra los 3 endpoints. `[PENDIENTE]`
- **Mapeo EF en runtime:** `OwnsMany`/`OwnsOne` y la conversión de `Email` (`Property(u => u.Email).HasConversion(...)`) sin validar en ejecución. `[PENDIENTE]`
- **Formato/lint/análisis estático:** no hay `.editorconfig`, ni `AnalysisLevel`, ni `TreatWarningsAsErrors`, ni CI → el "0 warnings" del build **no** demuestra ausencia de hallazgos. `[RIESGO]` [R-08](#r-08) sigue abierto.

### 5.4 Segunda tanda de verificación — SPEC-007 (2026-09-18)

| Comando | Resultado | Evidencia |
|---|---|---|
| `dotnet restore Zentric.slnx` | **PASS** | "All projects are up-to-date for restore." |
| `dotnet build Zentric.slnx --nologo` | **PASS** | `Build succeeded. 0 Warning(s) 0 Error(s)` |
| `dotnet test Zentric.slnx --nologo` | **PASS** | `Failed: 0, Passed: 206, Skipped: 0, Total: 206` (178 anteriores + 21 unitarias + 7 de integración DI) |
| `dotnet test --filter FullyQualifiedName~MediatRValidationPipelineTests` | **PASS** | 7/7 · prueba el contenedor DI real: comportamiento genérico abierto, validadores descubiertos y handlers ejecutados |
| El mismo filtro antes de corregir el arnés | **FAIL** | `MediatR requires ILoggerFactory to be registered` → defecto del arnés, no del cableado |

`[CONFIRMADO]` Detalle completo de la tanda, cambios y cobertura acumulada (206) en
[SDD.mdverification-baseline.md :11](#sdd/00-bootstrap/verification-baselinemd-:11).

### Historial de Iteraciones (Línea Base)
## 7. Segunda iteración— suite de pruebas y corrección [T-003](#t-003)

Fecha: 2026-09-17.

| Comando | Resultado | Observaciones |
|---|---|---|
| `dotnet new xunit -o Zentric.Tests` | `PASS` | Proyecto xUnit 2.9.3, `Microsoft.NET.Test.Sdk` 17.14.1, `coverlet.collector` |
| `dotnet sln Zentric.slnx add Zentric.Tests/Zentric.Tests.csproj` | `PASS` | La solución pasa de 1 a 2 proyectos |
| `dotnet add ... reference Zentric.Domain` | `PASS` | Dependencia de prueba hacia el dominio (dirección correcta) |
| `dotnet test Zentric.slnx` (antes de la corrección) | `FAIL` | **3 fallos / 105**: 2 eran la regresión esperada de [H-08](#h-08) (`ReturnToAvalible_NonPositiveQuantity`) y 1 era una prueba defectuosa propia (setup con stock > 0 en `MarkAsDeleted_AlreadyDeleted`), corregida |
| `dotnet test Zentric.slnx` (después de la corrección) | `PASS` | `Failed: 0, Passed: 105, Skipped: 0, Total: 105` (~0,3 s) |

Estado rojo inicial (evidencia textual):

```text
[xUnit.net] InventoryTests.ReturnToAvalible_NonPositiveQuantity_ThrowsArgumentOutOfRangeException(quantity: -100) [FAIL]
  Assert.Throws() Failure: No exception was thrown
Failed!  - Failed: 3, Passed: 102, Skipped: 0, Total: 105
```

Estado final:

```text
Passed!  - Failed: 0, Passed: 105, Skipped: 0, Total: 105
```

### Cambio de producción aplicado

- Archivo: `Zentric.Domain/Inventories/Inventory.cs` (`ReturnToAvalible`).
- Cambio: guarda `quantity <= 0` → `ArgumentOutOfRangeException`.
- Justificación: [INV-01](Domain/06-business-rules.md) (invariante absoluta de no-negatividad), que sí existe en
  la especificación. Sin la guarda, `ReturnToAvalible(-100)` dejaba
  `AvailableQuantity = -100`. **No se inventó ninguna regla nueva: se hizo cumplir
  una `[CONFIRMADO]`.**

### Cobertura por artefacto

| Artefacto | Pruebas |
|---|---|
| `Inventory` | 24 |
| `Warehouse` | 22 |
| `User` | 22 |
| `Product` | 19 |
| `Money` | 14 |
| VOs `Email` / `FullName` | pendiente ([T-002b](#t-002b)) |

---

## 8. Tercera iteración — [ADR-0002](Adr/0002-clave-inventario-variantid.md): clave del inventario = `VariantId`

Fecha: 2026-09-17. Alcance: [T-010](#t-010) (`ProductVariant`), [T-004a](#t-004a)
(`Inventory.ProductId` → `Inventory.VariantId`) y [T-002c](#t-002c) (migración de pruebas).

| Comando | Resultado | Observaciones |
|---|---|---|
| `dotnet build Zentric.slnx` | `PASS` | `0 Warning(s)`, `0 Error(s)` |
| `dotnet test Zentric.slnx` | `PASS` | `Failed: 0, Passed: 150, Skipped: 0, Total: 150` (~0,4 s) |

```text
Build succeeded.
    0 Warning(s)
    0 Error(s)

Passed!  - Failed: 0, Passed: 150, Skipped: 0, Total: 150
```

### Cambios de producción aplicados

| Archivo | Cambio |
|---|---|
| `Zentric.Domain/Products/ProductVariant.cs` | **Nuevo.** Entidad hija; `Id` = `VariantId` (SKU); atributos vía `VariantAttribute`; ciclo de vida Activate/Deactivate/Delete/Restore; atributos expuestos como solo lectura |
| `Zentric.Domain/Products/ValueObjects/VariantAttribute.cs` | **Nuevo.** VO inmutable nombre/valor con `Equals`/`GetHashCode`/`ToString` |
| `Zentric.Domain/Products/Product.cs` | Colección `_variants` expuesta como solo lectura + `AddVariant()` (normaliza SKU a mayúsculas, prohíbe SKU duplicado dentro del producto, prohíbe en producto borrado) y `RemoveVariant()` |
| `Zentric.Domain/Inventories/Inventory.cs` | `ProductId` → `VariantId` (propiedad, parámetro, guarda `ArgumentException("VariantId is required.")` y asignación) |

### Cambios de pruebas

| Archivo | Cambio |
|---|---|
| `Zentric.Tests/Inventories/InventoryTests.cs` | Migrado de `ProductId` a `VariantId` (incluye `Constructor_EmptyVariantId_ThrowsArgumentException` y el nombre del parámetro `"variantId"`) |
| `Zentric.Tests/Products/ProductVariantTests.cs` | **Nuevo.** 24 casos |
| `Zentric.Tests/Products/VariantAttributeTests.cs` | **Nuevo.** 10 casos |
| `Zentric.Tests/Products/ProductTests.cs` | +11 casos de `AddVariant`/`RemoveVariant` |

### Incidencia de proceso (Fase 7 del ciclo SDD)

Durante la migración usé `-replace` de PowerShell, que es **case-insensitive**, y
convirtió el literal `"productId"` en `"VariantId"`, haciendo fallar la aserción
de `ParamName` contra el `nameof(variantId)` real. Se detectó al inspeccionar el
archivo y se corrigió antes de ejecutar la suite; se clasifica como **prueba
defectuosa**, no como defecto de producción.

### Cobertura acumulada

| Artefacto | Casos |
|---|---|
| `Inventory` | 24 |
| `Warehouse` | 22 |
| `User` | 22 |
| `Product` (incl. variantes) | 30 |
| `ProductVariant` | 24 |
| `VariantAttribute` | 10 |
| `Money` | 14 |
| VOs `Email` / `FullName` y `Buyer` | pendiente ([T-002b](#t-002b)) |

---

## 9. Cuarta iteración — [ADR-0003](Adr/0003-variante-obligatoria-productos-fisicos.md): variante obligatoria en físicos ([Q-10](#q-10) = [C3](Adr/0003-variante-obligatoria-productos-fisicos.md))

Fecha: 2026-09-17. Alcance: [T-010c](#t-010c) (hacer cumplir [CAT-03](Domain/06-business-rules.md) en `Product`).

| Comando | Resultado | Observaciones |
|---|---|---|
| `dotnet build Zentric.slnx` | `PASS` | `0 Warning(s)`, `0 Error(s)` |
| `dotnet test Zentric.slnx` | `PASS` | `Failed: 0, Passed: 164, Skipped: 0, Total: 164` (~0,2 s) |

```text
Build succeeded.
    0 Warning(s)
    0 Error(s)

Passed!  - Failed: 0, Passed: 164, Skipped: 0, Total: 164
```

### Cambios de producción aplicados

| Archivo | Cambio |
|---|---|
| `Zentric.Domain/Products/Product.cs` | Nueva regla [CAT-03](Domain/06-business-rules.md): el constructor exige ≥1 variante si `Type == Physical` (`ArgumentException`, paramName `variants`); las semillas se crean vía `AddVariant` (heredan unicidad de SKU); `UpdateType(Physical)` lanza `InvalidOperationException` sin variante; `RemoveVariant` no puede dejar un `Physical` sin variantes; `CanBeSold` exige variante vendible (activa y no eliminada) en `Physical`. Nuevas propiedades `HasVariant` / `HasSellableVariant` / `CanBeSold` |
| `Zentric.Tests/Products/ProductTests.cs` | +14 casos C3 (constructor físico/digital, semillas duplicadas, `UpdateType`, `RemoveVariant`, `HasVariant` con variante eliminada, `CanBeSold` con variante desactivada/eliminada); el helper `CreateProduct()` por defecto ahora crea `Digital` para no romper los casos base |

### Cobertura acumulada

| Artefacto | Casos |
|---|---|
| `Inventory` | 24 |
| `Warehouse` | 22 |
| `User` | 22 |
| `Product` (incl. variantes + [CAT-03](Domain/06-business-rules.md)) | 44 |
| `ProductVariant` | 24 |
| `VariantAttribute` | 10 |
| `Money` | 14 |
| VOs `Email` / `FullName` y `Buyer` | pendiente ([T-002b](#t-002b)) |

---

## 10. Quinta iteración — auditoría de la tanda no registrada (2026-09-18)

Fecha: 2026-09-18. Alcance: **solo lectura + documentación**. **Cero archivos de código modificados**
(verificable con `git status --short`: únicamente `.md` y `scripts/sync-skill.ps1`).

| Comando | Resultado | Observaciones |
|---|---|---|
| `dotnet build Zentric.slnx --nologo` | `PASS` | `Build succeeded. 0 Warning(s) 0 Error(s)`; compilan **5** proyectos (Domain, Application, Infrastructure, Api, Tests) |
| `dotnet test Zentric.slnx --nologo` | `PASS` | `Failed: 0, Passed: 178, Skipped: 0, Total: 178` (~479 ms) |
| `Get-FileHash` de `.agents/skills/.../SKILL.md` vs copia instalada | `FAIL` → luego `PASS` | Repo = `24E7ED4F…` (v6.0.0) vs instalada = `EB4E7166…` (v3.1.0); sincronizada con `sync-skill.ps1` (corregido previamente) |
| `git diff --numstat [ZENTRIC.md](/ZENTRIC.md)` | `PASS` | 38 adiciones / 1 borrado (línea separadora) → **la Ley conserva el texto original** |

### Salida textual relevante

```text
Build succeeded.
    0 Warning(s)
    0 Error(s)

Passed!  - Failed:     0, Passed:   178, Skipped:     0, Total:   178 - Zentric.Tests.dll (net10.0)
```

### Delta de pruebas respecto a [:9](/ZENTRIC.md#dominio-9-gestin-de-devoluciones-y-reembolsos) (14 nuevas desde la última línea base)

| Suite nueva | Casos | Cubre |
|---|---|---|
| `Zentric.Tests/Orders/CustomerOrderTests.cs` | 6 | `CustomerOrder`: nacimiento en `Cart`, `AddItem`, ciclo completo, pedido entregado inmutable |
| `Zentric.Tests/Logistics/FulfillmentOrderTests.cs` | 3 | `FulfillmentOrder`: nacimiento en `Packed`, cancelación por quiebre |
| `Zentric.Tests/Billing/InvoiceTests.cs` | 2 | `Invoice`: maestra y detalle de vendedor |
| `Zentric.Tests/Returns/ReturnRequestTests.cs` | 3 | `ReturnRequest`: prohibición de digitales, doble aprobación |
| **Total** | **14** | 164 + 14 = **178** ✔ coherente con el resultado del comando |

### Cobertura acumulada (2026-09-18)

| Artefacto | Casos |
|---|---|
| `Inventory` | 24 |
| `Warehouse` | 22 |
| `User` | 22 |
| `Product` (incl. variantes + [CAT-03](Domain/06-business-rules.md)) | 44 |
| `ProductVariant` | 24 |
| `VariantAttribute` | 10 |
| `Money` | 14 |
| `CustomerOrder` (nuevo) | 6 |
| `FulfillmentOrder` (nuevo) | 3 |
| `Invoice` (nuevo) | 2 |
| `ReturnRequest` (nuevo) | 3 |
| VOs `Email` / `FullName` y `Buyer` | pendiente ([T-002b](#t-002b)) |

### Validaciones NO ejecutadas (honestidad de evidencia)

- Migraciones `InitialCreate` / `CompleteSchema` **nunca aplicadas** contra PostgreSQL; sin pruebas de integración.
- La API **no se levantó**: 0 peticiones HTTP ejecutadas contra los 3 endpoints.
- Mapeo EF en runtime sin validar (`OwnsMany`, `OwnsOne`, `HasConversion` de `Email`).
- Sin `.editorconfig`, sin analizadores, sin CI: el "0 warnings" no demuestra ausencia de hallazgos.

---

## 11. Sexta iteración — SPEC-007: validación de entrada, RFC 7807 e higiene (2026-09-18)

Alcance: cerrar **[H-09](#h-09)** (`try-catch` genérico prohibido en Application), **[H-11](#h-11)** (sin
`AddProblemDetails()` ni validadores FluentValidation) y **[H-12](#h-12)** (`Class1.cs` vacíos), los tres
incumplimientos literales de [AGENTS.md](#agentsmd). **No** se tocó nada que dependa de [Q-13](#q-13)/[Q-03](#q-03)/[Q-04](#q-04).

| Comando | Resultado | Observaciones |
|---|---|---|
| `dotnet restore Zentric.slnx` | `PASS` | "All projects are up-to-date for restore." |
| `dotnet build Zentric.slnx --nologo` | `PASS` | `Build succeeded. 0 Warning(s) 0 Error(s)` |
| `dotnet test Zentric.slnx --nologo` | `PASS` | `Failed: 0, Passed: 206, Skipped: 0, Total: 206` (~160 ms) |
| `dotnet test --filter FullyQualifiedName~MediatRValidationPipelineTests` | `PASS` | 7/7 · prueba el DI real (comportamiento genérico abierto + validadores descubiertos + handlers) |
| El mismo filtro **antes** del arreglo | `FAIL` (7) | `InvalidOperationException: MediatR requires ILoggerFactory to be registered. Call services.AddLogging() before services.AddMediatR()` → era el arnés, no el cableado |

### Salida textual relevante

```text
Build succeeded.
    0 Warning(s)
    0 Error(s)

Passed!  - Failed:     0, Passed:   206, Skipped:     0, Total:   206 - Zentric.Tests.dll (net10.0)
```

### Cambios de producción aplicados

| Archivo | Cambio |
|---|---|
| `Zentric.Application/Common/Behaviors/ValidationBehavior.cs` | **Nuevo.** `IPipelineBehavior<TRequest, TResponse> where TResponse : Result`: ejecuta los validadores y devuelve `Result.Failure` (sin excepción) cuando la entrada es inválida; construye el fallo del tipo concreto (`Result` o `Result<T>`) |
| `Zentric.Application/Orders/Validators/CreateCartCommandValidator.cs` | **Nuevo.** `BuyerId` obligatorio |
| `Zentric.Application/Orders/Validators/AddOrderItemCommandValidator.cs` | **Nuevo.** `OrderId`/`VariantId` obligatorios, `Quantity > 0`, `UnitPrice >= 0`, `Currency` ISO de 3 caracteres (reglas espejo de `OrderItem` y `Money`) |
| `Zentric.Application/Logistics/Validators/CreateFulfillmentOrderCommandValidator.cs` | **Nuevo.** `CustomerOrderId` y `VendorId` obligatorios |
| `Zentric.Application/Orders/Commands/AddOrderItemCommand.cs` | **[H-09](#h-09):** eliminado el `catch (Exception)`; precondición de negocio explícita (`Status != Cart` → `Result.Failure`). Las excepciones catastróficas ya no se silencian |
| `Zentric.Api/Program.cs` | **[H-11](#h-11):** `AddProblemDetails()`, `AddValidatorsFromAssemblyContaining<CreateCartCommand>()`, `AddOpenBehavior(typeof(ValidationBehavior<,>))` y `UseExceptionHandler()` (sin endpoint inexistente) |
| `Zentric.Application/Class1.cs`, `Zentric.Infrastructure/Class1.cs` | **[H-12](#h-12):** eliminados (verificado antes: `Class1` no estaba referenciado en ningún archivo) |

### Cambios de pruebas

| Archivo | Casos | Cubre |
|---|---|---|
| `Zentric.Tests/UseCases/CreateCartCommandValidatorTests.cs` | 2 | `CreateCartCommandValidator` |
| `Zentric.Tests/UseCases/AddOrderItemCommandValidatorTests.cs` | 12 | `AddOrderItemCommandValidator`, con teorías para cantidades, precios y monedas |
| `Zentric.Tests/UseCases/CreateFulfillmentOrderCommandValidatorTests.cs` | 3 | `CreateFulfillmentOrderCommandValidator` |
| `Zentric.Tests/UseCases/ValidationBehaviorTests.cs` | 4 | Comportamiento del pipeline: rama `Result` y rama `Result<T>`, sin invocar el handler cuando falla |
| `Zentric.Tests/UseCases/MediatRValidationPipelineTests.cs` | 7 | **Integración DI real** (mismo registro que `Program.cs`) con repositorios falsos: incluye la regresión de [H-09](#h-09) |
| `Zentric.Tests/Zentric.Tests.csproj` | — | Referencia a `Zentric.Application` + `Microsoft.Extensions.DependencyInjection` y `.Logging` 10.0.12 |

### Incidencia de proceso (Fase 7 del ciclo SDD)

`[CONFIRMADO]` Las 7 pruebas de integración fallaron en el primer intento: MediatR 14 **exige
`ILoggerFactory`** registrado antes de `AddMediatR()`. Se verificó que el stack trace provenía de
`MediatRServiceCollectionExtensions.CheckLicense` (no del pipeline) y se corrigió el arnés con
`services.AddLogging()`. En `Zentric.Api` esa dependencia la aporta `WebApplicationBuilder`.
Se clasifica como **defecto del arnés de pruebas**, no de producción.

### Cobertura acumulada (2026-09-18, tras SPEC-007)

| Artefacto | Casos |
|---|---|
| Dominio (`Inventory`, `Warehouse`, `User`, `Product`, `ProductVariant`, `VariantAttribute`, `Money`, `CustomerOrder`, `FulfillmentOrder`, `Invoice`, `ReturnRequest`) | 178 |
| Application (validadores + comportamiento del pipeline) | 21 |
| Integración DI (MediatR + FluentValidation + handlers con repositorios falsos) | 7 |
| **Total** | **206** |

`[PENDIENTE]` Sigue sin cobertura: los VOs `Email`/`FullName` y el agregado `Buyer` ([T-002b](#t-002b)), el
endpoint HTTP real (no se levantó la API) y el mapeo EF contra PostgreSQL.

### Matriz de conformidad Spec ↔ Código
Leyenda: **OK** conforme · **PARCIAL** existe pero incompleto · **DESVIADO** existe con nombre/formato distinto · **FALTA** no implementado · **CONTRA** contradice la especificación.

## 1. Identity(usuarios)

| Requisito de spec | Fuente | Código | Estado |
|---|---|---|---|
| AR `User` con `Id`, `FullName`, `Email`, `Role`, `Status` | [01-models.md :1](Domain/01-models.md#1-bounded-context-identity-access-usuarios) | `User.cs[:7](Domain/01-models.md#7-bounded-context-billing-facturacin)-43` | OK |
| `IdentityDocument` obligatorio y único | [01-models.md :1](Domain/01-models.md#1-bounded-context-identity-access-usuarios), `02-aggregates:9`, [ZENTRIC.md](/ZENTRIC.md) Dom.1 | — | **FALTA** |
| Correo único en todo el sistema | [01-models.md :1](Domain/01-models.md#1-bounded-context-identity-access-usuarios), [ZENTRIC.md](/ZENTRIC.md) 11 | `IUserRepository.GetByEmailAsync` es solo lectura | **PARCIAL** |
| Un solo rol por usuario | [ZENTRIC.md](/ZENTRIC.md) RG-02 | `User.Role` único | OK |
| `Lock()` / `Unlock()` / `ChangeRole()` | [01-models.md :1](Domain/01-models.md#1-bounded-context-identity-access-usuarios) | `Block()` / `Activate()` / `UpdateRole()` | **DESVIADO** |
| `Block()` dispara evento de suspensión en cascada | `02-aggregates:10`, [invariante 6](Domain/04-invariants-and-rules.md) | sin eventos de dominio | **FALTA** |
| Suspensión en cascada de productos del vendedor | [invariante 6](Domain/04-invariants-and-rules.md) (`04-invariants`) | — | **FALTA** |
| `UserStatus`: Activo, Bloqueado | [ZENTRIC.md](/ZENTRIC.md) Dom.1 | `Active, Blocked, Deleted` | OK + extra |

## 2. Buyer

| Requisito | Fuente | Código | Estado |
|---|---|---|---|
| Dirección principal obligatoria | [ZENTRIC.md](/ZENTRIC.md) Dom.2 | `Buyer.cs:26-29` | OK |
| Direcciones adicionales (opcional, añadir/quitar) | [ZENTRIC.md](/ZENTRIC.md) Dom.2 | `Buyer.cs:45-70` | OK |
| Estado comercial | [ZENTRIC.md](/ZENTRIC.md) Dom.2 | `IsActiveForCommerce` | OK |
| Agregado `Buyer` declarado en la spec de dominio | [01-models.md](Domain/01-models.md), `02-aggregates` | existe en código | **FALTA en spec** |
| Sin almacenamiento de medios de pago | [invariante 9](Domain/04-invariants-and-rules.md) (`04-invariants`) | `PaymentTokens` (`Buyer.cs:13,112-139`) | **CONTRA** |
| Dirección como VO `Address` | [02-value-objects.md](Domain/02-value-objects.md) | `string` (`Buyer.cs:10-11`) | **DESVIADO** |

## 3. Catalog (productos)

| Requisito | Fuente | Código | Estado |
|---|---|---|---|
| AR `Product` con `SellerId`, `Price`, `Type` | [01-models.md :2](Domain/01-models.md#2-bounded-context-catalog-catalogo) | `Product.cs` | OK |
| Propiedad `Title` | [01-models.md :2](Domain/01-models.md#2-bounded-context-catalog-catalogo) | `Name` | **DESVIADO** |
| `ProductStatus` = Published / Suspended / Discontinued | [02-value-objects.md](Domain/02-value-objects.md), [ZENTRIC.md](/ZENTRIC.md) Dom.5 | `bool IsActive` + `DeletedAt` | **DESVIADO** |
| Nace publicado sin aprobación ([CAT-01](Domain/06-business-rules.md)) | [invariante 5](Domain/04-invariants-and-rules.md), `06-business-rules` [CAT-01](Domain/06-business-rules.md) | `Product.cs:59` | OK |
| `Suspend()` reactivo | `02-aggregates:17` | existe | OK |
| `Discontinue()` | `01-models:22` | — | **FALTA** |
| `ProductVariant` (SKU) con `VariantId` | `02-aggregates:18-19`, `03-value-objects:15` | `ProductVariant.cs` + `Product.AddVariant()` | **OK** ([ADR-0002](Adr/0002-clave-inventario-variantid.md)) |
| Variante obligatoria solo en físicos ([CAT-03](Domain/06-business-rules.md)) | [06-business-rules.md](Domain/06-business-rules.md) [CAT-03](Domain/06-business-rules.md), [ADR-0003](Adr/0003-variante-obligatoria-productos-fisicos.md) ([Q-10](#q-10) = [C3](Adr/0003-variante-obligatoria-productos-fisicos.md)) | constructor + `UpdateType` + `RemoveVariant` + `CanBeSold` en `Product.cs` | **OK** (sub-decisiones [Q-12](#q-12) `[PROPUESTO]`) |
| Atributos de variante (Talla/Color) | `02-aggregates:18` | `VariantAttribute` (nombre + valor) | **OK** `[PROPUESTO]` ([Q-11](#q-11)) |
| SKU único dentro del producto | consecuencia de [ADR-0002](Adr/0002-clave-inventario-variantid.md) / [INV-03](Domain/06-business-rules.md) | `Product.AddVariant()` rechaza SKU duplicado | **OK** (decisión, unicidad global pendiente en persistencia) |
| `UpdatePrice(Money)` | `01-models:22` | `Product.cs:92` | OK |
| Soft delete / restaurar producto | no especificado | `Delete()` / `Restore()` | **PENDIENTE** documentar |

## 4. Inventory y Warehouse

| Requisito | Fuente | Código | Estado |
|---|---|---|---|
| AR `Warehouse` con `Type` y dueño opcional | `01-models` [Warehouse.cs:3](../Zentric.Domain/Warehouses/Warehouse.cs#L3) | `Warehouse.cs` | OK |
| `WarehouseType` = Marketplace / **Vendor** | `02-value-objects` | `Marketplace` / `Seller` | **DESVIADO** |
| `Location` como VO `Address` | `01-models:28` | `string` | **DESVIADO** |
| `OwnerId` / `OwnerSellerId` | `01-models:28`, `02-aggregates:25` | `SellerId` | **DESVIADO** |
| AR de stock con `WarehouseId` y cantidades | `01-models` [Inventory.cs:3](../Zentric.Domain/Inventories/Inventory.cs#L3) | `Inventory.cs` | OK |
| Nombre `InventoryItem` | `01-models:31` | `Inventory` | **DESVIADO** |
| `AvailableQuantity` (ortografía) | toda la spec | `AvalibleQuantity` | **DESVIADO (typo)** |
| Clave de stock = `VariantId` (SKU) | `02-aggregates:29`, `03-value-objects:15`, [INV-03](Domain/06-business-rules.md) | `Inventory.VariantId` | **OK** ([ADR-0002](Adr/0002-clave-inventario-variantid.md)) |
| Clave de stock = `(VariantId, WarehouseId)` | [INV-03](Domain/06-business-rules.md) ([ADR-0002](Adr/0002-clave-inventario-variantid.md)) | `VariantId` + `WarehouseId` | **OK** |
| `Reserve(qty)` | `01-models:35` | `ReserveStock(qty)` | DESVIADO (naming) |
| `Release(qty)` | `01-models:35` | `ReturnToAvalible(qty)` | DESVIADO (naming) |
| `Deduct(qty)` | `01-models:35` | `DispatchStock(qty)` (con defecto H-01) | PARCIAL |
| `Adjust(qty)` para devoluciones | `01-models:35`, servicio de devoluciones | `UpdateQuantities` / `AddStock` / `ReciveReturnedStock` | DESVIADO |
| `ManualAdjust(qty, UserId, Role)` con regla dura | `02-aggregates:34`, [invariante 4](Domain/04-invariants-and-rules.md) | ningún método recibe `Role` | **FALTA** |
| `AvailableQuantity` nunca negativo | [INV-01](Domain/06-business-rules.md), [invariante 1](Domain/04-invariants-and-rules.md) | protegido (`Inventory.cs:33,60,99,116,132`) | OK |
| Cantidad dañada no reservable | [ZENTRIC.md](/ZENTRIC.md) 11 | `DamagedQuantity` existe; no bloquea la reserva | **PARCIAL** |
| Movimientos: Ingreso, Reserva, Salida, Ajuste, Devolución | [ZENTRIC.md](/ZENTRIC.md) Dom.6 | 5 operaciones presentes | OK |

## 5. Ordering

| Requisito | Fuente | Código | Estado |
|---|---|---|---|
| AR `CustomerOrder` con `BuyerId`, `Items`, `TotalAmount`, `Status` | `01-models` [Buyer.cs[:4](/ZENTRIC.md#dominio-4-gestion-de-bodegas)](../Zentric.Domain/Buyers/Buyer.cs#L4) | `Orders/CustomerOrder.cs` | **PARCIAL** — `Items` (no `Lines`); `TotalAmount` con moneda "USD" **inventada** ([Buyer.cs:24](../Zentric.Domain/Buyers/Buyer.cs#L24)) |
| `OrderLine` (variante, cantidad, precio unitario, total, tipo) | `01-models:43`, `02-aggregates:41` | `Orders/Entities/OrderItem.cs` | **DESVIADO** (nombre: `OrderItem`) |
| `FlatShippingFee` | `02-aggregates:40`, [invariante 7](Domain/04-invariants-and-rules.md) | — | **FALTA** |
| Estados `Cart`, `PendingPayment`, `Paid`, `PartiallyDelivered`, `Completed`, `Cancelled` | `02-value-objects:45-51` | `Orders/Enums/OrderStatus.cs` | **PARCIAL** — la Ley (Dom.7) fija Cart, PendingPayment, Paid, Dispatched, Delivered; faltan `PartiallyDelivered`, `Completed`, `Cancelled` |
| `ConfirmPayment()`, `CancelEarly()`, `ApplyPartialRefund()` | `02-aggregates:42-45` | `MarkAsPaid()`, `Dispatch()`, `Deliver()` | **DESVIADO** — sin cancelación ni reembolso parcial |
| `PaymentReceipt` (simulación de pasarela) | [invariante 9](Domain/04-invariants-and-rules.md) | — | **FALTA** |
| Timeout de reserva 15 min ([PED-01](Domain/06-business-rules.md)) | `06-business-rules` [PED-01](Domain/06-business-rules.md), `07-lifecycle` | — | **FALTA** — y la reserva preventiva tampoco existe (**H-10**) |
| Pedido entregado **no modificable** | [ZENTRIC.md](/ZENTRIC.md) (Validaciones Críticas) | `CustomerOrder.EnsureNotDelivered()` ([CustomerOrder.cs:152-159](../Zentric.Domain/Orders/CustomerOrder.cs#L152-L159)) | **OK** (con prueba) |

## 6. Fulfillment, devoluciones, eventos y puertos

| Requisito | Fuente | Código | Estado |
|---|---|---|---|
| AR `FulfillmentOrder` + `Shipment` | `01-models` [FulfillmentOrder.cs[:5](/ZENTRIC.md#dominio-5-gestion-del-catalogo)](../Zentric.Domain/Logistics/FulfillmentOrder.cs#L5) | `Logistics/FulfillmentOrder.cs`, `Entities/Shipment.cs` | **PARCIAL** — agregados sin servicio ([T-014](#t-014) parcial) |
| Estados de despacho | `02-value-objects:53-58`, `03-value-objects:36-41`, [Ley [ADD-001](#ley-[add-001)](../SDD.md#addendum---dictado-por-owner-add-001-dominio-8-logistica-y-despachos-fulfillment) | `FulfillmentStatus` = Packed, Dispatched, Delivered, CancelledNoStock | **CONTRADICCIÓN** — 4 valores vs 5; **falta `PendingPack`** y el orden de la Ley (**[C-08](#c-08)**) |
| `CancelDueToGhostStock`, `RequestReturn` (no digital) | `02-aggregates:53-54`, [Ley [ADD-001](#ley-[add-001)](../SDD.md#addendum---dictado-por-owner-add-001-dominio-8-logistica-y-despachos-fulfillment)/002 | `CancelDueToNoStock()`, `ReturnRequest` (rechaza `Digital`) | **PARCIAL** — nombres desviados; sin la "devolución obligatoria" que exige [ADD-001](#add-001) |
| `ReturnStatus` (Requested → Refunded / Rejected) | `03-value-objects:43-48`, Ley Dom.10 | `Returns/Enums/ReturnStatus.cs` | **OK** |
| Retorno a stock con etiqueta "Usado" tras devolución aprobada | [Ley [ADD-002](#ley-[add-002)](../SDD.md#addendum---dictado-por-owner-add-002-dominio-9-devoluciones-y-reembolsos) | `Inventory.ReturnToUsedStock()` existe pero **nadie lo invoca** (`// TODO` en `ReturnRequest.cs:63`) | **CONTRADICCIÓN** (**H-14**) |
| Facturación: Maestra · Detalle Zentric · Vendedor | [Ley [ADD-003](#ley-[add-003)](../SDD.md#addendum---dictado-por-owner-add-003-dominio-10-facturacion-y-pagos) | `Billing/Invoice.cs` (`CreateMaster`, `CreateVendorDetail`) | **PARCIAL** — **falta "Detalle Zentric"** (**[C-08](#c-08)**) |
| 7 eventos de dominio | [04-domain-events.md](Domain/04-domain-events.md) | — | **FALTA** |
| Puerto `IDomainEventDispatcher` | [05-ports.md :2](Domain/05-ports.md#2-puertos-de-mensajeria-event-bus) | — | **FALTA** |
| Puertos `IProductRepository`, `IWarehouseRepository`, `IInventoryRepository`, `IReturnRequestRepository`, `IInvoiceRepository`, `ICustomerOrderRepository`, `IFulfillmentOrderRepository` | [05-ports.md :1](Domain/05-ports.md#1-puertos-de-repositorios-persistencia), `SDD/Application` [IFulfillmentOrderRepository.cs[:2](Domain/05-ports.md#2-puertos-de-mensajeria-event-bus)](../Zentric.Application/Logistics/Ports/IFulfillmentOrderRepository.cs#L2) | 2 de 7 (`ICustomerOrderRepository`, `IFulfillmentOrderRepository`); `IUserRepository` sigue en Domain | **PARCIAL** |
| 4 servicios de dominio | [03-domain-services.md](Domain/03-domain-services.md), `services/*` | — | **FALTA** |
| IDs fuertemente tipados (`UserId`, `VariantId`, …) | `02-value-objects:23-25`, `03-value-objects:13-15` | `Guid` plano | **FALTA** |
| VO `Address` compartido (bodega y despacho) | `02-value-objects:13-16` | — | **FALTA** |
| Contrato `Result<T>` / `Result<ReservationDetails>` en servicios | [services/inventory-reservation-service.md :3](Domain/services/inventory-reservation-service.md#3-entradas-y-salidas) | `Zentric.Application/Common/Models/Result.cs` | **PARCIAL** — existe el tipo, no se usa en servicios de dominio (no hay servicios) |

## 7. Defectos detectados en el código implementado

| ID | Evidencia | Riesgo |
|---|---|---|
| H-01 | `Inventory.cs:109-123`: `DispatchStock` valida `AvalibleQuantity` y decrementa `ReservedQuantity`; puede dejar el reservado negativo | Rompe el balance de stock — severidad **alta** |
| H-02 | `Inventory.cs:58-79`: `UpdateQuantities` sobrescribe los tres contadores saltándose las operaciones de negocio | Setter anémico: anula invariantes — severidad **alta** |
| H-03 | `Buyer.cs:13,112-139`: `PaymentTokens` contradice la [invariante 9](Domain/04-invariants-and-rules.md) | Regla de negocio violada por diseño — **media** |
| H-04 | `Warehouse.cs:206-209` y `Inventory.cs:167`: `MarkAsDeleted()` duplica `Delete()` | API redundante y lenguaje ambiguo — **baja** |
| H-05 | `User.cs:29-43`: `IdentityDocument` ausente pese a ser obligatorio y único | No se puede registrar un usuario conforme a spec — **alta** |
| H-06 | `DateTime.UtcNow` usado directamente en entidades de dominio: **46 usos en 5 archivos** (`Buyer` 8, `Inventory` 9, `Product` 10, `User` 10, `Warehouse` 9) | Reglas temporales (timeout 15 min) y pruebas no deterministas — **media** |
| H-07 | `IUserRepository.cs:10-11`: unicidad de correo marcada como TODO | Invariante no aplicada en ningún punto — **alta** |
| ~~H-08~~ | ~~`Inventory.cs:142-152`: `ReturnToAvalible` no valida `quantity > 0`~~ | **CORREGIDO en [T-003](#t-003)** (ver [verification-baseline.md :7](#verification-baselinemd-:7)). Cerró una violación real de [INV-01](Domain/06-business-rules.md): `ReturnToAvalible(-100)` dejaba el disponible en −100 |
| ~~H-09~~ | ~~`AddOrderItemCommand.cs:35-38`: `catch (Exception ex) { return Result.Failure(ex.Message); }`~~ | **CORREGIDO en SPEC-007 (2026-09-18):** precondición explícita (`Status != Cart`) → `Result.Failure`; sin `try-catch`. Regresión en `MediatRValidationPipelineTests` |
| H-10 | [SDD/Application/01-use-cases-and-ports.md :15](Application/01-use-cases-and-ports.md)` afirma que `AddOrderItemCommand` valida stock con `IInventoryRepository`: el puerto **no existe** y no se valida stock; [PED-01](Domain/06-business-rules.md) sin implementar | Especificación que miente sobre el código + regla de la Ley incumplida — **alta** |
| ~~H-11~~ | ~~`Program.cs:35` `UseExceptionHandler("/error")` sin endpoint ni `AddProblemDetails()`; FluentValidation referenciado con **0 validadores** y **0 pipeline**~~ | **CORREGIDO en SPEC-007 (2026-09-18):** `AddProblemDetails()` + `UseExceptionHandler()`; 3 validadores + `ValidationBehavior<,>` con `AddOpenBehavior`, verificados en contenedor DI real (`[PENDIENTE]` HTTP real) |
| ~~H-12~~ | ~~`Zentric.Application/Class1.cs` y `Zentric.Infrastructure/Class1.cs` vacíos~~ | **CORREGIDO en SPEC-007 (2026-09-18):** eliminados (no estaban referenciados) |
| H-13 | `ZentricDbContext.cs` mapea directamente los agregados de dominio | Contradice [AGENTS.md :4.2](#agentsmd-:42) ("aislamiento estricto de las entidades EF Core"); desviación sin ADR — **media** |
| H-14 | `FulfillmentOrder.cs:75` y `ReturnRequest.cs:63` con `// TODO` de eventos: `ReturnToUsedStock` **nunca se invoca** | La [Ley [ADD-002](#ley-[add-002)](../SDD.md#addendum---dictado-por-owner-add-002-dominio-9-devoluciones-y-reembolsos) (stock "Usado") no se cumple en runtime — **alta** |

## 8. Totales

> **Reconteo 2026-09-18** tras auditar la tanda no registrada (secciones [:4](#:4), [:5](#:5) y [:6](#:6)). La aritmética
> cierra: 50 filas antes + 2 filas nuevas = **52** filas (15+12+13+10+2).

| Métrica | [:8](#:8) anterior (2026-09-17) | Delta del 2026-09-18 | Ahora |
|---|---|---|---|
| Requisitos conformes | 13 | +2 (pedido entregado inmutable; `ReturnStatus`) | **15** |
| Parciales | 6 | +6 (`CustomerOrder`, estados del pedido, `FulfillmentOrder`+`Shipment`, `CancelDueToGhostStock`, puertos, `Result<T>`) | **12** |
| Desviados | 10 | +3 (`OrderItem`, estados de despacho, métodos de pago/cancelación) | **13** |
| Faltantes | 20 | −10 (dejaron de faltar 10 requisitos de [:5](#:5) y [:6](#:6)) | **10** |
| Contradicciones directas | 1 (medios de pago, H-03 → [Q-08](#q-08)) | +1 (retorno a stock "Usado" no se ejecuta, H-14) | **2** |
| Hallazgos de defecto **abiertos** | 7 (H-01…H-07) | +6 (**H-09…H-14**) −3 (H-09, H-11 y H-12 corregidos en SPEC-007) | **10** |
| Hallazgos **corregidos** | 1 (H-08) | — | 1 |
| Pruebas automatizadas en verde | 164 | +14 (Orders 6, Logistics 3, Billing 2, Returns 3) | **178** |

- **Deuda declarada:** `VariantId` es `Guid` plano, no un `record struct` fuertemente tipado (G-07 / [T-004](#t-004)).
- `[RIESGO]` Los 13 hallazgos abiertos **no** están clasificados como tareas con verificación en el roadmap; H-09, H-10, H-11 y H-14 son incumplimientos directos de [AGENTS.md](#h-09) o de la Ley.

## 6. Riesgos, contradicciones y preguntas
> El detalle completo con evidencia vive en [SDD.md :6 (Riesgos)](#sdd/00-bootstrap/risks-and-gapsmd) y [SDD.md :6 (Preguntas)](#sdd/00-bootstrap/questions-for-ownermd). Aquí quedan los **IDs nuevos** de esta auditoría y los bloqueos vigentes.

### 6.1 Contradicciones abiertas

| ID | Contradicción | Evidencia | Bloquea |
|---|---|---|---|
| **[C-08](#c-08)** | **La Ley define `DOMINIO 8/9/10` dos veces con contenidos distintos** (9 y 10 intercambian significado entre el bloque base y el ADDENDUM; el 8 tiene 5 estados vs 2). El código tomó una interpretación que el Owner no dictó: falta `PendingPack`, falta "Detalle Zentric" y la devolución aprobada no devuelve stock | [ZENTRIC.md](/ZENTRIC.md) líneas 254-270 vs 320-334 · `FulfillmentStatus.cs` · `InvoiceType.cs` · `ReturnRequest.cs` | Fases 3b/4 → requiere **[Q-13](#q-13)** |
| **[C-09](#c-09)** | Skill: el repo está en **v6.0.0** monolítico (sin `references/`) y la copia instalada seguía en **v3.1.0**; [AGENTS.md](#agentsmd) citaba la v3.0.0 con `references/10-zentric-overlay.md`, inexistente | hashes distintos · [AGENTS.md :0.0](#agentsmd-:00) | Gobernanza (**corregido** en esta tanda) |
| **[C-10](#c-10)** | Adiciones a la Biblia **sin rótulo** `[ADDENDUM - DICTADO POR OWNER]` (bloque previo al marcador) | `git diff` de [ZENTRIC.md](/ZENTRIC.md) | Trazabilidad de la Ley |
| [C-03](#c-03) | `Cancelled` vs `CancelledByStockBreak` | [02-value-objects.md :58](Domain/02-value-objects.md)` vs [03-value-objects.md :41](Domain/03-value-objects.md)` | Fulfillment → **[Q-03](#q-03)** |
| [C-04](#c-04) | ¿`Cart` es estado de `CustomerOrder`? | [02-value-objects.md :46](Domain/02-value-objects.md)` vs [03-value-objects.md :29](Domain/03-value-objects.md)` | Ordering → **[Q-04](#q-04)** |
| [C-05](#c-05) | `Vendor` (spec) vs `Seller` (código) | [02-value-objects.md :62](Domain/02-value-objects.md)` vs `WarehouseType.cs:6` | Renombrado → [Q-05](#q-05) |
| [C-06](#c-06) | [SDD/Domain/Software-arquitecture.md](Domain/Software-arquitecture.md) = **0 bytes**; [AGENTS.md :0.1](#agentsmd-:01) exige documentos que no existían | tamaño de archivo | Gobernanza documental |
| [C-07](#c-07) | Numeración duplicada en `SDD/Domain/` (dos `01-*`, `02-*`, `03-*`, `04-*`) | estructura de carpetas | Navegabilidad → [Q-06](#q-06) |

### 6.2 Hallazgos nuevos de código

| ID | Hallazgo | Evidencia | Severidad |
|---|---|---|---|
| ~~**[H-09](#h-09)**~~ | ~~`AddOrderItemCommandHandler` captura `catch (Exception ex)` y devuelve `Result.Failure(ex.Message)` → **prohibido por [AGENTS.md:3.2](#agentsmd:32)**~~ | **CERRADO en SPEC-007 (2026-09-18):** eliminado el `catch`; la precondición de negocio se informa con `Result.Failure` y las excepciones catastróficas ya no se silencian. Regresión cubierta por `Send_AddOrderItemCommandWhenOrderIsNotInCart_ReturnsBusinessFailureWithoutThrowing` | alta → **cerrada** |
| **[H-10](#h-10)** | `SDD/Application` [[AddOrderItemCommand.cs[:3](#[addorderitemcommandcs[:3)](../Zentric.Application/Orders/Commands/AddOrderItemCommand.cs#L3) afirma que `AddOrderItemCommand` "valida stock físico usando `IInventoryRepository`": **el puerto no existe y no se valida stock**; [PED-01](Domain/06-business-rules.md) (reserva preventiva + timeout 15 min) no está implementada | [SDD/Application/01-use-cases-and-ports.md [SDD/Application/01-use-cases-and-ports.md:15](Application/01-use-cases-and-ports.md) vs `AddOrderItemCommand.cs` | alta |
| ~~**[H-11](#h-11)**~~ | ~~Middleware "simplificado": `UseExceptionHandler("/error")` sin endpoint ni `AddProblemDetails()`; FluentValidation referenciado con **0 validadores** y **0 pipeline**~~ | **CERRADO en SPEC-007 (2026-09-18):** `AddProblemDetails()` + `UseExceptionHandler()` sin endpoint inexistente; 3 validadores FluentValidation y `ValidationBehavior<,>` registrado con `AddOpenBehavior`, verificado en un contenedor DI real | alta → **cerrada** |
| ~~**[H-12](#h-12)**~~ | ~~`Class1.cs` vacío en `Zentric.Application` y `Zentric.Infrastructure`~~ | **CERRADO en SPEC-007 (2026-09-18):** ambos archivos eliminados; antes se verificó que `Class1` no estaba referenciado en ningún archivo del repositorio | baja → **cerrada** |
| **[H-13](#h-13)** | `ZentricDbContext` mapea **directamente** los agregados de dominio; exige [AGENTS.md:4.2](#agentsmd:42) exige "aislamiento estricto de las entidades EF Core respecto a Application/Domain". Desviación sin ADR | `ZentricDbContext.cs` | media |
| **[H-14](#h-14)** | `FulfillmentOrder.CancelDueToNoStock()` y `ReturnRequest.ApproveByVendor()` contienen `// TODO` de eventos de dominio → el [ADD-002](#add-002) ("el producto vuelve al stock con etiqueta Usado") **no se ejecuta** | `FulfillmentOrder.cs:75` · `ReturnRequest.cs:63` | alta |

`[CONFIRMADO]` Los hallazgos previos **[H-01](#h-01) … [H-07](#h-07) siguen abiertos**; **[H-09](#h-09), [H-11](#h-11) y [H-12](#h-12) quedaron cerrados en SPEC-007** (2026-09-18). [H-01](#h-01) reverificado en el árbol actual: `Inventory.cs:121` valida `AvailableQuantity` y `[[Inventory.cs:126](../Zentric.Domain/Inventories/Inventory.cs#L126) ejecuta `ReservedQuantity -= quantity` → el reservado puede quedar negativo. Siguen abiertos [H-10](#h-10), [H-13](#h-13), [H-14](#h-14) y la tanda [H-01](#h-01)…[H-07](#h-07).

### 6.3 Preguntas al Owner

- **[Q-13](#q-13) (NUEVA, bloqueante)** — duplicación de `DOMINIO 8/9/10` en la Ley ([C-08](#c-08)). Ficha completa en [questions-for-owner.md](#questions-for-ownermd).
- **[Q-03](#q-03) y [Q-04](#q-04) reactivadas como bloqueantes:** la Fase 3b/4 se implementó **sin** respuesta del Owner ([T-011](#t-011) figuraba "bloqueada por [Q-04](#q-04)"; [T-014](#t-014) "bloqueada por [Q-03](#q-03)"). Se registra como **incumplimiento del freno de mano ([AGENTS.md:0.3](#agentsmd:03))**, no como decisión.
- Siguen abiertas: [Q-05](#q-05), [Q-06](#q-06), [Q-07](#q-07), [Q-08](#q-08), [Q-09](#q-09), [Q-11](#q-11) y **[Q-12](#q-12)** (4 sub-decisiones `[PROPUESTO]` de [ADR-0003](Adr/0003-variante-obligatoria-productos-fisicos.md) pendientes de confirmar).

### Fichas completas de Preguntas (Q-xx)
## [Q-01](#q-01-c-01-se-permite-fraccionar-la-reserva-entre-bodegas-resuelta-2026-09-17) ([C-01](#c-01)) — ¿Se permite fraccionar la reserva entre bodegas? · ✅ RESUELTA (2026-09-17)

**Decisión del owner: A3 híbrido.**

- **Regla base:** si una bodega individual cubre la cantidad total de la línea, la
  reserva se hace íntegramente en esa bodega (el envío no se divide).
- **Excepción (umbral exacto):** solo si **ninguna** bodega individual cubre la
  cantidad, se fracciona entre varias bodegas, de mayor a menor stock y con
  prioridad `Marketplace`.
- **Fallo:** si la suma de todas las bodegas no alcanza, la operación falla y se
  liberan las reservas parciales de esa misma transacción.
- **Logística:** cuando hubo fraccionamiento se crean múltiples `Shipment`.

Registrado en [SDD/Adr/0001-reserva-fragmentacion-contingencia.md](Adr/0001-reserva-fragmentacion-contingencia.md) y aplicado a
[SDD/Domain/04-invariants-and-rules.md](Domain/04-invariants-and-rules.md) ([invariante 3](Domain/04-invariants-and-rules.md)),
[SDD/Domain/06-business-rules.md](Domain/06-business-rules.md) ([INV-02](Domain/06-business-rules.md)) y
[SDD/Domain/services/inventory-reservation-service.md :4](Domain/services/inventory-reservation-service.md#4-flujo-logico-y-reglas-invariantes).

**Desbloquea:** [T-004](#t-004), [T-013](#t-013), [T-014](#t-014) (estas dos últimas quedan pendientes solo de [Q-02](#q-02-c-02-la-clave-del-inventario-es-el-producto-o-la-variante-sku-resuelta-2026-09-17)/[Q-03](#q-03-c-03-estado-de-cancelacion-de-despacho)).

### Contexto histórico de la contradicción

- **Fuente A** — [SDD/Domain/04-invariants-and-rules.md :8](Domain/04-invariants-and-rules.md)`: "Toda la cantidad
  solicitada de una Variante específica en una `OrderLine` debe poder surtirse
  desde **una (1) sola bodega**… Los envíos de un mismo SKU no se dividen".
- **Fuente B** — [SDD/Domain/06-business-rules.md :7](Domain/06-business-rules.md)` ([INV-02](Domain/06-business-rules.md)) y
  [SDD/Domain/services/inventory-reservation-service.md :18](Domain/services/inventory-reservation-service.md)`: "el servicio tiene
  permitido **fraccionar** la reserva en ambas bodegas".

| Opción | Consecuencia |
|---|---|
| **A1. Ganar A (no fraccionar)** | Regla simple; menos envíos; se rechaza o recorta la compra si ninguna bodega cubre la cantidad. `OrderSplitterService` crea 1 `Shipment` por línea |
| **A2. Ganar B (fraccionar)** | Se requiere `ReservationDetails` con desglose por bodega, múltiples `Shipment` por variante pese a "evitar costos exorbitantes", y compensación al liberar |
| **A3. Híbrido** | Fraccionar salvo que ninguna combinación cubra la cantidad total; documentar el umbral exacto |

**Desbloquea:** G-01, G-02, G-05, G-06, [R-04](#r-04). **No se ha tocado código de inventario.**

## [Q-02](#q-02-c-02-la-clave-del-inventario-es-el-producto-o-la-variante-sku-resuelta-2026-09-17) ([C-02](#c-02)) — ¿La clave del inventario es el producto o la variante (SKU)? · ✅ RESUELTA (2026-09-17)

**Decisión del owner: B2 — inventario por `VariantId` (SKU).**

- Se crea `ProductVariant` como entidad hija del agregado `Product`; su `Id` **es**
  el `VariantId` y actúa como SKU del inventario.
- `Inventory.VariantId` sustituye a `Inventory.ProductId`. Clave de stock:
  `(VariantId, WarehouseId)`.
- La variante se compone de atributos (`VariantAttribute`: nombre + valor).
- El SKU es único dentro del producto.
- **No decidido por B2:** si todo producto debe tener al menos una variante →
  nueva pregunta **[Q-10](#9-cuarta-iteracion-adr-0003-variante-obligatoria-en-fisicos-q-10-c3)**. Detalle del modelo de atributos → **[Q-11](#q-11-detalle-del-modelo-de-atributos-de-variante-abierta)**.

Registrado en [SDD/Adr/0002-clave-inventario-variantid.md](Adr/0002-clave-inventario-variantid.md) y aplicado a
[SDD/Domain/01-models.md](Domain/01-models.md), [SDD/Domain/06-business-rules.md](Domain/06-business-rules.md) ([INV-03](Domain/06-business-rules.md)),
[SDD/Domain/05-ports.md](Domain/05-ports.md) y [SDD/Domain/services/inventory-reservation-service.md](Domain/services/inventory-reservation-service.md).

### Contexto histórico de la contradicción

- **Fuente A** — [02-aggregates-and-entities.md :29](Domain/02-aggregates-and-entities.md)`, [03-value-objects.md :15](Domain/03-value-objects.md)`:
  `VariantId` como SKU.
- **Fuente B** — [01-models.md :33](Domain/01-models.md)` y [ZENTRIC.md](/ZENTRIC.md) Dominio 6: "vinculado
  obligatoriamente a un producto y una bodega"; el código usa `ProductId`.
- [ZENTRIC.md](/ZENTRIC.md) Dominio 5 sí menciona "Variantes: Diferencias de color, talla,
  modelo".

| Opción | Consecuencia |
|---|---|
| **B1. Inventario por `ProductId`** | Conforme al código actual; las variantes quedan como atributo informativo; no se puede controlar stock por talla/color |
| **B2. Inventario por `VariantId`** | Exige crear `ProductVariant`, cambiar `Inventory` y migrar datos futuros; es el modelo de marketplace real |
| **B3. `ProductVariant` obligatoria** | Todo producto tiene al menos 1 variante (la unidad); inventario siempre por SKU — el más consistente a largo plazo |

**Desbloquea:** G-07, G-10 y el diseño del esquema de inventario. **G-08 cerrada**
(`ProductVariant` implementada en código y en spec).

## [Q-03](#q-03-c-03-estado-de-cancelacion-de-despacho) ([C-03](#c-03)) — Estado de cancelación de despacho

`Cancelled` ([02-value-objects.md :58](Domain/02-value-objects.md)`) vs `CancelledByStockBreak`
([03-value-objects.md :41](Domain/03-value-objects.md)`). Opciones: **(a)** un único `Cancelled` con motivo
(`CancellationReason`); **(b)** dos estados distintos; **(c)** `Cancelled` +
campo `Reason` obligatorio. Desbloquea G-02.

## [Q-04](#q-04-c-04-cart-es-un-estado-de-customerorder) ([C-04](#c-04)) — ¿`Cart` es un estado de `CustomerOrder`?

[02-value-objects.md :46](Domain/02-value-objects.md)` y `07-lifecycle` [:1](Domain/02-value-objects.md#1-value-objects-objetos-de-valor) lo incluyen; [03-value-objects.md :29](Domain/03-value-objects.md)`
no. Opciones: **(a)** `Cart` es estado del agregado (con timeout de 15 min);
**(b)** el carrito es un agregado separado (`Cart`) y `CustomerOrder` nace en
`PendingPayment`; **(c)** `Cart` y `Order` comparten el agregado pero el carrito
nunca se factura. Desbloquea G-01 y G-10.

## [Q-05](#q-05-c-05-vendor-o-seller) ([C-05](#c-05)) — ¿`Vendor` o `Seller`?

La spec de dominio dice `Vendor`; el código dice `Seller`. Opciones: **(a)**
unificar a `Vendor` (renombrar código y `WarehouseType`); **(b)** unificar a
`Seller` (actualizar las specs); **(c)** mantener `Seller` en código y registrar
`Vendor` como sinónimo prohibido en el glosario. Recomendación del agente:
**(a)** por jerarquía de verdad (la spec manda) y porque aún no hay contratos.
Desbloquea [R-09](#r-09) y [T-008](#t-008).

## [Q-06](#q-06-c-07-consolidacion-de-los-documentos-numerados) ([C-07](#c-07)) — Consolidación de los documentos numerados

`SDD/Domain/` tiene parejas solapadas: `01-domain-overview`/`01-models`,
`02-aggregates`/`02-value-objects` (no son pareja real), `03-domain-services`/
`03-value-objects`, `04-domain-events`/`04-invariants-and-rules`. Opciones:
**(a)** renumerar por tema canónico (`01-overview`, `02-models`, `03-value-objects`,
`04-aggregates`, `05-invariants`, `06-business-rules`, `07-lifecycle`,
`08-domain-events`, `09-domain-services`, `10-ports`); **(b)** fusionar los
solapados en un documento por tema; **(c)** dejar como está y declarar un índice
canónico en `SDD/Domain/00-index.md`. Desbloquea la navegabilidad de la SSoT.

## [Q-07](#q-07-documento-de-identidad-del-usuario) — Documento de identidad del usuario

[ZENTRIC.md](/ZENTRIC.md) Dominio 1 lo marca obligatorio y único, y [AGENTS.md](#agentsmd) prohíbe
inventar. Opciones: **(a)** añadir `IdentityDocument` como VO obligatorio con
validación de formato (¿qué país/formato?); **(b)** añadirlo opcional; **(c)**
eliminarlo de la spec por no aplicar al negocio. Si es (a), se necesita el
formato exacto. Desbloquea [R-03](#r-03)/[T-006](#t-006).

## [Q-08](#q-08-r-07-buyerpaymenttokens-contra-la-invariante-9) ([R-07](#r-07)) — `Buyer.PaymentTokens` contra la [invariante 9](Domain/04-invariants-and-rules.md)

La [invariante 9](Domain/04-invariants-and-rules.md) dice: "No se almacenan tarjetas de crédito ni billeteras… el
Dominio avanza de estado con una simple entidad `PaymentReceipt`". El código
almacena `PaymentTokens`. Opciones: **(a)** eliminar `PaymentTokens` y crear
`PaymentReceipt`; **(b)** mantener tokens (referencias opacas del proveedor, no
datos de tarjeta) y **enmendar** la [invariante 9](Domain/04-invariants-and-rules.md); **(c)** dejar `PaymentTokens`
fuera del agregado `Buyer`. Requiere además la decisión de la pasarela.

## [Q-09](#q-09-naming-canonico-pendiente) — Naming canónico pendiente

Decisiones menores pero propagables: `Inventory` → `InventoryItem`;
`Avalible` → `Available` (typo); `Email` → `EmailAddress`; `Administrator` →
`Admin`; `Product.Name` → `Product.Title`; `Warehouse.SellerId` → `OwnerId`;
`User.FullName` como VO (la spec dice `string`). ¿Se autoriza el renombrado
masivo ahora (coste mínimo) o se difiere? Desbloquea [T-008](#t-008).

## [Q-10](#9-cuarta-iteracion-adr-0003-variante-obligatoria-en-fisicos-q-10-c3) — ¿Es obligatorio que todo producto tenga al menos una variante? · ✅ RESUELTA (2026-09-17)

**Decisión del owner: C3 — obligatoria solo para `Physical`.**

- Un producto `Physical` **exige al menos una variante** (la variante es la
  unidad de stock, [INV-03](Domain/06-business-rules.md)).
- Un producto `Digital` ([CAT-02](Domain/06-business-rules.md): sin logística ni inventario) **puede** nacer sin
  variantes.
- Aplicación en código ([T-010c](#t-010c), [ADR-0003](Adr/0003-variante-obligatoria-productos-fisicos.md)): constructor rechaza `Physical` sin
  variantes; `UpdateType(Physical)` exige variante; `RemoveVariant` no puede
  dejar un `Physical` sin variantes; `CanBeSold` exige variante vendible en
  `Physical`. Suite: 164/164 PASS.

Registrado en [SDD/Adr/0003-variante-obligatoria-productos-fisicos.md](Adr/0003-variante-obligatoria-productos-fisicos.md) y aplicado
a [SDD/Domain/01-models.md :2](Domain/01-models.md#2-bounded-context-catalog-catalogo) y [SDD/Domain/06-business-rules.md](Domain/06-business-rules.md) ([CAT-03](Domain/06-business-rules.md)).

### Contexto histórico de la pregunta

[Q-02](#q-02-c-02-la-clave-del-inventario-es-el-producto-o-la-variante-sku-resuelta-2026-09-17) (B2) decidió que el inventario se lleva por `VariantId`, pero **no** decidió
si un producto puede existir sin variantes.

| Opción | Consecuencia |
|---|---|
| **C1. Variante opcional** | Un producto sin variantes no puede tener inventario ni reservarse. Sirve para productos digitales o catálogos informativos, pero deja un estado inalcanzable para productos físicos |
| **C2. Variante obligatoria (equivalente a B3)** | Todo producto nace con al menos una variante "unidad". Elimina el estado inválido, pero cambia el alta de producto y las pruebas de catálogo |
| **C3. Obligatoria solo para `Physical`** | Los productos `Digital` ([CAT-02](Domain/06-business-rules.md): sin logística ni inventario) quedan sin variante; los físicos siempre tienen una |

**Impacto si no se decide:** `InventoryReservationService` no puede definir qué
hacer con un producto físico sin variantes (¿error de especificación o compra
rechazada?). **Bloqueaba:** [T-013](#t-013), [T-015](#t-015), [T-016](#t-016). **Sub-decisiones propuestas que
requieren confirmación:** ver **[Q-12](#sub-decisiones-propuesto-ver-q-12)**.

## [Q-11](#q-11-detalle-del-modelo-de-atributos-de-variante-abierta) — Detalle del modelo de atributos de variante · ABIERTA

`VariantAttribute` (nombre + valor) fue `[PROPUESTO]` por el agente a partir de
[02-aggregates-and-entities.md :2](Domain/02-aggregates-and-entities.md#2-catalog-module) ("maneja las combinaciones (ej. Talla/Color)").
Puntos a confirmar:

1. ¿Nombre/valor libres o catálogo cerrado de atributos por producto?
2. ¿Un atributo puede repetirse con distinto valor? Hoy se prohíbe repetir el
   mismo nombre dentro de una variante.
3. ¿Algún atributo es obligatorio (ej. talla en ropa)?
4. Límite de longitud propuesto: 50 caracteres por nombre/valor (defensivo, no
   definido en la spec).

**Bloquea:** [T-010b](#t-010b) (refinamiento de catálogo). **No** bloquea la reserva ni el inventario.

## [Q-12](#sub-decisiones-propuesto-ver-q-12) — Sub-decisiones `[PROPUESTO]` de [ADR-0003](Adr/0003-variante-obligatoria-productos-fisicos.md) (variante obligatoria) · ABIERTA

Al aplicar [Q-10](#9-cuarta-iteracion-adr-0003-variante-obligatoria-en-fisicos-q-10-c3) = [C3](Adr/0003-variante-obligatoria-productos-fisicos.md) el agente tomó 4 sub-decisiones que **van más allá del texto
literal** de la respuesta y requieren tu confirmación:

1. `HasVariant` cuenta solo variantes **no eliminadas lógicamente** (una variante
   con `Delete()` no cuenta como variante del producto).
2. `CanBeSold` en `Physical` exige una variante **activa y no eliminada** (una
   variante desactivada no hace vendible al producto, aunque `HasVariant` siga
   siendo `true`).
3. Los productos `Digital` **pueden** declarar variantes ([Q-10](#9-cuarta-iteracion-adr-0003-variante-obligatoria-en-fisicos-q-10-c3) solo dijo que
   *pueden no tener*; no prohibió tenerlas).
4. La eliminación lógica de la última variante de un `Physical` **no lanza**
   excepción: deja el producto no vendible (`CanBeSold == false`) hasta restaurar
   o añadir otra (se eligió no lanzar porque el borrado lógico es ciclo de vida
   de datos, no mutación de contrato).

**Si confirmas:** se marcan `[CONFIRMADO]` en [ADR-0003](Adr/0003-variante-obligatoria-productos-fisicos.md). **Si corriges alguna:**
indica el número (p. ej. `[Q-12](#sub-decisiones-propuesto-ver-q-12).2 = no`) y se ajusta código + pruebas + ADR.
**Bloquea:** el cierre definitivo de [T-010c](#t-010c) (el código ya está aplicado y en
verde, pero con etiqueta `[PROPUESTO]` en estos 4 puntos).

## [Q-13](#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante) ([C-08](#c-08)) — La Ley define `DOMINIO 8`, `9` y `10` **dos veces**, con significados cruzados · 🔴 ABIERTA · **BLOQUEANTE**

**Hallazgo (2026-09-18).** [ZENTRIC.md](/ZENTRIC.md) contiene hoy **dos bloques** que definen los mismos
números de dominio con contenidos distintos: el bloque base (líneas 254-270) y el bloque
`# [ADDENDUM - DICTADO POR OWNER]` (líneas 320-334).

| Nº | Bloque base (líneas 254-270) | Bloque ADDENDUM (líneas 320-334) |
|---|---|---|
| 8 | **5 estados**: Pendiente de Empaque → Empacado → Despachado → Entregado → Cancelado por Quiebre; "deriva del pedido del cliente" | "**Estados: Empacado y Despachado**" + Regla de Stock Fantasma (cancelación con devolución obligatoria) |
| 9 | **Facturación y Pagos**: validación de transacciones, generación de facturas, conciliación y split | **Devoluciones y Reembolsos**: prohibición de digitales + flujo físico (inspección → aprobación del Vendedor → vuelta al stock "Usado") |
| 10 | **Devoluciones y Reembolsos**: 4 estados (Solicitada, Aprobada, Rechazada, Reembolsada) | **Facturación y Pagos**: Factura Maestra · Detalle Zentric · Factura de Vendedor (Split) |

`[RIESGO]` El código de la Fase 3b/4 **ya tomó una interpretación que nadie dictó**:
`FulfillmentStatus` sin `PendingPack` (`FulfillmentOrder.cs:33`, "Según dictamen…"), `InvoiceType`
sin "Detalle Zentric", y la devolución aprobada **no** devuelve stock ([H-14](#h-14)). No es una decisión
registrada: es una asunción ([AGENTS.md :0.3](#agentsmd-:03)).

| Opción | Consecuencia |
|---|---|
| **[Q-13](#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante).a — Manda el bloque base** | Dom.8 con los **5 estados** (el código debe añadir `PendingPack` y renombrar a `Shipped`/`CancelledByStockBreak` si aplica); Dom.9 = Facturación; Dom.10 = Devoluciones. El ADDENDUM queda como **ampliación** de reglas, no de numeración |
| **[Q-13](#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante).b — Manda el ADDENDUM** | Dom.8 con 2 estados (Empacado y Despachado); Dom.9 = Devoluciones; Dom.10 = Facturación con los 3 documentos. Se debe **renumerar** la Ley y avisar que el bloque base queda derogado por este punto |
| **[Q-13](#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante).c — Fusión por tema (recomendación del agente, `[PROPUESTO]`)** | Dom.8 = **5 estados** de despacho; Dom.9 = **Devoluciones** (prohibición de digitales + flujo físico con stock "Usado"); Dom.10 = **Facturación** (Maestra, Detalle Zentric, Vendedor). Se conserva lo más específico de cada bloque y se renumeran los títulos |
| **[Q-13](#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante).d — Otra numeración** | El Owner dicta el texto exacto y el agente lo registra como ADDENDUM sin alterar el original |

**Desbloquea:** corrección de `FulfillmentStatus`, `InvoiceType`, el flujo de devolución a stock
([H-14](#h-14)), [T-013](#t-013)/[T-014](#t-014)/[T-016](#t-016) y el cierre de [R-11](#r-11)/[R-12](#r-12)/[R-13](#r-13).

**Pregunta adicional ([C-10](#c-10)):** el bloque base de `DOMINIO 8/9/10` **no lleva** el rótulo
`[ADDENDUM - DICTADO POR OWNER]` que exige [AGENTS.md :0.7](#addendum---dictado-por-owner]-que-exige-[agentsmd-:07). ¿Autorizas que se le añada el rótulo
(sin tocar el texto) para distinguir cliente de expansión?

## [Q-14](#q-14-licenciamiento-de-mediatr-14-y-fluentvalidation-abierta-no-bloquea-el-codigo-actual) — Licenciamiento de MediatR 14 y FluentValidation · 🔴 ABIERTA · no bloquea el código actual

**Hallazgo (SPEC-007, 2026-09-18).** Al montar el contenedor de dependencias en las pruebas apareció:

```text
System.InvalidOperationException : MediatR requires ILoggerFactory to be registered.
Call services.AddLogging() before services.AddMediatR().
   at MediatR.Registration.ServiceRegistrar.AddRequiredServices(...)
   at Microsoft.Extensions.DependencyInjection.MediatRServiceCollectionExtensions.CheckLicense(...)
```

`[CONFIRMADO]` MediatR 14 exige `ILoggerFactory` registrado antes de `AddMediatR()`. En `Zentric.Api`
lo aporta el host web (`WebApplicationBuilder`); en un `ServiceCollection` desnudo hay que llamar
`AddLogging()`. El arnés de pruebas se corrigió en consecuencia.

`[OBSERVADO]` La versión instalada ejecuta una **comprobación de licencia** al resolver el mediador
(el stack trace lo muestra). MediatR cambió a un modelo de licenciamiento comercial a partir de la
versión 13.

`[PENDIENTE]` **No verifiqué las condiciones legales ni económicas** — no es una decisión que el
agente pueda tomar. Opciones:

| Opción | Consecuencia |
|---|---|
| **a. Mantener MediatR 14** | Se debe confirmar y cumplir la licencia vigente (el proyecto ya depende de 14.2.0 en `Zentric.Application`) |
| **b. Fijar una versión anterior** (12.x, última con licencia permisiva) | Cambio de dependencia y de API de registro; hay que revisar los 3 handlers y el pipeline |
| **c. Sustituir por un mediador propio mínimo** | Sin dependencia de terceros; se pierden comportamientos y hay que escribir el pipeline y el registro |
| **d. Diferirlo** | Riesgo de cumplimiento si el producto se comercializa; se registra como deuda ([R-19](#r-19)) |

**Impacto actual:** ninguno en build ni en las 206 pruebas. **Bloquea:** nada del roadmap vigente;
afecta a la sostenibilidad y al cumplimiento de la dependencia.

### Catálogos Completos C-xx / R-xx / G-xx
## 1. Contradiccionesde especificación (bloqueantes)

| ID | Categoría | Evidencia | Impacto | Prob. | Acción | ¿Bloquea? |
|---|---|---|---|---|---|---|
| ~~C-01~~ | ~~Regla de negocio / inventario~~ | ~~[04-invariants-and-rules.md :8](Domain/04-invariants-and-rules.md)` vs [06-business-rules.md :7](Domain/06-business-rules.md)` vs [services/inventory-reservation-service.md :18](Domain/services/inventory-reservation-service.md)`~~ | **RESUELTA el 2026-09-17 por decisión del owner (A3 híbrido).** Regla canónica: bodega única si una bodega cubre la cantidad; fraccionamiento de contingencia solo si ninguna cubre; fallo si la suma no alcanza. Ver [SDD/Adr/0001-reserva-fragmentacion-contingencia.md](Adr/0001-reserva-fragmentacion-contingencia.md) | — | Cerrada | No |
| ~~C-02~~ | ~~Modelo de datos / clave de stock~~ | ~~[02-aggregates-and-entities.md :29](Domain/02-aggregates-and-entities.md)` y [03-value-objects.md :15](Domain/03-value-objects.md)` (`VariantId`) vs [01-models.md :33](Domain/01-models.md)`, [ZENTRIC.md](/ZENTRIC.md) Dom.6 y el código (`ProductId`)~~ | **RESUELTA el 2026-09-17 por decisión del owner (B2).** El stock se lleva por `VariantId` (SKU); `ProductVariant` es entidad hija de `Product`; clave `(VariantId, WarehouseId)`. Ver [SDD/Adr/0002-clave-inventario-variantid.md](Adr/0002-clave-inventario-variantid.md) | — | Cerrada (queda [Q-10](#q-10) sobre variante obligatoria) | Parcialmente |
| C-03 | Estado de dominio | [02-value-objects.md :58](Domain/02-value-objects.md)` (`FulfillmentStatus.Cancelled`) vs [03-value-objects.md :41](Domain/03-value-objects.md)` (`CancelledByStockBreak`) | Máquina de estados de despacho ambigua | Media | [Q-03](#q-03) | Sí (Fulfillment) |
| C-04 | Estado de dominio | [02-value-objects.md :46](Domain/02-value-objects.md)` incluye `Cart` en `OrderStatus` vs [03-value-objects.md :29](Domain/03-value-objects.md)-34` lo omite | Un carrito efímero y un pedido formal no pueden compartir agregado sin definirlo | Media | [Q-04](#q-04) | Sí (Ordering) |
| C-05 | Lenguaje ubicuo | [02-value-objects.md :62](Domain/02-value-objects.md)` (`Vendor`) vs `WarehouseType.cs:6` (`Seller`) | Nombres divergentes en API, base de datos y eventos | Media | [Q-05](#q-05) | No (se propaga) |
| C-06 | Gobernanza documental | [AGENTS.md :0.1](#agentsmd-:01) exige [SDD/01-system-overview.md](#sdd/01-system-overviewmd), [SDD/02-software-architecture.md](#sdd/02-software-architecturemd), `SDD/Infrastructure/`, `SDD/Presentation/`; no existían. Además [SDD/Domain/Software-arquitecture.md](Domain/Software-arquitecture.md) está **vacío (0 bytes)** | La SSoT declarada no era navegable y el agente no podía cumplir la consulta obligatoria | Alta | 2 documentos creados ahora; restan Infrastructure/Presentation | Parcialmente |
| C-07 | Numeración de specs | `SDD/Domain/` tiene dos `01-*`, dos `02-*`, dos `03-*`, dos `04-*` con contenido solapado | Cada lectura puede llevar a una regla distinta; no hay canonicidad | Alta | [Q-06](#q-06) | Sí (para consolidar) |
| **C-08** | **Duplicación de la Ley** | [ZENTRIC.md](/ZENTRIC.md) líneas **254-270** (bloque base: Dom.8 con 5 estados, Dom.9 facturación, Dom.10 devoluciones) vs líneas **320-334** (ADDENDUM: Dom.8 "Empacado y Despachado", Dom.9 devoluciones, Dom.10 facturación) — **9 y 10 intercambian significado** | El código ya tomó una interpretación **no dictada**: falta `PendingPack`, falta "Detalle Zentric" y la devolución no devuelve stock | **Alta** | **[Q-13](#q-13) (nueva)** | **Sí — bloquea Fase 3b/4** |
| **C-09** | Gobernanza de la skill | Repo en **v6.0.0** monolítico vs copia instalada **v3.1.0**; [AGENTS.md :0.0](#agentsmd-:00) citaba `references/10-zentric-overlay.md` (borrado) | La carga automática usaba metodología obsoleta; `sync-skill.ps1` habría destruido la copia instalada al no existir `references/` | Media | **Corregido el 2026-09-18** ([AGENTS.md :0.0](#agentsmd-:00), script y sincronización) | No |
| **C-10** | Trazabilidad de la Ley | Las adiciones `DOMINIO 8/9/10` previas al rótulo **no llevan** `[ADDENDUM - DICTADO POR OWNER]` ([AGENTS.md :0.7](#addendum---dictado-por-owner]-([agentsmd-:07)) | No se distingue el texto del cliente de la expansión del modelo | Media | Registrar en [SDD/SDD.md :4.2](#sdd/sddmd-:42))](../SDD.md#42-adiciones-a-la-biblia-formato-skill-07); requiere autorización del Owner para tocar la Biblia | No |

## 2. Riesgos técnicos

| ID | Categoría | Evidencia | Impacto | Prob. | Acción | ¿Bloquea? |
|---|---|---|---|---|---|---|
| R-01 | Integridad de datos | [H-01](#h-01) `Inventory.DispatchStock` (`Inventory.cs:109-123`) | Descuadre de stock: reservado negativo, balance roto | Alta | Corregir con prueba ([T-003](#t-003)) | No |
| R-02 | Integridad de datos | [H-02](#h-02) `UpdateQuantities` (`Inventory.cs:58-79`) | Se saltan `Reserve/Release/Dispatch`; anula invariantes | Alta | Rediseñar API ([T-003](#t-003)/[T-004](#t-004)) | No |
| R-03 | Cumplimiento de spec | [H-05](#h-05) `User` sin `IdentityDocument` | No se puede registrar un usuario conforme a [ZENTRIC.md](/ZENTRIC.md) Dom.1 | Alta | [T-006](#t-006) (tras [Q-07](#q-07)) | No |
| R-04 | Seguridad / autorización | Falta `ManualAdjust(qty, UserId, Role)` ([invariante 4](Domain/04-invariants-and-rules.md)) | Cualquiera podría ajustar stock en bodegas Marketplace | Media | [T-005](#t-005) | Sí (con C-01) |
| R-05 | Regresión | **164 pruebas** en `Zentric.Tests` (antes: 0) | Queda sin cobertura: `Buyer`, los VOs `Email`/`FullName`, eventos y todo lo faltante | Media | [T-002b](#t-002b), Fase 3 | No |
| R-06 | Testabilidad | [H-06](#h-06): 46 usos de `DateTime.UtcNow` en 5 entidades | Las reglas temporales (timeout 15 min) no son verificables determinísticamente | Media | [T-004](#t-004) (abstracción de tiempo) | No |
| R-07 | Contradicción de diseño | [H-03](#h-03) `Buyer.PaymentTokens` vs [invariante 9](Domain/04-invariants-and-rules.md) (YAGNI: sin medios de pago) | Se almacenarían tokens de pago que la spec prohíbe | Media | [Q-08](#q-08) | Parcial |
| R-08 | Higiene del repositorio | `bin/`/`obj/` en el árbol; sin `.editorconfig`; sin CI; sin pipeline de calidad | El build "limpio" no refleja la calidad real | Media | [T-007](#t-007) | No |
| R-09 | Lenguaje ubicuo | 12 desviaciones de naming (`Avalible`, `Inventory` vs `InventoryItem`, `Email` vs `EmailAddress`, `Administrator` vs `Admin`, `Name` vs `Title`, `SellerId` vs `OwnerId`) | Coste de renombrado creciente; confusión en contratos públicos | Alta | [Q-05](#q-05)/[Q-09](#q-09) + [T-008](#t-008) | No |
| R-11 | Integridad de la Ley | **C-08**: `DOMINIO 8/9/10` definidos dos veces con contenidos distintos (9 y 10 intercambiados) | El código implementó una interpretación no dictada; cualquier corrección posterior se rehará dos veces | **Alta** | **[Q-13](#q-13)** | **Sí — bloquea Fase 3b/4** |
| R-12 | Cumplimiento de la Ley | `FulfillmentStatus` sin `PendingPack`; `InvoiceType` sin "Detalle Zentric" | Incumplimiento directo de `DOMINIO 8` y de [ADD-003](#add-003) | Alta | [Q-13](#q-13) | **Sí** |
| R-13 | Cumplimiento de la Ley | `ReturnToUsedStock()` existe pero **nadie lo invoca** (`ReturnRequest.cs:63`, `FulfillmentOrder.cs:75` con `// TODO`) | [ADD-002](#add-002) ("el producto vuelve al stock con etiqueta Usado") no ocurre en runtime | Alta | [H-14](#h-14), [T-012](#t-012) | No |
| R-14 | Regla de negocio | [PED-01](Domain/06-business-rules.md) no implementada: añadir al carrito **no reserva stock** ni hay timeout de 15 min | Sobreventa posible; la Ley exige reserva preventiva con liberación | Alta | [T-013](#t-013)/[T-015](#t-015), [Q-04](#q-04) | No |
| ~~R-15~~ | Cumplimiento de [AGENTS.md](#agentsmd) | ~~**[H-09](#h-09)** `try-catch` genérico en Application; **[H-11](#h-11)** sin `AddProblemDetails()` ni validadores FluentValidation~~ | **CERRADO en SPEC-007 (2026-09-18):** [H-09](#h-09), [H-11](#h-11) y [H-12](#h-12) corregidos con 21 pruebas unitarias + 7 de integración DI ([verification-baseline.md :11](#verification-baselinemd-:11), 206/206) | ~~Alta~~ | — | No |
| R-19 | Dependencias y licencias | MediatR 14 exige `ILoggerFactory` antes de `AddMediatR()` (verificado: `MediatRServiceCollectionExtensions.CheckLicense` en el stack trace) y ejecuta una **verificación de licencia** en la resolución; la versión 13+ cambió su modelo de licenciamiento | `[CONFIRMADO]` el requisito técnico; `[PENDIENTE]` **confirmar con el Owner las condiciones de licencia de MediatR/FluentValidation** para uso comercial | `[OBSERVADO]` el host web sí registra logging, así que el fallo solo aparecía en un contenedor desnudo | **[Q-14](#q-14) (nueva):** verificar licenciamiento; evaluar alternativa si no se aprueba | No |
| R-16 | Seguridad / configuración | `Zentric.Api/appsettings.json` incluye credenciales en claro (`Username=postgres;Password=postgres`) | Violación del DoD [AGENTS.md:8](#agentsmd:8) ("no se introdujeron secretos"); riesgo de fuga al versionar | Media | mover a user-secrets / variables de entorno | No |

## 3. Vacíos funcionales (gaps de implementación)

| ID | Gap | Spec de origen | Bloqueo |
|---|---|---|---|
| G-01 | `CustomerOrder` + `OrderItem` + estados + checkout (agregado central) | `01-models` [CustomerOrder.cs:4](../Zentric.Domain/Orders/CustomerOrder.cs#L4), `07-lifecycle` | **Parcialmente cerrado en código sin autorización** (2026-09-18): agregado y 5 estados operativos; faltan `FlatShippingFee`, cancelación y reembolso parcial. **[Q-04](#q-04) sigue abierta** (C-08/R-11) |
| G-02 | `FulfillmentOrder` + `Shipment` + máquina de estados | `01-models` [FulfillmentOrder.cs:5](../Zentric.Domain/Logistics/FulfillmentOrder.cs#L5) | **Parcialmente cerrado en código sin autorización**: agregados creados con 4 de los 5 estados de la Ley; **falta `PendingPack`** y el servicio de split. **[Q-03](#q-03) sigue abierta** |
| G-03 | Devoluciones y reembolsos con doble aprobación | `ReturnStatus`, [services/returns-approval-service.md](Domain/services/returns-approval-service.md) | **Parcialmente cerrado en código**: doble aprobación implementada en el agregado; **no hay servicio ni retorno a stock** (R-13) |
| G-04 | 7 eventos de dominio + `IDomainEventDispatcher` | [04-domain-events.md](Domain/04-domain-events.md), [05-ports.md :2](Domain/05-ports.md#2-puertos-de-mensajeria-event-bus) | **Ninguno implementado** (`// TODO` en su lugar → [H-14](#h-14)) |
| G-05 | Puertos de repositorio restantes | [05-ports.md :1](Domain/05-ports.md#1-puertos-de-repositorios-persistencia) | **2 de 7 implementados** (`ICustomerOrderRepository`, `IFulfillmentOrderRepository`); `IUserRepository` sigue en `Domain` y sin implementación |
| G-06 | 4 servicios de dominio | [03-domain-services.md](Domain/03-domain-services.md) | [Q-03](#q-03) ([Q-10](#q-10) resuelta: todo `Physical` trae variante) |
| G-07 | IDs fuertemente tipados y VO `Address` | [02-value-objects.md](Domain/02-value-objects.md) | — (pendiente [T-004](#t-004); `VariantId` sigue siendo `Guid` plano) |
| ~~G-08~~ | ~~`ProductVariant` (SKU)~~ | `02-aggregates:18-19` | **CERRADA** en [ADR-0002](Adr/0002-clave-inventario-variantid.md) / [T-010](#t-010) |
| G-08b | Refinamiento del modelo de atributos de variante | `02-aggregates` [:2](#:2) | [Q-11](#q-11) |
| G-08c | Hacer cumplir [CAT-03](Domain/06-business-rules.md) en el agregado `Product` (constructor, `UpdateType`, `RemoveVariant`, `CanBeSold`) | [CAT-03](Domain/06-business-rules.md), [ADR-0003](Adr/0003-variante-obligatoria-productos-fisicos.md) | **CERRADA** en [ADR-0003](Adr/0003-variante-obligatoria-productos-fisicos.md) / [T-010c](#t-010c): 164/164 PASS |
| G-09 | Suspensión en cascada al bloquear un vendedor | [invariante 6](Domain/04-invariants-and-rules.md) | Ninguno |
| G-10 | Timeout de reserva de 15 minutos | [PED-01](Domain/06-business-rules.md), `07-lifecycle` | R-06 |
| G-11 | Capas `Application`, `Infrastructure`, `Api` y contrato OpenAPI | [AGENTS.md :2](#agentsmd-:2) | **Parcialmente cerradas en código sin registrar** (2026-09-18): existen las 3 capas y 3 endpoints; **sin contrato OpenAPI de negocio**, sin validadores, con ProblemDetails incompleto ([T-019](#t-019)…[T-021](#t-021)) |

### Defectos y Estado Actual del Código
## 2. Reglas de negocioefectivamente protegidas hoy

`[CONFIRMADO]` por lectura del código:

1. **No-negatividad de inventario ([INV-01](Domain/06-business-rules.md) / [invariante 1](Domain/04-invariants-and-rules.md)).** Protegida en el
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
6. **Producto creado nace activo ([CAT-01](Domain/06-business-rules.md)).** `Product.cs:59`.
7. **Variante obligatoria en productos físicos ([CAT-03](Domain/06-business-rules.md)).** Constructor,
   `UpdateType` y `RemoveVariant` en `Product.cs` ([ADR-0003](Adr/0003-variante-obligatoria-productos-fisicos.md)).
8. **Moneda homogénea en operaciones con `Money`.** `Money.cs:78-84`.
9. **Correo con formato mínimo validado.** `Email.cs:8-39`.

## 3. Arquitectura observada

`[CONFIRMADO]`

- Patrón: hexagonal + DDD respetado **en el papel**: `Zentric.Domain` no
  referencia nada (correcto y verificado en el `csproj`).
- Convención de carpetas: **módulo por bounded context** (`Users/`, `Products/`,
  `Warehouses/`, `Inventories/`, `Orders/`, `Buyers/`), con `Enums/`,
  `ValueObjects/` y `Ports/` anidados por módulo.
- "Ports" aparece dentro del módulo (`Users/Ports/`), coherente con [AGENTS.md :2.2](#agentsmd-:22) pero distinto del catálogo genérico de la skill.

`[CONFIRMADO]` Ausencias estructurales respecto a [AGENTS.md](#confirmado]-ausencias-estructurales-respecto-a-[agentsmd):
`[OBSOLETO — corregido el 2026-09-18]` El párrafo siguiente describía el estado del 2026-09-17. **Ya no es cierto:** existen los proyectos `Zentric.Application`, `Zentric.Infrastructure`, `Zentric.Api` y `Zentric.Tests` (ver [:6](#obsoleto-—-corregido-el-2026-09-18]-el-párrafo-siguiente-describía-el-estado-del-2026-09-17-**ya-no-es-cierto:**-existen-los-proyectos-zentricapplication,-zentricinfrastructure,-zentricapi-y-zentrictests-(ver-[:6)). Se conserva tachado para trazabilidad.

- ~~No existen `Application`, `Infrastructure`, `Api` ni pruebas.~~ → existen (5 proyectos en `Zentric.slnx`)
- No hay eventos de dominio ni dispatcher. ← **sigue siendo cierto** (G-04)
- No hay tipos fuertemente tipados de ID (todos `Guid`). ← sigue siendo cierto (G-07)
- No hay `Address` VO (las direcciones son `string`). ← sigue siendo cierto
- No hay abstracción de tiempo (`DateTime.UtcNow` directo en el dominio). ← sigue siendo cierto ([H-06](#h-06))
- No hay `.editorconfig`, ni analizadores, ni CI. ← sigue siendo cierto ([R-08](#r-08))

## 4. Contrato de dominio observable (puntos de dolor)

`[CONFIRMADO]` `Inventory.DispatchStock(int)` (`Inventory.cs:109-123`) valida
`AvalibleQuantity < quantity` cuando la cantidad que decrementa es
`ReservedQuantity`. Con `Available = 0`, `Reserved = 10`, `DispatchStock(5)`
decrementa `Reserved` a 5 sin error; con `quantity > ReservedQuantity` puede
dejar `ReservedQuantity` **negativo**, violando la [invariante 1](Domain/04-invariants-and-rules.md) referida al stock
reservado y rompiendo el balance
`Avalible + Reservado + Dañado`.

`[CONFIRMADO]` `Inventory.UpdateQuantities(...)` permite sobrescribir los tres
contadores de golpe, saltándose `Reserve/Release/Dispatch`, lo que anula la
protección de las invariantes por la puerta de atrás (setter anémico
disfrazado de método).

`[CONFIRMADO]` `MarkAsDeleted()` en `Inventory.cs:167` y en `Warehouse.cs:206`
es un alias de `Delete()`, duplicando API sin valor semántico.

## 5. Cambios aplicados desde la línea base

`[CONFIRMADO]` [T-003a](#t-003a) (2026-09-17): `Inventory.ReturnToAvalible` ahora valida
`quantity <= 0` y lanza `ArgumentOutOfRangeException`. Antes, una cantidad
negativa dejaba `AvalibleQuantity` por debajo de cero, violando [INV-01](Domain/06-business-rules.md). Se añadió
la prueba de regresión correspondiente.

`[CONFIRMADO]` Se creó el proyecto de pruebas `Zentric.Tests` (xUnit) con **164
pruebas** en verde ([verification-baseline.md :9](#verification-baselinemd-:9)), que fijan el comportamiento
actual de `Inventory`, `Warehouse`, `User`, `Product` (incl. [CAT-03](Domain/06-business-rules.md)),
`ProductVariant`, `VariantAttribute` y `Money`. Evidencia inicial en
[verification-baseline.md :7](#verification-baselinemd-:7) y [:8](#:8).

`[CONFIRMADO]` [T-010](#t-010)/[T-004a](#t-004a) (2026-09-17, [ADR-0002](Adr/0002-clave-inventario-variantid.md)): se implementó `ProductVariant`
(SKU = `VariantId`) y `VariantAttribute`, `Product` gestiona sus variantes con
SKU único por producto, y `Inventory` cambió su clave de `ProductId` a `VariantId`.
El inventario dejó de referenciar el producto directamente: la unidad de stock es
la variante.

`[CONFIRMADO]` [T-010c](#t-010c) (2026-09-17, [ADR-0003](Adr/0003-variante-obligatoria-productos-fisicos.md), [Q-10](#q-10) = [C3](Adr/0003-variante-obligatoria-productos-fisicos.md)): `Product` hace cumplir
[CAT-03](Domain/06-business-rules.md) — un `Physical` exige ≥1 variante (constructor, `UpdateType`,
`RemoveVariant`) y `CanBeSold` exige variante vendible en físicos. Evidencia en
[verification-baseline.md :9](#verification-baselinemd-:9) (164/164 PASS).

`[RIESGO]` Sigue **abierto** [H-01](#h-01): `DispatchStock` valida `AvalibleQuantity` y
decrementa `ReservedQuantity`, por lo que puede dejar el reservado negativo. Su
corrección ([T-003b](#t-003b)) requiere aprobación porque cambia comportamiento observable y
la especificación no fija explícitamente la no-negatividad del contador reservado.
---

## 7. Estado y próximos pasos
### 7.1 Hito actual

**SPEC-006 (trazabilidad) y SPEC-007 (validación de entrada + RFC 7807 + higiene) — hechas.** La SSoT describe el estado real y [H-09](#h-09), [H-11](#h-11) y [H-12](#h-12) quedaron cerrados con 206/206 pruebas. Lo siguiente sigue dependiendo **exclusivamente del Owner**: [Q-13](#q-13) (bloqueante, Ley duplicada `DOMINIO 8/9/10`) y [Q-03](#q-03)/[Q-04](#q-04); sin ellas no se debe escribir más código de Fulfillment, Facturación ni Devoluciones.

### 7.2 Tareas [T-011](#t-011)…[T-022](#t-022) (estado real, hasta ahora no registrado)

| ID | Tarea (plan) | Estado real verificado | Evidencia |
|---|---|---|---|
| [T-011](#t-011) | `CustomerOrder` + líneas + estados | **Hecha en código, sin autorización** ([Q-04](#q-04) abierta) | `Orders/CustomerOrder.cs`, 6 pruebas |
| [T-012](#t-012) | 7 eventos de dominio + `IDomainEventDispatcher` | **NO hecha** | 0 archivos de eventos en el repo |
| [T-013](#t-013) | `InventoryReservationService` (+ desglose de reserva) | **NO hecha** | no existe el servicio ni `ReservationDetails` |
| [T-014](#t-014) | `OrderSplitterService` (N guías si hubo fraccionamiento) | **Parcial**: agregados `FulfillmentOrder`/`Shipment` sin servicio, sin fraccionamiento ni N guías | `Logistics/`, 3 pruebas |
| [T-015](#t-015) | Timeout de 15 min (`CheckoutTimeoutService`) | **NO hecha** (depende de `IClock`, inexistente) | [H-06](#h-06) abierto |
| [T-016](#t-016) | Devoluciones con doble aprobación (`ReturnsApprovalService`) | **Parcial**: agregado `ReturnRequest` con doble aprobación, **sin servicio y sin retorno a stock** | `Returns/ReturnRequest.cs`, [H-14](#h-14) |
| [T-017](#t-017) | Suspensión en cascada al bloquear vendedor | **NO hecha** | [invariante 6](Domain/04-invariants-and-rules.md) sin implementar |
| [T-018](#t-018) | 5 puertos de repositorio restantes | **Parcial**: 2 de 5 | `Zentric.Application/**/Ports/` |
| [T-019](#t-019) | `Zentric.Application`: puertos de entrada, CQRS, FluentValidation, `Result<T>` | **Parcial**: 3 commands, 2 puertos, `Result<T>`; **0 validadores y 0 pipeline** | [H-09](#h-09), [H-11](#h-11) |
| [T-020](#t-020) | `Zentric.Infrastructure`: EF Core + PostgreSQL, mapeos, migraciones | **Parcial**: `ZentricDbContext` (9 `DbSet`), 2 repositorios, 2 migraciones **nunca aplicadas** | E-021…E-023 |
| [T-021](#t-021) | `Zentric.Api`: controladores delgados + Problem Details | **Parcial**: 3 endpoints; `ApiControllerBase` mapea `Result` → RFC 7807, pero el middleware global está "simplificado" | [H-11](#h-11) |
| [T-022](#t-022) | Pruebas de integración y contrato (OpenAPI) | **NO hecha** | 178 pruebas, todas unitarias de dominio |

[RIESGO]` **Ninguna** de las tareas [T-019](#t-019)…[T-021](#t-021) se registró en el plan, en [verification-baseline.md](#verification-baselinemd) ni en la matriz durante su ejecución; este documento y [migration-to-sdd-plan.md](#migration-to-sdd-planmd) Fase 3b/4 son el primer registro. `[ACTUALIZACIÓN 2026-09-18]` **SPEC-007** mejoró [T-019](#c-007) (validadores + pipeline reales) y [T-021](#t-021) (`AddProblemDetails() + `UseExceptionHandler()`), cerrando [H-09](#h-09)/[H-11](#h-11); [T-020](#t-020) sigue igual (migraciones sin aplicar).

### 7.3 Supuestos por confirmar (no son reglas)

| Supuesto aplicado en código | Origen | Estado |
|---|---|---|
| `FulfillmentOrder` nace en `Packed` y omite `PendingPack` | `FulfillmentOrder.cs:33` ("Según dictamen…") | `[SUPUESTO]` → **[Q-13](#q-13)** |
| `InvoiceType` = `{Master, VendorDetail}` (sin "Detalle Zentric") | `InvoiceType.cs` | `[SUPUESTO]` → **[Q-13](#q-13)** |
| `CustomerOrder.TotalAmount` de carrito vacío = `Money(0, "USD")` | `CustomerOrder.cs:24` | `[SUPUESTO]` → **[Q-13](#q-13)** |
| Precio en el endpoint como `decimal UnitPrice` + `string Currency` | `OrdersController.cs:32` | `[SUPUESTO]` |
| Sub-decisiones 1-4 de [ADR-0003](Adr/0003-variante-obligatoria-productos-fisicos.md) | `Adr/0003` | `[PROPUESTO]` → **[Q-12](#q-12)** |

### 7.4 Bloqueos

1. **[Q-13](#q-13)** (duplicación `DOMINIO 8/9/10`) — bloquea toda corrección de Fulfillment, Facturación y Devoluciones.
2. **[Q-03](#q-03)** (estado de cancelación de despacho) y **[Q-04](#q-04)** (`Cart` como estado) — bloquean [T-013](#t-013)/[T-014](#t-014)/[T-015](#t-015).
3. **[Q-12](#q-12)** — impide cerrar [T-010c](#t-010c) como `[CONFIRMADO]`.
4. **[Q-07](#q-07)** (`IdentityDocument`) — bloquea [T-006](#t-006) y la conformidad con `RG-01` y :11 de la Ley.
5. Aprobación del Owner para **[T-003b](#t-003b)** (corregir [H-01](#h-01)), porque cambia comportamiento observable.

### 7.5 Registro de acciones ([skill :17.2](#skill-:172) [:7](#:7))](../.agents/skills/generic-sdd-agent/SKILL.md#172-registro-de-acciones-incidentes-enterprise-y-sesiones-largas-en-sddsddmd-7))

```text
[2026-09-18 · hora no registrada] Lectura de la SSoT completa (AGENTS.md, SDD/**, 5 .csproj, Zentric.Api/Application/Infrastructure, dominio nuevo, 4 suites nuevas) · objeto: E-001…E-030 · resultado: mapa de entidades al 91 %
[2026-09-18 · hora no registrada] `dotnet build Zentric.slnx --nologo` · resultado: Build succeeded, 0 warnings, 0 errores
[2026-09-18 · hora no registrada] `dotnet test Zentric.slnx --nologo` · resultado: 178/178 PASS (línea base anterior: 164)
[2026-09-18 · hora no registrada] Auditorías (git diff/status/numstat, Select-String ProjectReference/DbSet, Get-FileHash) · resultado: regla de dependencia OK, anti-amnesia OK, Ley intacta, skill desincronizada
[2026-09-18 · hora no registrada] Creado [SDD/SDD.md](#2026-09-18-·-hora-no-registrada]-creado-[sdd/sddmd) (SPEC-006) · objeto: E-028 · aprobado por: Owner (opción "trazabilidad primero" síncrona, 2026-09-18)
[2026-09-18 · hora no registrada] Corregido [AGENTS.md :0.0](#2026-09-18-·-hora-no-registrada]-corregido-[agentsmd-:00) (v6.0.0 + contexto persistente) · objeto: E-027
[2026-09-18 · hora no registrada] Corregido `scripts/sync-skill.ps1` (no borra el destino si falta `references/`) · objeto: E-027 · motivo: el script habría destruido la skill instalada y fallado
[2026-09-18 · hora no registrada] Sincronizada la skill instalada · resultado: hash repositorio = hash instalada
[2026-09-18 · hora no registrada] Validaciones NO ejecutadas: PostgreSQL, HTTP en runtime, mapeo EF en runtime, lint/CI
[2026-09-18 · hora no registrada] SPEC-007 inicio · se verificó con Select-String que `Class1` no estaba referenciado antes de borrar ([H-12](#c-007)) y se leyeron las guardas reales de `Money` para espejarlas en los validadores (cero reglas inventadas)
[2026-09-18 · hora no registrada] `dotnet build Zentric.slnx --nologo` · resultado: Build succeeded, 0 warnings, 0 errores (tras crear `ValidationBehavior<,>`, 3 validadores y editar `Program.cs` y el handler)
[2026-09-18 · hora no registrada] `dotnet test Zentric.slnx --nologo` (1.er intento) · resultado: **FAIL 7** de las pruebas de integración DI · causa verificada en el stack trace: `MediatR requires ILoggerFactory to be registered` desde `MediatRServiceCollectionExtensions.CheckLicense` → defecto del arnés, no del cableado
[2026-09-18 · hora no registrada] Corrección del arnés: `services.AddLogging()` + paquetes `Microsoft.Extensions.DependencyInjection` y `.Logging` 10.0.12 en `Zentric.Tests`
[2026-09-18 · hora no registrada] `dotnet build` + `dotnet test Zentric.slnx --nologo` (2.º intento) · resultado: **build 0/0 · 206/206 PASS** (178 + 21 unitarias + 7 de integración DI)
[2026-09-18 · hora no registrada] Archivos de producción modificados: 5 nuevos (`ValidationBehavior.cs`, 3 validadores) + 3 corregidos (`AddOrderItemCommand.cs`, `Program.cs`, csproj de pruebas) + 2 eliminados (`Class1.cs` ×2)
[2026-09-18 · hora no registrada] Nueva pregunta registrada: **[Q-14](#q-14)** (licenciamiento de MediatR 14) · objeto E-027/[R-19](#r-19) · aprobado por: pendiente del Owner
```

### 7.6 Próximos pasos (recomendación, en este orden)

1. **El Owner dicta [Q-13](#q-13)** (`DOMINIO 8/9/10`: numeración única y estados de despacho) y responde **[Q-03](#q-03)/[Q-04](#q-04)** → desbloquea Fulfillment, Facturación y Devoluciones.
2. ~~Cerrar [H-09](#h-09), [H-11](#h-11) y [H-12](#h-12)~~ → **HECHO en SPEC-007** (2026-09-18): `try-catch` eliminado, validadores FluentValidation + `ValidationBehavior<,>`, `AddProblemDetails()` + `UseExceptionHandler()` y `Class1.cs` borrados. 206/206 pruebas.
2b. **Pendiente sin decisión del Owner:** [T-032](#t-032) (pruebas HTTP con TestServer) y **[Q-14](#q-14)** (licenciamiento de MediatR). Ninguna bloquea el roadmap.
3. **Commit atómico de esta tanda de código** (`feat(orders,logistics,billing,returns): [T-011](#t-011)…[T-021](#t-021)`) — hoy hay ~1.400 líneas sin versionar, lo que impide cualquier rollback.

### Roadmap Completo de Tareas (T-xxx)
## 1. Priorización(regla B5)

Orden aplicado: (1) módulo que se modificará próximamente → inventario y ordering
son el núcleo; (2) flujos con dinero/permisos/auditoría → reserva, pago, ajuste
de stock, devolución; (3) contratos públicos → aún no existen; (4) bugs → [H-01](#h-01),
[H-02](#h-02); (5) ausencia de pruebas → 0 % de cobertura; (6) deuda bloqueante → [C-01](#c-01)…[C-07](#c-07).

## 2. Fases y tareas

### Fase 0 — Gobernanza y línea base · `[x]` HECHA

| ID | Tarea | Evidencia |
|---|---|---|
| T-000a | Skill SDD v3.0.0 (router + referencias + overlay) en `.agents/skills/` | `SKILL.md` + 11 referencias |
| T-000b | Bootstrap brownfield en [SDD.md](SDD.md) | 7 documentos |
| T-000c | Crear [SDD/01-system-overview.md](#sdd/01-system-overviewmd) y [SDD/02-software-architecture.md](#sdd/02-software-architecturemd) exigidos por [AGENTS.md :0.1](#agentsmd-:01) | ambos creados |
| T-000d | Línea base de verificación ejecutada | [verification-baseline.md](#verification-baselinemd) |
| T-000e | Matriz de conformidad spec ↔ código | [spec-conformance-matrix.md](#spec-conformance-matrixmd) |

### Fase 1 — Aseguramiento sin cambios de comportamiento (no bloqueada)

| ID | Tarea | Cubre | Verificación | Estado |
|---|---|---|---|---|
| T-001 | Crear proyecto `Zentric.Tests` (xUnit) y añadirlo a `Zentric.slnx` | [R-05](#r-05) | `dotnet test` en verde | `[x]` **hecha** |
| T-002 | Tests de caracterización de invariantes confirmadas: [INV-01](Domain/06-business-rules.md) (`Inventory`), reglas de `Warehouse`, usuario bloqueado (`User`), [CAT-01](Domain/06-business-rules.md) (`Product`), `Money` | [INV-01](Domain/06-business-rules.md), [CAT-01](Domain/06-business-rules.md), RG-02, RG-03 | 105/105 PASS | `[x]` **hecha** |
| T-002b | Tests de los VOs `Email` y `FullName` y del agregado `Buyer` | [ZENTRIC.md](/ZENTRIC.md) Dom.1 y Dom.2 | pendiente | `[ ]` |
| T-002c | Migrar `InventoryTests` de `ProductId` a `VariantId` + pruebas de `ProductVariant` y `VariantAttribute` | [ADR-0002](Adr/0002-clave-inventario-variantid.md) | 150/150 PASS | `[x]` **hecha** |
| T-010c | Hacer cumplir [CAT-03](Domain/06-business-rules.md) en `Product` (constructor, `UpdateType`, `RemoveVariant`, `CanBeSold`) | [ADR-0003](Adr/0003-variante-obligatoria-productos-fisicos.md) ([Q-10](#q-10) = [C3](Adr/0003-variante-obligatoria-productos-fisicos.md)) | 164/164 PASS ([verification-baseline.md :9](#verification-baselinemd-:9)) | `[x]` **hecha** (sub-decisiones [Q-12](#q-12) `[PROPUESTO]`) |
| T-007 | Higiene: `.editorconfig`, `TreatWarningsAsErrors`, CI mínimo (`dotnet build` + `dotnet test`) | [R-08](#r-08) | pipeline verde | `[ ]` |
| T-008 | Renombrado de lenguaje ubicuo ([Q-09](#q-09)) | [R-09](#r-09) | build + suite en verde | `[ ]` bloqueada por [Q-05](#q-05)/[Q-09](#q-09) |

### Fase 2 — Correcciones y modelo de inventario (bloqueada por decisiones)

| ID | Tarea | Cubre | Bloqueada por | Riesgo |
|---|---|---|---|---|
| T-003a | **Corregir [H-08](#h-08)**: `ReturnToAvalible` no validaba cantidad no positiva → violación real de [INV-01](Domain/06-business-rules.md) | [INV-01](Domain/06-business-rules.md) | 105/105 PASS con prueba de regresión | `[x]` **hecha** |
| T-003b | Corregir [H-01](#h-01) (`DispatchStock` valida el contador equivocado y puede dejar el reservado negativo) + prueba de regresión | [INV-01](Domain/06-business-rules.md), balance de stock | aprobación (cambia comportamiento observable) | `[ ]` pendiente de aprobación |
| T-004 | Rediseñar la API de `Inventory` (eliminar `UpdateQuantities`, alinear `Reserve`/`Release`/`Deduct`/`Adjust`) + abstracción de tiempo (`IClock`) + IDs fuertemente tipados (`record struct VariantId`, G-07) | [H-02](#h-02), [H-06](#h-06), G-07, [R-06](#r-06) | — ([Q-01](#q-01) y [Q-02](#q-02) resueltas) | alto |
| T-004a | Cambiar la clave del inventario: `Inventory.ProductId` → `Inventory.VariantId` | [INV-03](Domain/06-business-rules.md), [ADR-0002](Adr/0002-clave-inventario-variantid.md) | — | `[x]` **hecha** (riesgo medio) |
| T-005 | `ManualAdjust(qty, userId, role)` con regla dura de privilegios | [INV-04](Domain/06-business-rules.md) ([invariante 4](Domain/04-invariants-and-rules.md)) | [Q-02](#q-02) | medio |
| T-006 | `IdentityDocument` como VO obligatorio y único en `User` | [ZENTRIC.md](/ZENTRIC.md) Dom.1, RG-01 | [Q-07](#q-07) | medio |
| T-009 | Resolver `Buyer.PaymentTokens` vs [invariante 9](Domain/04-invariants-and-rules.md) | [invariante 9](Domain/04-invariants-and-rules.md) | [Q-08](#q-08) | medio |

### Fase 3 — Núcleo de negocio faltante (tras Fase 2)

| ID | Tarea | Cubre | Bloqueada por |
|---|---|---|---|
| T-010 | `ProductVariant` (SKU) y clave de inventario definitiva | [CAT-01](Domain/06-business-rules.md), [CAT-02](Domain/06-business-rules.md), [INV-03](Domain/06-business-rules.md) | `[x]` **hecha** ([ADR-0002](Adr/0002-clave-inventario-variantid.md)) |
| T-010b | Refinamiento del modelo de atributos de variante | `02-aggregates` :2 | [Q-11](#q-11) |
| T-010c | Hacer cumplir [CAT-03](Domain/06-business-rules.md) (variante obligatoria en físicos) en `Product` | [ADR-0003](Adr/0003-variante-obligatoria-productos-fisicos.md) ([Q-10](#q-10) = [C3](Adr/0003-variante-obligatoria-productos-fisicos.md)) | `[x]` **hecha** (sub-decisiones [Q-12](#q-12) `[PROPUESTO]`) |
| T-011 | `CustomerOrder` + `OrderLine` + `FlatShippingFee` + estados | [PED-02](Domain/06-business-rules.md), [PED-03](Domain/06-business-rules.md), [invariante 7](Domain/04-invariants-and-rules.md) | [Q-04](#q-04) |
| T-012 | Eventos de dominio (7) + `IDomainEventDispatcher` | [04-domain-events.md](Domain/04-domain-events.md) | ninguna |
| T-013 | `InventoryReservationService` con desglose de reserva (`ReservationDetails`) | [INV-02](Domain/06-business-rules.md) revisada ([ADR-0001](Adr/0001-reserva-fragmentacion-contingencia.md)), [PED-01](Domain/06-business-rules.md) | [Q-03](#q-03) ([Q-10](#q-10) resuelta: todo `Physical` trae variante) |
| T-014 | `OrderSplitterService` (`FulfillmentOrder` + `Shipment`, N guías si hubo fraccionamiento) | [ADR-0001](Adr/0001-reserva-fragmentacion-contingencia.md), [services/order-splitter-service.md](Domain/services/order-splitter-service.md) | **[Q-03](#q-03)** |
| T-015 | Timeout de 15 min (`CheckoutTimeoutService`) | [PED-01](Domain/06-business-rules.md) | T-004, T-011 |
| T-016 | Devoluciones con doble aprobación (`ReturnsApprovalService`) | [DEV-01](Domain/06-business-rules.md) | T-012 |
| T-017 | Suspensión en cascada al bloquear vendedor | [invariante 6](Domain/04-invariants-and-rules.md) | T-012 |
| T-018 | Puertos de repositorio restantes (5) | [05-ports.md :1](Domain/05-ports.md#1-puertos-de-repositorios-persistencia) | — (clave definida por [ADR-0002](Adr/0002-clave-inventario-variantid.md)) |

### Fase 4 — Capas exteriores

| ID | Tarea | Cubre |
|---|---|---|
| T-019 | `Zentric.Application`: puertos de entrada, casos de uso CQRS, FluentValidation, `Result<T>` | [AGENTS.md :2](#agentsmd-:2) |
| T-020 | `Zentric.Infrastructure`: EF Core + PostgreSQL, mapeos, migraciones | [AGENTS.md :2.1](#agentsmd-:21) |
| T-021 | `Zentric.Api`: controladores delgados + Problem Details (RFC 7807) | [AGENTS.md :3.4](#agentsmd-:34) |
| T-022 | Tests de integración y contrato (OpenAPI) | [AGENTS.md :4.4](#agentsmd-:44) |

### Fase 3b — Núcleo de negocio implementado **sin registrar** (auditado el 2026-09-18)

`[RIESGO]` Estas tareas **no se registraron al ejecutarse**; este es su primer registro. Varias se
ejecutaron con preguntas bloqueantes aún abiertas ([AGENTS.md :0.3](#agentsmd-:03) → freno de mano).

| ID | Tarea | Cubre | Estado real verificado | Evidencia |
|---|---|---|---|---|
| T-011 | `CustomerOrder` + `OrderItem` + estados | [PED-02](Domain/06-business-rules.md), [PED-03](Domain/06-business-rules.md), [invariante 7](Domain/04-invariants-and-rules.md) | **hecha en código, sin autorización** ([Q-04](#q-04) seguía abierta) | `Zentric.Domain/Orders/`, 6 pruebas |
| T-014 (parte) | `FulfillmentOrder` + `Shipment` | [ADR-0001](Adr/0001-reserva-fragmentacion-contingencia.md), [order-splitter-service.md](Domain/services/order-splitter-service.md) | **parcial**: agregados sin servicio, sin fraccionamiento ni N guías, sin `PendingPack` | `Zentric.Domain/Logistics/`, 3 pruebas |
| T-016 (parte) | Doble aprobación de devoluciones | [DEV-01](Domain/06-business-rules.md) | **parcial**: agregado `ReturnRequest`, **sin** servicio y **sin** retorno a stock | `Zentric.Domain/Returns/`, 3 pruebas |
| T-018 (parte) | Puertos de repositorio | [05-ports.md :1](Domain/05-ports.md#1-puertos-de-repositorios-persistencia) | **parcial**: 2 de 5 (`ICustomerOrderRepository`, `IFulfillmentOrderRepository`) | `Zentric.Application/**/Ports/` |
| — | `Invoice` (maestra / detalle de vendedor) | [ADD-003](#add-003) de la Ley | **parcial**: falta "Detalle Zentric" | `Zentric.Domain/Billing/`, 2 pruebas |
| — | `ProductStatus` (Published/Suspended/Discontinued) | [ZENTRIC.md](/ZENTRIC.md) Dom.5 | **creado sin integrar** en `Product` (sigue con `bool IsActive`) | `Products/Enums/ProductStatus.cs` |

### Fase 4 — Capas exteriores (auditada el 2026-09-18)

| ID | Tarea | Estado real verificado | Evidencia / hallazgo |
|---|---|---|---|
| T-019 | `Zentric.Application`: puertos de entrada, CQRS, FluentValidation, `Result<T>` | **parcial** | 3 commands + 2 puertos + `Result<T>`; **0 validadores y 0 pipeline** ([H-11](#h-11)) y `try-catch` genérico ([H-09](#h-09)) |
| T-020 | `Zentric.Infrastructure`: EF Core + PostgreSQL, mapeos, migraciones | **parcial** | `ZentricDbContext` con **9 `DbSet`** (anti-amnesia OK), 2 repositorios, 2 migraciones **nunca aplicadas**; mapea agregados de dominio ([H-13](#h-13)) |
| T-021 | `Zentric.Api`: controladores delgados + Problem Details | **parcial** | 3 endpoints (`/api/orders/cart`, `/api/orders/{id}/items`, `/api/logistics/fulfillment`); middleware "simplificado" sin `AddProblemDetails()` ([H-11](#h-11)) |
| T-022 | Tests de integración y contrato | **no hecha** | 178 pruebas, todas unitarias de dominio |

### Fase 5 — Deuda de gobernanza cerrada en esta auditoría

| ID | Tarea | Estado |
|---|---|---|
| T-023 | Crear [SDD/SDD.md](#sdd/sddmd) (memoria viva exigida por la skill v6.0.0) | `[x]` **hecha** |
| T-024 | Sincronizar la skill instalada y corregir `sync-skill.ps1` | `[x]` **hecha** (el script habría borrado la skill instalada y fallado) |
| T-025 | Alinear [AGENTS.md :0.0](#agentsmd-:00) con la skill v6.0.0 (ya no usa `references/`) | `[x]` **hecha** |
| T-026 | Registrar [Q-13](#q-13) (duplicación de `DOMINIO 8/9/10` en la Ley) | `[x]` **registrada**, pendiente de dictamen |

### Fase 6 — Corrección de incumplimientos de [AGENTS.md](#agentsmd) (SPEC-007, 2026-09-18)

`[x]` HECHA. Alcance acotado a lo que **no** depende de [Q-13](#q-13)/[Q-03](#q-03)/[Q-04](#q-04).

| ID | Tarea | Cubre | Verificación | Estado |
|---|---|---|---|---|
| T-027 | `ValidationBehavior<,>` (pipeline de MediatR) que devuelve `Result.Failure` en vez de lanzar | [ValidationBehavior.cs:3](../Zentric.Application/Common/Behaviors/ValidationBehavior.cs#L3).2, [ValidationBehavior.cs:4](../Zentric.Application/Common/Behaviors/ValidationBehavior.cs#L4).3 · FR-01/FR-02 | 4 pruebas del comportamiento | `[x]` **hecha** |
| T-028 | 3 validadores FluentValidation (Orders y Logistics), espejo de las guardas del dominio | :4.3 · FR-01 | 17 pruebas de validadores | `[x]` **hecha** |
| T-029 | Eliminar el `catch (Exception)` genérico de `AddOrderItemCommandHandler` ([H-09](#h-09)) | [AddOrderItemCommand.cs[:3](#addorderitemcommandcs[:3)](../Zentric.Application/Orders/Commands/AddOrderItemCommand.cs#L3).2 · FR-03 | regresión de [H-09](#h-09) en verde | `[x]` **hecha** |
| T-030 | `AddProblemDetails()` + `UseExceptionHandler()` + registro de validadores y del comportamiento ([H-11](#h-11)) | :3.4 · FR-04 | 7 pruebas de integración DI | `[x]` **hecha** |
| T-031 | Eliminar `Class1.cs` de Application e Infrastructure ([H-12](#h-12)) | DoD [:8](#:8) · FR-05 | `Test-Path` = `False` | `[x]` **hecha** |
| T-032 | Pruebas HTTP de extremo a extremo (TestServer) | CA-04 | `[PENDIENTE]`: la API no se levantó | `[ ]` pendiente |
| [Q-14](#q-14) | Confirmar condiciones de licencia de MediatR 14 / FluentValidation (hallazgo de SPEC-007) | [R-19](#r-19) | pendiente de dictamen del Owner | `[ ]` bloqueante de dependencias |

Evidencia: [verification-baseline.md :11](#verification-baselinemd-:11) (`build 0/0` · `206/206 PASS`).

## 8. Investigación y discovery
### 8.1 Discovery Brief (2026-09-18)

- **Qué se hizo:** auditoría de solo lectura de la SSoT + verificación ejecutada de build/pruebas + creación de la memoria viva.
- **Estado del sistema:** 5 proyectos, arquitectura hexagonal correcta, 178/178 pruebas verdes, dominio completo para 9 agregados, capas `Application`/`Infrastructure`/`Api` funcionalmente mínimas.
- **Riesgo dominante:** la Ley se contradice a sí misma en `DOMINIO 8/9/10` y el código ya tomó una interpretación **no dictada** por el Owner → cualquier trabajo nuevo sobre esos dominios multiplica el retrabajo.
- **Recomendación:** no escribir más código de Fulfillment/Facturación/Devoluciones hasta [Q-13](#q-13); mientras tanto, cerrar [H-09](#h-09)/[H-11](#h-11)/[H-12](#h-12) y versionar lo existente.

### 8.2 Research Log

| Fecha | Fuente | Hallazgo |
|---|---|---|
| 2026-09-18 | `git diff --numstat [ZENTRIC.md](/ZENTRIC.md)` | La Ley solo tiene adiciones (38/1); el texto original no fue reescrito |
| 2026-09-18 | `Get-FileHash` de la skill | Repo v6.0.0 ≠ instalada v3.1.0 (la carga automática usaba la versión antigua) |
| 2026-09-18 | Búsqueda por patrón sobre carpetas sin versionar | La herramienta de búsqueda devuelve falsos negativos ahí; se cambió a escaneo directo por archivo |
| 2026-09-18 | Paquete NuGet `MediatR` 14.2.0 (`MediatR.xml` + stack trace) | Confirmada la firma `Handle(request, RequestHandlerDelegate<TResponse>, CancellationToken)` y `AddOpenBehavior(Type, ServiceLifetime)`; MediatR 14 exige `ILoggerFactory` y ejecuta comprobación de licencia ([Q-14](#q-14)) |
| 2026-09-18 | `Zentric.Domain/Products/ValueObjects/Money.cs` | Guardas reales: `amount >= 0`, `currency` no vacía y de 3 caracteres tras `Trim().ToUpperInvariant()` → reglas espejadas en `AddOrderItemCommandValidator`, no inventadas |

