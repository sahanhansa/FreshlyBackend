using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context; //Declares a variable to interact with the database.

        public UsersController(ApplicationDbContext context) //Injects the database context so we can access Users table.
        {
            _context = context;
        }

     
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Users>>> GetUsers() //Asynchronously returns a list of users.
        {
            return await _context.Users.ToListAsync(); //Fetches all users from the database.
        }

     
        [HttpGet("{id}")] //Defines an HTTP GET endpoint (api/users/{id}).
        public async Task<ActionResult<Users>> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id); //FindAsync(only for primary key)-Searches for a user with the given ID in the database.

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpGet("byname/{name}")]
        public async Task<ActionResult<Users>> GetUserByName(string name)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == name);//FirstOrDefaultAsync-Searches for the user by name

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        
        [HttpPost]
        public async Task<ActionResult<Users>> PostUser(Users user)
        {
            _context.Users.Add(user); //Add(user): Adds a new product to the database.
            await _context.SaveChangesAsync(); // SaveChangesAsync(): Saves data to MySQL.

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);//CreatedAtAction(): Returns the newly created user 
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, Users user)
        {
            if (id != user.Id) return BadRequest();
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsers(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    }
