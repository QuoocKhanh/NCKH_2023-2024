using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.CustomUI;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using NCKH.Models;
using NCKH.Repository;
using SixLabors.Fonts.Unicode;
using System.Data;
using System.Globalization;
using X.PagedList;

namespace NCKH.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ScoreController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Istudent stu;
        public IWebHostEnvironment Environment;

        public MyDbContext db = new MyDbContext();
        public ScoreController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment, Istudent istudent)
        {
            _logger = logger;
            Environment = webHostEnvironment;
            stu = istudent;
        }

        public IActionResult Choose(int? page)
        {
            string id_advisor = HttpContext.Session.GetString("advisor_id");
            int id = Convert.ToInt32(id_advisor);

            int _RecordPerPage = 10;
            //lay bien page truyen tu url
            int _CurrentPage = page ?? 1;

            List<ItemClass> lst_class = db.Class.Where(n=>n.Advisor_ID == id).ToList();

            return View("Choose",lst_class.ToPagedList(_CurrentPage, _RecordPerPage));
        }
        
        public IActionResult Index(int? stu_id)
        {
            int _id = stu_id ?? 0;
            var years = db.courseScores.Select(n => n.Year).Distinct().ToList();
            var semester = db.courseScores.Select(n => n.Semester).Distinct().ToList();

            ViewBag.Years = years;
            ViewBag.Semesters = semester;

            List<ItemCourseScore> scores = db.courseScores.Where(n=>n.Student_ID == _id).ToList();
            return View("Index",scores);
        }
        [HttpPost]
        public IActionResult FilterResults(string selectedOption)
        {
            if (selectedOption.StartsWith("Year_"))
            {
                // Xử lý khi chọn năm học
                var year = selectedOption.Replace("Year_", "");
                var results = GetResultsByYear(year);
                return View("Results", results);
            }
            else if (selectedOption.StartsWith("Semester_"))
            {
                // Xử lý khi chọn học kỳ
                var semester = selectedOption.Replace("Semester_", "");
                var results = GetResultsBySemester(semester);
                return View("Results", results);
            }

            return View();
        }
        public List<ItemCourseScore> GetResultsByYear(string year)
        {
            var results = db.courseScores.Where(n => n.Year == year).ToList();
            return results;
        }
        public List<ItemCourseScore> GetResultsBySemester(string semester)
        {
            var results = db.courseScores.Where(n => n.Semester == semester).ToList();
            return results;
        }
        public IActionResult ImportExcel(int id_class)
        {
            ViewBag.action = "/Admin/Score/ImportExcelPost";
            return View("ImportExcel", id_class);
        }     
        [HttpPost]
        public IActionResult ImportExcelPost(IFormCollection formfile)
        {
            int idClass = 0;

            // Check for required form fields and handle potential missing values gracefully
            if (!(formfile.ContainsKey("semester") && formfile.ContainsKey("year") && formfile.Files.Any()))
            {
                // Handle missing fields (e.g., display an error message)
                return BadRequest("Required fields (semester, year, or file) are missing.");
            }

            try
            {
                idClass = Convert.ToInt32(formfile["id_class"]);
                string _semester = formfile["semester"].ToString().Trim();
                string _year = formfile["year"].ToString().Trim();
                var file = formfile.Files.FirstOrDefault();

                if (file != null)
                {
                    string path = stu.DocumentUpload(file);
                    DataTable studentscore = stu.StudentInfoDataTable(path);

                    // Create a HashSet to keep track of processed student IDs
                    HashSet<string> processedStudentIDs = new HashSet<string>();

                    foreach (DataRow row in studentscore.Rows)
                    {
                        string studentCode = row["StudentID"].ToString();
                        string courseCode = row["CourseCode"].ToString();

                        // Use null-conditional operators for safer property access
                        ItemStudent itemStudent = db.Students.FirstOrDefault(item => item.StudentCode == studentCode);
                        ItemCourse itemCourse = db.Courses.FirstOrDefault(item => item.CourseCode == courseCode);

                        double mediumscore = TryParseDouble(row["MediumScore"].ToString(), out double convertedMediumScore) ? convertedMediumScore : 0.0;
                        double score4 = TryParseDouble(row["Score4"].ToString(), out double convertedScore4) ? convertedScore4 : 0.0;

                        if (itemStudent != null && itemCourse != null)
                        {
                            // Check if the student ID has already been processed
                            if (!processedStudentIDs.Contains(studentCode))
                            {
                                // Create a new student score record
                                ItemStudentScore itemStudentScore = new ItemStudentScore
                                {
                                    Student_ID = itemStudent.Student_ID,
                                    Year = _year,
                                    Semester = _semester,
                                    Score = mediumscore,
                                    Score4 = score4
                                };
                                db.StudentScores.Add(itemStudentScore);

                                // Add the processed student ID to the list
                                processedStudentIDs.Add(studentCode);
                            }

                            // Create a new course score record
                            ItemCourseScore itemCourseScore = new ItemCourseScore
                            {
                                Course_ID = itemCourse.Course_ID,
                                Student_ID = itemStudent.Student_ID,
                                LanHocThu = 1,
                                Year = _year,
                                Semester = _semester,
                                Score = mediumscore,
                                TextScore = row["Textscore"].ToString()
                            };
                            db.courseScores.Add(itemCourseScore);
                        }
                    }

                    db.SaveChanges(); // Save all changes at once
                }
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions (e.g., logging, displaying an error message)
                return StatusCode(500, "An error occurred during import. Please check the file format and try again.");
            }

            return RedirectToAction("Detail", new { id = idClass });
        }
        // Helper method for safer double parsing
        bool TryParseDouble(string str, out double result)
        {
            return double.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture, out result);
        }

        public IActionResult Detail(int? id,int? page)
        {
            int _RecordPerPage = 40;
            //lay bien page truyen tu url
            int _CurrentPage = page ?? 1;

            int _id = id ?? 0;
            string id_advisor = HttpContext.Session.GetString("advisor_id");
            int id_ad = Convert.ToInt32(id_advisor);

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




    }
}
