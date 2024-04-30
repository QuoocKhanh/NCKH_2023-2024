using Microsoft.AspNetCore.Mvc;
using NCKH.Models;
using System.Security.Cryptography;
using X.PagedList;

namespace NCKH.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdvisorController : Controller
    {
        public MyDbContext db = new MyDbContext();
        public IActionResult Index(int? page)
        {
            int _RecordPerPage = 10;
            //lay bien page truyen tu url
            int _CurrentPage = page ?? 1;
            List<ItemAdvisor> _advisor = db.Advisors.OrderByDescending(item => item.Advisor_ID).ToList();
            return View("Index",_advisor.ToPagedList(_CurrentPage, _RecordPerPage));
        }
        public IActionResult Create()
        {
            ViewBag.action = "/Admin/Advisor/CreatePost";
            return View("Create");
        }
        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            string _name = fc["name"].ToString().Trim();           
            string _AR = fc["AR"].ToString().Trim();
            string _degree = fc["degree"].ToString().Trim();          
            string _email = fc["email"].ToString().Trim();
            string _phone = fc["pnumber"].ToString().Trim();
            string _gender = fc["gender"].ToString().Trim();

            ItemAllUser user = new ItemAllUser();
            user.UserName = _email;
            user.Password = "123";
            user.Email = _email;
            user.Phone = _phone;
            user.Gender = _gender;            
            user.Type = 1;

            string _dob = fc["Dob"].ToString().Trim();
            DateTime dob;
            if (DateTime.TryParse(_dob, out dob))
            {
                user.Dob = dob;
            }

            db.AllUsers.Add(user);
            db.SaveChanges();

            ItemAdvisor record = new ItemAdvisor();
            record.AdvisorName = _name;
            record.AR = _AR;
            record.Degree = _degree;
            record.User_ID = user.User_ID;

            db.Advisors.Add(record);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Update(int? id)
        {
            int _id = id ?? 0;
            //lay mot ban ghi
            ItemAdvisor record = db.Advisors.Where(item => item.Advisor_ID == _id).FirstOrDefault();
            //tạo biến action để đưa vào thuộc tính action của thẻ form
            ViewBag.action = "/Admin/Advisor/UpdatePost/" + _id;
            //gọi view, truyền dữ liệu ra view
            return View("Update", record);
        }
        [HttpPost]
        public IActionResult UpdatePost(IFormCollection fc,int? id)
        {
            string _name = fc["name"].ToString().Trim();
            string _AR = fc["AR"].ToString().Trim();
            string _degree = fc["degree"].ToString().Trim();
            ItemAdvisor record = db.Advisors.Where(item => item.Advisor_ID == id).FirstOrDefault();

            if(record != null)
            {
                record.AdvisorName = _name;
                record.AR = _AR;
                record.Degree = _degree;

            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int? id)
        {
            int _id = id ?? 0;           
            ItemAdvisor record = db.Advisors.Where(item => item.Advisor_ID == _id).FirstOrDefault();          
            db.Advisors.Remove(record);         
            db.SaveChanges();          
            return RedirectToAction("Index");
        }

    }
}
