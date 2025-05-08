using FreshlyBackendNew.Data;
using Microsoft.AspNetCore.Mvc;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemporyBasketController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public TemporyBasketController(ApplicationDbContext context)
        {
            _context = context;
        }

    }
}
