using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BulkyWebRazer_Temp.Model
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Category Name Is Required ")]
        [MaxLength(30)]
        [DisplayName("Category Name")]
        public string Name { get; set; }
        [DisplayName("Dispaly Order")]
        [Range(1, 100, ErrorMessage = "Dispaly Order Must Be Between 1-100")]
        public int DisplayOrder { get; set; }
    }
}
