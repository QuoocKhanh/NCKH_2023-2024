using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NCKH.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Advisor",
                columns: table => new
                {
                    Advisor_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdvisorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AR = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Degree = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    User_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Advisor", x => x.Advisor_ID);
                });

            migrationBuilder.CreateTable(
                name: "AllUser",
                columns: table => new
                {
                    User_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dob = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllUser", x => x.User_ID);
                });

            migrationBuilder.CreateTable(
                name: "Bonus",
                columns: table => new
                {
                    Bonus_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bonus", x => x.Bonus_ID);
                });

            migrationBuilder.CreateTable(
                name: "Class",
                columns: table => new
                {
                    Class_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberStudent = table.Column<int>(type: "int", nullable: false),
                    Advisor_ID = table.Column<int>(type: "int", nullable: false),
                    Program_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Class", x => x.Class_ID);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    Comment_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Post_ID = table.Column<int>(type: "int", nullable: false),
                    User_ID = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Comment_ID);
                });

            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    Course_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Credit = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.Course_ID);
                });

            migrationBuilder.CreateTable(
                name: "CourseScore",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanHocThu = table.Column<int>(type: "int", nullable: false),
                    Student_ID = table.Column<int>(type: "int", nullable: false),
                    Course_ID = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Semester = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Score = table.Column<double>(type: "float", nullable: false),
                    TextScore = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseScore", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "GroupChat",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupChat", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ID_UserChat = table.Column<int>(type: "int", nullable: true),
                    ID_GroupChat = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Noti",
                columns: table => new
                {
                    Noti_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PostLink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Noti", x => x.Noti_ID);
                });

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    Post_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Class_ID = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.Post_ID);
                });

            migrationBuilder.CreateTable(
                name: "ProgramCourse",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Course_ID = table.Column<int>(type: "int", nullable: false),
                    Program_ID = table.Column<int>(type: "int", nullable: false),
                    Semester = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramCourse", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Student",
                columns: table => new
                {
                    Student_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StudentCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    User_ID = table.Column<int>(type: "int", nullable: false),
                    Class_ID = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student", x => x.Student_ID);
                });

            migrationBuilder.CreateTable(
                name: "StudentScore",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Student_ID = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Semester = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Score = table.Column<double>(type: "float", nullable: false),
                    Score4 = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentScore", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "StudentStatus",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanThu = table.Column<int>(type: "int", nullable: false),
                    Student_ID = table.Column<int>(type: "int", nullable: false),
                    Bonus_ID = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Semester = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentStatus", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TrainingProgram",
                columns: table => new
                {
                    Program_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProgramCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProgramName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MajorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalCredits = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingProgram", x => x.Program_ID);
                });

            migrationBuilder.CreateTable(
                name: "User_Group",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_UserChat = table.Column<int>(type: "int", nullable: false),
                    ID_GroupChat = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Group", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UserNoti",
                columns: table => new
                {
                    UserNoti_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Noti_ID = table.Column<int>(type: "int", nullable: false),
                    User_ID = table.Column<int>(type: "int", nullable: false),
                    IsRead = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNoti", x => x.UserNoti_ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Advisor");

            migrationBuilder.DropTable(
                name: "AllUser");

            migrationBuilder.DropTable(
                name: "Bonus");

            migrationBuilder.DropTable(
                name: "Class");

            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "Course");

            migrationBuilder.DropTable(
                name: "CourseScore");

            migrationBuilder.DropTable(
                name: "GroupChat");

            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "Noti");

            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.DropTable(
                name: "ProgramCourse");

            migrationBuilder.DropTable(
                name: "Student");

            migrationBuilder.DropTable(
                name: "StudentScore");

            migrationBuilder.DropTable(
                name: "StudentStatus");

            migrationBuilder.DropTable(
                name: "TrainingProgram");

            migrationBuilder.DropTable(
                name: "User_Group");

            migrationBuilder.DropTable(
                name: "UserNoti");
        }
    }
}
