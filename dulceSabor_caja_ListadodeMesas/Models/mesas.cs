using System.ComponentModel.DataAnnotations;
namespace dulceSabor_caja_ListadodeMesas.Models
{
    public class mesas
    {
        [Key]
        [Display(Name = "Id mesa")]
        public int id_mesa { get; set; }

        [Display(Name = "Cantidad personas")]
        public int? cantidad_personas { get; set; }

        [Display(Name = "Estado")]
        public int? id_estado { get; set; }
        [Display(Name = "Nombre mesa")]
        public string nombre_mesa { get; set; }

    }
}
