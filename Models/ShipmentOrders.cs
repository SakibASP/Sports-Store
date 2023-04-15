using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsStore.Models
{
    [Table("ShipmentOrders")]
    public class ShipmentOrders
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int? AUTO_ID { get; set; }

        public int? ShippingDetailsId { get; set; }
        public string? PRODUCT_NAME { get; set; }
        public int? PRODUCT_ID { get; set; }
        public int? CATEGORY_ID { get; set; }
        public int QUANTITY { get; set; }
        public double PRICE { get; set; }
        public string? CREATE_BY { get; set; }
        public DateTime? CREATE_DATE { get; set; }

        [ForeignKey("ShippingDetailsId")]
        public virtual ShippingDetails? _ShippingDetails  { get; set; }

    }
}
