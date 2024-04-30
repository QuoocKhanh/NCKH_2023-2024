using Microsoft.AspNetCore.Mvc;
using NCKH.Models;
using X.PagedList;

namespace NCKH.Controllers
{
    public class StatusStuController : Controller
    {
        public MyDbContext db = new MyDbContext();
        public IActionResult Index(int? id)
        {
            int _id = id ?? 0;

            List<ItemStudentStatus> result = db.StudentStatuses.Where(n=>n.Student_ID == _id).ToList();

            return View("Index", result);
        }
       
    }
}
