namespace Zentric.Domain.Inventories
{
    public sealed class Inventory
    {
        public Guid Id { get; init; }

        // VariantId (SKU): clave del inventario segun ADR-0002. Cada variante de un
        // producto tiene su propio control de stock por bodega.
        public Guid VariantId { get; private set; }
        public Guid WarehouseId { get; private set; }
        public int AvalibleQuantity { get; private set; }
        public int ReservedQuantity { get; private set; }
        public int DamagedQuantity { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public bool IsDeleted => DeletedAt.HasValue; //?  Si yo no quiero vender(Zentric) mas iphones en la bodega tal softDeleted

        private Inventory()
        {
            // For EF Core
        }
        
        public Inventory(Guid variantId, Guid warehouseId, int avalibleQuantity, int reservedQuantity, int damagedQuantity)
        {
            if (variantId == Guid.Empty)
            {
                throw new ArgumentException("VariantId is required.", nameof(variantId));
            }

            if (warehouseId == Guid.Empty)
            {
                throw new ArgumentException("WarehouseId is required.", nameof(warehouseId));
            }

            if (avalibleQuantity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(avalibleQuantity), "Available quantity cannot be negative.");
            }

            if (reservedQuantity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(reservedQuantity), "Reserved quantity cannot be negative.");
            }

            if (damagedQuantity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(damagedQuantity), "Damaged quantity cannot be negative.");
            }

            Id = Guid.NewGuid();
            VariantId = variantId;
            WarehouseId = warehouseId;
            AvalibleQuantity = avalibleQuantity;
            ReservedQuantity = reservedQuantity;
            DamagedQuantity = damagedQuantity;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = CreatedAt;
        }

        public void UpdateQuantities(int avalibleQuantity, int reservedQuantity, int damagedQuantity)
        {
            if (avalibleQuantity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(avalibleQuantity), "Available quantity cannot be negative.");
            }

            if (reservedQuantity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(reservedQuantity), "Reserved quantity cannot be negative.");
            }

            if (damagedQuantity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(damagedQuantity), "Damaged quantity cannot be negative.");
            }

            AvalibleQuantity = avalibleQuantity;
            ReservedQuantity = reservedQuantity;
            DamagedQuantity = damagedQuantity;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddStock(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity to add must be greater than zero.");
            }

            AvalibleQuantity += quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ReserveStock(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity to reserve must be greater than zero.");
            }

            if (AvalibleQuantity < quantity)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "Not enough available stock to reserve.");
            }

            AvalibleQuantity -= quantity;
            ReservedQuantity += quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        public void DispatchStock(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity to dispatch must be greater than zero.");
            }

            if (AvalibleQuantity < quantity)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "Not enough available stock.");
            }

            ReservedQuantity -= quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsDamaged(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity to mark as damaged must be greater than zero.");
            }

            if (AvalibleQuantity < quantity)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "Not enough available stock to mark as damaged.");
            }

            AvalibleQuantity -= quantity;
            DamagedQuantity += quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ReturnToAvalible(int quantity)
        {
            // INV-01 / invariante 1 (SDD/Domain/06-business-rules.md §1,
            // SDD/Domain/04-invariants-and-rules.md §1): AvailableQuantity nunca puede
            // ser negativo. Sin esta guarda, una cantidad no positiva (p. ej. -100)
            // reducia AvailableQuantity por debajo de cero. Correccion T-003 (H-08).
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity to return must be greater than zero.");
            }

            if (ReservedQuantity < quantity)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "Not enough reserved stock to return.");
            }

            ReservedQuantity -= quantity;
            AvalibleQuantity += quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ReciveReturnedStock(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity to receive must be greater than zero.");
            }

            AvalibleQuantity += quantity;
            UpdatedAt = DateTime.UtcNow;
        }


        //? Validar esto un inventario se puede eliminar si tiene existencias físicas, si tiene existencias físicas no se puede eliminar, se debe transferir a otro inventario o ajustar a cero.
        public void MarkAsDeleted()
        {
            if (IsDeleted)
            {
                throw new InvalidOperationException("Inventory is already deleted.");
            }

            if (AvalibleQuantity > 0 || ReservedQuantity > 0 || DamagedQuantity > 0)
            {
                throw new InvalidOperationException("No puedes eliminar un inventario que aún tiene existencias físicas. Debes transferirlas, ajustarlas a cero o despacharlas primero.");
            }

            DeletedAt = DateTime.UtcNow;
            UpdatedAt = DeletedAt.Value;
        }

    }
    
}