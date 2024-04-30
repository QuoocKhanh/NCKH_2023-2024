using Microsoft.AspNetCore.Mvc;
using NCKH.Areas.Admin.Attributes;
using NCKH.Models;
using X.PagedList;

namespace NCKH.Areas.Admin.Controllers
{
	[Area("Admin")]
    [CheckLogin]
    public class HomeController : Controller
    {
        public MyDbContext db = new MyDbContext();
        public IActionResult Index(int? page)
        {
            string id_advisor = HttpContext.Session.GetString("advisor_id");
            int id = Convert.ToInt32(id_advisor);

            int _RecordPerPage = 10;
            //lay bien page truyen tu url
            int _CurrentPage = page ?? 1;

            List<ItemClass> list = new List<ItemClass>();
            if (id_advisor != null)
            {
                list = db.Class.Where(item => item.Advisor_ID == id).ToList();
            }
            else
            {
                list = db.Class.OrderByDescending(item => item.Advisor_ID).ToList();
            }
            return View("Index", list.ToPagedList(_CurrentPage, _RecordPerPage));
        }
       
    }
}
