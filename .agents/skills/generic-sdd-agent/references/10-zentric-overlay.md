# 10. Overlay: repositorio zentric-backend

Este overlay **manda** sobre el catálogo genérico de esta skill. Si hay conflicto
entre este archivo y `03-artifacts.md`, gana el overlay.

## 1. Comandos oficiales (verificados)

```powershell
dotnet restore
dotnet build Zentric.slnx
dotnet test
```

Evidencia de línea base: ver `SDD/00-bootstrap/verification-baseline.md`.

## 2. Stack y reglas inviolables

- .NET 10 · C# · PostgreSQL · EF Core · xUnit · arquitectura hexagonal + DDD.
- Código en inglés; comentarios y documentación en español.
- Flujo de dependencias unidireccional hacia el dominio:
  `Api → Infrastructure → Application → Domain` y `Api → Application`.
- `Zentric.Domain` no referencia ningún otro proyecto ni paquete de
  infraestructura/web.
- `Zentric.Api` solo referencia `Infrastructure` para el registro de DI en
  `Program.cs`.
- Prohibido: setters públicos anémicos, excepciones para control de flujo
  habitual, `try-catch` técnico en dominio/aplicación.
- Patrón `Result<T>` en Application/Infrastructure; excepciones catastróficas
  suben al middleware global (RFC 7807 Problem Details).
- Puertos de entrada/salida en `Application` o `Domain`; adaptadores en
  `Infrastructure`.
- Cada capa expone su propio método de extensión de registro de DI.

Fuente: `AGENTS.md` §2 y §6.

## 3. Mapa de la fuente única de verdad

`/SDD` es la SSoT declarada por `AGENTS.md` §0. **No** crees `docs/` ni `specs/`
en paralelo.

| Zona | Ruta | Estado |
|---|---|---|
| Contexto de negocio global | `SDD/01-system-overview.md` | creado (deriva de `SDD/Domain/ZENTRIC.md`) |
| Arquitectura del software | `SDD/02-software-architecture.md` | creado (deriva de `AGENTS.md` §2) |
| Dominio | `SDD/Domain/*` | existente, con solapes de numeración |
| Especificación funcional original | `SDD/Domain/ZENTRIC.md` | fuente externa de negocio |
| Aplicación / Infraestructura / Presentación | `SDD/Application/`, `SDD/Infrastructure/`, `SDD/Presentation/` | **pendiente** (solo existe `Application/` vacío) |
| Línea base brownfield | `SDD/00-bootstrap/` | creado en esta adopción |
| Decisiones arquitectónicas duraderas | `SDD/Adr/` | creado (ADR-0001: reserva y fraccionamiento) |

## 4. Lenguaje ubicuo canónico

La especificación define los términos. El código debe usarlos **exactamente**.
Desviaciones detectadas en la línea base (ver `risks-and-gaps.md` para el detalle
y las decisiones pendientes):

| Término en spec | Estado en código | Acción |
|---|---|---|
| `Vendor` / `WarehouseType.Vendor` | `WarehouseType.Seller` | decidir y unificar |
| `InventoryItem` con `AvailableQuantity` | clase `Inventory` con `AvalibleQuantity` (typo) | unificar nombre y ortografía |
| `EmailAddress` | `Email` | unificar (o registrar alias en la spec) |
| `Admin` | `UserRole.Administrator` | unificar |
| `VariantId` (SKU) como clave de inventario | `Inventory.VariantId` (`Guid`) | **resuelto** por ADR-0002; queda pendiente el `record struct VariantId` (G-07) |
| Variante obligatoria solo en físicos (CAT-03) | constructor + `UpdateType` + `RemoveVariant` + `CanBeSold` en `Product.cs` | **resuelto** por ADR-0003 (sub-decisiones Q-12 `[PROPUESTO]`) |
| `ProductStatus` (`Published`/`Suspended`/`Discontinued`) | `bool IsActive` + `DeletedAt` | decidir modelo de estado |
| Agregado `Buyer` | existe `Buyer` en código, **no** en la spec | decidir si entra a la spec o se elimina |
| Sin almacenamiento de medios de pago (YAGNI, invariante 9) | `Buyer.PaymentTokens` | **contradicción** |

## 5. Estado de partida (línea base)

- Proyectos en la solución: **2** (`Zentric.Domain`, `Zentric.Tests`).
- Build: **OK, 0 warnings / 0 errores** (`dotnet build Zentric.slnx`).
- Tests: **164 pruebas en verde** (`dotnet test`), creadas en T-001/T-002/T-002c/T-010c.
- Contradicciones abiertas que bloquean: **4** de 7 (C-01 resuelta por ADR-0001,
  C-02 por ADR-0002 y Q-10 por ADR-0003 el 2026-09-17). Ver
  `SDD/00-bootstrap/risks-and-gaps.md` y `SDD/00-bootstrap/questions-for-owner.md`
  (Q-03 … Q-09, Q-11, Q-12).
- Hallazgos abiertos en código existente: **7** (H-01 … H-07). H-08 **corregido**
  en T-003a.
- Documentos de gobernanza creados: `SDD/01-system-overview.md`,
  `SDD/02-software-architecture.md`, `SDD/00-bootstrap/*` (7 archivos).

## 6. Protocolo específico del repositorio

1. Antes de implementar en un área, consulta la spec correspondiente de `/SDD`.
2. Si la spec no cubre el caso: **no inventes**. Actualiza o crea la spec primero
   y pide confirmación (AGENTS.md §0.3).
3. Si el código contradice la spec: registra `[CONTRADICCIÓN]` y aplica el
   protocolo de `07-special-rules.md` §8.
4. Todo cambio estructural o de regla de negocio actualiza `/SDD` en el mismo
   cambio (AGENTS.md §0.2).
5. Marca progreso con `[x]` en los documentos aplicables o en `SDD/TRACKING.md`.
6. Cierra con el bloque de Fase 9 e incluye los comandos ejecutados.
