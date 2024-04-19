using System.ComponentModel.DataAnnotations;
namespace dulceSabor_caja_ListadodeMesas.Models
{
    public class estados
    {
        [Key]
        [Display(Name = "Id de estados")]
        public int id_estado { get; set; }
        [Display(Name = "Nombre del estado")]
        public string nombre { get; set; }

        [Display(Name = "Tipo de estado")]
        public string tipo_estado { get; set; }

    }
}
