using Microsoft.AspNetCore.Mvc;
using FastReport.Web;
using System.Text;
using System.Data;
using System.Composition;
using Microsoft.Extensions.Hosting;
using SportsStore.Models;
using FastReport;
using FastReport.Data;
using SportsStore.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Reflection;
using SportsStore.ReportModels;
using System;
using System.Data.SqlClient;
using Braintree;
using System.Dynamic;
using iText.Html2pdf;
using SportsStore.Common;

namespace SportsStore.Controllers
{
    public class ReportController : BaseController<ReportController>
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IHostEnvironment _hostEnvironment;
        public ReportController(IConfiguration configuration,IHostEnvironment hostEnvironment,ApplicationDbContext context)
        {
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
            _context = context;
        }
        public IActionResult CategoryReport()
        {
            var webReport = new WebReport();
            var mssqlDataConnection = new MsSqlDataConnection();
            mssqlDataConnection.ConnectionString = _configuration.GetConnectionString("DefaultConnection");
            webReport.Report.Dictionary.Connections.Add(mssqlDataConnection);
            webReport.Report.Load(Path.Combine(_hostEnvironment.ContentRootPath, "Reports", "Category.frx"));
            var categories = GetTable<Category>(_context.Category.ToList(), "Category");
            webReport.Report.RegisterData(categories, "Categories");
            ViewData["WebReport"] = webReport;
            return View();
        }

        public IActionResult ProductReport()
        {
            var webReport = new WebReport();
            var mssqlDataConnection = new MsSqlDataConnection();
            mssqlDataConnection.ConnectionString = _configuration.GetConnectionString("DefaultConnection");
            webReport.Report.Dictionary.Connections.Add(mssqlDataConnection);
            webReport.Report.Load(Path.Combine(_hostEnvironment.ContentRootPath, "Reports", "Products.frx"));
            var products = GetTable<Product>(_context.Products.ToList(), "Products");
            webReport.Report.RegisterData(products, "Products");
            ViewData["WebReport"] = webReport;
            return View();
        }

        [HttpGet]
        public IActionResult PeriodicalSell()
        {
            List<ReportViewModel> repotParam = new List<ReportViewModel>();

            repotParam.Add(new ReportViewModel
            {
                StartDate = DateTime.Now.AddMonths(-1).Date,
                EndDate = DateTime.Now.Date
            });

            ViewData["ReportParams"] = repotParam;

            return View();
        }

        [HttpPost]
        public IActionResult PeriodicalSell(ReportViewModel rvm)
        {
            var orders = _context.ShipmentOrders.Where(x => x.CREATE_DATE >= rvm.StartDate && x.CREATE_DATE <= rvm.EndDate).ToList();

            List<ReportViewModel> repotParam = new List<ReportViewModel>();

            repotParam.Add(new ReportViewModel
            {
                StartDate = rvm.StartDate,
                EndDate = rvm.EndDate
            });

            ViewData["ReportParams"] = repotParam;
            ViewData["Orders"] = orders;
            return View();
        }

        [HttpGet]
        public IActionResult PeriodicalBuy()
        {
            List<ReportViewModel> repotParam = new List<ReportViewModel>();

            repotParam.Add(new ReportViewModel
            {
                StartDate = DateTime.Now.AddMonths(-1).Date,
                EndDate = DateTime.Now.Date
            });

            ViewData["ReportParams"] = repotParam;

            return View();
        }

        [HttpPost]
        public IActionResult PeriodicalBuy(ReportViewModel rvm)
        {
            var orders = _context.Products.Where(x => x.CREATED_DATE >= rvm.StartDate && x.CREATED_DATE <= rvm.EndDate).ToList();

            List<ReportViewModel> repotParam = new List<ReportViewModel>();

            repotParam.Add(new ReportViewModel
            {
                StartDate = rvm.StartDate,
                EndDate = rvm.EndDate
            });

            ViewData["ReportParams"] = repotParam;
            ViewData["Orders"] = orders;
            return View();

            // Fast Report
            //var webReport = new WebReport();
            //var mssqlDataConnection = new MsSqlDataConnection();
            //mssqlDataConnection.ConnectionString = _configuration.GetConnectionString("DefaultConnection");
            //webReport.Report.Dictionary.Connections.Add(mssqlDataConnection);
            //webReport.Report.Load(Path.Combine(_hostEnvironment.ContentRootPath, "Reports", "BuyHistory.frx"));

            ////var Periodical_Products = _context.Products.Where(x => (x.CREATED_DATE >= rvm.StartDate && x.CREATED_DATE <= rvm.EndDate)).ToList();
            ////var myproducts = GetTable<Product>(Periodical_Products, "Products");

            //var myproducts = GetFileDetails(rvm.StartDate,rvm.EndDate);

            //webReport.Report.RegisterData(myproducts, "Products");
            //ViewData["WebReport"] = webReport;
            //return View(rvm);
        }

        [HttpPost]
        public IActionResult Export(string GridHtml)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                HtmlConverter.ConvertToPdf(GridHtml, stream);
                return File(stream.ToArray(), "application/pdf");
            }
        }

        static DataTable GetTable<TEntity>(List<TEntity> table, string name) where TEntity : class
        {
            var offset = 78;
            DataTable result = new DataTable(name);
            PropertyInfo[] infos = typeof(TEntity).GetProperties();
            foreach (PropertyInfo info in infos)
            {
                if (info.PropertyType.IsGenericType&& info.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    result.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType)));
                }
                else
                {
                    result.Columns.Add(new DataColumn(info.Name, info.PropertyType));
                }
            }
            foreach (var el in table)
            {
                DataRow row = result.NewRow();
                foreach (PropertyInfo info in infos)
                {
                    if (info.PropertyType.IsGenericType && info.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        object t = info.GetValue(el);
                        if (t == null)
                        {
                            t = Activator.CreateInstance(Nullable.GetUnderlyingType(info.PropertyType));
                        }

                        row[info.Name] = t;
                    }
                    else
                    {
                        if (info.PropertyType == typeof(byte[]))
                        {
                            //Fix for Image issue.
                            var imageData = (byte[])info.GetValue(el);
                            if(imageData != null)
                            {
                                var bytes = new byte[imageData.Length - offset];
                                Array.Copy(imageData, offset, bytes, 0, bytes.Length);
                                row[info.Name] = bytes;
                            }                          
                        }
                        else
                        {
                            row[info.Name] = info.GetValue(el);
                        }
                    }
                }
                result.Rows.Add(row);
            }

            return result;
        }
        private DataTable GetFileDetails(DateTime start, DateTime end)
        {
            string conString = "Data Source=(local); Initial Catalog=SportsStore; uid=sa;pwd=sakib@123";
            string cmdText = "SELECT * FROM [SportsStore].[dbo].[Products]" +
                " WHERE CREATED_DATE BETWEEN @StartDate AND @EndDate";
            DataTable dtData = new DataTable();
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            SqlCommand command = new SqlCommand(cmdText, con);
            command.Parameters.AddWithValue("@StartDate", start);
            command.Parameters.AddWithValue("@EndDate", end);

            //command.Parameters.Add("@StartDate", SqlDbType.DateTime);
            //command.Parameters.Add("@EndDate", SqlDbType.DateTime);
            //command.Parameters["@StartDate"].Value = start;
            //command.Parameters["@EndDate"].Value = end;

            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dtData);
            con.Close();
            return dtData;
        }
    }
}
