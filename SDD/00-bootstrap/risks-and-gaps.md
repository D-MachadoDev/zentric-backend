# Risks & Gaps — zentric-backend

Formato de riesgo: `ID · Categoría · Evidencia · Impacto · Probabilidad · Acción ·
¿Bloquea?`.

## 1. Contradicciones de especificación (bloqueantes)

| ID | Categoría | Evidencia | Impacto | Prob. | Acción | ¿Bloquea? |
|---|---|---|---|---|---|---|
| ~~C-01~~ | ~~Regla de negocio / inventario~~ | ~~`04-invariants-and-rules.md:8` vs `06-business-rules.md:7` vs `services/inventory-reservation-service.md:18`~~ | **RESUELTA el 2026-09-17 por decisión del owner (A3 híbrido).** Regla canónica: bodega única si una bodega cubre la cantidad; fraccionamiento de contingencia solo si ninguna cubre; fallo si la suma no alcanza. Ver `SDD/Adr/0001-reserva-fragmentacion-contingencia.md` | — | Cerrada | No |
| ~~C-02~~ | ~~Modelo de datos / clave de stock~~ | ~~`02-aggregates-and-entities.md:29` y `03-value-objects.md:15` (`VariantId`) vs `01-models.md:33`, `ZENTRIC.md` Dom.6 y el código (`ProductId`)~~ | **RESUELTA el 2026-09-17 por decisión del owner (B2).** El stock se lleva por `VariantId` (SKU); `ProductVariant` es entidad hija de `Product`; clave `(VariantId, WarehouseId)`. Ver `SDD/Adr/0002-clave-inventario-variantid.md` | — | Cerrada (queda Q-10 sobre variante obligatoria) | Parcialmente |
| C-03 | Estado de dominio | `02-value-objects.md:58` (`FulfillmentStatus.Cancelled`) vs `03-value-objects.md:41` (`CancelledByStockBreak`) | Máquina de estados de despacho ambigua | Media | Q-03 | Sí (Fulfillment) |
| C-04 | Estado de dominio | `02-value-objects.md:46` incluye `Cart` en `OrderStatus` vs `03-value-objects.md:29-34` lo omite | Un carrito efímero y un pedido formal no pueden compartir agregado sin definirlo | Media | Q-04 | Sí (Ordering) |
| C-05 | Lenguaje ubicuo | `02-value-objects.md:62` (`Vendor`) vs `WarehouseType.cs:6` (`Seller`) | Nombres divergentes en API, base de datos y eventos | Media | Q-05 | No (se propaga) |
| C-06 | Gobernanza documental | `AGENTS.md` §0.1 exige `SDD/01-system-overview.md`, `SDD/02-software-architecture.md`, `SDD/Infrastructure/`, `SDD/Presentation/`; no existían. Además `SDD/Domain/Software-arquitecture.md` está **vacío (0 bytes)** | La SSoT declarada no era navegable y el agente no podía cumplir la consulta obligatoria | Alta | 2 documentos creados ahora; restan Infrastructure/Presentation | Parcialmente |
| C-07 | Numeración de specs | `SDD/Domain/` tiene dos `01-*`, dos `02-*`, dos `03-*`, dos `04-*` con contenido solapado | Cada lectura puede llevar a una regla distinta; no hay canonicidad | Alta | Q-06 | Sí (para consolidar) |

## 2. Riesgos técnicos

| ID | Categoría | Evidencia | Impacto | Prob. | Acción | ¿Bloquea? |
|---|---|---|---|---|---|---|
| R-01 | Integridad de datos | H-01 `Inventory.DispatchStock` (`Inventory.cs:109-123`) | Descuadre de stock: reservado negativo, balance roto | Alta | Corregir con prueba (T-003) | No |
| R-02 | Integridad de datos | H-02 `UpdateQuantities` (`Inventory.cs:58-79`) | Se saltan `Reserve/Release/Dispatch`; anula invariantes | Alta | Rediseñar API (T-003/T-004) | No |
| R-03 | Cumplimiento de spec | H-05 `User` sin `IdentityDocument` | No se puede registrar un usuario conforme a `ZENTRIC.md` Dom.1 | Alta | T-006 (tras Q-07) | No |
| R-04 | Seguridad / autorización | Falta `ManualAdjust(qty, UserId, Role)` (invariante 4) | Cualquiera podría ajustar stock en bodegas Marketplace | Media | T-005 | Sí (con C-01) |
| R-05 | Regresión | **164 pruebas** en `Zentric.Tests` (antes: 0) | Queda sin cobertura: `Buyer`, los VOs `Email`/`FullName`, eventos y todo lo faltante | Media | T-002b, Fase 3 | No |
| R-06 | Testabilidad | H-06: 46 usos de `DateTime.UtcNow` en 5 entidades | Las reglas temporales (timeout 15 min) no son verificables determinísticamente | Media | T-004 (abstracción de tiempo) | No |
| R-07 | Contradicción de diseño | H-03 `Buyer.PaymentTokens` vs invariante 9 (YAGNI: sin medios de pago) | Se almacenarían tokens de pago que la spec prohíbe | Media | Q-08 | Parcial |
| R-08 | Higiene del repositorio | `bin/`/`obj/` en el árbol; sin `.editorconfig`; sin CI; sin pipeline de calidad | El build "limpio" no refleja la calidad real | Media | T-007 | No |
| R-09 | Lenguaje ubicuo | 12 desviaciones de naming (`Avalible`, `Inventory` vs `InventoryItem`, `Email` vs `EmailAddress`, `Administrator` vs `Admin`, `Name` vs `Title`, `SellerId` vs `OwnerId`) | Coste de renombrado creciente; confusión en contratos públicos | Alta | Q-05/Q-09 + T-008 | No |
| R-10 | Modelo de datos | ~~Tras ADR-0002 el stock exige una variante, pero nada obliga a que un producto físico tenga al menos una (Q-10 abierta). `Product` permite nacer sin variantes~~ | **CERRADA el 2026-09-17 (Q-10 = C3, ADR-0003):** CAT-03 obliga variante en `Physical`; `Product` lo hace cumplir en constructor, `UpdateType` y `RemoveVariant` (T-010c, 164/164 PASS) | ~~Media~~ | ~~Q-10 antes de T-013/T-015/T-016~~ | ~~**Sí (para Ordering)**~~ |

## 3. Vacíos funcionales (gaps de implementación)

| ID | Gap | Spec de origen | Bloqueo |
|---|---|---|---|
| G-01 | `CustomerOrder` + `OrderLine` + estados + checkout (agregado central) | `01-models` §4, `07-lifecycle` | C-04 (Q-04) |
| G-02 | `FulfillmentOrder` + `Shipment` + máquina de estados | `01-models` §5 | C-03 (Q-03) |
| G-03 | Devoluciones y reembolsos con doble aprobación | `ReturnStatus`, `services/returns-approval-service.md` | Ninguno |
| G-04 | 7 eventos de dominio + `IDomainEventDispatcher` | `04-domain-events.md`, `05-ports.md` §2 | Ninguno |
| G-05 | 5 puertos de repositorio restantes | `05-ports.md` §1 | — (clave ya definida: `(VariantId, WarehouseId)`) |
| G-06 | 4 servicios de dominio | `03-domain-services.md` | Q-03 (Q-10 resuelta: todo `Physical` trae variante) |
| G-07 | IDs fuertemente tipados y VO `Address` | `02-value-objects.md` | — (pendiente T-004; `VariantId` sigue siendo `Guid` plano) |
| ~~G-08~~ | ~~`ProductVariant` (SKU)~~ | `02-aggregates:18-19` | **CERRADA** en ADR-0002 / T-010 |
| G-08b | Refinamiento del modelo de atributos de variante | `02-aggregates` §2 | Q-11 |
| G-08c | Hacer cumplir CAT-03 en el agregado `Product` (constructor, `UpdateType`, `RemoveVariant`, `CanBeSold`) | CAT-03, ADR-0003 | **CERRADA** en ADR-0003 / T-010c: 164/164 PASS |
| G-09 | Suspensión en cascada al bloquear un vendedor | invariante 6 | Ninguno |
| G-10 | Timeout de reserva de 15 minutos | PED-01, `07-lifecycle` | R-06 |
| G-11 | Capas `Application`, `Infrastructure`, `Api` y contrato OpenAPI | `AGENTS.md` §2 | Depende de un dominio estable |

## 4. Riesgo residual aceptado hoy

`[CONFIRMADO]` No hay entorno productivo, ni datos reales, ni consumidores del
contrato. El coste de un renombrado **hoy** es mínimo. Se registra como riesgo el
**aplazarlo**: cada nuevo artefacto (EF Core, DTOs, endpoints, eventos) multiplica
el número de sitios a renombrar.

## 5. Fuera de alcance de este bootstrap

- Implementar las capas `Application`, `Infrastructure` y `Api`.
- Diseñar el esquema de base de datos y migraciones (no hay EF Core aún).
- Autenticación técnica real (fuera del alcance declarado en `ZENTRIC.md` §3.2).

## 6. Riesgos cerrados

| ID | Riesgo | Cierre | Evidencia |
|---|---|---|---|
| R-01b | H-08: `ReturnToAvalible` permitía violar INV-01 con cantidades no positivas (`Inventory.cs:142-152`) | T-003a: guarda añadida + prueba de regresión | `verification-baseline.md` §7: rojo 3/105 → verde 105/105 |
| R-05b | Ausencia total de pruebas automatizadas en el dominio | T-001/T-002: proyecto `Zentric.Tests` con 105 pruebas | `Passed! - Failed: 0, Passed: 105` |
| R-06c | Clave de stock ambigua (`ProductId` vs `VariantId`), con `ProductVariant` inexistente | ADR-0002 + T-004a/T-010: `Inventory.VariantId`, `ProductVariant` y `VariantAttribute` implementados con 45 pruebas nuevas | `verification-baseline.md` §8: build 0/0 · 150/150 PASS |
| R-10b | Producto físico sin variante (estado sin unidad de stock) | ADR-0003 + T-010c: constructor, `UpdateType`, `RemoveVariant` y `CanBeSold` hacen cumplir CAT-03 con 14 pruebas nuevas | `verification-baseline.md` §9: build 0/0 · 164/164 PASS |