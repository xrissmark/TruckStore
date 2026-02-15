using System;
using TruckStore.Models;
using TruckStore.Services;
using Xunit;

namespace TruckStore.Tests.Services
{
    public class FleetDiscountServiceTests
    {
        private readonly FleetDiscountService _service;

        public FleetDiscountServiceTests()
        {
            _service = new FleetDiscountService();
        }

        [Fact]
        public void CalculateDiscount_QuantityGreaterThanFive_ReturnsTenPercentDiscount()
        {
            // Arrange
            var order = new TruckOrder
            {
                ModelName = "Model X",
                Quantity = 6,
                BasePrice = 100_000m
            };

            // Act
            var discount = _service.CalculateDiscount(order);

            // Assert
            Assert.Equal(60_000m, discount);
        }

        [Fact]
        public void CalculateDiscount_QuantityEqualToFive_ReturnsZeroDiscount()
        {
            // Arrange
            var order = new TruckOrder
            {
                ModelName = "Model Y",
                Quantity = 5,
                BasePrice = 80_000m
            };

            // Act
            var discount = _service.CalculateDiscount(order);

            // Assert
            Assert.Equal(0m, discount);
        }

        [Fact]
        public void CalculateDiscount_QuantityLessThanFive_ReturnsZeroDiscount()
        {
            // Arrange
            var order = new TruckOrder
            {
                ModelName = "Model Z",
                Quantity = 2,
                BasePrice = 50_000m
            };

            // Act
            var discount = _service.CalculateDiscount(order);

            // Assert
            Assert.Equal(0m, discount);
        }

        [Fact]
        public void CalculateDiscount_NullOrder_ThrowsNullReferenceException()
        {
            // Arrange
            TruckOrder order = null;

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => _service.CalculateDiscount(order));
        }
    }
}
