using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using NCKH.Models;
using NCKH.Repository;
using NPOI.Util;
using SixLabors.Fonts.Unicode;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using Microsoft.DotNet.Scaffolding.Shared;

namespace NCKH.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StudentController : Controller
    {
        public MyDbContext db = new MyDbContext();
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly Istudent stu;     

        public StudentController(IWebHostEnvironment webHostEnvironment, Istudent stu)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.stu = stu;
        }
        public IActionResult Index()
        {                  
            return View("Index");
        }
        public IActionResult CreateExcel(int id_class)
        {
            ViewBag.action = "/Admin/Student/CreateExcelPost";           
            return View("CreateExcel",id_class);
        }
        [HttpPost]
        public IActionResult CreateExcelPost(IFormCollection formfile)
        {
            string id_advisor = HttpContext.Session.GetString("advisor_id");
            int id = Convert.ToInt32(id_advisor);
            int idClass = Convert.ToInt32(formfile["id_class"]);
            List<ItemAllUser> user = new List<ItemAllUser>();
            var file = formfile.Files.FirstOrDefault();
            if (file != null)
            {
                string path = stu.DocumentUpload(file);
                DataTable dataTable = stu.StudentDataTable(path);

                foreach (DataRow row in dataTable.Rows)
                {
                    string dobstring = row.Field<string>("Ngày sinh");
                    string msv = row["Mã sinh viên"].ToString();
                    string ten = row["Họ tên"].ToString();
                    DateTime dob;

                    if (!string.IsNullOrEmpty(dobstring) && DateTime.TryParse(dobstring, out dob))
                    {
                        string studentEmail = GenerateStudentEmail(msv, ten);
                        ItemAllUser allUser = new ItemAllUser();
                        {
                            allUser.UserName = msv;
                            allUser.Password = "123";
                            allUser.Gender = row["Giới tính"].ToString();
                            allUser.Dob = dob;
                            allUser.Phone = row["Số điện thoại"].ToString();
                            allUser.Email = studentEmail; 
                            allUser.Type = 2;
                        }
                        user.Add(allUser);
                    }
                }
                db.AllUsers.AddRange(user);
                db.SaveChanges();

                foreach (var entity in user)
                {
                    int entityId = entity.User_ID;
                    DataRow row = dataTable.Rows.Cast<DataRow>().FirstOrDefault(r => r["Mã sinh viên"].ToString() == entity.UserName);
                    if (row != null)
                    {
                        row["User_ID"] = entityId;
                        row["Class_ID"] = idClass;
                    }
                }
                stu.ImportStu(dataTable);
            }

            return RedirectToAction("Detail", "Class", new { id = idClass });
        }
           
        private DateTime ConvertStringToDateTime(string dateString)
        {
            DateTime dob;
            if (DateTime.TryParse(dateString, out dob))
            {
                return dob;
            }
            else
            {
                return DateTime.MinValue;
            }
        }
        public static string GenerateStudentEmail(string msv, string ten)
        {
            string[] parts = ten.Split(' ');
            string lastName = parts[parts.Length - 1]; // Lấy phần tử cuối cùng trong mảng parts
            string cleanedLastName = RemoveAccents(lastName);
            string studentEmail = $"{cleanedLastName}{msv}@lms.utc.edu.vn";
            return studentEmail.ToLower();
        }
        public static string RemoveAccents(string input)
        {
            input = input.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char c in input)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            string result = sb.ToString().ToLower();
            return result;
        }
    
        public IActionResult ExportExcel()
        {
            // Khởi tạo workbook và sheet
            var workbook = new NPOI.XSSF.UserModel.XSSFWorkbook(); // Sử dụng XSSFWorkbook cho .xlsx
            var sheet = workbook.CreateSheet("Danh sách học sinh");

            // Tạo header row
            var headerRow = sheet.CreateRow(0);
            headerRow.CreateCell(0).SetCellValue("STT");
            headerRow.CreateCell(1).SetCellValue("Mã sinh viên");
            headerRow.CreateCell(2).SetCellValue("Họ tên");
            headerRow.CreateCell(3).SetCellValue("Ngày sinh");
            headerRow.CreateCell(4).SetCellValue("Nơi sinh");
            headerRow.CreateCell(5).SetCellValue("Giới tính");
            headerRow.CreateCell(6).SetCellValue("Số điện thoại");
            headerRow.CreateCell(7).SetCellValue("Email");
            headerRow.CreateCell(8).SetCellValue("Note");
            headerRow.CreateCell(9).SetCellValue("Role");
        
            // Lưu workbook vào memory stream
            var memoryStream = new MemoryStream();
            workbook.Write(memoryStream);

            // Create a copy of the stream (optional)
            var memoryStreamCopy = new MemoryStream(memoryStream.ToArray());

            using (memoryStream)
            {

            }

            // Return the copy of the stream
            return File(memoryStreamCopy, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "danh_sach_hoc_sinh.xlsx");
        }

        public IActionResult Create(int id_class)
        {
            ViewBag.action = "/Admin/Student/CreatePost";
            return View("Create", id_class);
        }
        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            string id_advisor = HttpContext.Session.GetString("advisor_id");
            int id = Convert.ToInt32(id_advisor);

            string _name = fc["name"].ToString().Trim();
            string _address = fc["address"].ToString().Trim();
            string _code = fc["studentcode"].ToString().Trim();
            string _email = fc["email"].ToString().Trim();
            string _phone = fc["pnumber"].ToString().Trim();
            string _gender = fc["gender"].ToString().Trim();
            string _note = fc["note"].ToString().Trim();
            string _role = fc["role"].ToString().Trim();

            int idClass = Convert.ToInt32(fc["id_class"]);

            string studentEmail = GenerateStudentEmail(_code, _name);

            ItemAllUser user = new ItemAllUser();
            {
                user.UserName = _code;
                user.Password = "123";
                user.Email = studentEmail;
                user.Phone = _phone;
                user.Gender = _gender;
                user.Type = 2;
            }         
            string _dob = fc["Dob"].ToString().Trim();
            DateTime dob;
            if (DateTime.TryParse(_dob, out dob))
            {
                user.Dob = dob;
            }

            db.AllUsers.Add(user);
            db.SaveChanges();

            ItemStudent student = new ItemStudent();
            {
                student.StudentName = _name;
                student.StudentCode = _code;
                student.Address = _address;
                student.Note = _note;
                student.User_ID = user.User_ID;
                student.Class_ID = idClass;
                student.Role = _role;
            }
            db.Students.Add(student);
            db.SaveChanges();

            return RedirectToAction("Detail", "Class", new { id = idClass });
        }
        public IActionResult Update(int? id)
        {
            int _id = id ?? 0;
            //lay mot ban ghi
            ItemStudent record = db.Students.Where(item => item.Student_ID == _id).FirstOrDefault();
            //tạo biến action để đưa vào thuộc tính action của thẻ form
            ViewBag.action = "/Admin/Student/UpdatePost/" + _id;
            //gọi view, truyền dữ liệu ra view
            return View("Update", record);
        }
        public IActionResult UpdatePost(IFormCollection fc,int? id)
        {
            int _id = id ?? 0;

            string _name = fc["name"].ToString().Trim();
            string _address = fc["address"].ToString().Trim();                   
            string _note = fc["note"].ToString().Trim();
            string _role = fc["role"].ToString().Trim();

            ItemStudent record = db.Students.Where(item => item.Student_ID == _id).FirstOrDefault();
            if(record != null)
            {
                record.Address = _address;
                record.StudentName  = _name;
                record.Role = _role;
                record.Note = _note;
            }
            db.SaveChanges();

            return RedirectToAction("Detail", "Class");
        }

        public IActionResult Delete(int? id)
        {
            int _id = id ?? 0;
            
            //lay mot ban ghi
            ItemStudent record = db.Students.Where(item => item.Student_ID == _id).FirstOrDefault();
            //xoa ban ghi khoi csdl
            db.Students.Remove(record);
            int classid = record.Class_ID;
            //cap nhat lai table Products
            db.SaveChanges();
            //di chuyển đến action có tên là Index
            return RedirectToAction("Detail","Class", new { id = classid });
        }
        public IActionResult ViewScore(int? id)
        {
            int _id = id ?? 0;

            var years = db.courseScores.Select(n => n.Year).Distinct().ToList();
            var semester = db.courseScores.Select(n => n.Semester).Distinct().ToList();

            ViewBag.Years = years;
            ViewBag.Semesters = semester;

            List<ItemCourseScore> scores = db.courseScores.Where(n => n.Student_ID == _id).ToList();

            ViewBag.Student_ID = _id;
            
            return View("ViewScore", scores);
        }          
        [HttpPost]
        public IActionResult FilterResults(IFormCollection form)
        {
            int studentId = Convert.ToInt32(form["_id"]);

            string year = form["YearSelect"];
            string semester = form["SemesterSelect"];

            var years = db.courseScores.Select(n => n.Year).Distinct().ToList();
            var semesters = db.courseScores.Select(n => n.Semester).Distinct().ToList();

            ViewBag.Years = years;
            ViewBag.Semesters = semesters;
            ViewBag.Student_ID = studentId; // Maintain student ID for overall scores

            // Handle filtering based on year and semester selection
            if (!string.IsNullOrEmpty(year) && !string.IsNullOrEmpty(semester))
            {
                var results = GetResultsByYearAndSemester(studentId, semester, year); // Pass student ID, year, and semester for filtering
                ViewBag.SelectedYear = year;
                ViewBag.SelectedSemester = semester;
                return View("ViewScore", results);
            }
            else if (!string.IsNullOrEmpty(year))
            {
                var results = GetResultsByYear(studentId, year); // Pass student ID and year for filtering
                ViewBag.SelectedSemester = ""; // Reset selected semester
                ViewBag.SelectedYear = year;
                return View("ViewScore", results);
            }
            else if (!string.IsNullOrEmpty(semester))
            {
                var results = GetResultsBySemester(studentId, semester); // Pass student ID and semester for filtering
                ViewBag.SelectedYear = ""; // Reset selected year
                ViewBag.SelectedSemester = semester;
                return View("ViewScore", results);
            }

            // No selection or error, retrieve all scores for the student (unchanged)
            var allScores = db.courseScores.Where(n => n.Student_ID == studentId).ToList();
            ViewBag.SelectedYear = "";
            ViewBag.SelectedSemester = "";
            return View("ViewScore", allScores);
        }

        public List<ItemCourseScore> GetResultsByYear(int studentId, string year)
        {
            var results = db.courseScores
              .Where(n => n.Student_ID == studentId)
              .Where(n => n.Year == year)
              .ToList();
            return results;
        }

        public List<ItemCourseScore> GetResultsBySemester(int studentId, string semester)
        {
            var results = db.courseScores
              .Where(n => n.Student_ID == studentId)
              .Where(n => n.Semester == semester)
              .ToList();
            return results;
        }
        public List<ItemCourseScore> GetResultsByYearAndSemester(int studentId, string semester, string year)
        {
            var results = db.courseScores
                .Where(n => n.Student_ID == studentId && n.Semester == semester && n.Year == year)
                .ToList();

            return results;
        }


    }
}
