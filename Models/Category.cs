using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsStore.Models
{
    [Table("Categories")]
    public class Category
    {
        [Key]
        public int AUTO_ID { get; set; }

        [DisplayName("Category")]
        [Required(ErrorMessage = "Please specify a category")]
        public string? CategoryName { get; set; }
        public string? CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        //public virtual Product? Products { get; set; }
    }
}
