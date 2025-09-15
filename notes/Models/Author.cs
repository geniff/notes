using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace notes.Models
{
    public class Author
    {
        public int Id { get; set; }
        [Required]
        public required string Login { get; set; }
        [Required]
        public required string PasswordHash { get; set; }
        public char Rights { get; set; } = 'u'; // u - user, a - admin
        public List <Note>? Notes { get; set; }
    }
}
