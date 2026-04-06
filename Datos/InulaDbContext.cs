using BackendLimpio.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendLimpio.Datos
{
    public class InulaDbContext : DbContext
    {
        public InulaDbContext(DbContextOptions<InulaDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<ExamenPrecio> ExamenesPrecio { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Order → Usuario (cliente) - especificando ambos lados
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Order → Motorizado
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Motorizado)
                .WithMany()
                .HasForeignKey(o => o.MotorizadoId)
                .OnDelete(DeleteBehavior.SetNull);

            // Pet → UserId
            modelBuilder.Entity<Pet>()
                .Property(p => p.UserId)
                .HasColumnName("UserId");

            // Factura → Order
            modelBuilder.Entity<Factura>()
                .HasOne(f => f.Order)
                .WithMany()
                .HasForeignKey(f => f.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}