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
