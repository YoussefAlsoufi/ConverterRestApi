﻿using System;
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
    public class LengthConvertersController : ControllerBase
    {
        private readonly ConverterRestApiContext _context;
        private readonly ConveterTools _converter;

        public LengthConvertersController(ConverterRestApiContext context, ConveterTools converter)
        {
            _context = context;
            _converter = converter;
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
        [Authorize(Roles = "admin")]
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
        [HttpPost]
        [Authorize]
        public IActionResult PostLengthConverter(Request req)
        {
            Response response = _converter.ConvertLength(req.Num, req.FromUnit, req.ToUnit);
            if (response.ResCode == 200)
            {
                return Ok(response.ResMsg);
            }
            else
                return BadRequest(response.ResMsg);
        }

        // DELETE: api/LengthConverters/5
        [Authorize(Roles = "admin")]
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
