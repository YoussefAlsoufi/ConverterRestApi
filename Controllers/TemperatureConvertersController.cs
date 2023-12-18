using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConverterRestApi.Data;
using ConverterRestApi.Model;
using Microsoft.CodeAnalysis.RulesetToEditorconfig;
using Microsoft.AspNetCore.Authorization;

namespace ConverterRestApi.Controllers
{
    [Route("api/[controller]")]
    public class TemperatureConvertersController : ControllerBase
    {
        private readonly ConverterRestApiContext _context;
        private readonly ConveterTools _converter;

        public TemperatureConvertersController(ConverterRestApiContext context, ConveterTools converter)
        {
            _context = context;
            _converter = converter;
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
        [Authorize(Roles = "admin")]
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
        [HttpPost]
        [Authorize]
        public IActionResult PostTemperatureConverter(Request req)
        {
            Response response = _converter.ConvertTemperature(req.Num, req.FromUnit, req.ToUnit);
            if (response.ResCode == 200)
            {
                return Ok(response.ResMsg);
            }
            else
                return BadRequest(response.ResMsg);
        }

        // DELETE: api/TemperatureConverters/5
        [Authorize(Roles = "admin")]
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
