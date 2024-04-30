using Microsoft.AspNetCore.Mvc.Filters;

namespace NCKH.Areas.Admin.Attributes
{
    public class CheckLogin : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //--
            //lay gtri cua bien session
            string _email = context.HttpContext.Session.GetString("email");

            if (string.IsNullOrEmpty(_email))
            {
                context.HttpContext.Response.Redirect("/Account/Login");
            }

            //--
            base.OnActionExecuting(context);
        }

    }
}
