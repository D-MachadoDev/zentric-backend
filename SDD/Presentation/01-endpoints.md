# Capa de Presentación - Endpoints API

Este documento detalla los contratos HTTP (Endpoints RESTful) expuestos por `Zentric.Api`.

## 1. Patrón Global de Respuestas
Todos los controladores heredan de `ApiControllerBase`.
- **Éxito (200 OK):** Retorna el valor directamente o vacío si es un comando sin retorno.
- **Fallo de Negocio (400 Bad Request):** Cualquier falla proveniente de un `Result<T>` de la capa de Aplicación es interceptada y mapeada automáticamente al estándar **RFC 7807 (Problem Details)** para respuestas consistentes.

## 2. API: Pedidos (Orders)
**Ruta Base:** `/api/orders`

| Método | Endpoint         | Comando (Application) | Descripción |
|--------|------------------|-----------------------|-------------|
| POST   | `/cart`          | `CreateCartCommand`   | Inicializa un nuevo pedido (carrito) para un BuyerId. |
| POST   | `/{id}/items`    | `AddOrderItemCommand` | Añade una variante (SKU) al carrito con cantidad y precio. |

## 3. API: Logística (Logistics)
**Ruta Base:** `/api/logistics`

| Método | Endpoint         | Comando (Application) | Descripción |
|--------|------------------|-----------------------|-------------|
| POST   | `/fulfillment`   | `CreateFulfillmentOrderCommand` | Genera una orden de despacho logístico a partir de un pedido. |

## 4. Próximos Endpoints a Implementar
- **Facturación (`/api/billing`):** Emisión de facturas maestras y detalle de vendedor.
- **Devoluciones (`/api/returns`):** Solicitud y flujos de aprobación de devoluciones.
- **Catálogo (`/api/products`):** Creación y publicación de productos.

## 5. Estado real verificado (2026-09-18)

`[CONFIRMADO]` Implementados **3 endpoints** en 2 controladores (`OrdersController`, `LogisticsController`),
exactamente los que documentan :2 y :3. Todo lo demás de esta spec es aspiracional.

| Hallazgo | Evidencia | Impacto |
|---|---|---|
| ~~**[H-11](../00-bootstrap/spec-conformance-matrix.md)** El middleware de excepciones es "simplificado": `app.UseExceptionHandler("/error")` **sin** endpoint `/error` ni `AddProblemDetails()`~~ | **CORREGIDO (SPEC-007, 2026-09-18):** `builder.Services.AddProblemDetails()` + `app.UseExceptionHandler()` | Incumplía [AGENTS.md :3.4](../../AGENTS.md) → **resuelto** (`[PENDIENTE]` verificación por HTTP real) |
| `ApiControllerBase.HandleResult` sí mapea `Result<T>` → `ProblemDetails` 400 | `ApiControllerBase.cs:14-42` | Cumple parcialmente la especificación de fallo de negocio, aunque con `Title` fijo en inglés |
| `FluentValidation` está referenciado pero **no hay validadores ni pipeline** | `Zentric.Application.csproj` vs 0 archivos `AbstractValidator` | La validación de entrada **no existe** (incumple [AGENTS.md :4.3](../../AGENTS.md#43-application-api-agent)) |
| Rutas verificadas por atributo de clase (`[Route("api/[controller]")]` heredado de `ApiControllerBase`) | `ApiControllerBase.cs:8` | Coincide con [`:2`](../../Zentric.Api/Controllers/ApiControllerBase.cs#L2) y [`:3`](../../Zentric.Api/Controllers/ApiControllerBase.cs#L3) |
| `[PENDIENTE]` La API **no se levantó**: 0 peticiones HTTP ejecutadas | — | No hay evidencia de runtime |
| `[RIESGO]` **[R-16](../00-bootstrap/risks-and-gaps.md)** credenciales en claro en `appsettings.json` | `Zentric.Api/appsettings.json` | Violación del DoD :8 ([`AGENTS.md`](../../AGENTS.md)) |


