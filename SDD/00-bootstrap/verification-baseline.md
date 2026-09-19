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
`quality-baseline` implícito en [risks-and-gaps.md](risks-and-gaps.md).

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

## 7. Segunda iteración — suite de pruebas y corrección [T-003](migration-to-sdd-plan.md)

Fecha: 2026-09-17.

| Comando | Resultado | Observaciones |
|---|---|---|
| `dotnet new xunit -o Zentric.Tests` | `PASS` | Proyecto xUnit 2.9.3, `Microsoft.NET.Test.Sdk` 17.14.1, `coverlet.collector` |
| `dotnet sln Zentric.slnx add Zentric.Tests/Zentric.Tests.csproj` | `PASS` | La solución pasa de 1 a 2 proyectos |
| `dotnet add ... reference Zentric.Domain` | `PASS` | Dependencia de prueba hacia el dominio (dirección correcta) |
| `dotnet test Zentric.slnx` (antes de la corrección) | `FAIL` | **3 fallos / 105**: 2 eran la regresión esperada de [H-08](spec-conformance-matrix.md) (`ReturnToAvalible_NonPositiveQuantity`) y 1 era una prueba defectuosa propia (setup con stock > 0 en `MarkAsDeleted_AlreadyDeleted`), corregida |
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
- Justificación: [INV-01](../Domain/06-business-rules.md) (invariante absoluta de no-negatividad), que sí existe en
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
| VOs `Email` / `FullName` | pendiente ([T-002b](migration-to-sdd-plan.md)) |

---

## 8. Tercera iteración — ADR-0002: clave del inventario = `VariantId`

Fecha: 2026-09-17. Alcance: [T-010](migration-to-sdd-plan.md) (`ProductVariant`), [T-004a](migration-to-sdd-plan.md)
(`Inventory.ProductId` → `Inventory.VariantId`) y [T-002c](migration-to-sdd-plan.md) (migración de pruebas).

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
| VOs `Email` / `FullName` y `Buyer` | pendiente ([T-002b](migration-to-sdd-plan.md)) |

---

## 9. Cuarta iteración — ADR-0003: variante obligatoria en físicos ([Q-10](questions-for-owner.md#9-cuarta-iteracion-adr-0003-variante-obligatoria-en-fisicos-q-10-c3) = C3)

Fecha: 2026-09-17. Alcance: [T-010c](migration-to-sdd-plan.md) (hacer cumplir [CAT-03](../Domain/06-business-rules.md) en `Product`).

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
| `Zentric.Domain/Products/Product.cs` | Nueva regla [CAT-03](../Domain/06-business-rules.md): el constructor exige ≥1 variante si `Type == Physical` (`ArgumentException`, paramName `variants`); las semillas se crean vía `AddVariant` (heredan unicidad de SKU); `UpdateType(Physical)` lanza `InvalidOperationException` sin variante; `RemoveVariant` no puede dejar un `Physical` sin variantes; `CanBeSold` exige variante vendible (activa y no eliminada) en `Physical`. Nuevas propiedades `HasVariant` / `HasSellableVariant` / `CanBeSold` |
| `Zentric.Tests/Products/ProductTests.cs` | +14 casos C3 (constructor físico/digital, semillas duplicadas, `UpdateType`, `RemoveVariant`, `HasVariant` con variante eliminada, `CanBeSold` con variante desactivada/eliminada); el helper `CreateProduct()` por defecto ahora crea `Digital` para no romper los casos base |

### Cobertura acumulada

| Artefacto | Casos |
|---|---|
| `Inventory` | 24 |
| `Warehouse` | 22 |
| `User` | 22 |
| `Product` (incl. variantes + [CAT-03](../Domain/06-business-rules.md)) | 44 |
| `ProductVariant` | 24 |
| `VariantAttribute` | 10 |
| `Money` | 14 |
| VOs `Email` / `FullName` y `Buyer` | pendiente ([T-002b](migration-to-sdd-plan.md)) |

---

## 10. Quinta iteración — auditoría de la tanda no registrada (2026-09-18)

Fecha: 2026-09-18. Alcance: **solo lectura + documentación**. **Cero archivos de código modificados**
(verificable con `git status --short`: únicamente `.md` y `scripts/sync-skill.ps1`).

| Comando | Resultado | Observaciones |
|---|---|---|
| `dotnet build Zentric.slnx --nologo` | `PASS` | `Build succeeded. 0 Warning(s) 0 Error(s)`; compilan **5** proyectos (Domain, Application, Infrastructure, Api, Tests) |
| `dotnet test Zentric.slnx --nologo` | `PASS` | `Failed: 0, Passed: 178, Skipped: 0, Total: 178` (~479 ms) |
| `Get-FileHash` de `.agents/skills/.../SKILL.md` vs copia instalada | `FAIL` → luego `PASS` | Repo = `24E7ED4F…` (v6.0.0) vs instalada = `EB4E7166…` (v3.1.0); sincronizada con `sync-skill.ps1` (corregido previamente) |
| `git diff --numstat [SDD/Domain/ZENTRIC.md](../Domain/ZENTRIC.md)` | `PASS` | 38 adiciones / 1 borrado (línea separadora) → **la Ley conserva el texto original** |

### Salida textual relevante

```text
Build succeeded.
    0 Warning(s)
    0 Error(s)

Passed!  - Failed:     0, Passed:   178, Skipped:     0, Total:   178 - Zentric.Tests.dll (net10.0)
```

### Delta de pruebas respecto a [:9](../Domain/ZENTRIC.md#dominio-9-gestin-de-devoluciones-y-reembolsos) (14 nuevas desde la última línea base)

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
| `Product` (incl. variantes + [CAT-03](../Domain/06-business-rules.md)) | 44 |
| `ProductVariant` | 24 |
| `VariantAttribute` | 10 |
| `Money` | 14 |
| `CustomerOrder` (nuevo) | 6 |
| `FulfillmentOrder` (nuevo) | 3 |
| `Invoice` (nuevo) | 2 |
| `ReturnRequest` (nuevo) | 3 |
| VOs `Email` / `FullName` y `Buyer` | pendiente ([T-002b](migration-to-sdd-plan.md)) |

### Validaciones NO ejecutadas (honestidad de evidencia)

- Migraciones `InitialCreate` / `CompleteSchema` **nunca aplicadas** contra PostgreSQL; sin pruebas de integración.
- La API **no se levantó**: 0 peticiones HTTP ejecutadas contra los 3 endpoints.
- Mapeo EF en runtime sin validar (`OwnsMany`, `OwnsOne`, `HasConversion` de `Email`).
- Sin `.editorconfig`, sin analizadores, sin CI: el "0 warnings" no demuestra ausencia de hallazgos.

---

## 11. Sexta iteración — SPEC-007: validación de entrada, RFC 7807 e higiene (2026-09-18)

Alcance: cerrar **[H-09](spec-conformance-matrix.md)** (`try-catch` genérico prohibido en Application), **[H-11](spec-conformance-matrix.md)** (sin
`AddProblemDetails()` ni validadores FluentValidation) y **[H-12](spec-conformance-matrix.md)** (`Class1.cs` vacíos), los tres
incumplimientos literales de [AGENTS.md](../../AGENTS.md). **No** se tocó nada que dependa de [Q-13](questions-for-owner.md#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante)/[Q-03](questions-for-owner.md#q-03-c-03-estado-de-cancelacion-de-despacho)/[Q-04](questions-for-owner.md#q-04-c-04-cart-es-un-estado-de-customerorder).

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
| `Zentric.Application/Orders/Commands/AddOrderItemCommand.cs` | **[H-09](spec-conformance-matrix.md):** eliminado el `catch (Exception)`; precondición de negocio explícita (`Status != Cart` → `Result.Failure`). Las excepciones catastróficas ya no se silencian |
| `Zentric.Api/Program.cs` | **[H-11](spec-conformance-matrix.md):** `AddProblemDetails()`, `AddValidatorsFromAssemblyContaining<CreateCartCommand>()`, `AddOpenBehavior(typeof(ValidationBehavior<,>))` y `UseExceptionHandler()` (sin endpoint inexistente) |
| `Zentric.Application/Class1.cs`, `Zentric.Infrastructure/Class1.cs` | **[H-12](spec-conformance-matrix.md):** eliminados (verificado antes: `Class1` no estaba referenciado en ningún archivo) |

### Cambios de pruebas

| Archivo | Casos | Cubre |
|---|---|---|
| `Zentric.Tests/UseCases/CreateCartCommandValidatorTests.cs` | 2 | `CreateCartCommandValidator` |
| `Zentric.Tests/UseCases/AddOrderItemCommandValidatorTests.cs` | 12 | `AddOrderItemCommandValidator`, con teorías para cantidades, precios y monedas |
| `Zentric.Tests/UseCases/CreateFulfillmentOrderCommandValidatorTests.cs` | 3 | `CreateFulfillmentOrderCommandValidator` |
| `Zentric.Tests/UseCases/ValidationBehaviorTests.cs` | 4 | Comportamiento del pipeline: rama `Result` y rama `Result<T>`, sin invocar el handler cuando falla |
| `Zentric.Tests/UseCases/MediatRValidationPipelineTests.cs` | 7 | **Integración DI real** (mismo registro que `Program.cs`) con repositorios falsos: incluye la regresión de [H-09](spec-conformance-matrix.md) |
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

`[PENDIENTE]` Sigue sin cobertura: los VOs `Email`/`FullName` y el agregado `Buyer` ([T-002b](migration-to-sdd-plan.md)), el
endpoint HTTP real (no se levantó la API) y el mapeo EF contra PostgreSQL.

