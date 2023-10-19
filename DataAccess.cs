using ConverterRestApi.Data;
using ConverterRestApi.Model;

namespace ConverterRestApi
{
    public class DataAccess
    {
        private readonly ConverterRestApiContext? _dbContext;

        public DataAccess(ConverterRestApiContext dbContext) 
        { 
            _dbContext = dbContext;
            DataUnits = _dbContext.DataUnits.ToArray();
            LengthUnits = _dbContext.LengthUnits.ToArray();
            TemperatureUnits = _dbContext.TemperatureUnits.ToArray();
        }


        public  IEnumerable<DataConverter>? DataUnits { get; }
        public IEnumerable<LengthConverter>? LengthUnits { get; }
        public IEnumerable<TemperatureConverter>? TemperatureUnits { get; }
    }
}
