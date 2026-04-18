using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.DAL
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        IOrderRepository Orders { get; }
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Favourite> Favourites { get; }
        IGenericRepository<Payment> Payments { get; }
        ICartRepository Carts { get; }

        Task<int> SaveChangesAsync();

    }
}
