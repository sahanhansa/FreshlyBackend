using FreshlyBackendNew.Data;
using FreshlyBackendNew.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DriverController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // GET: api/Driver
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Driver>>> GetDrivers()
        {
            return await _context.Drivers.Include(d => d.Address).ToListAsync();
        }
        
        // GET: api/Driver/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Driver>> GetDriver(Guid id)
        {
            var driver = await _context.Drivers
                .Include(d => d.Address)
                .FirstOrDefaultAsync(d => d.DriverId == id);
                
            if (driver == null)
            {
                return NotFound();
            }
            
            return driver;
        }
        
        // POST: api/Driver
        [HttpPost]
        public async Task<ActionResult<Driver>> CreateDriver(Driver driver)
        {
            if (driver == null)
            {
                return BadRequest("Driver data cannot be null");
            }
            
            // Ensure a new ID is generated
            driver.DriverId = Guid.NewGuid();
            
            _context.Drivers.Add(driver);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetDriver), new { id = driver.DriverId }, driver);
        }
        
        // PUT: api/Driver/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDriver(Guid id, Driver driver)
        {
            if (id != driver.DriverId)
            {
                return BadRequest("ID mismatch");
            }
            
            _context.Entry(driver).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DriverExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            
            return NoContent();
        }
        
        // DELETE: api/Driver/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDriver(Guid id)
        {
            var driver = await _context.Drivers.FindAsync(id);
            if (driver == null)
            {
                return NotFound();
            }
            
            _context.Drivers.Remove(driver);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
        
        private bool DriverExists(Guid id)
        {
            return _context.Drivers.Any(e => e.DriverId == id);
        }
    }
}
