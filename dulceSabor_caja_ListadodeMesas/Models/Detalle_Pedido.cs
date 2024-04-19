using System.ComponentModel.DataAnnotations;
namespace dulceSabor_caja_ListadodeMesas.Models
{
    public class Detalle_Pedido
    {
        [Key]

        [Display(Name = "Id de Detalle de Cuenta")]
        public int Id_DetalleCuenta { get; set; }

        [Display(Name = "Id de la cuenta")]
        public int Id_cuenta { get; set; }

        [Display(Name = "Id del plato")]
        public int Id_plato { get; set; }

        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        [Display(Name = "Estado del plato")]
        public string Estado { get; set; }

        [Display(Name = "Tipo de plato")]
        public char Tipo_Plato { get; set; }

        [Display(Name = "Precio del plato")]
        public decimal Precio { get; set; }
    }
}
