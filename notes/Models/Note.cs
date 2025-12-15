using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace notes.Models
{
    public class Note
    {
        [Key]
        public int NoteId { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        public required string Title { get; set; }

        public required string Article { get; set; }

        public int AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public Author? Author { get; set; }
    }
}