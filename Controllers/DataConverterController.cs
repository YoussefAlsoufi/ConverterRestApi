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
    public class DataConverterController : ControllerBase
    {
        private readonly ConverterRestApiContext _context;
        private readonly ConveterTools _converter;

        public DataConverterController(ConverterRestApiContext context, ConveterTools converter)
        {
            _context = context;
            _converter = converter;
        }

        // GET: api/DataConverter
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DataConverter>>> GetDataUnits()
        {
          if (_context.DataUnits == null)
          {
              return NotFound();
          }
            return await _context.DataUnits.ToListAsync();
        }

        // GET: api/DataConverter/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DataConverter>> GetDataConverter(string id)
        {
          if (_context.DataUnits == null)
          {
              return NotFound();
          }
            var dataConverter = await _context.DataUnits.FindAsync(id);

            if (dataConverter == null)
            {
                return NotFound();
            }

            return dataConverter;
        }

        // PUT: api/DataConverter/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDataConverter(string id, DataConverter dataConverter)
        {
            if (id != dataConverter.UnitName)
            {
                return BadRequest();
            }

            _context.Entry(dataConverter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DataConverterExists(id))
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

        // POST: api/DataConverter
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public IActionResult PostDataConverter(Request req)
        {
            Response response = _converter.ConvertData(req.Num, req.FromUnit, req.ToUnit);
            if (response.ResCode == 200)
            {
                return Ok(response.ResMsg);
            }
            else
                return BadRequest(response.ResMsg);
        }

        // DELETE: api/DataConverter/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDataConverter(string id)
        {
            if (_context.DataUnits == null)
            {
                return NotFound();
            }
            var dataConverter = await _context.DataUnits.FindAsync(id);
            if (dataConverter == null)
            {
                return NotFound();
            }

            _context.DataUnits.Remove(dataConverter);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DataConverterExists(string id)
        {
            return (_context.DataUnits?.Any(e => e.UnitName == id)).GetValueOrDefault();
        }
    }
}
