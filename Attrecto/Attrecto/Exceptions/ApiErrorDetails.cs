namespace Attrecto.Exceptions
{
    internal class ApiErrorDetails
    {
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDetail { get; set; }
    }
}