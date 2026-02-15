using TruckStore.Models;

namespace TruckStore.Services.Interfaces
{
    public interface IDiscountService 
    { 
        decimal CalculateDiscount(TruckOrder order); 
    }
}
