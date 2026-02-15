using TruckStore.Models;

namespace TruckStore.Services.Interfaces
{
    public interface IOrderValidator 
    { 
        bool Validate(TruckOrder order); 
    }
}
