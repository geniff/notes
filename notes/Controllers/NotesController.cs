using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using notes.Data;
using notes.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace notes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NotesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Notes
        // Получение комментариев для заметки
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetNotes()
        {
            var notes = await _context.Notes
                .Include(n => n.Author)
                .Select(n => new
                {
                    n.NoteId,
                    n.Created,
                    n.Title,
                    n.Article,
                    n.AuthorId,
                    Author = n.Author != null
                        ? new { n.Author.AuthorId, n.Author.Login }
                        : null
                })
                .ToListAsync();

            return Ok(notes);
        }

        // GET: api/Notes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetNote(int id)
        {
            var note = await _context.Notes
                .Include(n => n.Author)
                .Where(n => n.NoteId == id)
                .Select(n => new
                {
                    n.NoteId,
                    n.Created,
                    n.Title,
                    n.Article,
                    n.AuthorId,
                    Author = n.Author != null
                        ? new { n.Author.AuthorId, n.Author.Login }
                        : null
                })
                .FirstOrDefaultAsync();

            if (note == null)
                return NotFound();

            return Ok(note);
        }

        // POST: api/Notes
        // Создание комментария
        [HttpPost]
        public async Task<ActionResult<object>> PostNote([FromBody] CreateNoteRequest request)
        {
            // 1. Проверка, что запрос вообще пришел
            if (request == null)
            {
                return BadRequest(new
                {
                    Message = "Request body is null or invalid",
                    ExpectedFormat = new
                    {
                        Title = "string",
                        Article = "string",
                        AuthorId = "number"
                    }
                });
            }

            // 2. Проверка ModelState (автоматическая валидация)
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Any())
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(new
                {
                    Message = "Validation errors",
                    Errors = errors,
                    YourRequest = request // Для отладки - что пришло на сервер
                });
            }

            // 3. Проверка автора
            var author = await _context.Authors.FindAsync(request.AuthorId);
            if (author == null)
                return BadRequest(new { Message = $"Автор с ID {request.AuthorId} не найден." });

            // 4. Создание заметки
            var note = new Note
            {
                Title = request.Title,
                Article = request.Article,
                AuthorId = author.AuthorId,
                Created = DateTime.UtcNow
            };

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            // 5. Возврат результата
            return CreatedAtAction(nameof(GetNote), new { id = note.NoteId }, new
            {
                note.NoteId,
                note.Title,
                note.Article,
                note.Created,
                note.AuthorId,
                Author = new { author.AuthorId, author.Login }
            });
        }

        // PUT: api/Notes/5
        // Обновление комментария
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNote(int id, Note note)
        {
            if (id != note.NoteId)
                return BadRequest();

            _context.Entry(note).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NoteExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Notes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null)
                return NotFound();

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NoteExists(int id)
        {
            return _context.Notes.Any(e => e.NoteId == id);
        }
    }
}
