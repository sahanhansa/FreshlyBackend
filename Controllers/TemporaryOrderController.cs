using FreshlyBackendNew.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemporaryOrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TemporaryOrderController(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
