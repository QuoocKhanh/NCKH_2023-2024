using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NCKH.Models
{
    public class MyDbContext : DbContext
	{
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //tao doi tuong de doc thong tin
            var builder = new ConfigurationBuilder();
            //set duong dan cua file
            builder.SetBasePath(Directory.GetCurrentDirectory());
            //add file appsetting.json
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            // doc chuoi ket noi o trong file appsetting.json
            string strDbConnectString = configuration.GetConnectionString("DBConnectString").ToString();
            //ket noi vs csdl thong qua chuoi ket noui
            optionsBuilder.UseSqlServer(strDbConnectString);
        }
		/*public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }*/
		public DbSet<ItemAdvisor> Advisors { get; set; }
        public DbSet<ItemAllUser> AllUsers { get; set; }
        public DbSet<ItemBonus> Bonus { get; set; }
        public DbSet<ItemClass> Class { get; set; }
        public DbSet<ItemComment> Comment { get; set; }
        public DbSet<ItemCourse> Courses { get; set; }
        public DbSet<ItemNoti> Noti { get; set; }
        public DbSet<ItemPost> Posts { get; set; }
        public DbSet<ItemStudent> Students { get; set; }
        public DbSet<ItemStudentScore> StudentScores { get; set; }
        public DbSet<ItemStudentStatus> StudentStatuses { get; set; }
        public DbSet<ItemUserNoti> UserNoti { get; set; }
        public DbSet<ItemCourseScore> courseScores { get; set; }
        public DbSet<ItemProgramCourse> ProgramCourses { get; set; }
        public DbSet<ItemTrainingProgram> TrainingPrograms { get; set; }
        public DbSet<ItemMessage> itemMessages { get; set; }
        public DbSet<ItemGroupChat> itemGroupChats  { get; set; }
        public DbSet<ItemUserGroup> itemUserGroups { get; set; }

    }
}
