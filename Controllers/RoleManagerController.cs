using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsStore.Data;
using SportsStore.Models.ViewModels;

namespace SportsStore.Controllers
{
    public class RoleManagerController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public RoleManagerController(RoleManager<IdentityRole> roleManager, ApplicationDbContext dbContext)
        {
            _roleManager = roleManager;
            _context = dbContext;
        }
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }
        [HttpPost]
        public async Task<IActionResult> AddRole(string roleName)
        {
            if (roleName != null)
            {
                try
                {
                    var roleName_ = _context.Roles.Where(r => r.Name == roleName).Count();
                    if(roleName_ == 0)
                    {
                        await _roleManager.CreateAsync(new IdentityRole(roleName.Trim()));
                        TempData["Success"] = "New Role successfully added.";
                    }
                    else
                    {
                        TempData["Error"] = "Sorry! This role already exists.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"]="Failed! Something went wrong. "+ex.Message;
                }
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role_ = await _roleManager.FindByIdAsync(id);
            RolesViewModel rvm = new RolesViewModel
            {
                Id = role_.Id,
                roleName = role_.Name
            };
            return View(rvm);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(string roleName,string id)
        {
            if (id != null)
            {
                try
                {
                    var role = await _roleManager.FindByIdAsync(id);
                    if (await _roleManager.RoleExistsAsync(role.Name))
                    {
                        var roleName_ = _context.Roles.Where(x => x.Name == roleName).Count();
                        if (roleName_ == 0)
                        {
                            await _roleManager.SetRoleNameAsync(role,roleName);
                            TempData["Success"] = "Role successfully Updated";
                        }
                        else
                        {
                            TempData["Error"] = "Sorry! Already role exist under this name. ";
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("Role", "Role doesn't exist");
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Failed! Something went wrong. " + ex.Message;
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteRole(string roleName)
        {
            if (roleName != null)
            {
                try
                {         
                    if (await _roleManager.RoleExistsAsync(roleName.Trim()))
                    {
                        var role = await _roleManager.FindByNameAsync(roleName);
                        var User_ = _context.UserRoles.Where(x=>x.RoleId==role.Id).Count();
                        if (User_ == 0) 
                        {
                            await _roleManager.DeleteAsync(role);
                            TempData["Success"] = "Role successfully Removed";
                        }
                        else
                        {
                            TempData["Error"] = "Sorry! Already user exist under this role. ";
                        }
                         
                    }
                    else
                    {
                        ModelState.AddModelError("Role", "Role doesn't exist");
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Failed! Something went wrong. " + ex.Message;
                }
            }
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }
    }
}
