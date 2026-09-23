# Capa de Presentación - Endpoints API y Especificación OpenAPI / Swagger

Este documento define la especificación oficial (SSoT) de la capa de presentación de `Zentric.Api`, documentada interactivamente a través de **Swagger UI** (`/swagger`) y exportable como especificación **OpenAPI v1** (`/swagger/v1/swagger.json`).

---

## 1. Configuración de Seguridad y Esquema Bearer (OpenAPI)

Conforme a **ZENTRIC.md §3.2**, los mecanismos de autenticación técnica y sesiones web están fuera del alcance funcional central inicial del dominio. Sin embargo, para permitir la integración profesional con el frontend, pruebas de integración y colecciones de Postman:

- **Esquema de Seguridad:** `Bearer` (tipo HTTP, formato JWT).
- **Cabecera HTTP:** `Authorization: Bearer <token>`.
- **Swagger UI:** Dispone del botón interactivo **Authorize** configurado globalmente mediante `AddSecurityDefinition` y `AddSecurityRequirement`.
- **Comportamiento en esta fase:** Las rutas están abiertas a nivel de autorización técnica en desarrollo para facilitar la prueba de los 10 módulos de negocio, quedando preparadas para asociar `[Authorize]` y políticas RBAC basadas en los roles de `ZENTRIC.md §5` (`Buyer`, `Vendor`, `Admin`, `LogisticsOperator`, `Supervisor`) cuando se formalice el módulo técnico de Auth.

---

## 2. Patrón Global de Respuestas y Manejo de Errores

Todos los controladores heredan o implementan contratos HTTP RESTful con formato `application/json` y `application/problem+json`:

- **Éxito (200 OK):** Retorna el identificador `Guid` generado o el DTO correspondiente. Para operaciones de transición de estado sin retorno, devuelve `200 OK` vacío.
- **Fallo de Validación o Negocio (400 Bad Request):** Respuestas mapeadas obligatoriamente al estándar **RFC 7807 (Problem Details)**:
  ```json
  {
    "type": "https://tools.ietf.org/html/rfc7807",
    "title": "Bad Request",
    "status": 400,
    "detail": "Descripción de la invariante violada o error de validación."
  }
  ```
- **Falla Técnica no Controlada (500 Internal Server Error):** Interceptada por el middleware global `app.UseExceptionHandler()` para no filtrar detalles de infraestructura (cadenas de conexión, trazas de SQL) al cliente.

---

## 3. Catálogo Detallado de Endpoints por Bounded Context (Tags de Swagger)

### 3.1. Tag: `1. Usuarios y Roles` (`/api/users`)
| Método | Endpoint | Entrada / Comando | Respuestas | Descripción de Negocio e Invariantes |
|---|---|---|---|---|
| `POST` | `/api/users` | `CreateUserCommand` (Body) | `200 OK (Guid)`<br>`400 Bad Request (ProblemDetails)` | Registra un nuevo usuario en el sistema. Valida unicidad de `Email` e `IdentityDocument` de forma asíncrona. Asigna roles válidos: `Buyer`, `Vendor`, `Admin`, `LogisticsOperator`, `Supervisor`. |

---

### 3.2. Tag: `2. Bodegas` (`/api/warehouses`)
| Método | Endpoint | Entrada / Comando | Respuestas | Descripción de Negocio e Invariantes |
|---|---|---|---|---|
| `POST` | `/api/warehouses` | `CreateWarehouseCommand` (Body) | `200 OK (Guid)`<br>`400 Bad Request (ProblemDetails)` | Registra una bodega física de almacenamiento especificando nombre, ubicación y capacidad volumétrica ($m^3$). |

---

### 3.3. Tag: `3. Inventario y Stock` (`/api/inventories`)
| Método | Endpoint | Entrada / Comando | Respuestas | Descripción de Negocio e Invariantes |
|---|---|---|---|---|
| `POST` | `/api/inventories/stock` | `AddStockCommand` (Body) | `200 OK (Guid)`<br>`400 Bad Request (ProblemDetails)` | Ingresa existencias de un producto en una bodega, diferenciando entre unidades nuevas (`NewQuantity`) y usadas/reacondicionadas (`UsedQuantity`). |

---

### 3.4. Tag: `4. Catálogo de Productos` (`/api/catalog`)
| Método | Endpoint | Entrada / Comando | Respuestas | Descripción de Negocio e Invariantes |
|---|---|---|---|---|
| `POST` | `/api/catalog/products` | `CreateProductCommand` (Body) | `200 OK (Guid)`<br>`400 Bad Request (ProblemDetails)` | Da de alta la ficha técnica de un producto en estado borrador (`Draft`) con dimensiones físicas y precio base. |
| `POST` | `/api/catalog/products/{productId}/publish` | `productId` (Path) | `200 OK`<br>`400 Bad Request (ProblemDetails)` | Transita el estado del producto a `Published`, haciéndolo visible y adquirible por los compradores en el Marketplace. |

---

### 3.5. Tag: `5. Carrito y Órdenes` (`/api/orders`)
| Método | Endpoint | Entrada / Comando | Respuestas | Descripción de Negocio e Invariantes |
|---|---|---|---|---|
| `POST` | `/api/orders/cart` | `CreateCartCommand` (Body) | `200 OK (Guid)`<br>`400 Bad Request (ProblemDetails)` | Inicializa una orden de compra en estado inicial `Cart` vinculada a un comprador (`BuyerId`). |
| `POST` | `/api/orders/cart/items` | `AddOrderItemCommand` (Body) | `200 OK`<br>`400 Bad Request (ProblemDetails)` | Añade un producto al carrito verificando previamente existencias suficientes en el inventario disponible. |
| `POST` | `/api/orders/{orderId}/checkout` | `orderId` (Path) | `200 OK`<br>`400 Bad Request (ProblemDetails)` | Cierra el carrito, reserva el stock en bodega, inicia el temporizador de expiración de 15 minutos y genera paquetes (`FulfillmentOrders`) agrupados por `VendorId`. |
| `POST` | `/api/orders/{orderId}/pay` | `orderId` (Path) | `200 OK`<br>`400 Bad Request (ProblemDetails)` | Confirma la transacción económica exitosa, pasando el pedido a `Paid` y consolidando la reserva para despacho físico. |

---

### 3.6. Tag: `6. Logística y Despacho` (`/api/logistics`)
| Método | Endpoint | Entrada / Comando | Respuestas | Descripción de Negocio e Invariantes |
|---|---|---|---|---|
| `POST` | `/api/logistics/fulfillment` | `CreateFulfillmentOrderCommand` (Body) | `200 OK (Guid)`<br>`400 Bad Request (ProblemDetails)` | Crea una orden de preparación y empaque para los ítems pertenecientes a un vendedor y bodega específica. |
| `POST` | `/api/logistics/fulfillment/{id}/dispatch` | `id` (Path) | `200 OK`<br>`400 Bad Request (ProblemDetails)` | Marca el paquete como `Dispatched` y descuenta formalmente el stock reservado de la bodega. |
| `POST` | `/api/logistics/fulfillment/cancel-ghost-stock` | `CancelFulfillmentOrderDueToNoStockCommand` (Body) | `200 OK (Guid)`<br>`400 Bad Request (ProblemDetails)` | Reporta faltante físico en bodega (stock fantasma), cancela la orden de fulfillment y libera la reserva de existencias. |

---

### 3.7. Tag: `7. Devoluciones y Garantías` (`/api/returns`)
| Método | Endpoint | Entrada / Comando | Respuestas | Descripción de Negocio e Invariantes |
|---|---|---|---|---|
| `POST` | `/api/returns/request` | `RequestReturnCommand` (Body) | `200 OK (Guid)`<br>`400 Bad Request (ProblemDetails)` | Radica una solicitud de devolución para un producto entregado dentro del período de garantía legal. |
| `POST` | `/api/returns/{id}/inspect` | `id` (Path), `InspectReturnRequestDto` (Body) | `200 OK`<br>`400 Bad Request (ProblemDetails)` | Registra la inspección física en bodega dictaminando si la mercancía está en buen estado o dañada. |
| `POST` | `/api/returns/{id}/approve` | `id` (Path), `ApproveReturnCommand` (Body) | `200 OK`<br>`400 Bad Request (ProblemDetails)` | Aprueba comercialmente la devolución y dispara el evento de dominio `ReturnApprovedEvent` que reingresa el producto como stock usado (`UsedQuantity`). |

---

### 3.8. Tag: `8. Facturación y Liquidación` (`/api/billing`)
| Método | Endpoint | Entrada / Comando | Respuestas | Descripción de Negocio e Invariantes |
|---|---|---|---|---|
| `POST` | `/api/billing/invoices/generate/{orderId}` | `orderId` (Path) | `200 OK (bool)`<br>`400 Bad Request (ProblemDetails)` | Emite la Factura Maestra consolidada para el comprador, el detalle de comisión tecnológica para Zentric (`ZentricDetail`) y las facturas split para cada vendedor. |

---

## 4. Disponibilidad y Consumo

- **Swagger UI Interactivo:** `http://localhost:5000/swagger` (o puerto local configurado).
- **Especificación OpenAPI JSON:** `http://localhost:5000/swagger/v1/swagger.json` listo para importación en Postman o generación de SDKs clientes para el frontend.
