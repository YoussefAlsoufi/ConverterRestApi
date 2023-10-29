using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ConverterRestApi.Model;

namespace ConverterRestApi.Data
{
    public class ConverterRestApiContext : DbContext
    {
        public ConverterRestApiContext (DbContextOptions<ConverterRestApiContext> options)
            : base(options)
        {
        }

        public DbSet<DataConverter> DataUnits { get; set; } = default!;
        public DbSet<LengthConverter> LengthUnits { get; set; } = default!;
        public DbSet<TemperatureConverter> TemperatureUnits { get; set; } = default!;
        public DbSet<CredentialsParameters> Credentials { get; set; } = default!;
    }
}
