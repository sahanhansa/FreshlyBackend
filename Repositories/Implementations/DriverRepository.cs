using FreshlyBackendNew.Data;
using FreshlyBackendNew.Repositories.Interfaces;

namespace FreshlyBackendNew.Repositories.Implementations
{
    public class DriverRepository : IDriverRepository
    {
        private readonly ApplicationDbContext _context;

        public DriverRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Implement repository methods here
    }
}
