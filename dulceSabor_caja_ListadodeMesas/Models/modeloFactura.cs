using System.ComponentModel.DataAnnotations;
namespace dulceSabor_caja_ListadodeMesas.Models
{
    public class modeloFactura
    {
        [Key]
        [Display(Name = "Id cuenta")]
        public int id_cuenta { get; set; }

        [Display(Name = "Id encabezado factura")]
        public int id_encFactura { get; set; }

        [Display(Name = "Ids detalles pedidos")]
        public string ids_detFactura { get; set; }
    }
}
