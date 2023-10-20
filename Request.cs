namespace ConverterRestApi
{
    /// <summary>
    /// Represents a data conversion request.
    /// </summary>
    public class Request
    {
        /// <summary>
        /// Gets or sets the number to convert.
        /// </summary>
        public string Num { get; set; }

        /// <summary>
        /// Gets or sets the unit to convert from.
        /// </summary>
        public string FromUnit { get; set; }

        /// <summary>
        /// Gets or sets the unit to convert to.
        /// </summary>
        public string ToUnit { get; set; }
    }
}
