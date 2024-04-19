using System.ComponentModel.DataAnnotations;
namespace dulceSabor_caja_ListadodeMesas.Models
{
    public class cuenta
    {
        [Key]

        [Display(Name = "Id de cuenta")]
        public int Id_cuenta { get; set; }

        [Display(Name = "Id de mesa")]
        public int Id_mesa { get; set; }

        [Display(Name = "Nombre del consumidor")]
        public string? Nombre { get; set; }

        [Display(Name = "Cantidad de Personas")]
        public int? Cantidad_Personas { get; set; }

        [Display(Name = "Estado de la cuenta")]
        public string? Estado_cuenta { get; set; }

        [Display(Name = "Fecha y Hora")]
        public DateTime Fecha_Hora { get; set; }

    }
}
