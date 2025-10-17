using FreshlyBackendNew.Repositories.Interfaces;

namespace FreshlyBackendNew.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IOrderRepository Orders { get; }
        ICustomerRepository Customers { get; }
        ILaundryRepository Laundries { get; }
        IDriverRepository Drivers { get; }
        IRepository<T> Repository<T>() where T : class;
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}