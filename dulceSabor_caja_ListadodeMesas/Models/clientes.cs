using System.ComponentModel.DataAnnotations;
namespace dulceSabor_caja_ListadodeMesas.Models
{
    public class clientes
    {
        [Key]

        [Display(Name = "Id de cliente")]
        public int id_cliente { get; set; }

        [Display(Name = "Nombre")]
        public string nombre { get; set; }

        [Display(Name = "Apellido")]
        public string apellido { get; set; }

        [Display(Name = "Correo")]
        public string correo { get; set; }

        [Display(Name = "dui")]
        public string dui { get; set; }
    }
}
