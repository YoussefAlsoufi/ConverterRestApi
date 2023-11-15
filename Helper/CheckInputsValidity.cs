using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace ConverterRestApi.Helper
{
    public class CheckInputsValidity
    {
        public static bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._-]+@gmail.com";
            Regex regex = new(pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(email);
        }
        public static bool IsValidUserName(string userName) 
        {
            string pattern = @"^[0-9]+&";
            Regex regex = new(pattern);
            return regex.IsMatch(userName);
        }

        public static string EncryptPassword(string password) 
        {
            var salt = GenerateSalt();
            return HashPasssword(password, salt);
        }

        static string HashPasssword(string password, byte[] salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);
            return Convert.ToBase64String(hash);
        }

        static byte[] GenerateSalt()
        {
            byte[] salt = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
    }
}
