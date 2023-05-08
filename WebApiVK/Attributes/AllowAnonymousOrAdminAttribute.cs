using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiVK.Attributes;

// Запретить доступ для аутентифицированных пользователей
public class AllowAnonymousOrAdminAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (user.Identity.IsAuthenticated && !user.IsInRole("admin"))
        {
            context.Result = new ForbidResult();
        }
    }
}