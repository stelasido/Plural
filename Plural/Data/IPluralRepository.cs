using System.Collections.Generic;
using Plural.Data.Entities;

namespace Plural.Data
{
    public interface IPluralRepository
    {
        IEnumerable<Product> GetAllProducts();
        IEnumerable<Product> GetProductsByCategory(string category);
        bool SaveAll();
        IEnumerable<Order> GetAllOrders(bool includeOrders);
        Order GetOrderById(string username, int id);
        void AddEntity(object model);
        IEnumerable<Order> GetAllOrdersByUser(string username, bool includeItems);
    }
}