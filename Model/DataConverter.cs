using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConverterRestApi.Model
{
    [Table("DataUnits")]
    public class DataConverter
    {
        [Key]
        public string UnitName { get; set; }
        public string? Value { get; set; }
    }
}
