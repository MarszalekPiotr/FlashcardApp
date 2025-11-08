
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

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            assemblies.ToList().ForEach(assembly =>
            {
                modelBuilder.ApplyConfigurationsFromAssembly(assembly);
            });

          
            //modelBuilder.ApplyConfigurationsFromAssembly(typeof(Word).Assembly);  // z modułu Words
           
        }
    }
}
