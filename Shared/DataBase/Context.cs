using Flashcard.Modules.Users.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace DataBase
{
    public class Context : DbContext 
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Automatycznie wczytaj wszystkie konfiguracje z modułów
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Context).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(User).Assembly);  // z modułu Users
            //modelBuilder.ApplyConfigurationsFromAssembly(typeof(Word).Assembly);  // z modułu Words
           
        }
    }
}
