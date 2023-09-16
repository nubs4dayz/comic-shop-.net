using Microsoft.EntityFrameworkCore;
using Shop.Domain;
using Shop.Domain.DomainModels;
using Shop.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Repository.Implementation
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<Order> entities;
        string errorMessage = string.Empty;

        public OrderRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<Order>();
        }

        public List<Order> getAllOrders()
        {
            return entities
                .Include(z => z.User)
                .Include(z => z.ComicInOrders)
                .Include("ComicInOrders.Comic")
                .ToListAsync().Result;
        }

        public Order getOrderDetails(BaseEntity model)
        {
            return entities
               .Include(z => z.User)
               .Include(z => z.ComicInOrders)
               .Include("ComicInOrders.Comic")
               .SingleOrDefaultAsync(z => z.Id == model.Id).Result;
        }
    }
}
