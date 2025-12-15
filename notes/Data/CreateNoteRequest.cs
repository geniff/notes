using System.ComponentModel.DataAnnotations;

namespace notes.Data
{
    // DTO для создания заметки
    public class CreateNoteRequest
    {
        [Required(ErrorMessage = "Поле Title обязательно.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Поле Article обязательно.")]
        public string Article { get; set; }

        [Required(ErrorMessage = "Поле AuthorId обязательно.")]
        [Range(1, int.MaxValue, ErrorMessage = "AuthorId должен быть больше 0.")]
        public int AuthorId { get; set; }
    }
}
