namespace ConverterRestApi
{
    public interface IRefreshToken
    {
        (string RefreshToken, DateTime ExpirationTime) GenerateToken (); 
    }
}
