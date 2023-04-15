using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using SportsStore.Models;

namespace SportsStore.Binder
{
    public class CartModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            else if (context.Metadata.ModelType == typeof(Cart))
            {
                return new BinderTypeModelBinder(typeof(CartModelBinder));
            }

            else
            {
                return null;
            }
        }
    }
}
