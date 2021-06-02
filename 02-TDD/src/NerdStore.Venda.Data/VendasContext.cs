using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NerdStore.Core.Data;
using NerdStore.Core.DomainObjects;
using NerdStore.Core.Messages;
using NerdStore.Venda.Domain;

namespace NerdStore.Venda.Data
{
    public class VendasContext : DbContext, IUnitOfWork
    {
        private readonly IMediator _mediator;
        
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoItem> PedidoItems { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }

        public VendasContext(DbContextOptions<VendasContext> options) : base(options)
        {
            
        }

        protected VendasContext(DbContextOptions<VendasContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var camposNVarchar = modelBuilder.Model.GetEntityTypes()
                .SelectMany(x => x.GetProperties())
                .Where(x => x.ClrType == typeof(string) && x.GetColumnType() == null);

            foreach (var property in camposNVarchar)
                property.SetIsUnicode(false);

            modelBuilder.Ignore<Event>();

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(VendasContext).Assembly);

            var foreignKeys = modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys());
            
            foreach (var relationship in foreignKeys) 
                relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;

            modelBuilder.HasSequence<int>("MinhaSequencia").StartsAt(1000).IncrementsBy(1);
            base.OnModelCreating(modelBuilder);
        }

        public async Task<bool> Commit()
        {
            var gravadoComSucesso = await base.SaveChangesAsync() > 0;
            if (gravadoComSucesso) await _mediator.PublicarEventos(this);
            
            return gravadoComSucesso;
        }


        
    }
    
    public static class MediatorExtension
    {
        public static async Task PublicarEventos(this IMediator mediator, VendasContext context)
        {
            var domainEntities = context.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.Notificacoes != null && x.Entity.Notificacoes.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.Notificacoes)
                .ToList();

            domainEntities.ToList()
                .ForEach(x => x.Entity.LimparEventos());

            var tasks = domainEvents
                .Select(async (domainEvent) =>
                {
                    await mediator.Publish(domainEvent);
                });

            await Task.WhenAll(tasks);
        }
    }
    
    public class ApplicationContextFactory : IDesignTimeDbContextFactory<VendasContext>
    {
        public VendasContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<VendasContext>();
            builder
                .UseSqlServer("Server=localhost, 1433;Database=CursoTDD;User Id=sa;Password=!123Senha;Application Name=\"CursoTDD\";pooling=true;")
                .LogTo(Console.WriteLine)
                .EnableSensitiveDataLogging();

            return new VendasContext(builder.Options);
        }
    }
}