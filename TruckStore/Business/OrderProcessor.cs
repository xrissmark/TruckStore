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
            Console.WriteLine($"\nOrder processed successfully. Total: ${order.TotalAmount}");
        }
    }
}
