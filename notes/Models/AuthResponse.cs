namespace notes.Models
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public Author Author { get; set; } = null!;
        public string Message { get; set; } = string.Empty;
    }
}
