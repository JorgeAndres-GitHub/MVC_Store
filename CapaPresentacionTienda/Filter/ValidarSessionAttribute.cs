using CapaEntidad;
using CapaPresentacionTienda.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CapaPresentacionTienda.NewFolder
{
    public class ValidarSessionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Session.GetObjectFromJson<Cliente>("Cliente") == null)
            {
                context.Result = new RedirectResult("~/Acceso/Index");
                return;
            }
            base.OnActionExecuting(context);
        }
    }
}
