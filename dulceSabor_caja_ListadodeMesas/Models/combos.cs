using System.ComponentModel.DataAnnotations;
namespace dulceSabor_caja_ListadodeMesas.Models
{
    public class combos
    {
        [Key]
        [Display(Name = "Id combo")]
        public int id_combo { get; set; }

        [Display(Name = "Descripción")]
        public string? descripcion { get; set; }

        [Display(Name = "Precio")]
        public decimal? precio { get; set; }

        [Display(Name = "Imagen")]
        public string? imagen { get; set; }

        [Display(Name = "Id estado")]
        public int? id_estado { get; set; }
    }
}
