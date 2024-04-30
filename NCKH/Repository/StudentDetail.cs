using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Security.Cryptography;

namespace NCKH.Repository
{
    public class StudentDetail : Istudent
    {
        private IConfiguration configuration;
        private IWebHostEnvironment webHostEnvironment;

        public StudentDetail(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            this.configuration = configuration;
            this.webHostEnvironment = webHostEnvironment;
        }

        public string DocumentUpload(IFormFile formFile)
        {
            string uploadedPath = "";
            string uploadPath = webHostEnvironment.WebRootPath;
            string dest_path = Path.Combine(uploadPath, "uploaded_doc");

            if (!Directory.Exists(dest_path))
            {
                Directory.CreateDirectory(dest_path);
            }

            if (formFile != null && formFile.Length > 0)
            {
                string sourceFile = Path.GetFileName(formFile.FileName);
                string path = Path.Combine(dest_path, sourceFile);

                using (FileStream fileStream = new FileStream(path, FileMode.Create))
                {
                    formFile.CopyTo(fileStream);
                }

                uploadedPath = path;
            }

            return uploadedPath;
        }


        public void ImportStu(DataTable stu)
        {
            var sqlconn = configuration.GetConnectionString("DbConnectString");
            using (SqlConnection sqlconn2 = new SqlConnection(sqlconn))
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlconn2))
                {
                    sqlBulkCopy.DestinationTableName = "Student";
                    sqlBulkCopy.ColumnMappings.Add("Họ tên", "StudentName");
                    sqlBulkCopy.ColumnMappings.Add("Nơi sinh", "Address");
                    sqlBulkCopy.ColumnMappings.Add("Mã sinh viên", "StudentCode");
                    sqlBulkCopy.ColumnMappings.Add("User_ID", "User_ID");
                    sqlBulkCopy.ColumnMappings.Add("Class_ID", "Class_ID");
                    sqlBulkCopy.ColumnMappings.Add("Note", "Note");
                    sqlBulkCopy.ColumnMappings.Add("Role", "Role");
                    sqlconn2.Open();

                    sqlBulkCopy.WriteToServer(stu);
                    sqlconn2.Close();
                }
            }

        }     
       

        public DataTable StudentDataTable(string path)
        {
            var conStr = configuration.GetConnectionString("excelconnection");
            DataTable dt = new DataTable();
            conStr = string.Format(conStr, path);

            using (OleDbConnection excelconn = new OleDbConnection(conStr))
            {
                excelconn.Open();

                DataTable excelschema = excelconn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (excelschema != null && excelschema.Rows.Count > 0)
                {
                    var sheetname = excelschema.Rows[0]["Table_Name"].ToString();

                    string query = "SELECT [Mã sinh viên],[Họ tên], [Nơi sinh],[Giới tính],[Ngày sinh],[Số điện thoại],[Email],[Note],[Role] FROM [" + sheetname + "]";

                    using (OleDbCommand command = new OleDbCommand(query, excelconn))
                    {
                        using (OleDbDataAdapter da = new OleDbDataAdapter(command))
                        {
                            
                            da.Fill(dt);
                        }
                    }
                }
            }
            DataTable newData = new DataTable();
            {
                newData.Columns.Add("Mã sinh viên");
                newData.Columns.Add("Họ tên");
                newData.Columns.Add("Nơi sinh");
                newData.Columns.Add("Giới tính");
                newData.Columns.Add("Ngày sinh");
                newData.Columns.Add("Số điện thoại");
                newData.Columns.Add("Email");
                newData.Columns.Add("Note");
                newData.Columns.Add("Role");
                newData.Columns.Add("User_ID");
                newData.Columns.Add("Class_ID");
                              
            }          

            foreach (DataRow row in dt.Rows)
            {
                if (row["Mã sinh viên"] != DBNull.Value && row["Họ tên"] != DBNull.Value && row["Nơi sinh"] != DBNull.Value && row["Giới tính"] != DBNull.Value && row["Ngày sinh"] != DBNull.Value && row["Số điện thoại"] != DBNull.Value && row["Email"] != DBNull.Value && row["Note"] != DBNull.Value && row["Role"]!=DBNull.Value)
                {
                    newData.Rows.Add(row["Mã sinh viên"], row["Họ tên"], row["Nơi sinh"], row["Giới tính"], row["Ngày sinh"], row["Số điện thoại"], row["Email"], row["Note"], row["Role"]);
                }
                
            }

            return newData;
        }

        public DataTable CourseInfoDataTable(string path)
        {
            var conStr = configuration.GetConnectionString("excelconnection");

            DataTable subjectGrades = new DataTable();
            subjectGrades.Columns.Add("CourseCode", typeof(string));
            subjectGrades.Columns.Add("CourseName", typeof(string));
            subjectGrades.Columns.Add("Credits", typeof(int));

            conStr = string.Format(conStr, path);

            using (OleDbConnection connection = new OleDbConnection(conStr))
            {
                connection.Open();
                DataTable dt = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string sheetName = row["TABLE_NAME"].ToString();
                        if (sheetName.Contains("$"))
                        {
                            OleDbCommand command = new OleDbCommand($"SELECT * FROM [{sheetName}]", connection);
                            using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
                            {
                                DataTable sheetData = new DataTable();
                                adapter.Fill(sheetData);

                                // Lấy ra các thông tin về các học phần 
                                int courseRow = 5;
                                int courseCollumn = 11;
                                bool isReading = true;
                                while (courseCollumn < sheetData.Columns.Count && isReading)
                                {
                                    string subjectName = sheetData.Rows[courseRow][courseCollumn].ToString() + " " + sheetData.Rows[courseRow][courseCollumn + 1].ToString();

                                    string[] subjectInfoArray = subjectName.Split('_');
                                    if (subjectInfoArray.Length == 2)
                                    {
                                        string courseCode = subjectInfoArray[0]; // Mã HP
                                        string courseInfo = subjectInfoArray[1]; // Thông tin HP và số tín chỉ

                                        // Tách tên môn học và số tín chỉ từ thông tin HP
                                        string[] courseInfoArray = courseInfo.Split('(');
                                        if (courseInfoArray.Length == 2)
                                        {
                                            string courseName = courseInfoArray[0].Trim();
                                            string credits = courseInfoArray[1].Replace(")", "").Trim();
                                            DataRow newRow = subjectGrades.NewRow();
                                            {
                                                newRow["CourseCode"] = courseCode;
                                                newRow["CourseName"] = courseName;
                                                newRow["Credits"] = credits;
                                            }
                                            subjectGrades.Rows.Add(newRow);
                                        }
                                    }
                                    // Di chuyển đến cột gộp tiếp theo
                                    courseCollumn += 2;

                                    // Kiểm tra xem còn dữ liệu trong cột gộp và cột tiếp theo hay không
                                    if (courseCollumn >= sheetData.Columns.Count ||
                                        (string.IsNullOrEmpty(sheetData.Rows[courseRow][courseCollumn].ToString()) && string.IsNullOrEmpty(sheetData.Rows[courseRow][courseCollumn + 1].ToString())))
                                    {
                                        // Nếu không còn dữ liệu trong cột gộp và cột tiếp theo, dừng vòng while
                                        isReading = false;
                                    }
                                }
                                //
                            }
                        }
                    }
                }
            }
            return subjectGrades;
        }
        public DataTable StudentInfoDataTable(string path)
        {
            var conStr = configuration.GetConnectionString("excelconnection");

            DataTable studentGrades = new DataTable();
            studentGrades.Columns.Add("StudentID", typeof(string));
            studentGrades.Columns.Add("StudentName", typeof(string));
            studentGrades.Columns.Add("CourseCode", typeof(string));
            studentGrades.Columns.Add("CourseName", typeof(string));
            studentGrades.Columns.Add("Score", typeof(double));
            studentGrades.Columns.Add("TextScore", typeof(string));
            studentGrades.Columns.Add("MediumScore", typeof(double));
            studentGrades.Columns.Add("Score4", typeof(double));

            conStr = string.Format(conStr, path);

            using (OleDbConnection connection = new OleDbConnection(conStr))
            {
                connection.Open();
                DataTable dt = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string sheetName = row["TABLE_NAME"].ToString();
                        if (sheetName.Contains("$"))
                        {
                            OleDbCommand command = new OleDbCommand($"SELECT * FROM [{sheetName}]", connection);
                            using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
                            {
                                DataTable sheetData = new DataTable();
                                adapter.Fill(sheetData);
                                
                                int studentRow = 7;
                                                              
                                // Lấy ra thông tin sinh viên và điểm
                                for (int i = studentRow; i < sheetData.Rows.Count; i++)
                                {
                                    string studentCode = sheetData.Rows[i][1].ToString();
                                    string studentName = sheetData.Rows[i][2].ToString() + " " + sheetData.Rows[i][3].ToString();
                                    string mediumscore = sheetData.Rows[i][7].ToString();

                                    double score = 0.0;
                                    if (!string.IsNullOrEmpty(mediumscore))
                                    {
                                        score = double.Parse(mediumscore);
                                    }


                                    for (int j = 11; j < sheetData.Columns.Count; j ++ )
                                    {

                                        string subjectName = sheetData.Rows[5][j].ToString();
                                        string[] subjectInfoArray = subjectName.Split('_');
                                        string courseCode = "";
                                        string courseName = "";
                                        if (subjectInfoArray.Length == 2)
                                        {
                                            courseCode = subjectInfoArray[0]; // Mã HP
                                            string courseInfo = subjectInfoArray[1]; // Thông tin HP và số tín chỉ

                                            string[] courseInfoArray = courseInfo.Split('(');
                                            if (courseInfoArray.Length == 2)
                                            {
                                                courseName = courseInfoArray[0].Trim();
                                                string credits = courseInfoArray[1].Replace(")", "").Trim();
                                            }
                                        }
                                        double grade;                                     
                                        if (double.TryParse(sheetData.Rows[i][j].ToString(), out grade))
                                        {
                                            string textscore = ConvertToLetterGrade(grade);
                                            double score4 = ConvertGradeTo4Scale(score);
                                            DataRow newRow = studentGrades.NewRow();
                                            {
                                                newRow["StudentID"] = studentCode;
                                                newRow["StudentName"] = studentName;
                                                newRow["CourseCode"] = courseCode;
                                                newRow["CourseName"] = courseName;
                                                newRow["Score"] = grade;
                                                newRow["Textscore"] = textscore;
                                                newRow["MediumScore"] = mediumscore;                                              
                                                newRow["Score4"] = score4;
                                            }
                                            studentGrades.Rows.Add(newRow);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return studentGrades;
        }
      

        public string ConvertToLetterGrade(double grade)
        {
            string result = "";
            if (grade >= 9.0)
            {
                result = "A+";
            }
            else if (grade >= 8.5)
            {
                result = "A";
            }
            else if (grade >= 8.0)
            {
                 result = "A-";
            }
            else if (grade >= 7.0)
            {
                result = "B";
            }
            else if (grade >= 6.0)
            {
                result = "C";
            }
            else if (grade >= 5.0)
            {
                result = "D";
            }
            else
            {
                result = "F";
            }
            return result;
        }
        public double ConvertGradeTo4Scale(double grade)
        {
            double result = 0;
            if (grade >= 9.0)
            {
                result = 4.0;
            }
            else if (grade >= 8.5)
            {
                result = 3.5;
            }
            else if (grade >= 8.0)
            {
                result = 3.0;
            }
            else if (grade >= 7.5)
            {
                result = 2.5;
            }
            else if (grade >= 7.0)
            {
                result = 2.0;
            }
            else if (grade >= 6.5)
            {
                result = 1.5;
            }
            else if (grade >= 6.0)
            {
                result = 1.0;
            }
            else if (grade >= 5.5)
            {
                result = 0.5;
            }           
            else
            {
                result = 0.0; // Điểm dưới 5.0 sẽ không được tính vào GPA
            }
            return result;
        }

    }
}
