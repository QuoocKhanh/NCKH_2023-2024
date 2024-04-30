using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using NCKH.Models;
using NPOI.Util;
using SixLabors.Fonts.Unicode;
using System.Linq;
using X.PagedList;

namespace NCKH.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StatusController : Controller
    {

        public MyDbContext db = new MyDbContext();
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ChooseClass(int? page)
        {
            string id_advisor = HttpContext.Session.GetString("advisor_id");
            int id = Convert.ToInt32(id_advisor);
            int _RecordPerPage = 10;
            //lay bien page truyen tu url
            int _CurrentPage = page ?? 1;

            List<ItemClass> lst_class = db.Class.Where(n => n.Advisor_ID == id).ToList();

            return View("ChooseClass", lst_class.ToPagedList(_CurrentPage, _RecordPerPage));
        }
        public IActionResult Create()
        {
            ViewBag.action = "/Admin/Status/CreatePost";
            return View("Create");
        }
        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            string _name = fc["name"].ToString().Trim();
            string _content = fc["content"].ToString().Trim();
            int _type = fc["Type"] != "" && fc["Type"] == "on" ? 1 : 0;

            ItemBonus itemBonus = new ItemBonus()
            {
                Name = _name,
                Content = _content,
                Type = _type
            };

            db.Bonus.Add(itemBonus);
            db.SaveChanges();
            
            return RedirectToAction("ChooseClass");
        }
        public IActionResult Detail(int? id,int? page)
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
            return View("Detail", stu.ToPagedList(_CurrentPage, _RecordPerPage));
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

        public IActionResult AddBonus(int? stu_id)
        {
            ViewBag.Stu_id = stu_id;
            ViewBag.action = "/Admin/Status/AddBonusPost";
            return View("AddBonus");

        }
        [HttpPost]
        public IActionResult AddBonusPost(IFormCollection fc)
        {
            string _semester = fc["semester"].ToString().Trim();
            string _year = fc["year"].ToString().Trim();
            string _note = fc["note"].ToString().Trim();

            string studentIdString = fc["stu_id"]; // Assuming you get the string from form data

            int idStu = Convert.ToInt32(studentIdString);

            int bonus_id = 0;

            List<ItemBonus> list_ = db.Bonus.Where(n => n.Type == 1).ToList();
            foreach (var item in list_)
            {
                string formName = "category_" + item.Bonus_ID;
                if (!String.IsNullOrEmpty(Request.Form[formName]))
                {
                    bonus_id = item.Bonus_ID;
                    break;
                }
            }

            var existingStatus = db.StudentStatuses
                      .Where(s => s.Bonus_ID == bonus_id && s.Semester == _semester && s.Year == _year && s.Student_ID == idStu )
                      .FirstOrDefault();

            if (existingStatus != null)
            {
                // Do nothing, LanThu remains the same for Khen Thuong
                existingStatus.LanThu++;
                db.StudentStatuses.Update(existingStatus);
            }
            else
            {
                // Tạo mới bản ghi cho Khen Thuong
                ItemStudentStatus itemStudentStatus = new ItemStudentStatus()
                {
                    Student_ID = idStu,
                    Bonus_ID = bonus_id,
                    Semester = _semester,
                    Year = _year,
                    LanThu = 1,
                    Note = _note,
                };

                db.StudentStatuses.Add(itemStudentStatus);
            }
            db.SaveChanges();

            return RedirectToAction("ViewStatus", new { id = idStu });
        }
        public IActionResult AddPunish(int? stu_id)
        {
            ViewBag.Stu_id = stu_id;
            ViewBag.action = "/Admin/Status/AddPunishPost";
            return View("AddPunish");

        }
        [HttpPost]
        public IActionResult AddPunishPost(IFormCollection fc)
        {
            string _semester = fc["semester"].ToString().Trim();
            string _year = fc["year"].ToString().Trim();
            string _note = fc["note"].ToString().Trim();

            string studentIdString = fc["stu_id"]; // Assuming you get the string from form data

            int idStu = Convert.ToInt32(studentIdString);

            int bonus_id = 0;

            List<ItemBonus> list_ = db.Bonus.Where(n => n.Type == 0).ToList();
            foreach (var item in list_)
            {
                string formName = "category_" + item.Bonus_ID;
                if (!String.IsNullOrEmpty(Request.Form[formName]))
                {
                    bonus_id = item.Bonus_ID;
                    break;
                }
            }

            var existingStatus = db.StudentStatuses
                      .Where(s => s.Bonus_ID == bonus_id && s.Semester == _semester && s.Year == _year && s.Student_ID == idStu)
                      .FirstOrDefault();

            if (existingStatus != null)
            {
                // Do nothing, LanThu remains the same for Khen Thuong
                existingStatus.LanThu++;
                db.StudentStatuses.Update(existingStatus);
            }
            else
            {
                // Tạo mới bản ghi cho Khen Thuong
                ItemStudentStatus itemStudentStatus = new ItemStudentStatus()
                {
                    Student_ID = idStu,
                    Bonus_ID = bonus_id,
                    Semester = _semester,
                    Year = _year,
                    LanThu = 1,
                    Note = _note,
                };

                db.StudentStatuses.Add(itemStudentStatus);
            }
            db.SaveChanges();


            return RedirectToAction("ViewStatus", new { id = idStu });
        }
        public IActionResult ViewStatus(int? id,int? page)
        {
            int _id = id ?? 0;

            int _RecordPerPage = 10;
            //lay bien page truyen tu url
            int _CurrentPage = page ?? 1;

            List<ItemStudentStatus> status = db.StudentStatuses.Where(n => n.Student_ID == _id).ToList();          

            return View("ViewStatus", status.ToPagedList(_CurrentPage, _RecordPerPage));
        }
        public IActionResult ShowAll(int? id)
        {

            int _id = id ?? 0;
            var years = db.StudentStatuses.Select(n => n.Year).Distinct().ToList();
            var semesters = db.StudentStatuses.Select(n => n.Semester).Distinct().ToList();

            ViewBag.Years = years;
            ViewBag.Semesters = semesters;

            List<ItemStudent> itemStudents = db.Students.Where(n => n.Class_ID == id).ToList();

            // Lấy danh sách trạng thái của sinh viên
            List<ItemStudentStatus> studentStatuses = db.StudentStatuses.ToList();

            // Kết hợp dữ liệu từ hai danh sách để trả về danh sách sinh viên có trạng thái trong lớp
            var result = (from student in itemStudents
                          join status in studentStatuses on student.Student_ID equals status.Student_ID
                          select status).ToList();

            /*ViewBag.StudentStatuses = result;*/

            ViewBag.ClassID = _id;

            return View("ShowAll",result);
        }

       
        [HttpPost]
        public IActionResult FilterResults(IFormCollection form)
        {
            int classId = Convert.ToInt32(form["_idclass"]);
            string year = form["YearSelect"];
            string semester = form["SemesterSelect"];

            var years = db.StudentStatuses.Select(n => n.Year).Distinct().ToList();
            var semesters = db.StudentStatuses.Select(n => n.Semester).Distinct().ToList();

            ViewBag.Years = years;
            ViewBag.Semesters = semesters;
            ViewBag.ClassID = classId;

            // Handle filtering based on year and semester selection
            if (!string.IsNullOrEmpty(year) && !string.IsNullOrEmpty(semester))
            {
                var results = GetResultsByYearAndSemester(semester, year, classId); // Pass student ID, year, and semester for filtering
                ViewBag.SelectedYear = year;
                ViewBag.SelectedSemester = semester;
                return View("ShowAll", results);
            }
            else if (!string.IsNullOrEmpty(year))
            {
                var results = GetResultsByYear( year, classId); // Pass student ID and year for filtering
                ViewBag.SelectedSemester = ""; // Reset selected semester
                ViewBag.SelectedYear = year;
                return View("ShowAll", results);
            }
            else if (!string.IsNullOrEmpty(semester))
            {
                var results = GetResultsBySemester( semester, classId); // Pass student ID and semester for filtering
                ViewBag.SelectedYear = ""; // Reset selected year
                ViewBag.SelectedSemester = semester;
                return View("ShowAll", results);
            }

            // No selection or error, retrieve all scores for the student (unchanged)
            /*var allScores = db.courseScores.Where(n => n.Student_ID == studentId).ToList();*/
            ViewBag.SelectedYear = "";
            ViewBag.SelectedSemester = "";
            return View("ShowAll");
        }

        public List<ItemStudentStatus> GetResultsByYear(string year, int classId)
        {
            var results = (from status in db.StudentStatuses
                           join student in db.Students on status.Student_ID equals student.Student_ID
                           where status.Year == year && student.Class_ID == classId
                           select status).ToList();
            return results;
        }

        public List<ItemStudentStatus> GetResultsBySemester( string semester,int classId)
        {
            var results = (from status in db.StudentStatuses
                           join student in db.Students on status.Student_ID equals student.Student_ID
                           where status.Semester == semester && student.Class_ID == classId
                           select status).ToList();
            return results;
        }
        public List<ItemStudentStatus> GetResultsByYearAndSemester( string semester, string year,int classId)
        {
            var results = (from status in db.StudentStatuses
                           join student in db.Students on status.Student_ID equals student.Student_ID
                           where status.Semester == semester && student.Class_ID == classId && status.Year == year
                           select status).ToList();

            return results;
        }

    }
}
