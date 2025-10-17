using FreshlyBackendNew.Data;
using FreshlyBackendNew.Repositories.Interfaces;
using FreshlyBackendNew.Repositories.Implementations;
using Microsoft.EntityFrameworkCore.Storage;

namespace FreshlyBackendNew.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction _transaction;
        private readonly Dictionary<Type, object> _repositories = new();

        public IOrderRepository Orders { get; }
        public ICustomerRepository Customers { get; }
        public ILaundryRepository Laundries { get; }
        public IDriverRepository Drivers { get; }

        public UnitOfWork(
            ApplicationDbContext context,
            IOrderRepository orders,
            ICustomerRepository customers,
            ILaundryRepository laundries,
            IDriverRepository drivers)
        {
            _context = context;
            Orders = orders;
            Customers = customers;
            Laundries = laundries;
            Drivers = drivers;
        }

        public IRepository<T> Repository<T>() where T : class
        {
            if (_repositories.ContainsKey(typeof(T)))
                return _repositories[typeof(T)] as IRepository<T>;

            var repository = new Repository<T>(_context);
            _repositories.Add(typeof(T), repository);
            return repository;
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task BeginTransactionAsync() =>
            _transaction = await _context.Database.BeginTransactionAsync();

        public async Task CommitTransactionAsync()
        {
            await _transaction?.CommitAsync();
            _transaction?.Dispose();
        }

        public async Task RollbackTransactionAsync()
        {
            await _transaction?.RollbackAsync();
            _transaction?.Dispose();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}