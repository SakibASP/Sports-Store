using SportsStore.Models;

namespace SportsStore.Interfaces
{
    public interface IOrderProcessor
    {
        Task ProcessOrder(Cart cart, ShippingDetails shippingDetails);
    }
}
