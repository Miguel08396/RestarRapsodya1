using Microsoft.EntityFrameworkCore;
using RestarRapsodya.Models;

namespace RestarRapsodya.Data
{
    public class RestarRapsodyaContext:DbContext
    {
        public RestarRapsodyaContext(DbContextOptions<RestarRapsodyaContext> options) : base(options) { }
        public DbSet<Estado> Estado { get; set; }
        public DbSet<Mesa> Mesa { get; set; }
        public DbSet<Pedido> Pedido { get; set; }
        public DbSet<Rol> Rol { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
    }
}
