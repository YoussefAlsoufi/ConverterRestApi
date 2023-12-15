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
            string pattern = @"^[a-zA-Z0-9_]+$";
            Regex regex = new(pattern);
            return regex.IsMatch(userName);
        }
        public static bool IsValidPhone(string phoneNumber)
        {
            string pattern = @"^\d{3}\d{3}\d{4}$";
            return Regex.IsMatch(phoneNumber,pattern);
        }
        public static bool IsPasswordValid(string password)
        {
            // Check if the password is null or empty
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }

            // Check for at least one uppercase letter
            if (!password.Any(char.IsUpper))
            {
                return false;
            }

            // Check for at least one lowercase letter
            if (!password.Any(char.IsLower))
            {
                return false;
            }

            // Check for at least one digit
            if (!password.Any(char.IsDigit))
            {
                return false;
            }

            // Check for at least one special character (non-alphanumeric)
            if (password.All(char.IsLetterOrDigit))
            {
                return false;
            }

            // If all criteria are met, the password is valid
            return true;
        }

    } 
}
