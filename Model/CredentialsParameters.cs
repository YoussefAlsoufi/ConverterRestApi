using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConverterRestApi.Model
{
    public class CredentialsParameters
    {
        [Key]
        public string UserName { get; set; }
        public string Password { get; set; }

    }
}
