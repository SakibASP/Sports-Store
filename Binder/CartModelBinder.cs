using Microsoft.AspNetCore.Mvc.ModelBinding;
using SportsStore.Helper;
using SportsStore.Models;

namespace SportsStore.Binder
{
    public class CartModelBinder : IModelBinder
    {
        private const string sessionKey = "Cart";
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            //get the cart from the session
            Cart? cart = null;
            var session = bindingContext.HttpContext.Session;
            if (session == null)
            {
                return null;
            }
            else
            {
                var jsonString = session.GetString(sessionKey);

                if (!String.IsNullOrWhiteSpace(jsonString))
                {
                    cart = SessionHelper.GetObjectFromJson<Cart>(session, sessionKey);

                }
                if (cart == null)
                {
                    cart = new Cart();
                    if (session != null)
                    {
                        SessionHelper.SetObjectAsJson<Cart>(session, sessionKey, cart);
                    }
                }

                bindingContext.Result = ModelBindingResult.Success(cart);
                return Task.CompletedTask;
            }
        }

    }
}
