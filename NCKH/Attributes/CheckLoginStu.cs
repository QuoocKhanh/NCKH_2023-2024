using Microsoft.AspNetCore.Mvc.Filters;

namespace NCKH.Attributes
{
    public class CheckLoginStu : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //--
            //lay gtri cua bien session
            string _email = context.HttpContext.Session.GetString("stucode");

            if (string.IsNullOrEmpty(_email))
            {
                context.HttpContext.Response.Redirect("/Account/Login");
            }

            //--
            base.OnActionExecuting(context);
        }

    }
}
