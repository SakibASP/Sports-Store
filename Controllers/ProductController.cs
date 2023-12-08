using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportsStore.Common;
using SportsStore.Data;
using SportsStore.Helper;
using SportsStore.Models.ViewModels;
//using X.PagedList;


namespace SportsStore.Controllers
{
    public class ProductController : BaseController<ProductController>
    {
        private readonly ApplicationDbContext _context;
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Product
        //image/jpeg
        public async Task<ActionResult> Index(int? cat_id,int? price, string? sortOrder, string? currentFilter, string? searchString,int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString ?? "";
            ViewData["Price"] = price;

            var product_mv =  Utility.GetProducts(_context, null, cat_id, price, searchString).Where(p => p.IsCover == 1 || p.ImageName == null).ToList();

            product_mv = sortOrder switch
            {
                "name_desc" =>  product_mv
                                        .Where(p => p.IsCover == 1).OrderByDescending(s => s.Name)
                                        .ToList(),
                _ =>  product_mv.ToList(),
            };
            int pageSize = 6;
            int pageNumber = (page ?? 1);

            ViewData["Cat_Id"] = new SelectList(_context.Category, "AUTO_ID", "CategoryName");

            var product_vm = product_mv.AsQueryable().AsNoTracking();
            return View(PaginatedList<ProductViewModel>.CreateAsync(product_vm, pageNumber, pageSize));

            //Turned off PagedList
            //return View(product_mv.ToPagedList(pageNumber, pageSize));

        }

        // GET: Product/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null || _context.ProductImages == null)
            {
                return NotFound();
            }

            List<ProductViewModel> product_mv = Utility.GetProducts(_context, id, null, null, null);

            if (product_mv == null)
            {
                return NotFound();
            }

            return View(product_mv);
        }
    }
}
