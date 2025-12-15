using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace notes.Models
{
    public class Comments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_comment")]
        public int IdComment { get; set; }

        [Required]
        [Column("created")]
        public DateTime Created { get; set; } = DateTime.Now;

        [Required]
        [Column("author_id")]
        public int AuthorId { get; set; } // 0 = Гость

        [Required]
        [Column("comment")]
        public string Comment { get; set; }

        [Required]
        [Column("article_id")]
        public int NoteId { get; set; }

        [ForeignKey("NoteId")]
        public Note? Note { get; set; }
    }
}
