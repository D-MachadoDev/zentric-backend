# 03. Value Objects y Enumeraciones

## 1. Value Objects Principales (Inmutables)

### `Money`
- **Estructura:** `Amount` (decimal), `Currency` (string, ej. "USD").
- **Reglas:** Inmutable. El monto no puede ser negativo al instanciarse para un precio. Provee métodos limpios para `Add(Money)` y `Subtract(Money)`.

### `Address`
- **Estructura:** `Street`, `City`, `State`, `ZipCode`, `Country`.
- **Reglas:** Inmutable. Representa un punto de entrega o ubicación de bodega.

### Identificadores Fuertemente Tipados (Strongly-Typed IDs)
Implementados para evitar cruce de tipos primitivos.
- `UserId`, `ProductId`, `VariantId`, `WarehouseId`, `CustomerOrderId`, `FulfillmentOrderId`.

---

## 2. Enumeraciones Clave (State Definitions)

### `ProductType`
- `Physical`: Sujeto a flujos logísticos, inventario y devoluciones.
- `Digital`: Venta final. Entrega inmediata tras pago. Devoluciones prohibidas.

### `WarehouseType`
- `Marketplace`: Custodiado por Zentric. Prioridad alta en algoritmos de reserva. Ajuste manual exclusivo de Admin/Logística.
- `Vendor`: Custodiado por el Vendedor. Ajuste manual permitido al Vendedor dueño.

### `CustomerOrderStatus`
- `PendingPayment`: Nace al hacer checkout. Dispara el timer de reserva.
- `Paid`: Pago simulado exitosamente. Dispara orquestación logística.
- `PartiallyDelivered`: Estado mixto (ej. digital entregado, físico en tránsito).
- `Completed`: Todo cerrado.
- `Cancelled`: Por timeout de pago, o por Retracto Temprano (`CancelEarly`).

### `FulfillmentOrderStatus`
- `Pending`: Listo para iniciar empaque por el vendedor o bodega.
- `Packed`: Empaque finalizado.
- `Shipped`: Entregado a la transportadora.
- `Delivered`: Recibido por el comprador.
- `CancelledByStockBreak`: Stock fantasma detectado.

### `ReturnStatus`
- `Requested`: Iniciado por comprador.
- `LogisticsApproved`: Validado físicamente en bodega.
- `SellerApproved`: Aprobación comercial final del vendedor (Doble verificación exitosa).
- `Refunded`: Dinero devuelto.
- `Rejected`: Denegado por mal uso o condición inaceptable.
