# Repository Map — zentric-backend

`[CONFIRMADO]` por inspección directa del árbol de archivos y de `Zentric.slnx`.

## 1. Raíz del repositorio

| Elemento | Tipo | Nota |
|---|---|---|
| `Zentric.slnx` | solución | referencia **1** proyecto: `Zentric.Domain` |
| `AGENTS.md` | gobernanza | directrices de agentes, arquitectura y DoD |
| `generic-sdd-agent.md` | gobernanza | prompt SDD v2.0.0 monolítico — **superseded** por `.agents/skills/generic-sdd-agent/` |
| `SDD/` | especificaciones | SSoT declarada por `AGENTS.md` §0 |
| `Zentric.Domain/` | código | único proyecto de la solución |
| `README.md` | doc | 2 líneas, sin instrucciones de ejecución |
| `LICENSE` | legal | — |
| `.gitignore` | configuración | 7.906 bytes |
| `.vscode/settings.json` | configuración | preferencias del editor |
| `.agents/skills/generic-sdd-agent/` | tooling | skill SDD v3.0.0 (router + referencias) |

## 2. Proyectos de la solución

| Proyecto | Framework | Referencias | Estado |
|---|---|---|---|
| `Zentric.Domain` | `net10.0` | ninguna (correcto: centro puro) | implementación parcial |
| `Zentric.Application` | — | — | **no existe** |
| `Zentric.Infrastructure` | — | — | **no existe** |
| `Zentric.Api` | — | — | **no existe** |
| proyecto de pruebas | — | — | **no existe** |

`[CONFIRMADO]` `Zentric.Domain.csproj`: `net10.0`, `ImplicitUsings` y `Nullable`
habilitados, sin `PackageReference`.

## 3. Estructura de `Zentric.Domain`

```text
Zentric.Domain/
├── Buyers/Buyer.cs
├── Inventories/Inventory.cs
├── Orders/Order.cs                        # stub vacío (solo Id)
├── Products/Product.cs
├── Products/Enums/ProductType.cs
├── Products/ValueObjects/Money.cs
├── Users/User.cs
├── Users/Enums/UserRole.cs, UserStatus.cs
├── Users/Ports/IUserRepository.cs
├── Users/ValueObjects/Email.cs, FullName.cs
└── Warehouses/Warehouse.cs
    Warehouses/Enum/WarehouseType.cs
```

Observaciones `[CONFIRMADO]`:

- El código se organiza por módulo de dominio (carpeta por contexto), no por
  `Entities/` + `ValueObjects/` global. Convención del repo: **respetarla**.
- `Ports/` ya aparece dentro del módulo (`Users/Ports/IUserRepository.cs`), lo
  que es coherente con `AGENTS.md` §2.2 (los puertos pueden vivir en Domain).
- No existe carpeta de eventos de dominio, ni `Result`, ni tipos fuertemente
  tipados de ID.
- Los archivos `bin/` y `obj/` están presentes en el árbol de trabajo y **no**
  están excluidos del análisis de búsqueda (no afecta al build).

## 4. Estructura de `SDD/`

```text
SDD/
├── Application/                                   # carpeta vacía
└── Domain/
    ├── ZENTRIC.md                                 # especificación funcional de negocio (14 KB)
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

`[CONTRADICCIÓN]` `AGENTS.md` §0.1 exige como lectura obligatoria
`SDD/01-system-overview.md` y `SDD/02-software-architecture.md`, y menciona
`SDD/Infrastructure/` y `SDD/Presentation/`: **ninguno existe** al inicio de esta
adopción. Se crean los dos primeros en esta iteración (derivados, no inventados);
los dos últimos quedan pendientes de las specs de sus capas.

`[CONTRADICCIÓN]` Numeración duplicada dentro de `SDD/Domain/`: dos `01-*`, dos
`02-*`, dos `03-*`, dos `04-*`, con solapes y contradicciones de contenido.

## 5. Puntos de entrada ejecutables

`[CONFIRMADO]` No hay punto de entrada HTTP, worker, CLI ni migración. El único
artefacto compilable es una biblioteca de dominio.
