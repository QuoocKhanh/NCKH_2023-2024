using System.Data;

namespace NCKH.Repository
{
    public interface Istudent
    {
        string DocumentUpload(IFormFile formFile);
        DataTable StudentDataTable(string path);
        DataTable CourseInfoDataTable(string path);
        DataTable StudentInfoDataTable(string path);
        void ImportStu(DataTable stu);
    }
}
