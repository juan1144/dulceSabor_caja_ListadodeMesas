﻿using Microsoft.EntityFrameworkCore;

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
        public DbSet<detalle_fac> detalle_fac { get; set; }
        public DbSet<modeloFactura> modeloFactura { get; set; }
        public DbSet<items_combo> items_combo { get; set; }
        public DbSet<combos> combos { get; set; }
        public DbSet<items_promo> items_promo { get; set; }
        public DbSet<promociones> promociones { get; set; }
    }
}
