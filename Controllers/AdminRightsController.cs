using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SportsStore.Common;
using SportsStore.Data;
using SportsStore.Models;
using SportsStore.Models.ViewModels;

namespace SportsStore.Controllers
{
    public class AdminRightsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminRightsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var rolelist = _context.Roles.OrderBy(r => r.Name).ToList().Select(rr =>
            new SelectListItem { Value = rr.Id.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = rolelist;

            //List<DynamicMenuItem> MenuList = context.Database.SqlQuery<DynamicMenuItem>("exec usp_GetAllMenuData").ToList();

            var menuMaster = _context.MenuItem.ToList();

            List<MenuItem> mi = menuMaster
            .Where(e => e.MenuParentId == null) /* grab only the root parent nodes */
            .Select(e => new MenuItem
            {
                MenuId = e.MenuId,
                MenuName = e.MenuName,
                MenuParentId = e.MenuParentId,
                Children = GetChildren(menuMaster, e.MenuId) /* Recursively grab the children */
            }).ToList();

            //ViewBag.Json = JsonConvert.SerializeObject(mi);
            ViewBag.menusList = mi;
            return View();
        }
        private static List<MenuItem> GetChildren(List<MenuItem> items, int parentId)
        {
            return items
                .Where(x => x.MenuParentId == parentId)
                .Select(e => new MenuItem
                {
                    MenuId = e.MenuId,
                    MenuName = e.MenuName,
                    MenuParentId = e.MenuParentId,
                    Children = GetChildren(items, e.MenuId)
                }).ToList();
        }
        [HttpPost]
        public JsonResult GetRoleWiseSelectedPages(string roleId)
        {
            var menuMaster = _context.MenuItem.ToList();
            var MenuToRoleViewModel = new MenuToRoleViewModel();
            List<MenuSelection> menuSelections = new List<MenuSelection>();

            var MenuToRole = _context.MenuToRole.ToList();
            var RoleMapppingResults = MenuToRole.Join(menuMaster,     /// Inner Collection  
                                          r => r.MenuId,   /// PK  
                                          m => m.MenuId,   /// FK  
                                          (r, m) =>           /// Result Collection  
                                          new MenuItemViewModel
                                          {
                                              MenuSelections = new List<MenuSelection>() {
                                                  new MenuSelection() {

                                                      MenuId =r.MenuId,
                                                      MenuName =m.MenuName,
                                                      IsSelected =r.IsSelected ?? false,
                                                      MenuParentId=m.MenuParentId,
                                                  }
                                              },
                                              RoleId = r.RoleId
                                          }).ToList();


            var resultMenuToRole = RoleMapppingResults.Where(w => w.RoleId == roleId).ToList();

            int count = 0;
            foreach (var item in resultMenuToRole)
            {

                foreach (var i in item.MenuSelections)
                {
                    i.Count = count;
                }
                count++;
            }

            return Json(resultMenuToRole, new JsonSerializerSettings());
        }

        public ActionResult Create()
        {
            var rolelist = _context.Roles.OrderBy(r => r.Name).ToList().Select(rr =>
            new SelectListItem { Value = rr.Id.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = rolelist;

            return View();
        }

        [HttpPost]
        public ActionResult Create(MenuItemViewModel viewMenuItemObj)
        {
            //MenuToRoleViewModel roleMenuMappingViewModel = new MenuToRoleViewModel();
            try
            {

                if (viewMenuItemObj.RoleId != null)
                {
                    var roleMenuMappingtblList = _context.MenuToRole
                                                 .Where(w => w.RoleId == viewMenuItemObj.RoleId)
                                                 .ToList();

                    MenuToRole MenuToRoleObj = new MenuToRole();

                    foreach (var roleNMenu in viewMenuItemObj.MenuSelections)
                    {
                        if (roleMenuMappingtblList.Count > 0)
                        {
                            var Id = roleMenuMappingtblList.Where(f => f.MenuId == roleNMenu.MenuId
                                     && f.RoleId == viewMenuItemObj.RoleId).Select(s => s.Id).FirstOrDefault();

                            var roleMenuMappingObjFromDb = roleMenuMappingtblList.Where(x => x.Id == Id)
                                                           .FirstOrDefault();
                            if (roleMenuMappingObjFromDb != null)
                            {

                                roleMenuMappingObjFromDb.RoleId = viewMenuItemObj.RoleId;
                                roleMenuMappingObjFromDb.MenuId = roleNMenu.MenuId;
                                roleMenuMappingObjFromDb.Active = true;
                                roleMenuMappingObjFromDb.IsSelected = roleNMenu.IsSelected;

                                _context.Entry(roleMenuMappingObjFromDb).State = EntityState.Modified;
                                _context.SaveChanges();

                                //var entry = _contex.Entry(MenuToRoleObj);
                                //if (entry.State == EntityState.Detached || entry.State == EntityState.Modified)
                                //{
                                //    entry.State = EntityState.Modified; //do it here

                                //    _contex.MenuToRole.Attach(MenuToRoleObj); //attach

                                //    _contex.SaveChanges(); //save it
                                //}



                                TempData["successMessage"] = "Update Success !";
                            }
                            else
                            {
                                MenuToRoleObj.RoleId = viewMenuItemObj.RoleId;
                                MenuToRoleObj.MenuId = roleNMenu.MenuId;
                                MenuToRoleObj.Active = true;
                                //   MenuToRoleObj.IsSelected = roleNMenu.IsSelected;
                                MenuToRoleObj.IsSelected = true;
                                _context.MenuToRole.Add(MenuToRoleObj);
                                _context.SaveChanges();
                                TempData["successMessage"] = "Update Success !";
                            }

                        }
                        else
                        {

                            MenuToRoleObj.RoleId = viewMenuItemObj.RoleId;
                            MenuToRoleObj.MenuId = roleNMenu.MenuId;
                            MenuToRoleObj.Active = true;
                            MenuToRoleObj.IsSelected = roleNMenu.IsSelected;

                            _context.MenuToRole.Add(MenuToRoleObj);
                            _context.SaveChanges();
                            TempData["successMessage"] = "Save Success !";
                        }
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Please Select a Role !";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public JsonResult UpdateRecords(List<MenuSelection> model, string roleId)
        {
            try
            {
                if (roleId != null)
                {
                    var roleMenuMappingtblList = _context.MenuToRole
                                                 .Where(w => w.RoleId == roleId)
                                                 .ToList();

                    MenuToRole MenuToRoleObj = new MenuToRole();

                    foreach (var roleNMenu in model)
                    {
                        if (roleMenuMappingtblList.Count > 0)
                        {
                            var Id = roleMenuMappingtblList.Where(f => f.MenuId == roleNMenu.MenuId
                                     && f.RoleId == roleId).Select(s => s.Id).FirstOrDefault();

                            var roleMenuMappingObjFromDb = roleMenuMappingtblList.Where(x => x.Id == Id)
                                                           .FirstOrDefault();
                            if (roleMenuMappingObjFromDb != null)
                            {

                                roleMenuMappingObjFromDb.RoleId = roleId;
                                roleMenuMappingObjFromDb.MenuId = roleNMenu.MenuId;
                                roleMenuMappingObjFromDb.Active = true;
                                roleMenuMappingObjFromDb.IsSelected = roleNMenu.IsSelected;

                                _context.Entry(roleMenuMappingObjFromDb).State = EntityState.Modified;
                                _context.SaveChanges();

                                TempData["successMessage"] = "Update Success !";
                            }
                            else
                            {
                                MenuToRoleObj.RoleId = roleId;
                                MenuToRoleObj.MenuId = roleNMenu.MenuId;
                                MenuToRoleObj.Active = true;
                                MenuToRoleObj.IsSelected = roleNMenu.IsSelected;
                                MenuToRoleObj.Id = null;

                                _context.MenuToRole.Add(MenuToRoleObj);
                                _context.SaveChanges();
                                TempData["successMessage"] = "Update Success !";
                                //return Json("Update Success !", JsonRequestBehavior.AllowGet);
                            }

                        }
                        else
                        {

                            MenuToRoleObj.RoleId = roleId;
                            MenuToRoleObj.MenuId = roleNMenu.MenuId;
                            MenuToRoleObj.Active = true;
                            MenuToRoleObj.IsSelected = roleNMenu.IsSelected;
                            MenuToRoleObj.Id = null;

                            _context.MenuToRole.Add(MenuToRoleObj);
                            _context.SaveChanges();
                            TempData["successMessage"] = "Save Success !";
                        }

                    }
                    return Json(TempData["successMessage"], new JsonSerializerSettings());
                }
                else
                {
                    //TempData["ErrorMessage"] = "Please Select a Role !";
                    return Json("Please Select a Role !", new JsonSerializerSettings());
                }

                //return RedirectToAction("Index");
                //return Json("Updated", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //return RedirectToAction("Index");
                Console.WriteLine(ex.Message);
                return Json("Error!", new JsonSerializerSettings());
            }
        }
    }
}
