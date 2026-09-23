# Capa de Presentación - Endpoints API

Este documento detalla los contratos HTTP (Endpoints RESTful) expuestos por `Zentric.Api` y documentados interactivamente mediante **Swagger UI** (`/swagger`).

## 1. Patrón Global de Respuestas
Todos los controladores exponen rutas bajo `/api/[controller]` y manejan respuestas consistentes:
- **Éxito (200 OK):** Retorna el valor o identificador generado, o 200 OK vacío en comandos de mutación.
- **Fallo de Negocio o Validación (400 Bad Request):** Respuestas mapeadas automáticamente al estándar **RFC 7807 (Problem Details)** a través de `ProblemDetails` y el pipeline de `ValidationBehavior`.
- **Fallas Técnicas (500 Internal Server Error):** Interceptadas por `app.UseExceptionHandler()` para no filtrar detalles de infraestructura hacia el cliente.

---

## 2. Catálogo Completo de Endpoints REST

### 2.1. API: Usuarios y Vendedores (`/api/users`)
| Método | Endpoint | Comando (Application) | Descripción |
|---|---|---|---|
| POST | `/api/users` | `CreateUserCommand` | Registra un usuario (Comprador, Vendedor, Operador, Admin, Supervisor) con validación de unicidad de email y cédula. |

### 2.2. API: Bodegas (`/api/warehouses`)
| Método | Endpoint | Comando (Application) | Descripción |
|---|---|---|---|
| POST | `/api/warehouses` | `CreateWarehouseCommand` | Crea una bodega (Marketplace o Vendor) validando coherencia de asignación de vendedor. |

### 2.3. API: Inventario Distribuido (`/api/inventories`)
| Método | Endpoint | Comando (Application) | Descripción |
|---|---|---|---|
| POST | `/api/inventories/stock` | `AddStockCommand` | Da de alta o reabastece existencias disponibles para una variante en una bodega específica. |

### 2.4. API: Catálogo de Productos (`/api/catalog`)
| Método | Endpoint | Comando (Application) | Descripción |
|---|---|---|---|
| POST | `/api/catalog/products` | `CreateProductCommand` | Crea un producto físico o digital con sus variantes forzosas (SKUs). |
| POST | `/api/catalog/products/{id}/publish` | `PublishProductCommand` | Hace visible el producto para la compra pública. |

### 2.5. API: Carrito y Pedidos (`/api/orders`)
| Método | Endpoint | Comando (Application) | Descripción |
|---|---|---|---|
| POST | `/api/orders/cart` | `CreateCartCommand` | Inicializa un nuevo pedido (carrito) para un comprador. |
| POST | `/api/orders/cart/items` | `AddOrderItemCommand` | Añade una variante al carrito con cantidad y precio. |
| POST | `/api/orders/{orderId}/checkout` | `CheckoutOrderCommand` | Pasa de Carrito a Pendiente de Pago, ejecutando la reserva de stock. |
| POST | `/api/orders/{orderId}/pay` | `PayOrderCommand` | Confirma la transacción y pasa el pedido a Pagado (`Paid`). |

### 2.6. API: Logística y Despachos (`/api/logistics`)
| Método | Endpoint | Comando (Application) | Descripción |
|---|---|---|---|
| POST | `/api/logistics/fulfillment` | `CreateFulfillmentOrderCommand` | Genera una orden de despacho logístico a partir de un pedido pagado. |
| POST | `/api/logistics/fulfillment/{id}/dispatch` | `DispatchFulfillmentCommand` | Marca el despacho como enviado (`Shipped`). |
| POST | `/api/logistics/fulfillment/cancel-ghost-stock` | `CancelFulfillmentOrderDueToNoStockCommand` | Cancela por falta física de stock, reconcilia inventario y detona solicitud de devolución. |

### 2.7. API: Devoluciones y Reembolsos (`/api/returns`)
| Método | Endpoint | Comando (Application) | Descripción |
|---|---|---|---|
| POST | `/api/returns/request` | `RequestReturnCommand` | Inicia una solicitud de devolución (prohibido para productos digitales). |
| POST | `/api/returns/{id}/inspect` | `InspectReturnCommand` | Registra la inspección física de control de calidad por el operador logístico. |
| POST | `/api/returns/{id}/approve` | `ApproveReturnCommand` | Aprobación comercial final del vendedor con reingreso al inventario como Usado. |

### 2.8. API: Facturación y Pagos (`/api/billing`)
| Método | Endpoint | Comando (Application) | Descripción |
|---|---|---|---|
| POST | `/api/billing/invoices/generate/{orderId}` | `GenerateInvoicesCommand` | Genera la Factura Maestra consolidada, el Detalle Zentric y la Factura de Vendedor (Split). |

---

## 3. Documentación OpenAPI y Swagger UI

- **Swagger UI:** Accesible en entorno de desarrollo y Docker en `http://localhost:5076/swagger`.
- **Especificación OpenAPI v1:** `http://localhost:5076/swagger/v1/swagger.json` listo para importación directa en Postman.
