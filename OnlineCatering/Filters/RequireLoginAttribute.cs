using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OnlineCatering.Services;

namespace OnlineCatering.Filters;

public class RequireLoginAttribute : ActionFilterAttribute
{
    public string? RequiredUserType { get; set; }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;
        var userId = session.GetInt32(SessionKeys.UserId);

        if (userId == null)
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }

        if (!string.IsNullOrEmpty(RequiredUserType))
        {
            var userType = session.GetString(SessionKeys.UserType);
            if (userType != RequiredUserType)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }
        }
    }
}
