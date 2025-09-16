using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using notes.Models;

namespace notes.Data
{
    public class notesContext : DbContext
    {
        public notesContext (DbContextOptions<notesContext> options)
            : base(options)
        {
        }

        public DbSet<notes.Models.Author> Author { get; set; } = default!;
    }
}
