using System.ComponentModel.DataAnnotations;
namespace dulceSabor_caja_ListadodeMesas.Models
{
    public class promociones
    {
        [Key]
        [Display(Name = "Id promoción")]
        public int id_promo { get; set; }

        [Display(Name = "Descripcion")]
        public string? descripcion { get; set; }

        [Display(Name = "Precio")]
        public decimal? precio { get; set; }

        [Display(Name = "Fecha inicio")]
        public DateTime? fecha_inicio { get; set; }

        [Display(Name = "Fecha final")]
        public DateTime? fecha_final { get; set; }

        [Display(Name = "Imagen")]
        public string? imagen { get; set; }

        [Display(Name = "Id estado")]
        public int? id_estado { get; set; }

        [Display(Name = "Nombre")]
        public string? nombre { get; set; }
    }
}
