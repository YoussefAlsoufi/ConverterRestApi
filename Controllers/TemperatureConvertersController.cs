using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConverterRestApi.Data;
using ConverterRestApi.Model;

namespace ConverterRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemperatureConvertersController : ControllerBase
    {
        private readonly ConverterRestApiContext _context;

        public TemperatureConvertersController(ConverterRestApiContext context)
        {
            _context = context;
        }

        // GET: api/TemperatureConverters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TemperatureConverter>>> GetTemperatureUnits()
        {
          if (_context.TemperatureUnits == null)
          {
              return NotFound();
          }
            return await _context.TemperatureUnits.ToListAsync();
        }

        // GET: api/TemperatureConverters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TemperatureConverter>> GetTemperatureConverter(string id)
        {
          if (_context.TemperatureUnits == null)
          {
              return NotFound();
          }
            var temperatureConverter = await _context.TemperatureUnits.FindAsync(id);

            if (temperatureConverter == null)
            {
                return NotFound();
            }

            return temperatureConverter;
        }

        // PUT: api/TemperatureConverters/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTemperatureConverter(string id, TemperatureConverter temperatureConverter)
        {
            if (id != temperatureConverter.UnitName)
            {
                return BadRequest();
            }

            _context.Entry(temperatureConverter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TemperatureConverterExists(id))
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

        // POST: api/TemperatureConverters
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TemperatureConverter>> PostTemperatureConverter(TemperatureConverter temperatureConverter)
        {
          if (_context.TemperatureUnits == null)
          {
              return Problem("Entity set 'ConverterRestApiContext.TemperatureUnits'  is null.");
          }
            _context.TemperatureUnits.Add(temperatureConverter);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TemperatureConverterExists(temperatureConverter.UnitName))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTemperatureConverter", new { id = temperatureConverter.UnitName }, temperatureConverter);
        }

        // DELETE: api/TemperatureConverters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemperatureConverter(string id)
        {
            if (_context.TemperatureUnits == null)
            {
                return NotFound();
            }
            var temperatureConverter = await _context.TemperatureUnits.FindAsync(id);
            if (temperatureConverter == null)
            {
                return NotFound();
            }

            _context.TemperatureUnits.Remove(temperatureConverter);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TemperatureConverterExists(string id)
        {
            return (_context.TemperatureUnits?.Any(e => e.UnitName == id)).GetValueOrDefault();
        }
    }
}
