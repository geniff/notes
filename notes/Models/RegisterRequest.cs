namespace notes.Models
{
    public class RegisterRequest
    {
        public required string Login { get; set; }
        public required string Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}
