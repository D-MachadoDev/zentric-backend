# Questions for Owner — decisiones que el código no puede responder

Cada pregunta incluye opciones, consecuencias y qué desbloquea. Ninguna se
resolvió por decisión del agente (regla de cero asunciones, [AGENTS.md :0.3](../../AGENTS.md#03-el-freno-de-mano-cero-asunciones)).

## [Q-01](#q-01-c-01-se-permite-fraccionar-la-reserva-entre-bodegas-resuelta-2026-09-17) ([C-01](risks-and-gaps.md)) — ¿Se permite fraccionar la reserva entre bodegas? · ✅ RESUELTA (2026-09-17)

**Decisión del owner: A3 híbrido.**

- **Regla base:** si una bodega individual cubre la cantidad total de la línea, la
  reserva se hace íntegramente en esa bodega (el envío no se divide).
- **Excepción (umbral exacto):** solo si **ninguna** bodega individual cubre la
  cantidad, se fracciona entre varias bodegas, de mayor a menor stock y con
  prioridad `Marketplace`.
- **Fallo:** si la suma de todas las bodegas no alcanza, la operación falla y se
  liberan las reservas parciales de esa misma transacción.
- **Logística:** cuando hubo fraccionamiento se crean múltiples `Shipment`.

Registrado en [`SDD/Adr/0001-reserva-fragmentacion-contingencia.md`](../Adr/0001-reserva-fragmentacion-contingencia.md) y aplicado a
[`SDD/Domain/04-invariants-and-rules.md`](../Domain/04-invariants-and-rules.md) (invariante 3),
[`SDD/Domain/06-business-rules.md`](../Domain/06-business-rules.md) (INV-02) y
[SDD/Domain/services/inventory-reservation-service.md :4](../Domain/services/inventory-reservation-service.md#4-flujo-logico-y-reglas-invariantes).

**Desbloquea:** [T-004](migration-to-sdd-plan.md), [T-013](migration-to-sdd-plan.md), [T-014](migration-to-sdd-plan.md) (estas dos últimas quedan pendientes solo de [Q-02](#q-02-c-02-la-clave-del-inventario-es-el-producto-o-la-variante-sku-resuelta-2026-09-17)/[Q-03](#q-03-c-03-estado-de-cancelacion-de-despacho)).

### Contexto histórico de la contradicción

- **Fuente A** — [SDD/Domain/04-invariants-and-rules.md :8](../Domain/04-invariants-and-rules.md)`: "Toda la cantidad
  solicitada de una Variante específica en una `OrderLine` debe poder surtirse
  desde **una (1) sola bodega**… Los envíos de un mismo SKU no se dividen".
- **Fuente B** — [SDD/Domain/06-business-rules.md :7](../Domain/06-business-rules.md)` (INV-02) y
  [SDD/Domain/services/inventory-reservation-service.md :18](../Domain/services/inventory-reservation-service.md)`: "el servicio tiene
  permitido **fraccionar** la reserva en ambas bodegas".

| Opción | Consecuencia |
|---|---|
| **A1. Ganar A (no fraccionar)** | Regla simple; menos envíos; se rechaza o recorta la compra si ninguna bodega cubre la cantidad. `OrderSplitterService` crea 1 `Shipment` por línea |
| **A2. Ganar B (fraccionar)** | Se requiere `ReservationDetails` con desglose por bodega, múltiples `Shipment` por variante pese a "evitar costos exorbitantes", y compensación al liberar |
| **A3. Híbrido** | Fraccionar salvo que ninguna combinación cubra la cantidad total; documentar el umbral exacto |

**Desbloquea:** G-01, G-02, G-05, G-06, [R-04](risks-and-gaps.md). **No se ha tocado código de inventario.**

## [Q-02](#q-02-c-02-la-clave-del-inventario-es-el-producto-o-la-variante-sku-resuelta-2026-09-17) ([C-02](risks-and-gaps.md)) — ¿La clave del inventario es el producto o la variante (SKU)? · ✅ RESUELTA (2026-09-17)

**Decisión del owner: B2 — inventario por `VariantId` (SKU).**

- Se crea `ProductVariant` como entidad hija del agregado `Product`; su `Id` **es**
  el `VariantId` y actúa como SKU del inventario.
- `Inventory.VariantId` sustituye a `Inventory.ProductId`. Clave de stock:
  `(VariantId, WarehouseId)`.
- La variante se compone de atributos (`VariantAttribute`: nombre + valor).
- El SKU es único dentro del producto.
- **No decidido por B2:** si todo producto debe tener al menos una variante →
  nueva pregunta **[Q-10](#9-cuarta-iteracion-adr-0003-variante-obligatoria-en-fisicos-q-10-c3)**. Detalle del modelo de atributos → **[Q-11](#q-11-detalle-del-modelo-de-atributos-de-variante-abierta)**.

Registrado en [`SDD/Adr/0002-clave-inventario-variantid.md`](../Adr/0002-clave-inventario-variantid.md) y aplicado a
[`SDD/Domain/01-models.md`](../Domain/01-models.md), [`SDD/Domain/06-business-rules.md`](../Domain/06-business-rules.md) (INV-03),
[`SDD/Domain/05-ports.md`](../Domain/05-ports.md) y [`SDD/Domain/services/inventory-reservation-service.md`](../Domain/services/inventory-reservation-service.md).

### Contexto histórico de la contradicción

- **Fuente A** — [02-aggregates-and-entities.md :29](../Domain/02-aggregates-and-entities.md)`, [03-value-objects.md :15](../Domain/03-value-objects.md)`:
  `VariantId` como SKU.
- **Fuente B** — [01-models.md :33](../Domain/01-models.md)` y [`ZENTRIC.md`](../Domain/ZENTRIC.md) Dominio 6: "vinculado
  obligatoriamente a un producto y una bodega"; el código usa `ProductId`.
- [`ZENTRIC.md`](../Domain/ZENTRIC.md) Dominio 5 sí menciona "Variantes: Diferencias de color, talla,
  modelo".

| Opción | Consecuencia |
|---|---|
| **B1. Inventario por `ProductId`** | Conforme al código actual; las variantes quedan como atributo informativo; no se puede controlar stock por talla/color |
| **B2. Inventario por `VariantId`** | Exige crear `ProductVariant`, cambiar `Inventory` y migrar datos futuros; es el modelo de marketplace real |
| **B3. `ProductVariant` obligatoria** | Todo producto tiene al menos 1 variante (la unidad); inventario siempre por SKU — el más consistente a largo plazo |

**Desbloquea:** G-07, G-10 y el diseño del esquema de inventario. **G-08 cerrada**
(`ProductVariant` implementada en código y en spec).

## [Q-03](#q-03-c-03-estado-de-cancelacion-de-despacho) ([C-03](risks-and-gaps.md)) — Estado de cancelación de despacho

`Cancelled` ([02-value-objects.md :58](../Domain/02-value-objects.md)`) vs `CancelledByStockBreak`
([03-value-objects.md :41](../Domain/03-value-objects.md)`). Opciones: **(a)** un único `Cancelled` con motivo
(`CancellationReason`); **(b)** dos estados distintos; **(c)** `Cancelled` +
campo `Reason` obligatorio. Desbloquea G-02.

## [Q-04](#q-04-c-04-cart-es-un-estado-de-customerorder) ([C-04](risks-and-gaps.md)) — ¿`Cart` es un estado de `CustomerOrder`?

[02-value-objects.md :46](../Domain/02-value-objects.md)` y `07-lifecycle` :1 lo incluyen; [03-value-objects.md :29](../Domain/03-value-objects.md)`
no. Opciones: **(a)** `Cart` es estado del agregado (con timeout de 15 min);
**(b)** el carrito es un agregado separado (`Cart`) y `CustomerOrder` nace en
`PendingPayment`; **(c)** `Cart` y `Order` comparten el agregado pero el carrito
nunca se factura. Desbloquea G-01 y G-10.

## [Q-05](#q-05-c-05-vendor-o-seller) ([C-05](risks-and-gaps.md)) — ¿`Vendor` o `Seller`?

La spec de dominio dice `Vendor`; el código dice `Seller`. Opciones: **(a)**
unificar a `Vendor` (renombrar código y `WarehouseType`); **(b)** unificar a
`Seller` (actualizar las specs); **(c)** mantener `Seller` en código y registrar
`Vendor` como sinónimo prohibido en el glosario. Recomendación del agente:
**(a)** por jerarquía de verdad (la spec manda) y porque aún no hay contratos.
Desbloquea [R-09](risks-and-gaps.md) y [T-008](migration-to-sdd-plan.md).

## [Q-06](#q-06-c-07-consolidacion-de-los-documentos-numerados) ([C-07](risks-and-gaps.md)) — Consolidación de los documentos numerados

`SDD/Domain/` tiene parejas solapadas: `01-domain-overview`/`01-models`,
`02-aggregates`/`02-value-objects` (no son pareja real), `03-domain-services`/
`03-value-objects`, `04-domain-events`/`04-invariants-and-rules`. Opciones:
**(a)** renumerar por tema canónico (`01-overview`, `02-models`, `03-value-objects`,
`04-aggregates`, `05-invariants`, `06-business-rules`, `07-lifecycle`,
`08-domain-events`, `09-domain-services`, `10-ports`); **(b)** fusionar los
solapados en un documento por tema; **(c)** dejar como está y declarar un índice
canónico en `SDD/Domain/00-index.md`. Desbloquea la navegabilidad de la SSoT.

## [Q-07](#q-07-documento-de-identidad-del-usuario) — Documento de identidad del usuario

[`ZENTRIC.md`](../Domain/ZENTRIC.md) Dominio 1 lo marca obligatorio y único, y [`AGENTS.md`](../../AGENTS.md) prohíbe
inventar. Opciones: **(a)** añadir `IdentityDocument` como VO obligatorio con
validación de formato (¿qué país/formato?); **(b)** añadirlo opcional; **(c)**
eliminarlo de la spec por no aplicar al negocio. Si es (a), se necesita el
formato exacto. Desbloquea [R-03](risks-and-gaps.md)/[T-006](migration-to-sdd-plan.md).

## [Q-08](#q-08-r-07-buyerpaymenttokens-contra-la-invariante-9) ([R-07](risks-and-gaps.md)) — `Buyer.PaymentTokens` contra la invariante 9

La invariante 9 dice: "No se almacenan tarjetas de crédito ni billeteras… el
Dominio avanza de estado con una simple entidad `PaymentReceipt`". El código
almacena `PaymentTokens`. Opciones: **(a)** eliminar `PaymentTokens` y crear
`PaymentReceipt`; **(b)** mantener tokens (referencias opacas del proveedor, no
datos de tarjeta) y **enmendar** la invariante 9; **(c)** dejar `PaymentTokens`
fuera del agregado `Buyer`. Requiere además la decisión de la pasarela.

## [Q-09](#q-09-naming-canonico-pendiente) — Naming canónico pendiente

Decisiones menores pero propagables: `Inventory` → `InventoryItem`;
`Avalible` → `Available` (typo); `Email` → `EmailAddress`; `Administrator` →
`Admin`; `Product.Name` → `Product.Title`; `Warehouse.SellerId` → `OwnerId`;
`User.FullName` como VO (la spec dice `string`). ¿Se autoriza el renombrado
masivo ahora (coste mínimo) o se difiere? Desbloquea [T-008](migration-to-sdd-plan.md).

## [Q-10](#9-cuarta-iteracion-adr-0003-variante-obligatoria-en-fisicos-q-10-c3) — ¿Es obligatorio que todo producto tenga al menos una variante? · ✅ RESUELTA (2026-09-17)

**Decisión del owner: C3 — obligatoria solo para `Physical`.**

- Un producto `Physical` **exige al menos una variante** (la variante es la
  unidad de stock, INV-03).
- Un producto `Digital` (CAT-02: sin logística ni inventario) **puede** nacer sin
  variantes.
- Aplicación en código ([T-010c](migration-to-sdd-plan.md), ADR-0003): constructor rechaza `Physical` sin
  variantes; `UpdateType(Physical)` exige variante; `RemoveVariant` no puede
  dejar un `Physical` sin variantes; `CanBeSold` exige variante vendible en
  `Physical`. Suite: 164/164 PASS.

Registrado en [`SDD/Adr/0003-variante-obligatoria-productos-fisicos.md`](../Adr/0003-variante-obligatoria-productos-fisicos.md) y aplicado
a [SDD/Domain/01-models.md :2](../Domain/01-models.md#2-bounded-context-catalog-catalogo) y [`SDD/Domain/06-business-rules.md`](../Domain/06-business-rules.md) (CAT-03).

### Contexto histórico de la pregunta

[Q-02](#q-02-c-02-la-clave-del-inventario-es-el-producto-o-la-variante-sku-resuelta-2026-09-17) (B2) decidió que el inventario se lleva por `VariantId`, pero **no** decidió
si un producto puede existir sin variantes.

| Opción | Consecuencia |
|---|---|
| **C1. Variante opcional** | Un producto sin variantes no puede tener inventario ni reservarse. Sirve para productos digitales o catálogos informativos, pero deja un estado inalcanzable para productos físicos |
| **C2. Variante obligatoria (equivalente a B3)** | Todo producto nace con al menos una variante "unidad". Elimina el estado inválido, pero cambia el alta de producto y las pruebas de catálogo |
| **C3. Obligatoria solo para `Physical`** | Los productos `Digital` (CAT-02: sin logística ni inventario) quedan sin variante; los físicos siempre tienen una |

**Impacto si no se decide:** `InventoryReservationService` no puede definir qué
hacer con un producto físico sin variantes (¿error de especificación o compra
rechazada?). **Bloqueaba:** [T-013](migration-to-sdd-plan.md), [T-015](migration-to-sdd-plan.md), [T-016](migration-to-sdd-plan.md). **Sub-decisiones propuestas que
requieren confirmación:** ver **[Q-12](#sub-decisiones-propuesto-ver-q-12)**.

## [Q-11](#q-11-detalle-del-modelo-de-atributos-de-variante-abierta) — Detalle del modelo de atributos de variante · ABIERTA

`VariantAttribute` (nombre + valor) fue `[PROPUESTO]` por el agente a partir de
[02-aggregates-and-entities.md :2](../Domain/02-aggregates-and-entities.md#2-catalog-module) ("maneja las combinaciones (ej. Talla/Color)").
Puntos a confirmar:

1. ¿Nombre/valor libres o catálogo cerrado de atributos por producto?
2. ¿Un atributo puede repetirse con distinto valor? Hoy se prohíbe repetir el
   mismo nombre dentro de una variante.
3. ¿Algún atributo es obligatorio (ej. talla en ropa)?
4. Límite de longitud propuesto: 50 caracteres por nombre/valor (defensivo, no
   definido en la spec).

**Bloquea:** [T-010b](migration-to-sdd-plan.md) (refinamiento de catálogo). **No** bloquea la reserva ni el inventario.

## [Q-12](#sub-decisiones-propuesto-ver-q-12) — Sub-decisiones `[PROPUESTO]` de ADR-0003 (variante obligatoria) · ABIERTA

Al aplicar [Q-10](#9-cuarta-iteracion-adr-0003-variante-obligatoria-en-fisicos-q-10-c3) = C3 el agente tomó 4 sub-decisiones que **van más allá del texto
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

**Si confirmas:** se marcan `[CONFIRMADO]` en ADR-0003. **Si corriges alguna:**
indica el número (p. ej. `[Q-12](#sub-decisiones-propuesto-ver-q-12).2 = no`) y se ajusta código + pruebas + ADR.
**Bloquea:** el cierre definitivo de [T-010c](migration-to-sdd-plan.md) (el código ya está aplicado y en
verde, pero con etiqueta `[PROPUESTO]` en estos 4 puntos).

## [Q-13](#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante) ([C-08](risks-and-gaps.md)) — La Ley define `DOMINIO 8`, `9` y `10` **dos veces**, con significados cruzados · 🔴 ABIERTA · **BLOQUEANTE**

**Hallazgo (2026-09-18).** [`SDD/Domain/ZENTRIC.md`](../Domain/ZENTRIC.md) contiene hoy **dos bloques** que definen los mismos
números de dominio con contenidos distintos: el bloque base (líneas 254-270) y el bloque
`# [ADDENDUM - DICTADO POR OWNER]` (líneas 320-334).

| Nº | Bloque base (líneas 254-270) | Bloque ADDENDUM (líneas 320-334) |
|---|---|---|
| 8 | **5 estados**: Pendiente de Empaque → Empacado → Despachado → Entregado → Cancelado por Quiebre; "deriva del pedido del cliente" | "**Estados: Empacado y Despachado**" + Regla de Stock Fantasma (cancelación con devolución obligatoria) |
| 9 | **Facturación y Pagos**: validación de transacciones, generación de facturas, conciliación y split | **Devoluciones y Reembolsos**: prohibición de digitales + flujo físico (inspección → aprobación del Vendedor → vuelta al stock "Usado") |
| 10 | **Devoluciones y Reembolsos**: 4 estados (Solicitada, Aprobada, Rechazada, Reembolsada) | **Facturación y Pagos**: Factura Maestra · Detalle Zentric · Factura de Vendedor (Split) |

`[RIESGO]` El código de la Fase 3b/4 **ya tomó una interpretación que nadie dictó**:
`FulfillmentStatus` sin `PendingPack` (`FulfillmentOrder.cs:33`, "Según dictamen…"), `InvoiceType`
sin "Detalle Zentric", y la devolución aprobada **no** devuelve stock ([H-14](spec-conformance-matrix.md)). No es una decisión
registrada: es una asunción ([AGENTS.md :0.3](../../AGENTS.md#03-el-freno-de-mano-cero-asunciones)).

| Opción | Consecuencia |
|---|---|
| **[Q-13](#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante).a — Manda el bloque base** | Dom.8 con los **5 estados** (el código debe añadir `PendingPack` y renombrar a `Shipped`/`CancelledByStockBreak` si aplica); Dom.9 = Facturación; Dom.10 = Devoluciones. El ADDENDUM queda como **ampliación** de reglas, no de numeración |
| **[Q-13](#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante).b — Manda el ADDENDUM** | Dom.8 con 2 estados (Empacado y Despachado); Dom.9 = Devoluciones; Dom.10 = Facturación con los 3 documentos. Se debe **renumerar** la Ley y avisar que el bloque base queda derogado por este punto |
| **[Q-13](#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante).c — Fusión por tema (recomendación del agente, `[PROPUESTO]`)** | Dom.8 = **5 estados** de despacho; Dom.9 = **Devoluciones** (prohibición de digitales + flujo físico con stock "Usado"); Dom.10 = **Facturación** (Maestra, Detalle Zentric, Vendedor). Se conserva lo más específico de cada bloque y se renumeran los títulos |
| **[Q-13](#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante).d — Otra numeración** | El Owner dicta el texto exacto y el agente lo registra como ADDENDUM sin alterar el original |

**Desbloquea:** corrección de `FulfillmentStatus`, `InvoiceType`, el flujo de devolución a stock
([H-14](spec-conformance-matrix.md)), [T-013](migration-to-sdd-plan.md)/[T-014](migration-to-sdd-plan.md)/[T-016](migration-to-sdd-plan.md) y el cierre de [R-11](risks-and-gaps.md)/[R-12](risks-and-gaps.md)/[R-13](risks-and-gaps.md).

**Pregunta adicional ([C-10](risks-and-gaps.md)):** el bloque base de `DOMINIO 8/9/10` **no lleva** el rótulo
`[ADDENDUM - DICTADO POR OWNER]` que exige [AGENTS.md :0.7](../../AGENTS.md#07-inmutabilidad-de-los-documentos-biblia-y-registro-de-cambios). ¿Autorizas que se le añada el rótulo
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
| **d. Diferirlo** | Riesgo de cumplimiento si el producto se comercializa; se registra como deuda ([R-19](risks-and-gaps.md)) |

**Impacto actual:** ninguno en build ni en las 206 pruebas. **Bloquea:** nada del roadmap vigente;
afecta a la sostenibilidad y al cumplimiento de la dependencia.

## Cómo responder

Basta indicar el identificador y la opción elegida (p. ej. `[Q-01](#q-01-c-01-se-permite-fraccionar-la-reserva-entre-bodegas-resuelta-2026-09-17) = A1, [Q-05](#q-05-c-05-vendor-o-seller) = a`).
Con eso el agente actualiza las specs afectadas, registra la decisión en
`SDD/Adr/` si aplica y desbloquea las tareas correspondientes.
