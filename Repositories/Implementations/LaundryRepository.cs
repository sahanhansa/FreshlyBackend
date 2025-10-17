using FreshlyBackendNew.Data;
using FreshlyBackendNew.Repositories.Interfaces;

namespace FreshlyBackendNew.Repositories.Implementations
{
    public class LaundryRepository : ILaundryRepository
    {
        private readonly ApplicationDbContext _context;

        public LaundryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Implement repository methods here
    }
}
