using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WarehouseSystem.Security;
using WarehouseSystem.Services;

namespace WarehouseSystem.ModelBinders
{
    public class UserInfoBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var principal = bindingContext.HttpContext.User;

            if (principal == null)
            {
                return Task.CompletedTask;
            }
            
            var user = new UserInfo
            {
                UserId = long.Parse(principal.FindFirst(WarehouseClaims.UserId).Value),
                IsManager = principal.FindFirst(WarehouseClaims.Manager) != null 
                            && principal.FindFirst(WarehouseClaims.Manager).Value != null
                            && bool.Parse(principal.FindFirst(WarehouseClaims.Manager).Value)
            }; 
            
            bindingContext.Result = ModelBindingResult.Success(user);
            return Task.CompletedTask;
        }
    }
}