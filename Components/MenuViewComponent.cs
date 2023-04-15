using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SportsStore.Common;
using SportsStore.Data;
using SportsStore.Helper;
using SportsStore.Models;

namespace SportsStore.Components
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public MenuViewComponent(ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            HttpContext.Session.Remove(Constant.Menu);
            var UserId = _userManager.GetUserId(HttpContext.User);
            var MenuList = _context.DynamicMenuItem.FromSqlRaw(
                                "exec usp_GetMenuData @UserId",
                                new SqlParameter("UserId", UserId)).ToList();

            List<DynamicMenuItem>? Menu = null;
            var SessionMenu = HttpContext.Session.GetObjectFromJsonList<DynamicMenuItem>(Constant.Menu);
            if (SessionMenu != null)
            {
                Menu = (List<DynamicMenuItem>?)SessionMenu;
            }
            else
            {
                Menu = MenuList;
                HttpContext.Session.SetObjectAsJson<DynamicMenuItem>(Constant.Menu, MenuList);
            }
            return await Task.Run(() => View("_Menu", Menu));
            //return View("_Menu", Menu);
        }
    }
}
