using Microsoft.AspNetCore.Mvc;
using NCKH.Models;

namespace NCKH.Controllers
{
	public class StudentViewController : Controller
	{

        public MyDbContext db = new MyDbContext();
        public IActionResult Index()
		{
            string id_stu = HttpContext.Session.GetString("stu_id");
            int id = Convert.ToInt32(id_stu);

            var years = db.courseScores.Select(n => n.Year).Distinct().ToList();
            var semester = db.courseScores.Select(n => n.Semester).Distinct().ToList();

            ViewBag.Years = years;
            ViewBag.Semesters = semester;

            List<ItemCourseScore> scores = db.courseScores.Where(n => n.Student_ID == id).ToList();

            ViewBag.Student_ID = id;

            return View("Index", scores);
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
                return View("Index", results);
            }
            else if (!string.IsNullOrEmpty(year))
            {
                var results = GetResultsByYear(studentId, year); // Pass student ID and year for filtering
                ViewBag.SelectedSemester = ""; // Reset selected semester
                ViewBag.SelectedYear = year;
                return View("Index", results);
            }
            else if (!string.IsNullOrEmpty(semester))
            {
                var results = GetResultsBySemester(studentId, semester); // Pass student ID and semester for filtering
                ViewBag.SelectedYear = ""; // Reset selected year
                ViewBag.SelectedSemester = semester;
                return View("Index", results);
            }

            // No selection or error, retrieve all scores for the student (unchanged)
            var allScores = db.courseScores.Where(n => n.Student_ID == studentId).ToList();
            ViewBag.SelectedYear = "";
            ViewBag.SelectedSemester = "";
            return View("Index", allScores);
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
