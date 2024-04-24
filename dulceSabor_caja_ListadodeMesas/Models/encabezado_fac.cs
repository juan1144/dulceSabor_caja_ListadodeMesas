using System.ComponentModel.DataAnnotations;
namespace dulceSabor_caja_ListadodeMesas.Models
{
    public class encabezado_fac
    {
        [Key]

        [Display(Name = "Id de factura")]
        public int id_factura { get; set; }

        [Display(Name = "Id de Pedido")]
        public int? id_pedido { get; set; }

        [Display(Name = "Id de cliente")]
        public int? id_cliente { get; set; }


        [Display(Name = "Fecha de cobro")]
        public DateTime fecha_cobro { get; set; }

        [Display(Name = "Total cobrado")]
        public decimal? total_cobrado { get; set; }

        [Display(Name = "Metodo de pago")]
        public string? metodo_pago { get; set; }
    }
}
