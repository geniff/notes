// Models/Author.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace notes.Models
{
    public class Author
    {
        [Key]
        [Column("id_author")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AuthorId { get; set; }

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
        public char Rights { get; set; } = 'u';

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<Note>? Notes { get; set; }
    }
}