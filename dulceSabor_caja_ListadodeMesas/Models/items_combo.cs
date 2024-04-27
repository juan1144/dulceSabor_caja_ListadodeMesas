using System.ComponentModel.DataAnnotations;
namespace dulceSabor_caja_ListadodeMesas.Models
{
    public class items_combo
    {
        [Key]
        [Display(Name = "Id combo")]
        public int? id_combo { get; set; }

        [Display(Name = "Id item menu")]
        public int? id_item_menu { get; set; }

    }
}
