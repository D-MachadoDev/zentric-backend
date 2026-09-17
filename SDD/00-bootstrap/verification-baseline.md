# Verification Baseline — zentric-backend

Fecha de línea base: 2026-09-17 · Entorno: Windows, PowerShell 7, .NET SDK 10
(interprete `dotnet` de la máquina), sin PostgreSQL ni servicios externos.

## 1. Comandos oficiales

| Comando | Resultado | Observaciones |
|---|---|---|
| `dotnet restore` | `PASS` | "All projects are up-to-date for restore." |
| `dotnet build Zentric.slnx` | `PASS` | `0 Warning(s)`, `0 Error(s)`, `Zentric.Domain.dll` generado, ~6 s |
| `dotnet test` | `N/A` | **No existe ningún proyecto de pruebas**: no hay suites que ejecutar |

Salida relevante del build:

```text
Zentric.Domain -> ...\Zentric.Domain\bin\Debug\net10.0\Zentric.Domain.dll
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

## 2. Suites existentes

`[CONFIRMADO]` **No hay pruebas.** No existe `*.Tests.csproj`, ni xUnit, ni
fixtures, ni datos de prueba. La cobertura de reglas de negocio es **0 %**.

`[RIESGO]` El repositorio implementa invariantes críticas (no-negatividad de
inventario, unicidad de correo, transiciones de estado) **sin ninguna prueba
automatizada**. Cualquier refactor posterior es una regresión silenciosa
potencial.

## 3. Formato, lint y análisis estático

| Herramienta | Estado |
|---|---|
| `.editorconfig` | **no existe** |
| `dotnet format` | no configurado |
| Análisis estático / `AnalysisLevel` | no configurado en `csproj` |
| `TreatWarningsAsErrors` | no habilitado |

`[INFERIDO]` El build limpio (0 warnings) se debe a que no hay analizadores
estrictos activos, no a que el código esté libre de hallazgos. Ver
`quality-baseline` implícito en `risks-and-gaps.md`.

## 4. CI/CD e infraestructura

`[CONFIRMADO]` No hay pipelines (`.github/workflows`, `azure-pipelines.yml`),
Dockerfile, `docker-compose`, ni manifiestos de infraestructura.

## 5. Limitaciones del entorno de esta línea base

- No se ejecutó ninguna prueba de integración con PostgreSQL: no hay proyecto ni
  cadena de conexión configurada.
- No se inspeccionaron secretos ni variables de entorno: no hay `.env*`.
- `dotnet test` sobre una solución sin proyectos de prueba no produce resultado
  verificable; se registra como ausencia de suites, no como éxito.

## 6. Cómo repetir esta línea base

```powershell
cd c:\Users\da-V7\Desktop\zentric-backend
dotnet restore
dotnet build Zentric.slnx
dotnet test
```

---

## 7. Segunda iteración — suite de pruebas y corrección T-003

Fecha: 2026-09-17.

| Comando | Resultado | Observaciones |
|---|---|---|
| `dotnet new xunit -o Zentric.Tests` | `PASS` | Proyecto xUnit 2.9.3, `Microsoft.NET.Test.Sdk` 17.14.1, `coverlet.collector` |
| `dotnet sln Zentric.slnx add Zentric.Tests/Zentric.Tests.csproj` | `PASS` | La solución pasa de 1 a 2 proyectos |
| `dotnet add ... reference Zentric.Domain` | `PASS` | Dependencia de prueba hacia el dominio (dirección correcta) |
| `dotnet test Zentric.slnx` (antes de la corrección) | `FAIL` | **3 fallos / 105**: 2 eran la regresión esperada de H-08 (`ReturnToAvalible_NonPositiveQuantity`) y 1 era una prueba defectuosa propia (setup con stock > 0 en `MarkAsDeleted_AlreadyDeleted`), corregida |
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
- Justificación: INV-01 (invariante absoluta de no-negatividad), que sí existe en
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
| VOs `Email` / `FullName` | pendiente (T-002b) |

---

## 8. Tercera iteración — ADR-0002: clave del inventario = `VariantId`

Fecha: 2026-09-17. Alcance: T-010 (`ProductVariant`), T-004a
(`Inventory.ProductId` → `Inventory.VariantId`) y T-002c (migración de pruebas).

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
| VOs `Email` / `FullName` y `Buyer` | pendiente (T-002b) |

---

## 9. Cuarta iteración — ADR-0003: variante obligatoria en físicos (Q-10 = C3)

Fecha: 2026-09-17. Alcance: T-010c (hacer cumplir CAT-03 en `Product`).

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
| `Zentric.Domain/Products/Product.cs` | Nueva regla CAT-03: el constructor exige ≥1 variante si `Type == Physical` (`ArgumentException`, paramName `variants`); las semillas se crean vía `AddVariant` (heredan unicidad de SKU); `UpdateType(Physical)` lanza `InvalidOperationException` sin variante; `RemoveVariant` no puede dejar un `Physical` sin variantes; `CanBeSold` exige variante vendible (activa y no eliminada) en `Physical`. Nuevas propiedades `HasVariant` / `HasSellableVariant` / `CanBeSold` |
| `Zentric.Tests/Products/ProductTests.cs` | +14 casos C3 (constructor físico/digital, semillas duplicadas, `UpdateType`, `RemoveVariant`, `HasVariant` con variante eliminada, `CanBeSold` con variante desactivada/eliminada); el helper `CreateProduct()` por defecto ahora crea `Digital` para no romper los casos base |

### Cobertura acumulada

| Artefacto | Casos |
|---|---|
| `Inventory` | 24 |
| `Warehouse` | 22 |
| `User` | 22 |
| `Product` (incl. variantes + CAT-03) | 44 |
| `ProductVariant` | 24 |
| `VariantAttribute` | 10 |
| `Money` | 14 |
| VOs `Email` / `FullName` y `Buyer` | pendiente (T-002b) |
