# 06. Reglas de Negocio e Invariantes

Este documento consolida y lista las reglas de negocio estrictas de Zentric. El código de la capa de Dominio debe defender estas reglas mediante validaciones defensivas, lanzando excepciones de dominio o retornando fallos funcionales si se intentan quebrantar.

## 1. Gestión de Inventario
- **INV-01 (Invariante Absoluta):** Bajo ninguna circunstancia (ni siquiera por condiciones de carrera) el `AvailableQuantity` de un `InventoryItem` puede ser un valor negativo.
- **INV-02 (Bodega única con fraccionamiento de contingencia):** cuando exista una bodega individual que cubra la cantidad total de la línea, el despacho se cumple desde esa única bodega (el envío no se divide). Solo si **ninguna** bodega individual cubre la cantidad solicitada, la reserva se fracciona entre varias bodegas; en ese caso el despacho se fragmenta en múltiples guías (Shipments), una por bodega de origen. Si la suma de todas las bodegas no alcanza la cantidad solicitada, la operación falla y no se reserva nada. *(Contradicción C-01 resuelta por decisión del owner el 2026-09-17; ver `SDD/Adr/0001-reserva-fragmentacion-contingencia.md`.)*
- **INV-03 (Unidad de stock = variante):** el inventario se lleva por `VariantId` (SKU) y bodega, nunca por producto. La clave de stock es la pareja `(VariantId, WarehouseId)` y el SKU es único dentro del producto. *(Decisión del owner el 2026-09-17; ver `SDD/Adr/0002-clave-inventario-variantid.md`.)*

## 2. Gestión de Pedidos
- **PED-01 (Reserva Temporal y Timeout):** La adición al carrito descuenta el stock de manera preventiva. Existe un límite fijo (estándar: 15 minutos) sin concretar el pago, tras el cual el inventario se libera automáticamente (`CartExpiredDomainEvent`).
- **PED-02 (Consolidación Comercial):** El comprador siempre debe ver y pagar un **único pedido consolidado** (`CustomerOrder`), incluso si compra productos de 5 vendedores diferentes. 
- **PED-03 (Facturación Centralizada):** ZENTRIC actúa como *Merchant of Record*. Por lo tanto, emite una sola factura global directamente al comprador (no exigiéndole a cada vendedor facturar al cliente final).

## 3. Catálogo de Productos
- **CAT-01 (Auto-publicación):** Los Vendedores son autónomos sobre su catálogo. Todo producto creado o editado entra de inmediato a estado `Published`, sin procesos de aprobación administrativa previa.
- **CAT-02 (Productos Digitales):** No requieren logística ni inventario, y su entrega se considera instantánea tras la aprobación del pago.
- **CAT-03 (Variante obligatoria en físicos):** un producto `Physical` exige al
  menos una variante, porque la variante es la unidad de stock (INV-03): sin ella
  no hay inventario ni reserva posible. Un producto `Digital` (CAT-02) puede nacer
  sin variantes. *(Decisión del owner el 2026-09-17 — Q-10 = C3; ver
  `SDD/Adr/0003-variante-obligatoria-productos-fisicos.md`.)*

## 4. Excepciones Logísticas
- **EXC-01 (Cancelación por Stock Fantasma):** Si al empacar físicamente un producto la unidad está dañada o no existe, el Vendedor (u Operador Logístico) puede cancelar unilateralmente esa orden de despacho específica.
- **EXC-02 (Reembolsos Parciales Automáticos):** La cancelación mencionada en EXC-01 no afecta todo el Pedido Maestro. Detona un reembolso exclusivo por la línea cancelada, mientras que el resto del pedido continúa su flujo logístico normal.

## 5. Proceso de Posventa (Devoluciones)
- **DEV-01 (Aprobación Dual):** El procesamiento de un reintegro de producto físico exige estricta validación en dos niveles: 
  1. Revisión de condición física (Operador Logístico).
  2. Aceptación comercial final y reingreso de stock (Vendedor).
