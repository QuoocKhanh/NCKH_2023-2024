using Microsoft.AspNetCore.Mvc;
using NCKH.Models;
using System.Data.Common;

namespace NCKH.Controllers
{
    public class AccountController : Controller
    {
        public MyDbContext db = new MyDbContext();
        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
		public IActionResult LoginPost(IFormCollection fc)
		{
			string _email = fc["email"].ToString().Trim();
			string _pass = fc["pass"].ToString().Trim();

			ItemAllUser user = db.AllUsers.FirstOrDefault(n => n.Email == _email && n.Password == _pass);

			if (user != null)
			{
				HttpContext.Session.SetString("user_id", user.User_ID.ToString());
				HttpContext.Session.SetString("email", user.Email);
               
				if (user.Type == 1 ) // Advsior
				{
					ItemAdvisor advisor = db.Advisors.FirstOrDefault(n => n.User_ID == user.User_ID);
					if (advisor != null)
					{
						HttpContext.Session.SetString("advisor_id", advisor.Advisor_ID.ToString());
						HttpContext.Session.SetString("name", advisor.AdvisorName);
						return Redirect("/Admin/Home");
					}
				}
				else if (user.Type == 2) // Student
				{
					ItemStudent stu = db.Students.FirstOrDefault(n => n.User_ID == user.User_ID);
					if (stu != null)
					{
						HttpContext.Session.SetString("name", stu.StudentName);
						HttpContext.Session.SetString("stu_id", stu.Student_ID.ToString());
						return RedirectToAction("Index", "StudentView");
					}
				}
                else if(user.Type == 0)
                {
					HttpContext.Session.SetString("name", user.UserName);
					return Redirect("/Admin/Home");
				}    
			}

			return Redirect("/Account/Login?notify=invalid");
		}

		public IActionResult Logout()
        {
            HttpContext.Session.Remove("email");
            HttpContext.Session.Remove("user_id");
            HttpContext.Session.Remove("advisor_id");
            HttpContext.Session.Remove("name");
            HttpContext.Session.Remove("stu_id");
			return Redirect("/Account/Login");
        }
        public IActionResult Info(int? id)
        {
            int _id = id ?? 0;
            //lay mot ban ghi
            ItemAllUser record = db.AllUsers.Where(item => item.User_ID == _id).FirstOrDefault();
            //tạo biến action để đưa vào thuộc tính action của thẻ form
            ViewBag.action = "/Account/UpdatePost/" + _id;
            //gọi view, truyền dữ liệu ra view

            return View("Info",record);
        }
        [HttpPost]
        public IActionResult UpdatePost(IFormCollection fc, int? id)
        {           
            string _phone = fc["pnumber"].ToString().Trim();
            string _gender = fc["gender"].ToString().Trim();
            string _password = fc["password"].ToString().Trim();
            string _dob = fc["Dob"].ToString().Trim();

            ItemAllUser record = db.AllUsers.Where(item => item.User_ID == id).FirstOrDefault();

            if (record != null)
            {
                
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
            return RedirectToAction("Index", "StudentView");
        }
    }
}
