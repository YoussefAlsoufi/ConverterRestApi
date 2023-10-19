﻿using System;
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
    public class LengthConvertersController : ControllerBase
    {
        private readonly ConverterRestApiContext _context;

        public LengthConvertersController(ConverterRestApiContext context)
        {
            _context = context;
        }

        // GET: api/LengthConverters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LengthConverter>>> GetLengthUnits()
        {
          if (_context.LengthUnits == null)
          {
              return NotFound();
          }
            return await _context.LengthUnits.ToListAsync();
        }

        // GET: api/LengthConverters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LengthConverter>> GetLengthConverter(string id)
        {
          if (_context.LengthUnits == null)
          {
              return NotFound();
          }
            var lengthConverter = await _context.LengthUnits.FindAsync(id);

            if (lengthConverter == null)
            {
                return NotFound();
            }

            return lengthConverter;
        }

        // PUT: api/LengthConverters/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLengthConverter(string id, LengthConverter lengthConverter)
        {
            if (id != lengthConverter.UnitName)
            {
                return BadRequest();
            }

            _context.Entry(lengthConverter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LengthConverterExists(id))
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

        // POST: api/LengthConverters
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LengthConverter>> PostLengthConverter(LengthConverter lengthConverter)
        {
          if (_context.LengthUnits == null)
          {
              return Problem("Entity set 'ConverterRestApiContext.LengthUnits'  is null.");
          }
            _context.LengthUnits.Add(lengthConverter);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LengthConverterExists(lengthConverter.UnitName))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLengthConverter", new { id = lengthConverter.UnitName }, lengthConverter);
        }

        // DELETE: api/LengthConverters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLengthConverter(string id)
        {
            if (_context.LengthUnits == null)
            {
                return NotFound();
            }
            var lengthConverter = await _context.LengthUnits.FindAsync(id);
            if (lengthConverter == null)
            {
                return NotFound();
            }

            _context.LengthUnits.Remove(lengthConverter);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LengthConverterExists(string id)
        {
            return (_context.LengthUnits?.Any(e => e.UnitName == id)).GetValueOrDefault();
        }
    }
}