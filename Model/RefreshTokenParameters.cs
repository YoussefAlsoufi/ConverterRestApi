using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConverterRestApi.Model
{
    public class RefreshTokenParameters
    {
        [Key]
        /// <summary>
        /// UserName Property.
        /// </summary>
        public string? UserId { get; set; }
        /// <summary>
        /// Password Property.
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Phone Property.
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// Email Property.
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// TokenId Property.
        /// </summary>
        public string TokenId { get; set; }
        /// <summary>
        /// RefreshToken Property.
        /// </summary>
        public string RefreshToken { get; set; }
        /// <summary>
        /// IsActive Property.
        /// </summary>
        public bool IsActive { get; set; } =true;
        /// <summary>
        /// ExpirationTime Property.
        /// </summary>
        public DateTime ExpirationTime { get; set; }
    }
}
