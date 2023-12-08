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
        public async Task<IActionResult> Index(int? cat_id,int? price, string? sortOrder, string? currentFilter, string? searchString,int? page)
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

            List<ProductViewModel> allProducts = await Utility.GetProducts(_context, null, cat_id, price, searchString);
            var coveredProduct = allProducts.Where(p => p.IsCover == 1).Distinct();
            var noCoveredProduct = allProducts.Where(p => p.ImagePath is null && !coveredProduct.Any(c => c.ProductID == p.ProductID)).Distinct();
            List<ProductViewModel> product_mv = [.. coveredProduct, .. noCoveredProduct];
            //same as the above code, we can use both Concat or '..'
            //List<ProductViewModel> product_mv2 = new List<ProductViewModel>(coveredProduct.Concat(noCoveredProduct));
            product_mv = sortOrder switch
            {
                "name_desc" => [.. product_mv.OrderByDescending(s => s.Name)],
                _ => [.. product_mv.OrderByDescending(x => x.CREATED_DATE)],
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
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ProductImages == null)
            {
                return NotFound();
            }

            List<ProductViewModel> product_mv = await Utility.GetProducts(_context, id, null, null, null);

            if (product_mv == null)
            {
                return NotFound();
            }

            return View(product_mv);
        }
    }
}
