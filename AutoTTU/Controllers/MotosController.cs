using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoTTU.Connection;
using AutoTTU.Models;

namespace AutoTTU.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MotosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MotosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Motos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Motos>>> GetMotos()
        {
            return await _context.Motos.ToListAsync();
        }

        // GET: api/Motos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Motos>> GetMotos(int id)
        {
            var motos = await _context.Motos.FindAsync(id);

            if (motos == null)
            {
                return NotFound();
            }

            return motos;
        }

        // PUT: api/Motos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMotos(int id, Motos motos)
        {
            if (id != motos.IdMoto)
            {
                return BadRequest();
            }

            _context.Entry(motos).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MotosExists(id))
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

        // POST: api/Motos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Motos>> PostMotos(Motos motos)
        {
            _context.Motos.Add(motos);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMotos", new { id = motos.IdMoto }, motos);
        }

        // DELETE: api/Motos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMotos(int id)
        {
            var motos = await _context.Motos.FindAsync(id);
            if (motos == null)
            {
                return NotFound();
            }

            _context.Motos.Remove(motos);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MotosExists(int id)
        {
            return _context.Motos.Any(e => e.IdMoto == id);
        }

        
    }
}
