using Microsoft.AspNetCore.Mvc;

namespace NextHolidaysIn.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult GenerateOTP()
        {
            return View();
        }
    }
}
