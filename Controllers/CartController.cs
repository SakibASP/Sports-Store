using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsStore.Data;
using SportsStore.Helper;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using SportsStore.Common;
using SportsStore.Interfaces;

namespace SportsStore.Controllers
{
    [Authorize]
    public class CartController(ApplicationDbContext context, IOrderProcessor orderProcessor) : BaseController<CartController>
    {
        public ViewResult Index(Cart cart,string returnUrl)
        {
            CartIndexViewModel cartIndex = new()
            {
                //Cart = GetCart(),
                Cart= cart,
                ReturnUrl = returnUrl,
            };
            //HttpContext.Session.SetInt32(Constant.CartTotal, cart.Lines.Sum(x => x.Quantity));
            //ViewData["CartTotal"] = HttpContext.Session.GetInt32(Constant.CartTotal);
            return View(cartIndex);

        }

        //adding new items to the cart
        public ActionResult AddToCart(Cart cart,int productId, string returnUrl)
        {
            //var cart = GetCart();
            Product? product = context.Products.Include(c=>c.Category1).FirstOrDefault(p => p.ProductID == productId);
            if (product != null)
            {
                try
                {
                    var _cart = cart.Lines.Where(x => x.Product.ProductID == product.ProductID).FirstOrDefault();
                    if (_cart != null && _cart.Quantity >= product.CURRENT_STOCK)
                    {
                        TempData["Error"] = "Sorry! You can not add more than available stock.";
                    }
                    else if(product.CURRENT_STOCK == 0)
                    {
                        TempData["Error"] = "Sorry! This product is out of stock.";
                    }
                    else
                    {
                        cart.AddItem(product, 1);
                        HttpContext.Session.SetInt32(Constant.CartTotal, cart.Lines.Sum(x => x.Quantity));
                        ViewData["CartTotal"] = HttpContext.Session.GetInt32(Constant.CartTotal);
                        SessionHelper.SetObjectAsJson<Cart>(HttpContext.Session, Constant.CART, cart);
                        TempData["Success"] = "Added to the cart successfully";
                    }
                }
                catch(Exception ex)
                {
                    TempData["Error"]="Error raised. The error is "+ex.Message;
                }
                
            }
            CartIndexViewModel cartIndex = new CartIndexViewModel
            {
                //Cart = GetCart(),
                Cart = cart,
                ReturnUrl = returnUrl,
            };
            //return RedirectToAction("Index","Product", new { returnUrl });
            return View(nameof(Index), cartIndex);
        }


        //removing an item from cart 
        public RedirectToActionResult RemoveFromCart(Cart cart,int productId, string returnUrl)
        {
            //var cart = GetCart();
            Product? product = context.Products.Include(c=>c.Category1).FirstOrDefault(p => p.ProductID == productId);
            if (product != null)
            {
                try
                {
                    cart.RemoveLine(product);

                    HttpContext.Session.SetInt32(Constant.CartTotal, cart.Lines.Sum(x => x.Quantity));
                    ViewData["CartTotal"] = HttpContext.Session.GetInt32(Constant.CartTotal);
                    SessionHelper.SetObjectAsJson<Cart>(HttpContext.Session, Constant.CART, cart);
                    TempData["Success"] = "Removed successfully";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error raised. The error is " + ex.Message;
                }
            }
            return RedirectToAction("Index", new { returnUrl });
        }

        // my total cart details
        public PartialViewResult Summary(Cart cart)
        {
            return PartialView(cart);
        }

        //private Cart GetCart()
        //{
        //    //Cart cart = (Cart)Session[CART];
        //    Cart cart = SessionHelper.GetObjectFromJson<Cart>(HttpContext.Session, CART);
        //    if (cart == null)
        //    {
        //        cart = new Cart();
        //        //Session[CART] = cart;
        //        SessionHelper.SetObjectAsJson<Cart>(HttpContext.Session,CART,cart);
        //    }
        //    return cart;
        //}

        //Shipping Details
        public ViewResult Checkout(Cart cart)
        {
            ShippingDetails shippingDetails = new()
            { 
                IsConfirmed = false,
                CREATE_BY = CurrentUserName,
                CREATE_DATE = DateTime.Today
            };
            if (cart.Lines.Count() == 0)
            {
                ViewData["EmptyCart"] = "True";
            }
            return View(shippingDetails);
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(Cart cart, ShippingDetails shippingDetails)
        {
            ShipmentOrders shipmentOrders = new();
            if (!cart.Lines.Any())
            {
                ModelState.AddModelError("", "Sorry, your cart is empty!");               
            }
            if (ModelState.IsValid)
            {
                try
                {                   
                    await context.ShippingDetails.AddAsync(shippingDetails);
                    await context.SaveChangesAsync();
                    foreach(var i in cart.Lines)
                    {
                        shipmentOrders.AUTO_ID = null;
                        shipmentOrders.ShippingDetailsId = shippingDetails.AUTO_ID;
                        shipmentOrders.QUANTITY = i.Quantity;
                        shipmentOrders.PRODUCT_NAME = i.Product.Name;
                        shipmentOrders.PRODUCT_ID = i.Product.ProductID;
                        shipmentOrders.CATEGORY_ID = i.Product.Cat_Id;
                        shipmentOrders.PRICE = (float)(i.Product.Price * i.Quantity);
                        shipmentOrders.CREATE_BY = shippingDetails.CREATE_BY;
                        shipmentOrders.CREATE_DATE = shippingDetails.CREATE_DATE;

                        await context.ShipmentOrders.AddAsync(shipmentOrders);
                        await context.SaveChangesAsync();
                    }
                    
                    await orderProcessor.ProcessOrder(cart, shippingDetails);
                    cart.Clear();

                    TempData["Success"] = "Mail has been sent successfully.";
                    HttpContext.Session.Remove(Constant.CART);
                    HttpContext.Session.Remove(Constant.CartTotal);
                    HttpContext.Session.Remove(Constant.PENDING_ORDERS);
                    
                    return View("Completed");
                }
                catch(Exception ex)
                {
                    TempData["Error"] = "Failed!Something went wrong! " + ex.Message;
                    return View(shippingDetails);
                }
            }
            else
            {
                return View(shippingDetails);
            }
        }
        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }
    }
}

