using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConverterRestApi.Model
{
    [Table("TempertureUnits")]
    public class TemperatureConverter
    {
        [Key]
        public string UnitName { get; set; }
        public string? Value { get; set; }
    }
}
