using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CRUD_Application.Controllers
{
    public class HomeController : Controller
    {
        [Route("[action]")]
        public IActionResult Error()
        {
            IExceptionHandlerPathFeature? exceptionHandlerPathFeature =
                HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if(exceptionHandlerPathFeature != null && exceptionHandlerPathFeature.Error != null)
            {
                ViewBag.ErrorMessage = exceptionHandlerPathFeature.Error.Message;
            }
            return View(); //Views/Shared/Error
        }
    }
}
