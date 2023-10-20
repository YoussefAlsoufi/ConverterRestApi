namespace ConverterRestApi
{
    /// <summary>
    /// Represents the response object returned by the API.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Gets or sets the message associated with the response.
        /// </summary>
        public string ResMsg { get; set; }

        /// <summary>
        /// Gets or sets the code associated with the response.
        /// </summary>
        public int ResCode { get; set; }
    }
}