using ANNIE_SHOP.Models;
using Microsoft.EntityFrameworkCore;

namespace ANNIE_SHOP.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        :base(options){}

        //SE MAPEAN LAS ENTIDADES QUE SE CONVERTIRAN EN TABLAS
        public DbSet<Usuario>Usuarios{get; set;} = null!;
        public DbSet<Rol>Roles{get; set;} = null!;
        public DbSet<Producto>Productos{get; set;} = null!;
        public DbSet<Pedido>Pedidos{get; set;} = null!;
        public DbSet<Direccion>Direccion{get; set;} = null!;
        public DbSet<Detalle_Pedido>DetallesPedidos{get; set;} = null!;
        public DbSet<Categoria>Categorias{get; set;} = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //SE CONFIGURAN LAS RELACIONES ENTRE LAS ENTIDADES.
            modelBuilder.Entity<Usuario>()
            .HasMany(p=>p.Pedidos)
            .WithOne(u=>u.Usuario)
            .HasForeignKey(p=>p.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Producto>()
            .HasMany(p=>p.DetallePedido)
            .WithOne(pd=>pd.Producto)
            .HasForeignKey(p=>p.ProductoId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Pedido>()
            .HasMany(p=>p.DetallesPedidos)
            .WithOne(pd=>pd.Pedido)
            .HasForeignKey(p=>p.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Pedido>()
                .Ignore(p=>p.Direccion);

            modelBuilder.Entity<Categoria>()
            .HasMany(p=>p.Productos)
            .WithOne(pd=>pd.Categoria)
            .HasForeignKey(p=>p.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}