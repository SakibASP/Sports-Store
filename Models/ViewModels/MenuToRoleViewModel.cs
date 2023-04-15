using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportsStore.Models.ViewModels
{
    public class MenuToRoleViewModel
    {
       // public List<int> MenuParentIds { set; get; }
        public List<MenuSelection> MenuSelections { set; get; }
        public string RoleId { set; get; }

        public List<int> MenuParentIds { get; internal set; }
        public MenuToRoleViewModel()
        {
            MenuSelections = new List<MenuSelection>();
        }
    }
}