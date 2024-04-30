using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using NCKH.Models;
using X.PagedList;

namespace NCKH.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        public MyDbContext db = new MyDbContext();
        public IActionResult Index(int? page )
        {
            int _RecordPerPage = 40;
            //lay bien page truyen tu url
            int _CurrentPage = page ?? 1;

            List<ItemAllUser> _user = db.AllUsers.OrderByDescending(item => item.User_ID).ToList();
            return View("Index", _user.ToPagedList(_CurrentPage, _RecordPerPage));
        }
        public IActionResult Create()
        {
            ViewBag.action = "/Admin/User/CreatePost";
            return View("CreateUpdate");
        }
        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            string _name = fc["name"].ToString().Trim();           
            string _email = fc["email"].ToString().Trim();
            string _phone = fc["pnumber"].ToString().Trim();
            string _gender = fc["gender"].ToString().Trim();
            string _password = fc["password"].ToString().Trim();

            ItemAllUser user = new ItemAllUser();
            {
                user.UserName = _name;
                user.Password = _password;
                user.Email = _email;
                user.Phone = _phone;
                user.Gender = _gender;
                user.Type = 0;
            }
           
            string _dob = fc["Dob"].ToString().Trim();
            DateTime dob;
            if (DateTime.TryParse(_dob, out dob))
            {
                user.Dob = dob;
            }

            db.AllUsers.Add(user);
            db.SaveChanges();
          
            return RedirectToAction("Index");
        }
        public IActionResult Update(int? id)
        {
            int _id = id ?? 0;
            //lay mot ban ghi
            ItemAllUser record = db.AllUsers.Where(item => item.User_ID == _id).FirstOrDefault();
            //tạo biến action để đưa vào thuộc tính action của thẻ form
            ViewBag.action = "/Admin/User/UpdatePost/" + _id;
            //gọi view, truyền dữ liệu ra view
            return View("CreateUpdate", record);
        }
        [HttpPost]
        public IActionResult UpdatePost(IFormCollection fc, int? id)
        {
            string _name = fc["name"].ToString().Trim();
            string _email = fc["email"].ToString().Trim();
            string _phone = fc["pnumber"].ToString().Trim();
            string _gender = fc["gender"].ToString().Trim();
            string _password = fc["password"].ToString().Trim();
            string _dob = fc["Dob"].ToString().Trim();
            
            ItemAllUser record = db.AllUsers.Where(item => item.User_ID == id).FirstOrDefault();

            if (record != null)
            {
               record.UserName = _name;
               record.Email = _email;
               record.Phone = _phone;
               record.Gender = _gender;
               record.Password = _password;
               DateTime dob;
               if (DateTime.TryParse(_dob, out dob))
               {
                    record.Dob = dob;
               }               
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int? id)
        {
            int _id = id ?? 0;
            ItemAllUser record = db.AllUsers.Where(item => item.User_ID == _id).FirstOrDefault();
            db.AllUsers.Remove(record);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
