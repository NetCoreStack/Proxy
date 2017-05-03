using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreStack.WebClient.Hosting.Controllers
{
    public class ErrorController : Controller
    {
        // This controller is called to generate response bodies for 400-599 status codes from
        [Route("error/{id:int}")]
        public IActionResult Index(int id)
        {
            if (id == StatusCodes.Status404NotFound)
            {
                
            }
            if (id == StatusCodes.Status401Unauthorized)
            {
                
            }

            return View("Error");
        }
    }
}
