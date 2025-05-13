using Microsoft.AspNetCore.Mvc;

namespace EmpAppADO.UIController
{
    public class EmpViewController : Controller
    {
        public IActionResult List()
        {
            return View();
        }
    }
}
