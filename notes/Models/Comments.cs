using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace notes.Models
{
    public class Comments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // AI
        [Required]
        [Column("id_comment")]
        public int IdComment { get; set; }

        [Required]
        [Column("created")]
        public DateTime Created { get; set; } = DateTime.Now;

        [Required]
        [Column("author_id")]
        public int AuthorId { get; set; }

        [Required]
        [Column("comment")]
        public required string Comment { get; set; }

        [Required]
        [Column("article_id")]
        public int NoteId { get; set; }

        [ForeignKey("NoteId")]
        public required Note Note { get; set; }

    }
}
