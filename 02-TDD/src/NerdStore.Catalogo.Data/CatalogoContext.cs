using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NerdStore.Catalogo.Domain;
using NerdStore.Core.Data;
using NerdStore.Core.Messages;

namespace NerdStore.Catalogo.Data
{
    public class CatalogoContext : DbContext, IUnitOfWork
    {
        public CatalogoContext(DbContextOptions<CatalogoContext> options)
            : base(options) { }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var camposNVarchar = modelBuilder.Model.GetEntityTypes()
                .SelectMany(x => x.GetProperties())
                .Where(x => x.ClrType == typeof(string) && x.GetColumnType() == null);

            foreach (var property in camposNVarchar)
                property.SetIsUnicode(false);

            modelBuilder.Ignore<Event>();

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogoContext).Assembly);
        }

        public async Task<bool> Commit()
        {
            foreach (var entry in ChangeTracker.Entries().Where(entry => entry.Entity.GetType().GetProperty("DataCadastro") != null))
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("DataCadastro").CurrentValue = DateTime.Now;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Property("DataCadastro").IsModified = false;
                }
            }
            
            return await base.SaveChangesAsync() > 0;
        }
    }
    
    public class ApplicationContextFactory : IDesignTimeDbContextFactory<CatalogoContext>
    {
        public CatalogoContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<CatalogoContext>();
            builder
                .UseSqlServer("Server=localhost, 1433;Database=CursoTDD;User Id=sa;Password=!123Senha;Application Name=\"CursoTDD\";pooling=true;")
                .LogTo(Console.WriteLine)
                .EnableSensitiveDataLogging();

            return new CatalogoContext(builder.Options);
        }
    }
    
}