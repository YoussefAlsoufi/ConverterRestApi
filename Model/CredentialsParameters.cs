using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConverterRestApi.Model
{
    public class CredentialsParameters
    {
        [Key]
        public string UserName { get; set; } 
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty; 
        public string Phone { get; set; } = string.Empty;

    }
}
