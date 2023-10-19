using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConverterRestApi.Model
{
    [Table("LengthUnits")]
    public class LengthConverter
    {
        [Key]
        public string UnitName { get; set; }
        public string? Value { get; set; }
    }
}
