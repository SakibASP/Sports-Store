using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsStore.Models
{
    [Table("ShippingDetails")]
    public class ShippingDetails
    {
        [Key]
        public int AUTO_ID { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Please enter a name")]
        public string? NAME { get; set; }

        [Display(Name = "Mobile")]
        [Required(ErrorMessage = "Please enter your phone number")]
        public string? MOBLIE_NUMBER { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Please enter your e-mail address")]
        public string? EMAIL { get; set; }

        [Display(Name = "AddressLine 1")]
        [Required(ErrorMessage = "Please enter the first address line")]
        public string? LINE_1 { get; set; }
        [Display(Name = "AddressLine 2")]
        public string? LINE_2 { get; set; }
        [Display(Name = "AddressLine 3")]
        public string? LINE_3 { get; set; }

        [Display(Name = "District")]
        [Required(ErrorMessage = "Please enter a city name")]
        public string? CITY { get; set; }

        [Display(Name = "Division")]
        [Required(ErrorMessage = "Please enter a state name")]
        public string? STATE { get; set; }

        [Display(Name = "Zip Code")]
        public string? ZIP { get; set; }

        [Display(Name = "Country")]
        [Required(ErrorMessage = "Please enter a country name")]
        public string? COUNTRY { get; set; }

        [Display(Name = "Gift Wrap")]
        public bool GIFTWRAP { get; set; }
        public bool IsConfirmed { get; set; }
        public string? CREATE_BY { get; set; }
        public DateTime? CREATE_DATE { get; set; }
        public DateTime? CONFIRM_DATE { get; set; }

        [Display(Name="Confirm By")]
        public string? CONFIRM_BY { get; set; }
    }
}
