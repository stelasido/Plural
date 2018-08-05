using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Plural.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plural.Data
{
    public class PluralRepository : IPluralRepository
    {
        private readonly PluralContext _ctx;
        private readonly ILogger _logger;

        public PluralRepository(PluralContext ctx, ILogger<PluralRepository> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        public void AddEntity(object model)
        {
            _ctx.Add(model);
        }

        public IEnumerable<Order> GetAllOrders(bool includeItems)
        {
            if (includeItems)
            {
                return _ctx.Orders.Include(o => o.Items)
                    .ThenInclude(i => i.Product).ToList();
            }
            return _ctx.Orders.ToList();
        }

        public IEnumerable<Order> GetAllOrdersByUser(string username, bool includeItems)
        {
            if (includeItems)
            {
                return _ctx.Orders
                    .Where(o => o.User.UserName == username)
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                    .ToList();
            }
            return _ctx.Orders.ToList();
        }

        public IEnumerable<Product> GetAllProducts()
        {
            _logger.LogInformation("HELLO");
            return _ctx.Products.OrderBy(p => p.Title).ToList();
        }

        public Order GetOrderById(string username, int id)
        {
            return _ctx.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.Id == id && o.User.UserName == username)
                .FirstOrDefault();
        }

        public IEnumerable<Product> GetProductsByCategory(string category)
        {
            return _ctx.Products.Where(p => p.Category == category).ToList();
        }

        public bool SaveAll()
        {
            return _ctx.SaveChanges() > 0;
        }
    }
}
