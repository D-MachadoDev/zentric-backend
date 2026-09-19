# 01. System Overview — Zentric

> Documento exigido por [AGENTS.md:0.1](../AGENTS.md#01-consulta-obligatoria-antes-de-codificar). **Derivado**, no inventado: consolida y
> enlaza la especificación funcional de negocio existente
> ([SDD/Domain/ZENTRIC.md](Domain/ZENTRIC.md)) y los documentos de dominio. No introduce reglas
> nuevas. Todo lo marcado `[INFERIDO]` requiere confirmación.

## 1. Qué es Zentric

`[CONFIRMADO]` [SDD/Domain/ZENTRIC.md :1](Domain/ZENTRIC.md#dominio-1-administracion-de-usuarios): plataforma digital centralizada que
actúa como **intermediario comercial** entre compradores y vendedores. Administra
la operación completa: registro de usuarios, publicación de productos, logística,
facturación y posventa, con trazabilidad y coordinación entre participantes.

## 2. Objetivos estratégicos

`[CONFIRMADO]` [ZENTRIC.md :2](Domain/ZENTRIC.md#dominio-2-gestion-de-compradores):

| Código | Objetivo |
|---|---|
| OBJ-01 | Administrar la información de todos los usuarios del Marketplace |
| OBJ-02 | Gestionar el registro y administración de vendedores |
| OBJ-03 | Administrar compradores registrados |
| OBJ-04 | Controlar la información de las bodegas |
| OBJ-05 | Gestionar el catálogo de productos |
| OBJ-06 | Administrar el inventario distribuido |
| OBJ-07 | Gestionar el carrito de compras |
| OBJ-08 | Controlar el ciclo completo de los pedidos |
| OBJ-09 | Administrar la facturación de las compras |
| OBJ-10 | Gestionar los procesos logísticos |
| OBJ-11 | Administrar devoluciones y reembolsos |
| OBJ-12 | Consolidar información administrativa para consulta |

## 3. Alcance

`[CONFIRMADO]` [ZENTRIC.md :3.1](Domain/ZENTRIC.md#31-procesos-incluidos) incluye: registro de compradores, administración
de usuarios, registro **administrativo** de vendedores, administración de bodegas,
catálogo, inventario, carrito, pedidos, facturación, envíos, devoluciones,
reembolsos y consulta de reportes administrativos.

`[CONFIRMADO]` [ZENTRIC.md :3.2](Domain/ZENTRIC.md#32-procesos-fuera-del-alcance) excluye explícitamente: interfaces gráficas,
aplicaciones móviles, portales web, mecanismos de autenticación técnica,
tecnologías de implementación, arquitectura del software y almacenamiento de la
información. **Este repositorio cubre el core de dominio y su API, no la UI.**

## 4. Participantes y roles

`[CONFIRMADO]` [ZENTRIC.md :5](Domain/ZENTRIC.md#dominio-5-gestion-del-catalogo): Comprador, Vendedor, Operador Logístico,
Administrador y Supervisor. Cada participante desempeña **un único rol**.

`[CONFIRMADO]` [AGENTS.md :1](../AGENTS.md#1-vision-general-del-proyecto) enumera: vendedores, compradores, bodegas,
productos, catálogos y pedidos.

## 5. Modelo operativo (flujo extremo a extremo)

`[CONFIRMADO]` [ZENTRIC.md :6.1](Domain/ZENTRIC.md#61-flujo-general-del-negocio):

1. **Incorporación** — el Administrador registra al vendedor y su primera bodega.
2. **Catálogo** — el vendedor registra productos y características.
3. **Inventario** — se registran existencias iniciales en las bodegas.
4. **Publicación** — los productos se hacen visibles en el catálogo.
5. **Compra** — el comprador selecciona productos vía carrito y confirma.
6. **Transacción** — se valida el pago y se inicia la preparación.
7. **Logística** — empaque, despacho y transporte.
8. **Cierre** — el pedido se marca finalizado tras entrega confirmada.

## 6. Bounded context y módulos

`[CONFIRMADO]` [SDD/Domain/01-domain-overview.md :2](Domain/01-domain-overview.md#2-limite-de-contexto-context-boundary): **un único Bounded Context
(Unified Domain)** dividido en módulos de dominio de alta cohesión:

| Módulo | Responsabilidad | Documento de referencia |
|---|---|---|
| Identity | Usuarios y estado comercial de participantes | [SDD/Domain/01-models.md :1](Domain/01-models.md#1-bounded-context-identity-access-usuarios) |
| Catalog | Oferta de vendedores, productos y variantes | [SDD/Domain/01-models.md :2](Domain/01-models.md#2-bounded-context-catalog-catalogo) |
| Inventory | Inventario distribuido, bodegas y cantidades | [SDD/Domain/01-models.md :3](Domain/01-models.md#3-bounded-context-inventory-inventario-y-bodegas) |
| Ordering | Carrito, pedido maestro, facturación centralizada | [SDD/Domain/01-models.md :4](Domain/01-models.md#4-bounded-context-ordering-pedidos---interfaz-del-comprador) |
| Fulfillment | Órdenes de despacho por vendedor y posventa | [SDD/Domain/01-models.md :5](Domain/01-models.md#5-bounded-context-fulfillment-logistics-despachos---interfaz-del-vendedor) |

## 7. Restricciones transversales

`[CONFIRMADO]` [ZENTRIC.md :10](Domain/ZENTRIC.md#dominio-10-gestin-de-facturacin-y-pagos) y [:11](Domain/ZENTRIC.md#11-validaciones-criticas):

| Código | Restricción |
|---|---|
| RG-01 | Toda operación debe ejecutarse por un usuario autenticado |
| RG-02 | Cada usuario tendrá un único rol dentro del sistema |
| RG-03 | Ningún participante podrá administrar información fuera de su rol |

Validaciones críticas: no reservar inventario inexistente o marcado como dañado ·
un pedido finalizado no puede modificarse · documento de identidad y correo
electrónico únicos en la plataforma.

## 8. Reglas de negocio consolidadas

`[CONFIRMADO]` El catálogo canónico de reglas vive en:

- [SDD/Domain/06-business-rules.md](Domain/06-business-rules.md): [INV-01](Domain/06-business-rules.md), [INV-02](Domain/06-business-rules.md), [PED-01](Domain/06-business-rules.md), [PED-02](Domain/06-business-rules.md), [PED-03](Domain/06-business-rules.md),
  [CAT-01](Domain/06-business-rules.md), [CAT-02](Domain/06-business-rules.md), [EXC-01](Domain/06-business-rules.md), [EXC-02](Domain/06-business-rules.md), [DEV-01](Domain/06-business-rules.md).
- [SDD/Domain/04-invariants-and-rules.md](Domain/04-invariants-and-rules.md): [invariantes 1–13](Domain/04-invariants-and-rules.md) (inventario,
  catálogo, pagos, posventa).
- [SDD/Domain/07-lifecycle.md](Domain/07-lifecycle.md): máquinas de estado de `CustomerOrder` y
  `FulfillmentOrder`.

`[RESUELTO]` Las contradicciones de reglas del dominio quedaron resueltas el
2026-09-17:

- **[C-01](00-bootstrap/risks-and-gaps.md)** ([INV-02](Domain/06-business-rules.md) vs [invariante 3](Domain/04-invariants-and-rules.md)) → [SDD/Adr/0001-reserva-fragmentacion-contingencia.md](Adr/0001-reserva-fragmentacion-contingencia.md): bodega única con
  fraccionamiento de contingencia.
- **[C-02](00-bootstrap/risks-and-gaps.md)** (clave de stock `ProductId` vs `VariantId`) → [SDD/Adr/0002-clave-inventario-variantid.md](Adr/0002-clave-inventario-variantid.md):
  el inventario se lleva por `VariantId` (SKU).
- **[Q-10](00-bootstrap/questions-for-owner.md#9-cuarta-iteracion-adr-0003-variante-obligatoria-en-fisicos-q-10-c3)** (¿variante obligatoria?) → [SDD/Adr/0003-variante-obligatoria-productos-fisicos.md](Adr/0003-variante-obligatoria-productos-fisicos.md): obligatoria solo en
  `Physical` ([CAT-03](Domain/06-business-rules.md)), aplicado en código con 164/164 pruebas. Sub-decisiones
  `[PROPUESTO]` pendientes de confirmar (**[Q-12](00-bootstrap/questions-for-owner.md#sub-decisiones-propuesto-ver-q-12)**).

Reglas vigentes de inventario: **[INV-01](Domain/06-business-rules.md), [INV-02](Domain/06-business-rules.md), [INV-03](Domain/06-business-rules.md)** ([06-business-rules.md](Domain/06-business-rules.md)).

`[RIESGO]` Estado de preguntas al **2026-09-18**: **[Q-13](00-bootstrap/questions-for-owner.md#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante)** (nueva, **bloqueante**: la Ley duplica
`DOMINIO 8/9/10` con significados cruzados → [risks-and-gaps.md](00-bootstrap/risks-and-gaps.md) [C-08](00-bootstrap/risks-and-gaps.md)), **[Q-03](00-bootstrap/questions-for-owner.md#q-03-c-03-estado-de-cancelacion-de-despacho)** y **[Q-04](00-bootstrap/questions-for-owner.md#q-04-c-04-cart-es-un-estado-de-customerorder)**
reactivadas como bloqueantes (su dominio se implementó sin respuesta), y siguen abiertas [Q-05](00-bootstrap/questions-for-owner.md#q-05-c-05-vendor-o-seller),
[Q-06](00-bootstrap/questions-for-owner.md#q-06-c-07-consolidacion-de-los-documentos-numerados), [Q-07](00-bootstrap/questions-for-owner.md#q-07-documento-de-identidad-del-usuario), [Q-08](00-bootstrap/questions-for-owner.md#q-08-r-07-buyerpaymenttokens-contra-la-invariante-9), [Q-09](00-bootstrap/questions-for-owner.md#q-09-naming-canonico-pendiente), [Q-11](00-bootstrap/questions-for-owner.md#q-11-detalle-del-modelo-de-atributos-de-variante-abierta) y [Q-12](00-bootstrap/questions-for-owner.md#sub-decisiones-propuesto-ver-q-12). Detalle: [SDD/00-bootstrap/questions-for-owner.md](00-bootstrap/questions-for-owner.md).

## 9. Estado de implementación

`[CONFIRMADO]` Ver [SDD/00-bootstrap/spec-conformance-matrix.md](00-bootstrap/spec-conformance-matrix.md) y
[SDD/00-bootstrap/current-state.md](00-bootstrap/current-state.md): solo existe el dominio, parcialmente.
El roadmap de adopción es [SDD/00-bootstrap/migration-to-sdd-plan.md](00-bootstrap/migration-to-sdd-plan.md).
