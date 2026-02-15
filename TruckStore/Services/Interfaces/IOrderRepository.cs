using TruckStore.Models;

namespace TruckStore.Services.Interfaces
{
    public interface IOrderRepository 
    { 
        void Save(TruckOrder order); 
    }
}
