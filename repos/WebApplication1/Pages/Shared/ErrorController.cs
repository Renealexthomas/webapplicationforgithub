using Microsoft.AspNetCore.Mvc;

namespace Web_applogs2.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Error500()
        {
            throw new System.Exception("This is an intentional 500 error");
        }
    }
}