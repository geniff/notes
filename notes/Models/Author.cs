using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;

namespace notes.Models
{
    public class Author
    {
        [Key]
        [Column ("id_author")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // AI
        public int Id { get; set; }
        [Required]
        [Column("login")]
        [MaxLength(45)]
        public required string Login { get; set; }
        [Required]
        [Column("password")]
        [MaxLength(255)]
        public required string PasswordHash { get; set; }
        [Required]
        [Column("rights", TypeName = "ENUM('a','u')")]
        public char Rights { get; set; } = 'u'; // u - user, a - admin
        [EmailAddress]
        public String? email { get; set; }
        public List <Note>? Notes { get; set; }
    }
}
