using System.Security.Cryptography;

namespace ConverterRestApi.Helper
{
    public class EncryptCredentials
    {
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
