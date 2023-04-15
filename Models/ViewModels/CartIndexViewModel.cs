using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportsStore.Models.ViewModels
{
    public class CartIndexViewModel
    {
        public Cart Cart { get; set; }
        public string ReturnUrl { get; set; }
        public string? Nonce { get; set; }
    }
}