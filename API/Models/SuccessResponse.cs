namespace API.Models
{
    public class SuccessResponse
    {
        public bool Success { get; set; } = true;
        public string? Message { get; set; }
        public object? Data { get; set; }
    }
}
