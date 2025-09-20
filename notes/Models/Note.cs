using System.ComponentModel.DataAnnotations;

namespace notes.Models
{
    // Класс записок
    public class Note 
    {
        [Key]
        public int NoteId { get; set; }
        public DateTime Created {get; set;} = DateTime.Now;
        [Required]
        public required String Title { get; set; }
        [Required]
        public required String Article { get; set; }
        public int AuthorId { get; set; }
    }
}
