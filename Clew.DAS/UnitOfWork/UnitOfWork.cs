using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IProductRepository Products { get; }
        public IOrderRepository Orders { get; }
        public IGenericRepository<Category> Categories { get; }
        public IGenericRepository<Favourite> Favourites { get; }
        public IGenericRepository<Payment> Payments { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Products = new ProductRepository(context);
            Orders = new OrderRepository(context);
            Categories = new GenericRepository<Category>(context);
            Favourites = new GenericRepository<Favourite>(context);
            Payments = new GenericRepository<Payment>(context);
        }

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public void Dispose()
            => _context.Dispose();
    }
}
