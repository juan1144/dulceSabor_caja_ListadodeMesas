using Microsoft.EntityFrameworkCore;

namespace dulceSabor_caja_ListadodeMesas.Models
{
    public class dulceSaborDbContext : DbContext
    {
        public dulceSaborDbContext(DbContextOptions<dulceSaborDbContext> options) : base(options) { }

        public DbSet<mesas> mesas { get; set; }
        public DbSet<estados> estados { get; set; }
        public DbSet<cuenta> cuenta { get; set; }
        public DbSet<Detalle_Pedido> Detalle_Pedido { get; set; }
        public DbSet<items_menu> items_menu { get; set; }
        public DbSet<encabezado_fac> encabezado_fac { get; set; }
        public DbSet<clientes> clientes { get; set; }
    }
}
