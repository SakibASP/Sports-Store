using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsStore.Models
{
    [Table("Products")]
    public class Product
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Please enter a product name")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Please enter a description")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Required]
        //[DisplayName("Sale Price")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a positive price")]
        public double Price { get; set; }

        [Required]
        [DisplayName("Buy Price")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a positive price")]
        public double Buying_Price { get; set; }
        public string? CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }

        [DisplayName("Stock")]
        [Range(0,50, ErrorMessage = "Please enter between 1 to 50")]
        public int CURRENT_STOCK { get; set; }

        [DisplayName("Category")]
        [Required(ErrorMessage = "Please specify a category")]
        public int? Cat_Id { get; set; }
        public bool IsAvailabe { get; set; }

        [NotMapped]
        public string? ImageName { get; set; }  
        [NotMapped]
        public string? ImagePath { get; set; }

        [NotMapped]
        public double TOTAL_PRICE { get { return (CURRENT_STOCK == 0 ? 1 : CURRENT_STOCK) * Buying_Price; } }

        [ForeignKey("Cat_Id")]
        [DisplayName("Category")]
        public virtual Category? Category1 { get; set; }


    }
}
