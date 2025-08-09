using FreshlyBackendNew.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverNoteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DriverNoteController(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
