using FreshlyBackendNew.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LaundryItemServiceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LaundryItemServiceController(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
