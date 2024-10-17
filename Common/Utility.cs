using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SportsStore.Common;
using SportsStore.Data;
using SportsStore.Models;
using SportsStore.Models.ViewModels;

namespace SportsStore.Common
{
    public static class Utility 
    {
        //public static byte[]? Getimage(byte[]? img, IFormFileCollection files)
        //{
        //    Product prod = new Product();
        //    MemoryStream ms = new MemoryStream();
        //    if (files != null)
        //    {
        //        foreach (var file in files)
        //        {
        //            file.CopyTo(ms);
        //            prod.ImageData = ms.ToArray();

        //            ms.Close();
        //            ms.Dispose();

        //            img = prod.ImageData;
        //        }
        //    }
        //    return img;
        //}
        public static string TruncateDescription(string myString, int maxLength)
        {
            // If the string isn't null or empty
            if (!String.IsNullOrEmpty(myString))
            {
                // Return the appropriate string size
                return (myString.Length <= maxLength) ? myString : myString.Substring(0, maxLength) + "...";
            }
            else
            {
                // Otherwise return the empty string
                return string.Empty;
            }
        }
        public static async Task<List<ProductViewModel>> GetProducts(ApplicationDbContext db,int? p_id, int? cat_id_, int? price_, string? searchString_)
        {
            db.Database.SetCommandTimeout(600);

            var product_id = new SqlParameter("@product_id", SqlDbType.Int)
            {
                Value = (object?)p_id ?? DBNull.Value
            };

            var cat_id = new SqlParameter("@cat_id", SqlDbType.Int)
            {
                Value = (object?)cat_id_ ?? DBNull.Value
            };

            var price = new SqlParameter("@price", SqlDbType.Int)
            {
                Value = (object?)price_ ?? DBNull.Value
            };

            var searchString = new SqlParameter("@searchString", SqlDbType.VarChar)
            {
                Value = (object?)searchString_ ?? DBNull.Value
            };

            var @params = new[] { product_id, cat_id, price, searchString };

            //var cmdText = "EXEC GetProducts @product_id=@product_id, @cat_id=@cat_id,@price=@price,@searchString=@searchString";
            //var productViewModel = db.Set<ProductViewModel>().FromSqlRaw(cmdText, product_id,cat_id, price,searchString).ToList();

            var productViewModel = await db.ProductViewModel.FromSqlRaw(SP.GetProducts, @params).ToListAsync();
            return productViewModel;
        }
        
        public static List<ProductViewModel> GetProducts2(ApplicationDbContext _context)
        {
            var myprod = (from e in _context.Products
                          join c in _context.Category on e.Cat_Id equals c.AUTO_ID
                          join d in _context.ProductImages
                          on e.ProductID equals d.ProductID into prodDept
                          from ed in prodDept.DefaultIfEmpty()
                          select new ProductViewModel
                          {
                              ProductID = ed.ProductID,
                              //ImageData = ed.ImageData,
                              ImageName = ed.ImageName,
                              CREATED_BY = ed.CREATED_BY,
                              CREATED_DATE = ed.CREATED_DATE,
                              CURRENT_STOCK = e.CURRENT_STOCK,
                              Cat_Id = e.Cat_Id,
                              Category = c.CategoryName,
                              IsCover = ed.IsCover,
                              IsAvailabe = e.IsAvailabe,
                              Name = e.Name,
                              ProductImageID = ed.AUTO_ID,
                              Buying_Price = e.Buying_Price,
                              Price = e.Price,
                              Description = e.Description,
                              ShortDesc = Utility.TruncateDescription(e.Description, 10),
                          }).ToList();
            return myprod;
        }
        
        public static List<Product> GetTotalProducts(ApplicationDbContext db)
        {          
            var productList = db.Products.Include(x=>x.Category1).ToList();

            return productList;
        }
        public static void GetProductStock(out decimal? SoccerPercentage, out decimal? WatersportsPercentage, out decimal? ChessPercentage, out decimal? CricketPercentage, ApplicationDbContext _context)
        {
            var Products = Convert.ToDecimal(_context.Products.Count());
            var Soccer = Convert.ToDecimal(_context.Products.Where(p => p.Cat_Id == 1).Sum(x=>x.CURRENT_STOCK));
            var Watersports = Convert.ToDecimal(_context.Products.Where(p => p.Cat_Id == 2).Sum(x => x.CURRENT_STOCK));
            var Chess = Convert.ToDecimal(_context.Products.Where(p => p.Cat_Id == 3).Sum(x => x.CURRENT_STOCK));
            var Cricket = Convert.ToDecimal(_context.Products.Where(p => p.Cat_Id == 4).Sum(x => x.CURRENT_STOCK));

            SoccerPercentage = (Soccer / Products) * 100;
            WatersportsPercentage = (Watersports / Products) * 100;
            ChessPercentage = (Chess / Products) * 100;
            CricketPercentage = (Cricket / Products) * 100;
        }
        public static void GetPendingOrders(ApplicationDbContext _context, out int? order_count)
        {
            var Total_order_pending = _context.ShippingDetails.Where(x => x.IsConfirmed == false).ToList();
            if(Total_order_pending.Count > 0)
            {
                order_count = Total_order_pending.Count;
            }
            else
            {
                order_count = 0;
            }
        }

    }
}
