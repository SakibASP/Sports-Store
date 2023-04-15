using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportsStore.Models.ViewModels
{
    public class MenuSelection
    {
       
        public int? MenuId { get; set; }
        public string MenuName { get; set; }
        public bool IsSelected { get; set; }
        public int? MenuParentId { get; set; }
        public int Count { get; set; }
    }
}