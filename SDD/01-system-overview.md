# 01. System Overview — Zentric

> Documento exigido por `AGENTS.md` §0.1. **Derivado**, no inventado: consolida y
> enlaza la especificación funcional de negocio existente
> (`SDD/Domain/ZENTRIC.md`) y los documentos de dominio. No introduce reglas
> nuevas. Todo lo marcado `[INFERIDO]` requiere confirmación.

## 1. Qué es Zentric

`[CONFIRMADO]` `SDD/Domain/ZENTRIC.md` §1: plataforma digital centralizada que
actúa como **intermediario comercial** entre compradores y vendedores. Administra
la operación completa: registro de usuarios, publicación de productos, logística,
facturación y posventa, con trazabilidad y coordinación entre participantes.

## 2. Objetivos estratégicos

`[CONFIRMADO]` `ZENTRIC.md` §2:

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

`[CONFIRMADO]` `ZENTRIC.md` §3.1 incluye: registro de compradores, administración
de usuarios, registro **administrativo** de vendedores, administración de bodegas,
catálogo, inventario, carrito, pedidos, facturación, envíos, devoluciones,
reembolsos y consulta de reportes administrativos.

`[CONFIRMADO]` `ZENTRIC.md` §3.2 excluye explícitamente: interfaces gráficas,
aplicaciones móviles, portales web, mecanismos de autenticación técnica,
tecnologías de implementación, arquitectura del software y almacenamiento de la
información. **Este repositorio cubre el core de dominio y su API, no la UI.**

## 4. Participantes y roles

`[CONFIRMADO]` `ZENTRIC.md` §5: Comprador, Vendedor, Operador Logístico,
Administrador y Supervisor. Cada participante desempeña **un único rol**.

`[CONFIRMADO]` `AGENTS.md` §1 enumera: vendedores, compradores, bodegas,
productos, catálogos y pedidos.

## 5. Modelo operativo (flujo extremo a extremo)

`[CONFIRMADO]` `ZENTRIC.md` §6.1:

1. **Incorporación** — el Administrador registra al vendedor y su primera bodega.
2. **Catálogo** — el vendedor registra productos y características.
3. **Inventario** — se registran existencias iniciales en las bodegas.
4. **Publicación** — los productos se hacen visibles en el catálogo.
5. **Compra** — el comprador selecciona productos vía carrito y confirma.
6. **Transacción** — se valida el pago y se inicia la preparación.
7. **Logística** — empaque, despacho y transporte.
8. **Cierre** — el pedido se marca finalizado tras entrega confirmada.

## 6. Bounded context y módulos

`[CONFIRMADO]` `SDD/Domain/01-domain-overview.md` §2: **un único Bounded Context
(Unified Domain)** dividido en módulos de dominio de alta cohesión:

| Módulo | Responsabilidad | Documento de referencia |
|---|---|---|
| Identity | Usuarios y estado comercial de participantes | `SDD/Domain/01-models.md` §1 |
| Catalog | Oferta de vendedores, productos y variantes | `SDD/Domain/01-models.md` §2 |
| Inventory | Inventario distribuido, bodegas y cantidades | `SDD/Domain/01-models.md` §3 |
| Ordering | Carrito, pedido maestro, facturación centralizada | `SDD/Domain/01-models.md` §4 |
| Fulfillment | Órdenes de despacho por vendedor y posventa | `SDD/Domain/01-models.md` §5 |

## 7. Restricciones transversales

`[CONFIRMADO]` `ZENTRIC.md` §10 y §11:

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

- `SDD/Domain/06-business-rules.md`: INV-01, INV-02, PED-01, PED-02, PED-03,
  CAT-01, CAT-02, EXC-01, EXC-02, DEV-01.
- `SDD/Domain/04-invariants-and-rules.md`: invariantes 1–13 (inventario,
  catálogo, pagos, posventa).
- `SDD/Domain/07-lifecycle.md`: máquinas de estado de `CustomerOrder` y
  `FulfillmentOrder`.

`[RESUELTO]` Las contradicciones de reglas del dominio quedaron resueltas el
2026-09-17:

- **C-01** (INV-02 vs invariante 3) → `SDD/Adr/0001-...`: bodega única con
  fraccionamiento de contingencia.
- **C-02** (clave de stock `ProductId` vs `VariantId`) → `SDD/Adr/0002-...`:
  el inventario se lleva por `VariantId` (SKU).

Reglas vigentes de inventario: **INV-01, INV-02, INV-03** (`06-business-rules.md`).
Quedan abiertas **Q-10** (¿variante obligatoria?) y **Q-11** (modelo de atributos).

## 9. Estado de implementación

`[CONFIRMADO]` Ver `SDD/00-bootstrap/spec-conformance-matrix.md` y
`SDD/00-bootstrap/current-state.md`: solo existe el dominio, parcialmente.
El roadmap de adopción es `SDD/00-bootstrap/migration-to-sdd-plan.md`.