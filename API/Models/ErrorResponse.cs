namespace API.Models
{
    public class ErrorResponse
    {
        public bool Success { get; set; } = false;
        public string? ErrorMessage { get; set; }
    }
}
