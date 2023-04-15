using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsStore.Models
{
    [Table("ProductImages")]
    public class ProductImages
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int AUTO_ID { get; set; }
        public int? ProductID { get; set; }
        public int? IsCover { get; set; }
        [DisplayName("Image")]
        public byte[]? ImageData { get; set; }
        public string? ImageName { get; set; }
        public string? CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set;}
        [ForeignKey("ProductID")]
        public virtual Product? Product_ { get; set; }
    }
}
