using Zentric.Domain.Logistics.Entities;
using Zentric.Domain.Logistics.Enums;

namespace Zentric.Domain.Logistics
{
    public sealed class FulfillmentOrder
    {
        public Guid Id { get; init; }
        public Guid CustomerOrderId { get; private set; }
        public Guid VendorId { get; private set; }
        public FulfillmentStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private readonly List<Shipment> _shipments = new();
        public IReadOnlyCollection<Shipment> Shipments => _shipments.AsReadOnly();

        private FulfillmentOrder()
        {
        }

        public FulfillmentOrder(Guid customerOrderId, Guid vendorId)
        {
            if (customerOrderId == Guid.Empty)
                throw new ArgumentException("Customer order ID is required.", nameof(customerOrderId));

            if (vendorId == Guid.Empty)
                throw new ArgumentException("Vendor ID is required.", nameof(vendorId));

            Id = Guid.NewGuid();
            CustomerOrderId = customerOrderId;
            VendorId = vendorId;
            Status = FulfillmentStatus.Packed; // Según dictamen, es el estado base de logística.
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = CreatedAt;
        }

        public void AddShipment(Guid warehouseId, string trackingNumber)
        {
            if (Status == FulfillmentStatus.CancelledNoStock)
                throw new InvalidOperationException("Cannot add shipments to a cancelled order.");

            _shipments.Add(new Shipment(Id, warehouseId, trackingNumber));
            UpdatedAt = DateTime.UtcNow;
        }

        public void Dispatch()
        {
            if (Status == FulfillmentStatus.CancelledNoStock)
                throw new InvalidOperationException("Cannot dispatch a cancelled order.");

            if (Status == FulfillmentStatus.Dispatched)
                throw new InvalidOperationException("Order is already dispatched.");

            Status = FulfillmentStatus.Dispatched;
            UpdatedAt = DateTime.UtcNow;
        }
        
        public void Deliver()
        {
            if (Status != FulfillmentStatus.Dispatched)
                throw new InvalidOperationException("Order must be dispatched before being delivered.");
                
            Status = FulfillmentStatus.Delivered;
            UpdatedAt = DateTime.UtcNow;
        }

        public void CancelDueToNoStock()
        {
            if (Status == FulfillmentStatus.Dispatched || Status == FulfillmentStatus.Delivered)
                throw new InvalidOperationException("Cannot cancel a dispatched or delivered order due to stock ghost.");

            Status = FulfillmentStatus.CancelledNoStock;
            UpdatedAt = DateTime.UtcNow;
            // TODO: Domain event para detonar reembolso
        }
    }
}

