using System.ComponentModel.DataAnnotations;
namespace dulceSabor_caja_ListadodeMesas.Models
{
    public class items_promo
    {
        [Key]

        [Display(Name = "Id promoción")]
        public int? id_promo { get; set; }

        [Display(Name = "Id item menu")]
        public int? id_item_menu { get; set; }
    }
}
