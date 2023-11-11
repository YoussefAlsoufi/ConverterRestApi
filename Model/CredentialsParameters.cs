using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConverterRestApi.Model
{
    public class CredentialsParameters
    {
        /// <summary>
        /// UserName Property.
        /// </summary>
        [Key]
        [Required(ErrorMessage = "filling this field is mandatory!")]
        public string UserName { get; set; }
        /// <summary>
        /// Password Property.
        /// </summary>
        [Required(ErrorMessage = "filling this field is mandatory!")]
        public string Password { get; set; } = string.Empty;
        /// <summary>
        /// Email Property.
        /// </summary>
        [Required(ErrorMessage = "filling this field is mandatory!")]
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Phone Property.
        /// </summary>
        [Required(ErrorMessage = "filling this field is mandatory!")]
        public string Phone { get; set; } = string.Empty;

    }
}
