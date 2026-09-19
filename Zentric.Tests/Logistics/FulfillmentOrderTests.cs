using Xunit;
using Zentric.Domain.Logistics;
using Zentric.Domain.Logistics.Enums;
using System;

namespace Zentric.Tests.Logistics
{
    public class FulfillmentOrderTests
    {
        [Fact]
        public void Constructor_Valid_CreatesOrder()
        {
            var order = new FulfillmentOrder(Guid.NewGuid(), Guid.NewGuid());
            Assert.Equal(FulfillmentStatus.Packed, order.Status);
        }

        [Fact]
        public void CancelDueToNoStock_WhenPacked_ChangesStatus()
        {
            var order = new FulfillmentOrder(Guid.NewGuid(), Guid.NewGuid());
            order.CancelDueToNoStock();
            Assert.Equal(FulfillmentStatus.CancelledNoStock, order.Status);
        }

        [Fact]
        public void CancelDueToNoStock_WhenDispatched_Throws()
        {
            var order = new FulfillmentOrder(Guid.NewGuid(), Guid.NewGuid());
            order.Dispatch();
            Assert.Throws<InvalidOperationException>(() => order.CancelDueToNoStock());
        }
    }
}
