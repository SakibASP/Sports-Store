﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using SportsStore.Common;
using SportsStore.Data;
using SportsStore.Helper;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
//using X.PagedList;

namespace SportsStore.Controllers
{
    [Authorize]
    public class SystemAdminController : BaseController<SystemAdminController>
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public SystemAdminController(ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: SystemAdmin
        public ActionResult Index(int? cat_id,string? sortOrder, string? currentFilter, string? searchString, int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewData["CurrentFilter"] = searchString ?? "";

            List<Product>? products = _context.Products.Include(x=>x.Category1).Take(10000).ToList();

            // From session
            if (cat_id != null)
                products = _context.Products.Where(c => c.Cat_Id == cat_id).Take(10000).OrderBy(s => s.Name).ToList();

            if (!String.IsNullOrEmpty(searchString))
                products = _context.Products.Include(x => x.Category1).Where(s => s.Name.ToUpper().Contains(searchString.ToUpper()) || s.Category1.CategoryName.ToUpper().Contains(searchString.ToUpper())).Take(10000).ToList();

            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(s => s.Name).ToList();
                    break;
                default:  // Name ascending 
                        products = products.OrderBy(s => s.Name).ToList();
                    break;
            }

            int pageSize = 6;
            int pageNumber = (page ?? 1);

            ViewData["Cat_Id"] = new SelectList(_context.Category, "AUTO_ID", "CategoryName");
            //return View(products.ToPagedList(pageNumber, pageSize));
            var product_ = products.AsQueryable().AsNoTracking();
            return View(PaginatedList<Product>.CreateAsync(product_, pageNumber, pageSize));
        }

        // GET: SystemAdmin/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null || _context.ProductImages == null)
                return NotFound();

            List<ProductViewModel> product_mv = Utility.GetProducts(_context, id, null, null, null);

            // From session
            //if (S_PRODUCT_LIST != null)
            //{
            //    product_mv = S_PRODUCT_LIST.Where(p => p.ProductID == id).ToList();
            //}
            //else
            //{
            //    var product = _context.Products.Include(c => c.Category1).Where(m => m.ProductID == id).FirstOrDefault();
            //    var product_img = _context.ProductImages.Where(m => m.ProductID == id).ToList();

            //    if (product_img.Count > 0)
            //    {
            //        foreach (var i in product_img)
            //        {
            //            product_mv.Add(new ProductViewModel
            //            {
            //                ProductID = i.ProductID,
            //                ProductImageID = i.AUTO_ID,
            //                ImageData = i.ImageData,
            //                ImageName = i.ImageName,
            //                Category = product.Category1.CategoryName,
            //                Description = product.Description,
            //                Name = product.Name,
            //                Cat_Id = product.Cat_Id,
            //                Price = product.Price,
            //                Buying_Price = product.Buying_Price
            //            });
            //        }
            //    }
            //    else
            //    {
            //        product_mv.Add(new ProductViewModel
            //        {
            //            ProductID = product.ProductID,
            //            ImageData = product.ImageData,
            //            ImageName = product.ImageName,
            //            Category = product.Category1.CategoryName,
            //            Description = product.Description,
            //            Name = product.Name,
            //            Cat_Id = product.Cat_Id,
            //            Price = product.Price,
            //            Buying_Price = product.Buying_Price
            //        });
            //    }
            //}

            if (product_mv == null)
                return NotFound();

            return View(product_mv);
        }

        //For removing stored photos in image tables
        public async Task<IActionResult> RemoveImage(int? id)
        {
            try
            {
                var productImages = await _context.ProductImages.FindAsync(id);
                if(productImages != null)
                {
                    _context.Remove(productImages);
                    await _context.SaveChangesAsync();

                    HttpContext.Session.Remove(Constant.PRODUCTS_LIST);
                    HttpContext.Session.Remove(Constant.TOTAL_PRODUCTS_LIST);
                    TempData["Success"] = "Removed succesfully";
                }
                else
                {
                    TempData["Error"] = "Sorry! You have to delete the product.";
                }
            }
            catch(Exception ex)
            {
                TempData["Error"] = "Failed! Something went wrong";
            }
            return RedirectToAction(nameof(Index));
        }
        //For making cover
        public async Task<IActionResult> MakeCover(int? id)
        {
            try
            {
                var productImages = await _context.ProductImages.FindAsync(id);
                if (productImages != null)
                {
                    var CoverProductImage = _context.ProductImages.Where(p => p.ProductID == productImages.ProductID && p.IsCover == 1).ToList();
                    if (CoverProductImage.Count() > 0) 
                    {
                        CoverProductImage.FirstOrDefault().IsCover = null;
                        _context.Update(productImages);
                        await _context.SaveChangesAsync();
                    }

                    productImages.IsCover = 1;
                    _context.Update(productImages);
                    await _context.SaveChangesAsync();

                    HttpContext.Session.Remove(Constant.PRODUCTS_LIST);
                    HttpContext.Session.Remove(Constant.TOTAL_PRODUCTS_LIST);
                    TempData["Success"] = "Cover setted succesfully";
                }
                else
                {
                    TempData["Error"] = "Sorry! You have to delete the product.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed! Something went wrong";
            }
            return RedirectToAction(nameof(Index));
        }

        //For adding photos to the image table
        public async Task<IActionResult> AddImages(int? ProductID,int? isCover)
        {
            try
            {
                ProductImages productImages = new ProductImages();
                var img = Request.Form.Files.FirstOrDefault();
                if (img != null)
                {
                    productImages.ProductID = ProductID;
                    productImages.IsCover = isCover;
                    productImages.CREATED_BY = CurrentUserName;
                    productImages.CREATED_DATE = DateTime.Now;
                    productImages.ImageName = img.FileName;
                    productImages.ImageData = Utility.Getimage(productImages.ImageData, Request.Form.Files);

                    _context.Add(productImages);
                    await _context.SaveChangesAsync();

                    HttpContext.Session.Remove(Constant.PRODUCTS_LIST);
                    HttpContext.Session.Remove(Constant.TOTAL_PRODUCTS_LIST);
                    TempData["Success"] = "Successfully added.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed! Something went wrong";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: SystemAdmin/Create
        public IActionResult Create()
        {
            ViewData["Cat_Id"] = new SelectList(_context.Category, "AUTO_ID", "CategoryName");
            return View();
        }

        // POST: SystemAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductID,Name,Description,Price,Buying_Price,ImageData,ImageName,CREATED_BY,CREATED_DATE,CURRENT_STOCK,Cat_Id,IsAvailabe,Category1")] Product product)
        { 
            if (ModelState.IsValid)
            {
                try
                {
                    var imgFile = Request.Form.Files.FirstOrDefault();
                    if (imgFile != null)
                    {
                        product.ImageName = imgFile.FileName;
                        product.ImageData = Utility.Getimage(product.ImageData, Request.Form.Files);
                    }
                    product.CREATED_BY = CurrentUserName;
                    product.CREATED_DATE = DateTime.Now;
                    if (product.CURRENT_STOCK > 0)
                    {
                        product.IsAvailabe = true;
                    }

                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    await AddImages(product.ProductID,1);

                    HttpContext.Session.Remove(Constant.PRODUCTS_LIST);
                    HttpContext.Session.Remove(Constant.TOTAL_PRODUCTS_LIST);
                    TempData["Success"] = "Added succesfully";    
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Failed! Something went wrong. Alert : "+ex.Message;
                }
            }
            ViewData["Cat_Id"] = new SelectList(_context.Category, "AUTO_ID", "CategoryName", product.Cat_Id);
            return View(product);
        }

        // GET: SystemAdmin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["Cat_Id"] = new SelectList(_context.Category, "AUTO_ID", "CategoryName", product.Cat_Id);
            return View(product);
        }

        // POST: SystemAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,Name,Description,Price,Buying_Price,ImageData,ImageName,CREATED_BY,CREATED_DATE,CURRENT_STOCK,Cat_Id,IsAvailabe,Category1")] Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (id != product.ProductID)
                    {
                        return NotFound();
                    }                  
                    var img_file = Request.Form.Files.FirstOrDefault();
                    if(img_file != null)
                    {
                        product.ImageName = img_file.FileName;
                        product.ImageData = Utility.Getimage(product.ImageData, Request.Form.Files);
                    }
                    if (product.CURRENT_STOCK > 0)
                    {
                        product.IsAvailabe = true;
                    }
                    else
                    {
                        product.IsAvailabe = false;
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();

                    HttpContext.Session.Remove(Constant.PRODUCTS_LIST);
                    HttpContext.Session.Remove(Constant.TOTAL_PRODUCTS_LIST);
                    TempData["Success"] = "Successfully updated";                   
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Cat_Id"] = new SelectList(_context.Category, "AUTO_ID", "CategoryName", product.Cat_Id);
            return View(product);
        }
        // GET: SystemAdmin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category1)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: SystemAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                var productImages = _context.ProductImages.Where(m => m.ProductID == id).ToList();
                if (!User.IsInRole("SuperAdmin"))
                {
                    if (product.CURRENT_STOCK > 0)
                    {
                        TempData["Error"] = "Sorry! This product is in stock. Please contact with Super Admin";
                    }
                    else
                    {
                        if (productImages != null) _context.ProductImages.RemoveRange(productImages);
                        _context.Products.Remove(product);

                        HttpContext.Session.Remove(Constant.PRODUCTS_LIST);
                        HttpContext.Session.Remove(Constant.TOTAL_PRODUCTS_LIST);
                        TempData["Success"] = string.Format("{0} ! Successfully deleted", product.Name);
                    }
                }
                else
                {
                    if (productImages != null) _context.ProductImages.RemoveRange(productImages);
                    _context.Products.Remove(product);

                    HttpContext.Session.Remove(Constant.PRODUCTS_LIST);
                    HttpContext.Session.Remove(Constant.TOTAL_PRODUCTS_LIST);
                    TempData["Success"] = string.Format("{0} ! Successfully deleted", product.Name);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.ProductID == id)).GetValueOrDefault();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }
    }
}