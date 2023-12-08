using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SportsStore.Models.ViewModels
{
    public class ProductViewModel
    {
        public int? ProductID { get; set; }
        public int? ProductImageID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a positive price")]
        public double Price { get; set; }

        [DisplayName("Buy Price")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a positive price")]
        public double Buying_Price { get; set; }
        public string? ImageName { get; set; }
        public string? ImagePath { get; set; }
        public string? CREATED_BY { get; set; }
        public string? ShortDesc { get; set; }
        public DateTime? CREATED_DATE { get; set; }

        public int? CURRENT_STOCK { get; set; }

        [DisplayName("Category")]
        public int? Cat_Id { get; set; }
        public string? Category { get; set; }
        public int? IsCover { get; set; }
        public bool IsAvailabe { get; set; }
    }
}
