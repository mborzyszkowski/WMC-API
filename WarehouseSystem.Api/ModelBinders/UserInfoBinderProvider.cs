using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using WarehouseSystem.Security;

namespace WarehouseSystem.ModelBinders
{
    public class UserInfoBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(UserInfo))
            {
                return new BinderTypeModelBinder(typeof(UserInfoBinder));
            }

            return null;
        }
    }
}