## Refactoring OrderProcessor

The best approach to handling bloated classes is to refactor them using the SOLID principles. For example, in my truck sales application, I improved maintainability by decomposing the `OrderProcessor` into three separate services, each with a single responsibility.

- `IOrderValidator`: to check if the order is valid.
- `IDiscountService`: to apply the business rules. With this, we can unit test discount logic without needing a database.
- `IOrderRepository`: to handle the persistence in a database. This enables our code to be flexible and easy to swap between databases, like from SQL to NoSQL.

### Source Code

The source code for this project can be found on my GitHub.
https://github.com/xrissmark/TruckStore

### Implementation

![](https://media.discordapp.net/attachments/1090805418298712084/1472851478270971916/image.png?ex=69941354&is=6992c1d4&hm=54175a99914a327f0ac133fb338bd01feccd24537adffa3988737cf5917c4b68&=&format=webp&quality=lossless)

---

### Model

A model was created to represent the orders

```csharp
// Models/TruckOrder.cs
public class TruckOrder
{
    public string ModelName { get; set; }
    public int Quantity { get; set; }
    public decimal BasePrice { get; set; }
    public decimal TotalAmount { get; set; }
}
```

### Interfaces

Created some interfaces, as we need to use abstractions to reduce coupling. This way, we will depend on those interfaces, not on implementations, which we can swap anytime without rewriting the class that is calling it.

```csharp
// Services/Interfaces/IDiscountService.cs
using TruckStore.Models;
namespace TruckStore.Services.Interfaces
{
    public interface IDiscountService
    {
        decimal CalculateDiscount(TruckOrder order);
    }
}

// Services/Interfaces/IOrderRepository.cs
using TruckStore.Models;
namespace TruckStore.Services.Interfaces
{
    public interface IOrderRepository
    {
        void Save(TruckOrder order);
    }
}

// Services/Interfaces/IOrderValidator.cs
using TruckStore.Models;
namespace TruckStore.Services.Interfaces
{
    public interface IOrderValidator
    {
        bool Validate(TruckOrder order);
    }
}
```

### Implementation Services

Then created the implementation services, where the real logic resides... Now, if we need to create a new type of discount, such as Holiday Discounts, we can create it without changing a single line of code in the `OrderProcessor`.

```csharp
// Services/FleetDiscountService.cs
using TruckStore.Models;
using TruckStore.Services.Interfaces;

namespace TruckStore.Services
{
    public class FleetDiscountService : IDiscountService
    {
        public decimal CalculateDiscount(TruckOrder order)
        {
            Console.WriteLine($"[Service] Calculating applicable discount...");
            Thread.Sleep(1000); // Simulation purposes
            return order.Quantity > 5 ? order.BasePrice * order.Quantity * 0.10m : 0; ;
        }
    }
}
```

```csharp
// Services/OrderRepository.cs
using TruckStore.Models;
using TruckStore.Services.Interfaces;

namespace TruckStore.Services
{
    public class OrderRepository : IOrderRepository
    {
        public void Save(TruckOrder order)
        {
            Console.WriteLine($"[DB] Saving {order.Quantity} {order.ModelName} order to SQL Database.");
            Thread.Sleep(2000);
        }
    }
}
```

```csharp
// Services/TruckOrderValidator.cs
using TruckStore.Models;
using TruckStore.Services.Interfaces;

namespace TruckStore.Services
{
    public class TruckOrderValidator : IOrderValidator
    {
        public bool Validate(TruckOrder order)
        {
            Console.WriteLine("[Validation] Begining order validation.");
            Thread.Sleep(1000);

            if (order == null)
            {
                Console.WriteLine("[Validation Error] Order cannot be null.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(order.ModelName))
            {
                Console.WriteLine("[Validation Error] Truck model name is required.");
                return false;
            }

            if (order.Quantity <= 0)
            {
                Console.WriteLine("[Validation Error] Quantity must be at least 1 truck.");
                return false;
            }

            if (order.BasePrice <= 0)
            {
                Console.WriteLine("[Validation Error] Base price must be greater than zero.");
                return false;
            }

            Console.WriteLine($"[Validation] Order for {order.Quantity} {order.ModelName} verified.");
            return true;
        }
    }
}
```

### OrderProcessor

And finally, the refactored OrderProcessor, much more cleaner. Now it only orchestrates the process.

```csharp
// Business/OrderProcessor.cs
using TruckStore.Models;
using TruckStore.Services.Interfaces;

namespace TruckStore.Business
{
    public class OrderProcessor
    {
        private readonly IOrderValidator _validator;
        private readonly IDiscountService _discountService;
        private readonly IOrderRepository _repository;

        public OrderProcessor(IOrderValidator validator, IDiscountService discountService, IOrderRepository repository)
        {
            _validator = validator;
            _discountService = discountService;
            _repository = repository;
        }

        public void Process(TruckOrder order)
        {
            if (!_validator.Validate(order))
            {
                Console.WriteLine("Order validation failed.");
                return;
            }

            decimal discount = _discountService.CalculateDiscount(order);
            order.TotalAmount = (order.BasePrice * order.Quantity) - discount;

            _repository.Save(order);
            Console.WriteLine($"\nOrder processed successfully. Total: ${order.TotalAmount:N}");
        }
    }
}
```

### The Console Application / Dependency Injection

In the end, I transformed this example into a console application to fully implement the Dependency Injection. For this I used the `Microsoft.Extensions.DependencyInjection` NuGet package.
There, the `ServiceCollection` is the DI container, and the `ServiceProvider` is the engine that resolves the objects we registered in the container.

```csharp
// Program.cs
using Microsoft.Extensions.DependencyInjection;
using TruckStore.Business;
using TruckStore.Models;
using TruckStore.Services;
using TruckStore.Services.Interfaces;

namespace TruckStore
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient<IOrderValidator, TruckOrderValidator>()
                .AddTransient<IDiscountService, FleetDiscountService>()
                .AddTransient<IDiscountService, HolidaysDiscountService>()
                .AddTransient<IOrderRepository, OrderRepository>()
                .AddTransient<OrderProcessor>()
                .BuildServiceProvider();

            var processor = serviceProvider.GetRequiredService<OrderProcessor>();

            var myOrder = new TruckOrder
            {
                ModelName = "Volvo FH 540",
                Quantity = 10,
                BasePrice = 1_000_000m
            };

            processor.Process(myOrder);

            CreateEndMessage();
        }

        static void CreateEndMessage()
        {
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
```

---

### Extra: Unit Tests

I've created the unit tests for the `FleetDiscountService`. As we can see, we don't need a database, just a mock, to be able to test our discount service.

```csharp
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

```

![](https://media.discordapp.net/attachments/1090805418298712084/1472862692837429248/image.png?ex=69941dc6&is=6992cc46&hm=41477b517e890f032a3845c2fd2da7d8fae46b67b681959a57ba1e107c7567a5&=&format=webp&quality=lossless)
