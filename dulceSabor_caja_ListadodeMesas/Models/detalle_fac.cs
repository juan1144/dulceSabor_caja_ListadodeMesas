using System.ComponentModel.DataAnnotations;
namespace dulceSabor_caja_ListadodeMesas.Models
{
    public class detalle_fac
    {
        [Key]

        [Display(Name = "id del detalle de factura")]
        public int id_detallefac { get; set; }

        [Display(Name = "id del encabezado de factura")]
        public int? id_factura { get; set; }

        [Display(Name = "id del detalleDePedido")]
        public int? id_detallepedido { get; set; }

        [Display(Name = "Precio")]
        public decimal? precio { get; set; }

        [Display(Name = "Total")]
        public decimal? total_plato { get; set; }

        [Display(Name = "Cantidad")]
        public int? cantidad { get; set; }
    }
}
