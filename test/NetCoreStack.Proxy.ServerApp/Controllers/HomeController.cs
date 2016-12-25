using Microsoft.AspNetCore.Mvc;

namespace NetCoreStack.Proxy.ServerApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Redirect("/swagger/ui/");
        }
    }
}
