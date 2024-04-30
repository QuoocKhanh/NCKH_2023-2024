using Microsoft.AspNetCore.Mvc;
using NCKH.Models;
using SixLabors.Fonts.Unicode;
using System.Collections.Generic;
using X.PagedList;

namespace NCKH.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ClassController : Controller
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
        public IActionResult Create() 
        {
            ViewBag.action = "/Admin/Class/CreatePost";
            return View("CreateUpdate");
        }
        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            string _name = fc["name"].ToString().Trim();
            int _number = Convert.ToInt32(fc["number"].ToString().Trim());
            string id_advisor = HttpContext.Session.GetString("advisor_id");
            int id = Convert.ToInt32(id_advisor);

            ItemClass record = new ItemClass();
            record.ClassName = _name;
            record.NumberStudent = _number;
            record.Advisor_ID = id;

            List<ItemTrainingProgram> list_ = db.TrainingPrograms.ToList();
            foreach (var item in list_)
            {
                string formName = "category_" + item.Program_ID;
                if (!String.IsNullOrEmpty(Request.Form[formName]))
                {
                    record.Program_ID = item.Program_ID;
                }
            }
            db.Class.Add(record);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Update(int? id)
        {
            int _id = id ?? 0;           
            ItemClass record = db.Class.Where(item => item.Class_ID == _id).FirstOrDefault();           
            ViewBag.action = "/Admin/Class/UpdatePost/" + _id;          
            return View("CreateUpdate", record);
        }
        [HttpPost]
        public IActionResult UpdatePost(IFormCollection fc, int? id)
        {
            int _id = id ?? 0;
            string _name = fc["name"].ToString().Trim();
            int _number = Convert.ToInt32(fc["number"].ToString().Trim());

            ItemClass record = db.Class.Where(item => item.Class_ID==_id).FirstOrDefault();
            if(record !=null)
            {
                record.ClassName = _name;
                record.NumberStudent = _number;
                
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int _id = id.Value;

            // Kiểm tra xem có sinh viên nào tham chiếu đến lớp học cần xóa không
            var studentsInClass = db.Students.Where(s => s.Class_ID == _id).ToList();

            // Nếu có sinh viên tham chiếu đến lớp học, xử lý trước
            if (studentsInClass.Any())
            {
                foreach (var student in studentsInClass)
                {
                    db.Students.Remove(student);
                }
                db.SaveChanges(); // Lưu thay đổi sau khi xóa sinh viên
            }

            // Tiếp tục xóa bản ghi lớp học
            ItemClass record = db.Class.FirstOrDefault(item => item.Class_ID == _id);
            if (record == null)
            {
                return NotFound();
            }

            db.Class.Remove(record);
            db.SaveChanges(); // Lưu thay đổi sau khi xóa lớp học

            return RedirectToAction("Index");
        }

        public IActionResult Detail(int? id, int? page)
        {
            int _id = id ?? 0;
            string id_advisor = HttpContext.Session.GetString("advisor_id");
            int id_ad = Convert.ToInt32(id_advisor);

            int _RecordPerPage = 40;
            //lay bien page truyen tu url
            int _CurrentPage = page ?? 1;

            ItemClass record = db.Class.Where(item => item.Class_ID == _id).FirstOrDefault();
            List<ItemStudent> stu = new List<ItemStudent>();
            if (record != null)
            {
                stu = db.Students.Where(n => n.Class_ID == record.Class_ID).ToList();
            }
            else
            {               
                ViewBag.ClassID = _id;
            }
            ViewBag.ClassID = _id;
            // Truyền dữ liệu qua ViewBag hoặc ViewData
            ViewBag.ClassName = GetClassNameById(_id);         
            return View("Detail",stu.ToPagedList(_CurrentPage, _RecordPerPage));
        }
        private string GetClassNameById(int classId)
        {
            // Thực hiện truy vấn hoặc xử lý để lấy tên lớp từ classId
            // Ví dụ:
            ItemClass classInfo = db.Class.Where(c => c.Class_ID == classId).FirstOrDefault();
            if (classInfo != null)
            {
                return classInfo.ClassName;
            }
            return "Unknown"; // Trả về giá trị mặc định nếu không tìm thấy thông tin lớp
        }


    }
}
