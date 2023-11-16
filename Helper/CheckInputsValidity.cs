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

    }
}
