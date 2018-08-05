using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Plural.Data.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Plural.Data
{
    public class PluralSeeder
    {
        private readonly PluralContext _ctx;
        private readonly IHostingEnvironment _hosting;
        private readonly UserManager<StoreUser> _userManager;
        public PluralSeeder(PluralContext ctx, IHostingEnvironment hosting, UserManager<StoreUser> userManager)
        {
            _ctx = ctx;
            _hosting = hosting;
            _userManager = userManager;
        }

        public async Task Seed()
        {
            _ctx.Database.EnsureCreated();

            var user = await _userManager.FindByEmailAsync("mail@mail.com");

            if(user == null)
            {
                user = new StoreUser()
                {
                    FirstName = "FirstName",
                    LastName = "LastName",
                    UserName = "stelasido",
                    Email = "mail@mail.com"
                };

                // BY DEFAULT THERE ARE RULES ABOUT PASSWORD COMPLEXITY
                var result = await _userManager.CreateAsync(user, "P@ssw0rd!");

                if(result == IdentityResult.Success)
                {
                    throw new InvalidOperationException("failled to create");
                }
            }

            if(!_ctx.Products.Any())
            {
                var filepath = Path.Combine(_hosting.ContentRootPath, "Data/art.json");
                var json = File.ReadAllText(filepath);

                var products = JsonConvert.DeserializeObject<IEnumerable<Product>>(json);
                _ctx.Products.AddRange(products);

                var order = new Order()
                {
                    OrderDate = DateTime.Now,
                    OrderNumber = "12345",
                    User = user,
                    Items = new List<OrderItem>()
                    {
                        new OrderItem()
                        {
                            Product = products.First(),
                            Quantity = 5,
                            UnitPrice = products.First().Price
                        }
                    }
                };
                _ctx.Orders.Add(order);
                _ctx.SaveChanges();
            }
        }
    }
}
