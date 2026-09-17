# 01. Modelos de Dominio (Aggregate Roots y Entidades)

Este documento define los **Aggregate Roots (AR)** y las **Entidades** principales del sistema Zentric, agrupados por *Bounded Contexts*, garantizando la pureza del modelo sin dependencias de infraestructura.

## 1. Bounded Context: Identity & Access (Usuarios)

### Aggregate Root: `User`
Representa a cualquier individuo con acceso a la plataforma (Comprador, Vendedor, Op. Logístico, Admin, Supervisor).
- **Propiedades:** `Id` (UserId), `FullName` (string), `Email` (EmailAddress), `Role` (UserRole), `Status` (UserStatus), `IdentityDocument` (string).
- **Reglas de Negocio:** 
  - El correo y documento de identidad deben ser únicos en todo el sistema.
  - Un usuario solo puede tener un único rol asignado.
- **Comportamientos (Métodos):** `Lock()`, `Unlock()`, `ChangeRole()`.

## 2. Bounded Context: Catalog (Catálogo)

### Aggregate Root: `Product`
Representa un bien ofrecido por un Vendedor.
- **Propiedades:** `Id` (ProductId), `SellerId` (UserId), `Title` (string), `Description` (string), `Price` (Money), `Type` (ProductType - Físico o Digital), `Status` (ProductStatus).
- **Entidades Hijas: `ProductVariant`** — combinación vendible del producto (ej. Talla/Color). Su `Id` **es** el `VariantId` y actúa como SKU para el inventario (ADR-0002). Se compone de atributos: `VariantAttribute` (nombre + valor). El SKU es único dentro del producto.
- **Reglas de Negocio:**
  - Al crearse, su estado es `Published` automáticamente (sin flujo de aprobación).
  - **Variante obligatoria solo para físicos (CAT-03):** un producto `Physical`
    exige al menos una variante (ver `SDD/Domain/06-business-rules.md` CAT-03 y
    `SDD/Adr/0003-variante-obligatoria-productos-fisicos.md`). Un producto
    `Digital` (CAT-02: sin logística ni inventario) puede nacer sin variantes.
- **Comportamientos:** `UpdatePrice()`, `Suspend()`, `Discontinue()`, `AddVariant()`, `RemoveVariant()`.

## 3. Bounded Context: Inventory (Inventario y Bodegas)

### Aggregate Root: `Warehouse`
Espacio físico donde se almacenan los productos.
- **Propiedades:** `Id` (WarehouseId), `OwnerId` (UserId - null si es bodega de Zentric), `Location` (Address), `Type` (WarehouseType).
- **Reglas de Negocio:** Distingue entre bodegas propias del marketplace y de los vendedores.

### Aggregate Root: `InventoryItem`
El control transaccional del stock de una variante específica (SKU) en una bodega específica.
- **Propiedades:** `Id` (InventoryItemId), `VariantId` (VariantId), `WarehouseId` (WarehouseId), `AvailableQuantity` (int), `ReservedQuantity` (int).
- **Clave de stock:** la pareja `(VariantId, WarehouseId)`, según ADR-0002.
- **Reglas de Negocio:** `AvailableQuantity` nunca puede ser negativo bajo ninguna circunstancia (invariante crítica).
- **Comportamientos:** `Reserve(quantity)`, `Release(quantity)`, `Deduct(quantity)`, `Adjust(quantity)`, `ManualAdjust(quantity, userId, role)`.

## 4. Bounded Context: Ordering (Pedidos - Interfaz del Comprador)

### Aggregate Root: `CustomerOrder`
Es el pedido unificado y global de cara al comprador.
- **Propiedades:** `Id` (OrderId), `BuyerId` (UserId), `Lines` (List<OrderLine>), `TotalAmount` (Money), `Status` (OrderStatus).
- **Entidades Hijas:**
  - `OrderLine`: Contiene `ProductId`, `Quantity`, `UnitPrice`, `LineTotal`, `ProductType`.
- **Reglas de Negocio:** 
  - Consolida compras de múltiples vendedores en una sola transacción financiera.
  - La factura global la emite Zentric basada en este objeto.
- **Comportamientos:** `Checkout()`, `MarkAsPaid()`, `ApplyPartialDelivery()`, `Cancel()`.

## 5. Bounded Context: Fulfillment & Logistics (Despachos - Interfaz del Vendedor)

### Aggregate Root: `FulfillmentOrder`
Orden de trabajo derivada de un `CustomerOrder`, pero específica para **un solo Vendedor**.
- **Propiedades:** `Id` (FulfillmentOrderId), `CustomerOrderId` (OrderId), `SellerId` (UserId), `Status` (FulfillmentStatus), `Shipments` (List<Shipment>).
- **Entidades Hijas:**
  - `Shipment`: Guía de despacho individual. Agrupa productos que salen de *una misma bodega*.
- **Reglas de Negocio:** 
  - Un `FulfillmentOrder` puede tener múltiples `Shipments` si el inventario del vendedor estaba fragmentado en varias bodegas.
  - El Vendedor puede cancelarlo unilateralmente si detecta quiebre de stock (Stock Fantasma).
- **Comportamientos:** `Pack()`, `Dispatch()`, `Deliver()`, `CancelByStockBreak()`.
