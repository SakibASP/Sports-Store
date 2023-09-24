using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Braintree;
using iText.IO.Image;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math;
using SportsStore.Common;
using SportsStore.Data;
using SportsStore.Helper;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using X.PagedList;
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

            //var myprod = (from e in _context.Products
            //             join c in _context.Category on e.Cat_Id equals c.AUTO_ID
            //             join d in _context.ProductImages
            //             on e.ProductID equals d.ProductID into prodDept
            //             from ed in prodDept.DefaultIfEmpty()
            //             select new ProductViewModel
            //             {
            //                 ProductID = ed.ProductID,
            //                 ImageData = ed.ImageData,
            //                 ImageName = ed.ImageName,
            //                 CREATED_BY = ed.CREATED_BY,
            //                 CREATED_DATE = ed.CREATED_DATE,
            //                 CURRENT_STOCK = e.CURRENT_STOCK,
            //                 Cat_Id = e.Cat_Id,
            //                 Category = c.CategoryName,
            //                 IsCover = ed.IsCover,
            //                 IsAvailabe = e.IsAvailabe,
            //                 Name = e.Name,
            //                 ProductImageID = ed.AUTO_ID,
            //                 Buying_Price = e.Buying_Price,
            //                 Price = e.Price,
            //                 Description = e.Description,
            //                 ShortDesc = Utility.TruncateDescription(e.Description, 10),
            //             }).Take(50000).ToList();

            var product_mv = await Utility.GetProducts(_context, null, cat_id, price, searchString).Where(p => p.IsCover == 1 || p.ImageData == null).ToListAsync();

            product_mv = sortOrder switch
            {
                "name_desc" => await product_mv
                                        .Where(p => p.IsCover == 1).OrderByDescending(s => s.Name)
                                        .ToListAsync(),
                _ => await product_mv.ToListAsync(),
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
        //protected override void Dispose(bool disposing)
        //{
        //    _context.Dispose();
        //    base.Dispose(disposing);
        //}
    }
}
