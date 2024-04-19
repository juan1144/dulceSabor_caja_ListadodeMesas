using System.ComponentModel.DataAnnotations;
namespace dulceSabor_caja_ListadodeMesas.Models
{
    public class items_menu
    {
        [Key]

        [Display(Name = "id del item del menu")]
        public int id_item_menu { get; set; }

        [Display(Name = "Nombre del item")]
        public string? nombre { get; set; }

        [Display(Name = "Descripcion del item")]
        public string? descripcion { get; set; }

        [Display(Name = "Precio del item")]
        public decimal? precio { get; set; }

        [Display(Name = "Imagen del item")]
        public string? imagen { get; set; }

        [Display(Name = "ID del estado del item")]
        public int? id_estado { get; set; }

        [Display(Name = "ID de la categoria del item")]
        public int? id_categoria { get; set; }
    }
}
