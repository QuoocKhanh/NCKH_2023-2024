using Microsoft.AspNetCore.Mvc;
using NCKH.Attributes;
using NCKH.Models;
using System.Diagnostics;

namespace NCKH.Controllers
{
    public class HomeController : Controller
    {
        [CheckLoginStu]
        public IActionResult Index()
        {
            return View();
        }   
        
    }
}
