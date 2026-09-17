# 02. Software Architecture — Zentric

> Documento exigido por `AGENTS.md` §0.1. **Derivado** de `AGENTS.md` (§1, §2, §3,
> §6) y de `SDD/Domain/01-domain-overview.md`, más el estado observado del
> repositorio. No introduce decisiones nuevas: consolida y enlaza.

## 1. Estilo arquitectónico

`[CONFIRMADO]` Arquitectura **Hexagonal (Puertos y Adaptadores)** + **DDD** con
**CQRS**, **Mediator**, **Repository**, **Unit of Work** y **Result Pattern**
(`AGENTS.md` §1). El flujo de dependencias es unidireccional hacia el centro.

## 2. Proyectos y regla de dependencia

```text
        ┌──────────────────────────┐
        │        Zentric.Api       │   (Composition Root)
        │  Presentation / REST     │
        └───────────┬──────────────
                    │ referencia Application e Infrastructure
        ┌───────────▼──────────────┐        ┌────────────────────────────
        │   Zentric.Infrastructure │───────▶│     Zentric.Application    │
        │  EF Core · PostgreSQL    │        │  Casos de uso · puertos    │
        ──────────────────────────┘        ───────────┬────────────────┘
                                                        │ referencia solo Domain
                                            ┌───────────▼────────────────┐
                                            │      Zentric.Domain        │
                                            │ Entidades · VO · invariantes│
                                            │  SIN dependencias externas  │
                                            └────────────────────────────┘
```

| Proyecto | Puede referenciar | Prohibido |
|---|---|---|
| `Zentric.Domain` | **nada** | EF Core, ASP.NET, HTTP, cualquier paquete de infraestructura |
| `Zentric.Application` | `Zentric.Domain` | `Infrastructure`, `Api`, SQL, detalles HTTP, EF Core |
| `Zentric.Infrastructure` | `Application`, `Domain` | `Api` |
| `Zentric.Api` | `Application`, `Infrastructure` | Lógica de negocio; la referencia a `Infrastructure` es **solo** para el registro de DI en `Program.cs` |

`[CONFIRMADO]` `Zentric.Domain.csproj` hoy no tiene ninguna `PackageReference`:
la regla se cumple en el único proyecto existente.

## 3. Puertos y adaptadores

`[CONFIRMADO]` `AGENTS.md` §2.2 y `SDD/Domain/05-ports.md`:

- **Puertos de salida:** interfaces en `Application` o `Domain` (p. ej.
  `IUserRepository`, `IDomainEventDispatcher`). Hoy existen en
  `Zentric.Domain/<Módulo>/Ports/`.
- **Adaptadores de salida:** implementaciones concretas en `Infrastructure`.
- **Puertos de entrada:** casos de uso en `Application`.
- **Adaptadores de entrada:** controladores REST delgados en `Api`.

## 4. Modelado táctico

`[CONFIRMADO]` `AGENTS.md` §2.3:

- Entidades y raíces de agregado con identidad propia, sin setters públicos
  anémicos; la mutación ocurre por métodos de negocio que validan invariantes.
- Value Objects estrictamente inmutables.
- Agregados pequeños con límites transaccionales duros
  (`SDD/Domain/02-aggregates-and-entities.md` §1).

## 5. Tratamiento de errores por capa

`[CONFIRMADO]` `AGENTS.md` §3:

| Capa | Regla |
|---|---|
| Domain | Guardas defensivas (`ArgumentException`/`InvalidOperationException`) o `Result`. Prohibido `try-catch` técnico |
| Application | Validación de entrada obligatoria (FluentValidation) y retorno de `Result<T>`. Prohibido capturar excepciones catastróficas |
| Infrastructure | Traduce errores predecibles a `Result.Failure`; no expone `SqlException` ni cadenas de conexión |
| Api | Middleware global de excepciones; mapea `Result<T>` a HTTP con RFC 7807 (Problem Details) |

Prohibido usar excepciones para el flujo lógico habitual.

## 6. Inyección de dependencias

`[CONFIRMADO]` `AGENTS.md` §6: cada capa expone su propio método de extensión de
registro; `Zentric.Api` es el Composition Root y el único autorizado a ensamblar
las capas.

## 7. Convenciones de código

`[CONFIRMADO]` `AGENTS.md` §1 y §5:

- Código en **inglés**; comentarios y documentación en **español**.
- Interfaces con prefijo `I`; `PascalCase` para tipos y miembros; `camelCase` para
  locales; `_camelCase` para campos privados.
- Preferir `record`, `init`, `sealed` donde aplique; evitar setters públicos.
- Documentación XML en APIs públicas o reglas complejas.
- Tests con patrón `Metodo_Condicion_ResultadoEsperado`, en estilo BDD mapeado a
  los casos de uso de la especificación.

## 8. Comandos oficiales

`[CONFIRMADO]` `AGENTS.md` §7:

```powershell
dotnet restore
dotnet build Zentric.slnx
dotnet test
```

## 9. Estado observado vs arquitectura objetivo

| Elemento | Objetivo | Estado real |
|---|---|---|
| `Zentric.Domain` aislado | sí | **cumple** |
| Organización por módulo de dominio | — | **cumple** (carpeta por contexto) |
| `Zentric.Application` | existe | **no existe** |
| `Zentric.Infrastructure` | existe | **no existe** |
| `Zentric.Api` | existe | **no existe** |
| Proyecto de pruebas | existe | **no existe** |
| `Result<T>` | en Application/Infrastructure | **no existe** (la spec de servicios lo requiere) |
| Eventos de dominio + dispatcher | puerto en Domain/Application | **no existen** |
| Abstracción de tiempo | para reglas temporales | **no existe** (`DateTime.UtcNow` directo, 46 usos) |
| IDs fuertemente tipados | `record struct` | **no existen** (`Guid` plano) |

Detalle y evidencia: `SDD/00-bootstrap/spec-conformance-matrix.md`.

## 10. Riesgos arquitectónicos abiertos

`[RIESGO]` Añadir `Infrastructure` (EF Core) antes de resolver los mapeos del
modelo obligaría a escribir y reescribir mapeos y migraciones. Orden recomendado:
estabilizar el dominio (Fases 1–3 del roadmap) y recién entonces construir
persistencia y API (`SDD/00-bootstrap/migration-to-sdd-plan.md`).

`[RIESGO]` La ausencia de `.editorconfig`, analizadores y CI permite que el
estilo y las reglas arquitectónicas se degraden sin que nada lo detecte
(ver R-08 del bootstrap).