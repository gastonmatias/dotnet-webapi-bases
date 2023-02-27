using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using webAPIAuthors.Entidades;

namespace webAPIAuthors
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { 

        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder){
            base.OnModelCreating(modelBuilder);

            // creacion PK compuesta de AutorLibro
            modelBuilder.Entity<AutorLibro>()
                .HasKey(autlib => new {autlib.AutorId, autlib.LibroId});
        }
        
        public DbSet<Autor> Autores { get; set; }
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }

        public DbSet<AutorLibro> AutoresLibros { get; set; }
    }
}