using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
namespace OrderManagement.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorsController : ControllerBase
    {
        [HttpGet]
        public ActionResult Error()
        {
            var exceptionHandler = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var error = exceptionHandler?.Error;
            return Problem(detail: error?.Message, statusCode: 500, title: "Error");
        }
    }
}
